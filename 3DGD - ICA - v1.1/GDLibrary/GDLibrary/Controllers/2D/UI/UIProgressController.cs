/*
Function: 		Sets the source rectangle dimensions on the parent DrawnActor2D to enable a progress bar 
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class UIProgressController : Controller
    {
        #region Fields
        private bool bDirty = false;
        int score;
        private UITextObject parent;
        #endregion

        #region Properties
        #endregion

        public UIProgressController(string id, ControllerType controllerType,UITextObject parent, EventDispatcher eventDispatcher)
            : base(id, controllerType)
        {
            score = 0;
            this.parent = parent;
            //register with the event dispatcher for the events of interest
            RegisterForEventHandling(eventDispatcher);
        }

        #region Event Handling
        protected virtual void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.RemoveActorChanged += EventDispatcher_PlayerChanged;
            eventDispatcher.restartGane += EventDispatcher_Restart;
        }

        protected virtual void EventDispatcher_PlayerChanged(EventData eventData)
        {
            score++;
            parent.Text = "Score : " + score;
            bDirty = true;
        }

        protected virtual void EventDispatcher_Restart(EventData eventData)
        {
            this.score = 0;
            parent.Text = "Score : " + score;
            bDirty = true;
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
