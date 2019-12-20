using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class PickingManager : PausableGameComponent
    {
        protected static readonly string NoObjectSelectedText = "no object selected";
        protected static readonly float DefaultMinPickPlaceDistance = 20;
        protected static readonly float DefaultMaxPickPlaceDistance = 100;
        private static readonly int DefaultDistanceToTargetPrecision = 1;

        private InputManagerParameters inputManagerParameters;
        private CameraManager cameraManager;
        private readonly ObjectManager objectManager;
        private PickingBehaviourType pickingBehaviourType;

        public PickingManager(Game game, EventDispatcher eventDispatcher, StatusType statusType,
           InputManagerParameters inputManagerParameters, CameraManager cameraManager,
           ObjectManager objectManager,
           PickingBehaviourType pickingBehaviourType)
           : base(game, eventDispatcher, statusType)
        {
            this.inputManagerParameters = inputManagerParameters;
            this.cameraManager = cameraManager;
            this.objectManager = objectManager;
            this.pickingBehaviourType = pickingBehaviourType;
        }

        #region Event Handling 
        #endregion

        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (this.cameraManager.ActiveCamera != null)
            {
                Ray mouseRay = this.inputManagerParameters.MouseManager.GetMouseRay(this.cameraManager.ActiveCamera);

                foreach (IActor actor in this.objectManager.OpaqueDrawList)
                {
                    if (PickObject(gameTime, actor as Actor3D, mouseRay))
                        break;
                }

                foreach (IActor actor in this.objectManager.TransparentDrawList)
                {
                    if (PickObject(gameTime, actor as Actor3D, mouseRay))
                        break;
                }
            }

            base.ApplyUpdate(gameTime);
        }

        private bool PickObject(GameTime gameTime, Actor3D actor3D, Ray mouseRay)
        {
            if (actor3D is CollidablePrimitiveObject)
            {
                CollidablePrimitiveObject collidee = actor3D as CollidablePrimitiveObject;

                //is the mouse ray intersecting with a collidable object?
                if (collidee.CollisionPrimitive.Intersects(mouseRay))
                {
                    HandleIntersection(gameTime, collidee);
                    NotifyIntersection(gameTime, collidee);
                    return true;
                }
                else
                {
                    //if we were picking but now we're not then reset mouse
                    NotifyNoIntersection();
                }
            }

            return false;
        }

        private void NotifyNoIntersection()
        {
            //notify listeners that we're no longer picking
            object[] additionalParameters = { NoObjectSelectedText };
            EventDispatcher.Publish(new EventData(EventActionType.OnNonePicked, EventCategoryType.ObjectPicking, additionalParameters));
        }

        private void NotifyIntersection(GameTime gameTime, CollidablePrimitiveObject collidee)
        {
            float distanceToObject = (float)Math.Round(Vector3.Distance(this.cameraManager.ActiveCamera.Transform.Translation, collidee.Transform.Translation), DefaultDistanceToTargetPrecision);
            object[] additionalParameters = { collidee, distanceToObject };
            EventDispatcher.Publish(new EventData(EventActionType.OnObjectPicked, EventCategoryType.ObjectPicking, additionalParameters));
        }

        private void HandleIntersection(GameTime gameTime, CollidablePrimitiveObject collidee)
        {
            //remove the object if we want
            if (this.pickingBehaviourType == PickingBehaviourType.PickAndRemove)
            {
                /*add code here to make a descision based on the ActorType and any keyboard, mouse or game pad input
                e.g. 
                if(this.collidee.ActorType == ActorType.CollidablePickup)
                {

                }
                else if(this.collidee.ActorType == ActorType.CollidablePlayer)
                {

                }
                */

                if (this.inputManagerParameters.MouseManager.IsLeftButtonClickedOnce())
                {
                    EventDispatcher.Publish(new EventData(collidee, EventActionType.OnRemoveActor, EventCategoryType.SystemRemove));
                }  
            }


        }

        protected override void HandleMouse(GameTime gameTime)
        {

        }

        protected override void HandleKeyboard(GameTime gameTime)
        {

        }

        protected override void HandleGamePad(GameTime gameTime)
        {

        }
    }
}
