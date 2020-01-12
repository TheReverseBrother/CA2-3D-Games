using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * Used To Change The Track Camera to the  Third Person Camera
 */
namespace GDLibrary
{
    public class CameraSwitcher : GameComponent
    {
        string id;
        int secondsBegin;
        int secondsToExecute;
        bool active;

        public CameraSwitcher(string id,Game game, EventDispatcher eventDispatcher) : base(game)
        {
            this.id = id;
            this.active = false;
            this.secondsBegin = 0;
            this.secondsToExecute = -1;
            RegisterForHandling(eventDispatcher);
        }

        protected void RegisterForHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.MenuChanged += startGame;
        }

        protected void startGame(EventData eventData)
        {
            //Sets the activate time
            if (eventData.EventType == EventActionType.OnStart)
            {
                this.active = true;
                this.secondsToExecute = this.secondsToExecute + 55;

            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.active)
            {
                this.secondsBegin = gameTime.TotalGameTime.Seconds;
            }

            if (gameTime.TotalGameTime.Seconds == this.secondsToExecute)
            {
                //Sends Change Event
                EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive,EventCategoryType.Camera, new object[] { "third" }));
            }
            base.Update(gameTime);
        }
    }
}
