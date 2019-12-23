using Microsoft.Xna.Framework;

namespace GDLibrary
{
    /// <summary>
    /// Represents an area that can detect collisions similat to ZoneObject but using only a simple
    /// BoundingSphere or BoundingBox. It does NOT have an associated model. We can use this class 
    /// to create activation zones e.g. for camera switching or event generation
    /// </summary>
    public class SimpleZoneObject : Actor3D
    {

        #region Fields
        private ICollisionPrimitive collisionPrimitive;
        private EventParameters eventParameters;
        #endregion

        #region Properties
        public ICollisionPrimitive CollisionPrimitive
        {
            get
            {
                return collisionPrimitive;
            }
            set
            {
                collisionPrimitive = value;
            }
        }
        public EventParameters EventParameters
        {
            get
            {
                return eventParameters;
            }
            set
            {
                eventParameters = value;
            }
        }
        #endregion

        //used when we dont want to pass event parameters in but instead intend to subclass SimpleZoneObject
        public SimpleZoneObject(string id, ActorType actorType,
                          Transform3D transform, StatusType statusType,
                          ICollisionPrimitive collisionPrimitive)
          : this(id, actorType, transform, statusType, collisionPrimitive, null)
        {
        }

        //used when we want to pass EventParameters in so that when we collide with the zone we can ask...what events do you trigger?
        public SimpleZoneObject(string id, ActorType actorType,
                            Transform3D transform, StatusType statusType,
                            ICollisionPrimitive collisionPrimitive, EventParameters eventParameters)
            : base(id, actorType, transform, statusType)
        {
            this.collisionPrimitive = collisionPrimitive;
            this.eventParameters = eventParameters;
        }

        public override void Update(GameTime gameTime)
        {
            //update collision primitive with new object position
            if (collisionPrimitive != null)
                collisionPrimitive.Update(gameTime, this.Transform);
        }

        public new object Clone()
        {
            if (this.CollisionPrimitive is BoxCollisionPrimitive)
                return new SimpleZoneObject("clone " + this.ID,
                this.ActorType,
                this.Transform.Clone() as Transform3D,
                    this.StatusType,
                    this.CollisionPrimitive.Clone() as BoxCollisionPrimitive,
                    this.eventParameters.Clone() as EventParameters);
            else
                return new SimpleZoneObject("clone " + this.ID,
                this.ActorType,
                this.Transform.Clone() as Transform3D,
                    this.StatusType,
                    this.CollisionPrimitive.Clone() as SphereCollisionPrimitive,
                    this.eventParameters.Clone() as EventParameters);
        }
    }
}
