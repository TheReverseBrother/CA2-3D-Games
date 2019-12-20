using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class CameraElapsedTimeController : Controller
    {
        private int elapsedTimeInMs;
        private string targetCameraID;
        private int totalElapsedTimeInMs;

        public CameraElapsedTimeController(string id, 
            ControllerType controllerType, PlayStatusType playStatusType,
            int elapsedTimeInMs, string targetCameraID) 
            : base(id, controllerType, playStatusType)
        {
            this.elapsedTimeInMs = elapsedTimeInMs;
            this.targetCameraID = targetCameraID;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            this.totalElapsedTimeInMs += gameTime.ElapsedGameTime.Milliseconds;

            if(this.totalElapsedTimeInMs >= elapsedTimeInMs)
            {
                object[] additionalParameters = { this.targetCameraID };
                EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive,
                    EventCategoryType.Camera, additionalParameters));
            }


            base.Update(gameTime, actor);
        }
    }
}
