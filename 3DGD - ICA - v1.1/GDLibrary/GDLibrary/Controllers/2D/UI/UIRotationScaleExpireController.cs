using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class UIRotationScaleExpireController : Controller
    {   
        private float rotationAngleMax, rotationAngularSpeed, timeToLiveInMs;
        private float scaleFactor;

        public UIRotationScaleExpireController(string id,
           ControllerType controllerType,
           float timeToLiveInMs, float scaleFactor)
           : this(id, controllerType, 45, 1, timeToLiveInMs, scaleFactor)
        {

        }

        public UIRotationScaleExpireController(string id, 
            ControllerType controllerType, 
            float rotationAngleMax, float rotationAngularSpeed, 
            float timeToLiveInMs, float scaleFactor) 
            : base(id, controllerType)
        {
            this.rotationAngleMax = rotationAngleMax;
            this.rotationAngularSpeed = rotationAngularSpeed;
            this.timeToLiveInMs = timeToLiveInMs;
            this.scaleFactor = scaleFactor;
        }

        private int totalTimeInMs;
        public override void Update(GameTime gameTime, IActor actor)
        {
            this.totalTimeInMs += gameTime.ElapsedGameTime.Milliseconds;

            DrawnActor2D parentActor = actor as DrawnActor2D;
            TrigonometricParameters trig = new TrigonometricParameters(this.rotationAngleMax, this.rotationAngularSpeed);
            parentActor.Transform.RotationInDegrees = MathUtility.Sin(trig, this.totalTimeInMs);
            parentActor.Transform.Scale *= this.scaleFactor;

            if(this.totalTimeInMs > this.timeToLiveInMs)
            {
                EventDispatcher.Publish(new EventData(parentActor.ID,
                   null, EventActionType.OnRemoveActor2D, EventCategoryType.SystemRemove));
            }

        }
    }
}
