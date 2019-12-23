using System;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class BoxCollisionPrimitive : ICollisionPrimitive
    {
        #region Variables
        private static Vector3 min = -1/2.0f * Vector3.One, max = 1/2.0f * Vector3.One;
        private BoundingBox boundingBox, originalBoundingBox;
        #endregion

        #region Properties
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }
        #endregion

        public BoxCollisionPrimitive() 
        {
        }

        public bool Intersects(BoundingBox box)
        {
            return this.boundingBox.Intersects(box);      
        }

        public bool Intersects(BoundingSphere sphere)
        {
            return this.boundingBox.Intersects(sphere);
        }

        public bool Intersects(ICollisionPrimitive collisionPrimitive)
        {
            return collisionPrimitive.Intersects(this.boundingBox);
        }

        //tests if the bounding box for this primitive, when moved, will intersect with the collisionPrimitive passed into the method
        public bool Intersects(ICollisionPrimitive collisionPrimitive, Vector3 translation)
        {
            BoundingBox projectedBox = this.boundingBox;
            projectedBox.Max += translation;
            projectedBox.Min += translation;
            return collisionPrimitive.Intersects(projectedBox);
        }

        public bool Intersects(Ray ray)
        {
            return (ray.Intersects(this.boundingBox) > 0);
        }

        //detect intersection and passes back distance to intersected primitive
        public bool Intersects(Ray ray, out float? distance)
        {
            distance = ray.Intersects(this.boundingBox);
            return (distance > 0);
        }

        public bool Intersects(BoundingFrustum frustum)
        {
            return ((frustum.Contains(this.boundingBox) == ContainmentType.Contains)
            || (frustum.Contains(this.boundingBox) == ContainmentType.Intersects));
        }

        public void Update(GameTime gameTime, Transform3D transform)
        {
            this.originalBoundingBox = new BoundingBox(min, max);
            this.boundingBox.Max = originalBoundingBox.Max * transform.Scale;
            this.boundingBox.Min = originalBoundingBox.Min * transform.Scale;
            this.boundingBox.Max += transform.Translation;
            this.boundingBox.Min += transform.Translation;
        }

        public override string ToString()
        {
            return this.boundingBox.ToString();
        }

        public object Clone()
        {
            return new BoxCollisionPrimitive();
        }
    }
}
