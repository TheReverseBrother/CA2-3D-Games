using Microsoft.Xna.Framework;
using System; 

namespace GDLibrary
{
    public class ProjectionParameters : ICloneable 
    {
        #region Statics
        //Deep relates to the distance between the near and far clipping planes i.e. 1 to 2500
        public static ProjectionParameters StandardDeepFiveThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 5.0f / 3, 0.1f, 2500);
            }
        }

        public static ProjectionParameters StandardDeepFourThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 2500);
            }
        }


        public static ProjectionParameters StandardDeepSixteenTen
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 2500);
            }
        }

        public static ProjectionParameters StandardDeepSixteenNine
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver4, 16.0f / 9, 1, 2500);
            }
        }

        //Medium relates to the distance between the near and far clipping planes i.e. 1 to 1000
        public static ProjectionParameters StandardMediumFiveThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 5.0f / 3, 0.1f, 1000);
            }
        }

        public static ProjectionParameters StandardMediumFourThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 1000);
            }
        }

        public static ProjectionParameters StandardMediumSixteenTen
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 1000);
            }
        }

        public static ProjectionParameters StandardMediumSixteenNine
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver4, 16.0f / 9, 0.1f, 1000);
            }
        }

        //Shallow relates to the distance between the near and far clipping planes i.e. 1 to 500
        public static ProjectionParameters StandardShallowFiveThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 5.0f / 3, 0.1f, 500);
            }
        }

        public static ProjectionParameters StandardShallowFourThree
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 4.0f / 3, 0.1f, 500);
            }
        }

        public static ProjectionParameters StandardShallowSixteenTen
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 16.0f / 10, 0.1f, 500);
            }
        }

        public static ProjectionParameters StandardShallowSixteenNine
        {
            get
            {
                return new ProjectionParameters(MathHelper.PiOver2, 16.0f / 9, 0.1f, 500);
            }
        }
        #endregion

        #region Fields
        private float fieldOfView;
        private float aspectRatio;
        private float nearClipPlane;
        private float farClipPlane;
		private bool bDirty = true;
		private Matrix projection;
        #endregion

        #region Properties
        public float FieldOfView
        {
            get
            {
                return this.fieldOfView;
            }
            set
            {
                this.fieldOfView = value <= 0 ? MathHelper.PiOver4 : value;
				this.bDirty = true;
            }
        }
        public float AspectRatio
        {
            get
            {
                return this.aspectRatio;
            }
            set
            {
                this.aspectRatio = value <= 0 ? 4.0f/3 : value;
				this.bDirty = true;
            }
        }
        public float NearClipPlane
        {
            get
            {
                return this.nearClipPlane;
            }
            set
            {
                this.nearClipPlane = value < 0 ? 1.0f : value;
				this.bDirty = true;
            }
        }
        public float FarClipPlane
        {
            get
            {
                return this.farClipPlane;
            }
            set
            {
                this.farClipPlane = value >= 10 ? value : 10.0f;
				this.bDirty = true;				
            }
        }
        public Matrix Projection
        {
            get
            {
				if(this.bDirty)
				{
					this.projection =  Matrix.CreatePerspectiveFieldOfView
						(this.fieldOfView, this.aspectRatio,
							this.nearClipPlane, this.farClipPlane);
					this.bDirty = false;		
				}			
				return this.projection;
            }
        }
        #endregion

        #region Constructors & Others
        public ProjectionParameters(float fieldOfView, float aspectRatio,
            float nearClipPlane, float farClipPlane)
        {
            //calling the setters -> apply validation
            FieldOfView = fieldOfView; 
            AspectRatio = aspectRatio;
            NearClipPlane = nearClipPlane;
            FarClipPlane = farClipPlane;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash += 31 * this.fieldOfView.GetHashCode();
            hash += 17 * this.aspectRatio.GetHashCode();
            hash += 53 * this.nearClipPlane.GetHashCode();
            hash += 11 * this.farClipPlane.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) //non-valid address
                return false;

            if (this == obj) //memory addresses
                return true;

            //results in null if fail to cast
            ProjectionParameters other = obj as ProjectionParameters; // =(Projection) obj;

            if (other == null) //cast fail
                return false;

            //TODO - comparison of floating-point values
            return (this.fieldOfView.Equals(other.FieldOfView)
                && this.aspectRatio.Equals(other.AspectRatio)
                    && this.nearClipPlane.Equals(other.NearClipPlane)
                        && this.farClipPlane.Equals(other.FarClipPlane));
        }

        public object Clone()
        {
            //deep copy of the ProjectionParameters
            //notice we call GETTER PROPERTY and not direct field access.
            return new ProjectionParameters(this.FieldOfView,
                this.AspectRatio, this.NearClipPlane, this.FarClipPlane);
        }

        #endregion

    }
}
