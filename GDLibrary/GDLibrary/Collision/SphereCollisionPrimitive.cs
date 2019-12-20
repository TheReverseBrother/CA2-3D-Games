using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class SphereCollisionPrimitive : ICollisionPrimitive
    {
        #region Variables
        private BoundingSphere boundingSphere;
        private float radius;
        #endregion

        #region Properties
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value > 0 ? value : 1;
            }
        }
        public BoundingSphere BoundingSphere
        {
            get
            {
                return boundingSphere;
            }
        }
        #endregion

        public SphereCollisionPrimitive(float radius)
        {
            this.radius = radius;
        }

        public bool Intersects(ICollisionPrimitive collisionPrimitive)
        {
            return collisionPrimitive.Intersects(this.boundingSphere);
        }

        //tests if the bounding sphere for this primitive, when moved, will intersect with the collisionPrimitive passed into the method
        public bool Intersects(ICollisionPrimitive collisionPrimitive, Vector3 translation)
        {
            BoundingSphere projectedSphere = this.boundingSphere;
            projectedSphere.Center += translation;
            return collisionPrimitive.Intersects(projectedSphere);
        }

        public bool Intersects(BoundingBox box)
        {
            return this.boundingSphere.Intersects(box);      
        }

        public bool Intersects(BoundingSphere sphere)
        {
            return this.boundingSphere.Intersects(sphere);
        }

        public bool Intersects(Ray ray)
        {
            return (ray.Intersects(this.boundingSphere) > 0);
        }

        //detect intersection and passes back distance to intersected primitive
        public bool Intersects(Ray ray, out float? distance)
        {
            distance = ray.Intersects(this.boundingSphere);
            return (distance > 0);
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            return ((frustum.Contains(this.boundingSphere) == ContainmentType.Contains)
            || (frustum.Contains(this.boundingSphere) == ContainmentType.Intersects));
        }

        public void Update(GameTime gameTime, Transform3D transform)
        {  
            this.boundingSphere = new BoundingSphere(transform.Translation, this.radius * (transform.Scale.Length()/2.0f));
        }

       
        public override string ToString()
        {
            return this.boundingSphere.ToString();
        }

        public object Clone()
        {
            return new SphereCollisionPrimitive(this.Radius);
        }
    }
}
