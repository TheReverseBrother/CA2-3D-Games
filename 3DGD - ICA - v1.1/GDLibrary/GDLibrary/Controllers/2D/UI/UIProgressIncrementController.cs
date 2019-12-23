using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class UIProgressIncrementController : Controller
    {
        private int timeBetweenIncrementInMs;
        private int progressDelta;
        private string targetProgressControllerID;
        private int totalElapsedTimeInMs;

        public UIProgressIncrementController(string id, 
            ControllerType controllerType,
            PlayStatusType playStatusType,             
            string targetProgressControllerID,
            int timeBetweenIncrementInMs,
            int progressDelta) 
            : base(id, controllerType, playStatusType)
        {
            this.targetProgressControllerID = targetProgressControllerID;
            this.timeBetweenIncrementInMs = timeBetweenIncrementInMs;           
            this.progressDelta = progressDelta;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            this.totalElapsedTimeInMs  += gameTime.ElapsedGameTime.Milliseconds;

            if(this.totalElapsedTimeInMs > this.timeBetweenIncrementInMs)
            {
                object[] additionalEventParams = {this.targetProgressControllerID, 1 };
                EventDispatcher.Publish(new EventData(EventActionType.OnHealthDelta, EventCategoryType.Player, additionalEventParams));
                this.totalElapsedTimeInMs = 0;
            }



            base.Update(gameTime, actor);
        }
    }
}
