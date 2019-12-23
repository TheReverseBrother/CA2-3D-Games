/*
Function: 		Store, update, and draw all visible UI objects based on PausableDrawableGameComponent
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class UIManager : PausableDrawableGameComponent
    {
        #region Variables
        private List<DrawnActor2D> drawList, removeList;
        private SpriteBatch spriteBatch;
        #endregion

        #region Properties
        #endregion

        public UIManager(Game game, SpriteBatch spriteBatch, EventDispatcher eventDispatcher, int initialSize, StatusType statusType)
          : base(game, eventDispatcher, statusType)
        {
            this.spriteBatch = spriteBatch;

            this.drawList = new List<DrawnActor2D>(initialSize);
            //create list to store objects to be removed at start of each update
            this.removeList = new List<DrawnActor2D>(initialSize);
        }

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.RemoveActorChanged += EventDispatcher_RemoveActorChanged;
            eventDispatcher.AddActorChanged += EventDispatcher_AddActorChanged;

            //dont forget to call the base method to register for OnStart, OnPause events!
            base.RegisterForEventHandling(eventDispatcher);
        }

        private void EventDispatcher_AddActorChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnAddActor2D)
            {
                DrawnActor2D actor = eventData.Sender as DrawnActor2D;
                if (actor != null)
                    this.Add(actor);
            }
        }

        private void EventDispatcher_RemoveActorChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnRemoveActor2D)
            {
                DrawnActor2D actor = eventData.Sender as DrawnActor2D;
                if (actor != null)
                    this.Remove(actor);
                else
                {
                    string removeID = eventData.ID;
                    if (removeID != null)
                        this.Remove(a => a.ID.Equals(removeID));
                }
            }
        }
        #endregion


        public void Add(DrawnActor2D actor)
        {
            this.drawList.Add(actor);
        }

        //call when we want to remove a drawn object from the scene
        public void Remove(DrawnActor2D actor)
        {
            this.removeList.Add(actor);
        }

        public int Remove(Predicate<DrawnActor2D> predicate)
        {
            List<DrawnActor2D> resultList = null;

            resultList = this.drawList.FindAll(predicate);
            if ((resultList != null) && (resultList.Count != 0)) //the actor(s) were found in the opaque list
            {
                foreach (DrawnActor2D actor in resultList)
                    this.removeList.Add(actor);
            }

            return resultList != null ? resultList.Count : 0;
        }

        public DrawnActor2D Find(Predicate<DrawnActor2D> predicate)
        {
            DrawnActor2D drawnActor = null;

            drawnActor = this.drawList.Find(predicate);
            if (drawnActor != null)
                return drawnActor;

            return null;

        }

        public List<DrawnActor2D> FindAll(Predicate<DrawnActor2D> predicate)
        {
            List<DrawnActor2D> resultList = new List<DrawnActor2D>();

            resultList.AddRange(this.drawList.FindAll(predicate));

            return resultList.Count == 0 ? null : resultList;

        }

        //batch remove on all objects that were requested to be removed
        protected virtual void ApplyRemove()
        {
            foreach (DrawnActor2D actor in this.removeList)
            {
                this.drawList.Remove(actor);
            }

            this.removeList.Clear();
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //remove any outstanding objects since the last update
            ApplyRemove();

            foreach (DrawnActor2D actor in this.drawList)
            {
                if ((actor.StatusType & StatusType.Update) == StatusType.Update)
                {
                    actor.Update(gameTime);
                }
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            foreach (DrawnActor2D actor in this.drawList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                {
                    actor.Draw(gameTime, spriteBatch);
                }
            }
            this.spriteBatch.End();
        }
    }
}
