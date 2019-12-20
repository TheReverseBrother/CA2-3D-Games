/*
Function: 		Stores and organises the cameras available within the game (used single and split screen layouts) 
                WORK IN PROGRESS - at present this class is only a barebones class to be used by the ObjectManager 
Author: 		NMCG
Version:		1.1
Date Updated:	
Bugs:			None
Fixes:			Added IEnumberable
*/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections;

namespace GDLibrary
{
    //See http://www.claudiobernasconi.ch/2013/07/22/when-to-use-ienumerable-icollection-ilist-and-list/
    public class CameraManager : PausableGameComponent, IEnumerable<Camera3D>
    {

        #region Fields
        private List<Camera3D> cameraList;
        private int activeCameraIndex = -1;
        #endregion

        #region Properties
        public Camera3D ActiveCamera
        {
            get
            {
                return this.cameraList[this.activeCameraIndex];
            }
        }
        public int ActiveCameraIndex
        {
            get
            {
                return this.activeCameraIndex;
            }
            set
            {
                this.activeCameraIndex = (value >= 0 && value < this.cameraList.Count) ? value : 0;
            }
        }
        #endregion

        public CameraManager(Game game, int initialSize, EventDispatcher eventDispatcher, 
            StatusType statusType) 
            : base(game, eventDispatcher, statusType)
        {
            this.cameraList = new List<Camera3D>(initialSize);
        }
        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.CameraChanged += EventDispatcher_CameraChanged;
            //dont forget to call base to register for Pause/Play
            base.RegisterForEventHandling(eventDispatcher);
        }

        private void EventDispatcher_CameraChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnCameraSetActive)
            {
                string cameraID = eventData.AdditionalParameters[0] as string;
                if (cameraID != null)
                {
                    SetActiveCamera(camera => camera.ID.Equals(cameraID));
                }
            }
            else if (eventData.EventType == EventActionType.OnCameraCycle)
            {
                CycleActiveCamera();
            }
        }

        #endregion

        public void Add(Camera3D camera)
        {
            //first time in ensures that we have a default active camera
            if (this.cameraList.Count == 0)
                this.activeCameraIndex = 0;

            this.cameraList.Add(camera);

            this.cameraList.Sort((a, b) => (a.DrawDepth <= b.DrawDepth ? 1 : -1));
        }

        public bool Remove(Predicate<Camera3D> predicate)
        {
            Camera3D foundCamera = this.cameraList.Find(predicate);
            if (foundCamera != null)
                return this.cameraList.Remove(foundCamera);

            return false;
        }

        public int RemoveAll(Predicate<Camera3D> predicate)
        {
            return this.cameraList.RemoveAll(predicate);
        }

        public bool SetActiveCamera(Predicate<Camera3D> predicate)
        {
            int index = this.cameraList.FindIndex(predicate);

            if(index != -1)
                this.ActiveCameraIndex = index;

            return (index != -1) ? true : false;
        }

        public void CycleActiveCamera()
        {
            this.ActiveCameraIndex = this.activeCameraIndex + 1;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            /* 
             * Update all the cameras in the list.
             * Remember that at the moment only 1 camera is visible so this foreach loop seems counter-intuitive.
             * Assuming each camera in the list had some form of automatic movement (e.g. like a security camera) then what would happen if we only updated the active camera?
             */
            foreach (Camera3D camera in this.cameraList)
            {
                    camera.Update(gameTime);
            }

            base.ApplyUpdate(gameTime);
        }

        public IEnumerator<Camera3D> GetEnumerator()
        {
            return this.cameraList.GetEnumerator();
        }

        //this method is called by any external foreach() loop with a handle to the camera manager
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); //calls IEnumerator<Camera3D> GetEnumerator() above
        }
    }
}
