/*
Function: 		Sets the source rectangle dimensions on the parent DrawnActor2D to enable a progress bar 
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

/* This gets score and changes it when an item is picked up
 */
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class UIProgressController : Controller
    {
        #region Fields
        private bool bDirty = false;
        int score;
        int level;
        int levelOneScore;
        private UITextObject parent;
        #endregion

        #region Properties
        #endregion

        public UIProgressController(string id, ControllerType controllerType,UITextObject parent, EventDispatcher eventDispatcher)
            : base(id, controllerType)
        {
            score = 0;
            this.parent = parent;
            this.levelOneScore = 0;
            this.level = 1;
            //register with the event dispatcher for the events of interest
            RegisterForEventHandling(eventDispatcher);
        }

        #region Event Handling
        protected virtual void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.RemoveActorChanged += EventDispatcher_PlayerChanged;
            eventDispatcher.restartGane += EventDispatcher_Restart;
            eventDispatcher.changeLevel += EventDispatcher_Level;
        }

        protected virtual void EventDispatcher_PlayerChanged(EventData eventData)
        {
            //Increments Score
            score++;
            parent.Text = "Score : " + score;
            bDirty = true;
        }
        protected virtual void EventDispatcher_Level(EventData eventData)
        {
            int levelSent = (int)eventData.AdditionalParameters[0];
            //If going to Level Two Score is kept 
            if (levelSent == 2)
            {
                int temp = this.score;
                this.levelOneScore = temp;
                this.level = levelSent;
            }
            else
            {
                levelOneScore = 0;
            }
        }
        protected virtual void EventDispatcher_Restart(EventData eventData)
        {
            //Restarts The Score
            if(level == 1)
            {
                this.score = 0;
                parent.Text = "Score : " + score;
                bDirty = true;
            }
            if(level == 2)
            {
                //If going to level two score is equal to the finish of level One
                this.score = levelOneScore;
                parent.Text = "Score : " + score;
                bDirty = true;
            }
        }
        #endregion

        public override void Update(GameTime gameTime, IActor actor)
        {
            //has the value changed?
            if (this.bDirty)
            {
                actor = parent;

                bDirty = false;
            }
            base.Update(gameTime, actor);
        }

        protected virtual void HandleWinLose()
        {

        }

    }
}
