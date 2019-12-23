/*
Function: 		Allows us to draw billboard primitives by explicitly defining the vertex data.
                Used in you I-CA project.
                 
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class BillboardParameters
    {
        public BillboardType BillboardType { get; set; }
        public Vector3 Up { get;  set; }
        public Vector3 Right { get; set; }
        public string Technique { get; set; }

        //scrolling
        public bool IsScrolling { get; protected set; }
        public Vector2 ScrollRate { get; protected set; }
        public Vector2 scrollValue;

        //animation
        public bool IsAnimated { get; protected set; }
        public int currentFrame { get; protected set; }
        public int startFrame { get; protected set; }
        public int totalFrames { get; protected set; }
        public Vector2 inverseFrameCount { get; protected set; }
        public float timeBetweenFrameInMs { get; protected set; }
        private int timeSinceLastFrameInMs;

        public void SetScrollRate(Vector2 scrollRate)
        {
            this.IsScrolling = true;
            this.ScrollRate = scrollRate;
            this.scrollValue = Vector2.Zero;
        }

        public void SetAnimationRate(int totalFrames, float timeBetweenFrameInMs, int startFrame)
        {
            this.IsAnimated = true;
            this.totalFrames = totalFrames; //remember frames are arranged in a 1 x N strip, so totalFrames == N
            this.inverseFrameCount = new Vector2(1.0f / totalFrames, 1);
            this.timeBetweenFrameInMs = timeBetweenFrameInMs;
            this.startFrame = startFrame;
            this.currentFrame = startFrame;
        }

        public void UpdateScroll(GameTime gameTime)
        {
            float invDt = 1.0f / (1000 * gameTime.ElapsedGameTime.Milliseconds);

            this.scrollValue.X += this.ScrollRate.X * invDt;
            this.scrollValue.X %= 1;

            this.scrollValue.Y += this.ScrollRate.Y * invDt;
            this.scrollValue.Y %= 1;
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            this.timeSinceLastFrameInMs += gameTime.ElapsedGameTime.Milliseconds;

            if (this.timeSinceLastFrameInMs >= this.timeBetweenFrameInMs)
            {
                this.currentFrame++;
                this.currentFrame %= this.totalFrames;
                this.timeSinceLastFrameInMs = 0;
            }
        }

        public void ResetAnimation()
        {
            this.timeSinceLastFrameInMs = 0;
            this.currentFrame = this.startFrame;
        }
        public void ResetScroll()
        {
            this.scrollValue = Vector2.Zero;
        }
    }

    public class BillboardPrimitiveObject : PrimitiveObject
    {
        #region Variables
        private BillboardParameters billboardParameters;
        #endregion

        #region Properties
        public BillboardType BillboardType
        {
            get
            {
                return this.BillboardParameters.BillboardType;
            }
            set
            {
                if (value == BillboardType.Normal)
                {
                    this.billboardParameters.Technique = "Normal";
                    this.billboardParameters.BillboardType = BillboardType.Normal;
                }
                else if (value == BillboardType.Cylindrical)
                {
                    this.billboardParameters.Technique = "Cylindrical";
                    this.billboardParameters.BillboardType = BillboardType.Cylindrical;                  
                }
                else
                {
                    this.billboardParameters.Technique = "Spherical";
                    this.billboardParameters.BillboardType = BillboardType.Spherical;
                }

                this.billboardParameters.Up = this.Transform.Up;
                this.billboardParameters.Right = this.Transform.Right;
            }
        }
        public BillboardParameters BillboardParameters
        {
            get
            {
                return this.billboardParameters;
            }
        }
        #endregion

        public BillboardPrimitiveObject(string id, ActorType actorType, Transform3D transform,
          EffectParameters effectParameters, StatusType statusType, IVertexData vertexData, BillboardType billboardType)
            : base(id, actorType, transform, effectParameters, statusType, vertexData)
        {
            //create blank set of parameters and set type to be Normal - developer can change after instanciation - see Main::InitializeBillboards()
            this.billboardParameters = new BillboardParameters();
            this.BillboardType = billboardType;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.billboardParameters.IsScrolling)
                this.billboardParameters.UpdateScroll(gameTime);

            if (this.billboardParameters.IsAnimated)
                this.billboardParameters.UpdateAnimation(gameTime);

            base.Update(gameTime);
        }



        public new object Clone()
        {
            BillboardEffectParameters effectParameters
                = new BillboardEffectParameters(this.EffectParameters.Effect,
                this.EffectParameters.Texture,
                this.EffectParameters.DiffuseColor, this.EffectParameters.Alpha);

            return new BillboardPrimitiveObject("clone - " + this.ID,
                this.ActorType,  //deep
                (Transform3D)this.Transform.Clone(), //deep
               effectParameters, //deep
               this.StatusType, //deep
                this.VertexData, this.BillboardType); //shallow
        }
    }
}
