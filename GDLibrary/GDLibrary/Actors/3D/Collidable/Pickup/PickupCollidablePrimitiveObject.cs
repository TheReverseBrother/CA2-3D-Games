namespace GDLibrary
{
    public class PickupCollidablePrimitiveObject : CollidablePrimitiveObject
    {
        #region Variables
        private PickupParameters pickupParameters;
        #endregion

        #region Properties
        public PickupParameters PickupParameters
        {
            get
            {
                return pickupParameters;
            }
            set
            {
                this.pickupParameters = value;
            }
        }
        #endregion

        //used to draw collidable primitives that a value associated with them e.g. health
        public PickupCollidablePrimitiveObject(string id, ActorType actorType, Transform3D transform,
            EffectParameters effectParameters, StatusType statusType, IVertexData vertexData,
            ICollisionPrimitive collisionPrimitive, ObjectManager objectManager,
            PickupParameters pickupParameters)
            : base(id, actorType, transform,
            effectParameters, statusType, vertexData,
            collisionPrimitive, objectManager)
        {
            this.pickupParameters = pickupParameters;
        }

        public new object Clone()
        {
            if (this.CollisionPrimitive is BoxCollisionPrimitive)
                return new PickupCollidablePrimitiveObject("clone " + this.ID,
                this.ActorType,
                this.Transform.Clone() as Transform3D,
                    this.EffectParameters.Clone() as EffectParameters,
                    this.StatusType,
                    this.VertexData,
                    this.CollisionPrimitive.Clone() as BoxCollisionPrimitive, ObjectManager,
                    this.pickupParameters.Clone() as PickupParameters);
            else
                return new PickupCollidablePrimitiveObject("clone " + this.ID,
                this.ActorType,
               this.Transform.Clone() as Transform3D,
                    this.EffectParameters.Clone() as EffectParameters,
                    this.StatusType,
                    this.VertexData,
                    (SphereCollisionPrimitive)this.CollisionPrimitive.Clone(), this.ObjectManager,
                    this.pickupParameters.Clone() as PickupParameters);
        }
    }
}
