/*
Function: 		Starting point for I-CA codebase which allows us to play with the core GDLibrary classes
                (e.g. VertexData, VertexFactory) related to rendering from first principles.

Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ICA_GDAPP
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Camera3D camera3D;
        private BasicEffect effect;
        private BasicEffect litEffect;
        private VertexPositionColor[] vertices;
        private BufferedVertexData<VertexPositionColor> vertexData;
        public float moveSpeed;
        public float rotationSpeed;
        private VertexData<VertexPositionNormalTexture> vertexDataPyramid;
        private VertexData<VertexPositionColor> vertexDataCircle;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitializeEffects();
            InitializeVertices();
            InitializeCamera();
            base.Initialize();
        }

        //2. Create an entity to store View and Projection matrices
        private void InitializeCamera() {
            this.camera3D = new Camera3D(
                new Vector3(0, 0, 10),
                -Vector3.UnitZ, 
                Vector3.Up,
                MathHelper.PiOver4, 
                graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight, 
                1, 
                1000
            );

            this.moveSpeed = 1 / 60.0f;
            this.rotationSpeed = MathHelper.Pi / 3000.0f;
        }

        //3. Add an array of data of IVertexType for passing to our GPU in draw()
        private void InitializeVertices() {
            InitializePyramid();
            InitializeWireframe();
            InitializeUnlitCircle();
        }

        private void InitializePyramid() {
            
            /*
             * Setup Vertices
             * Change BasicEffect - Different VertexType
             * BufferedVertexData, VertexData
             * Draw
             */

            //4 Side (4 Triangles @ 3 Vertices Each)
            //1 Base (2 Triangles @ 3 Vertices Each)
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[6];

            #region Common Points
            Vector3 top = new Vector3(0, 1, 0);
            Vector2 topUV = new Vector2(0.5f, 0f);

            Vector3 frontLeft = new Vector3(-1, 0, 1);
            Vector2 frontLeftUV = new Vector2(1, 1);

            Vector3 frontRight = new Vector3(1, 0, 1);
            Vector2 frontRightUV = new Vector2(0, 1);

            Vector3 backLeft = new Vector3(-1, 0, -1);
            Vector2 backLeftUV = new Vector2(0, 1);

            Vector3 backRight = new Vector3(1, 0, -1);
            Vector2 backRightUV = new Vector2(1, 0);
            #endregion

            #region Left Side
            vertices[0] = new VertexPositionNormalTexture(top, new Vector3(-1, 1, 0), topUV);

            vertices[1] = new VertexPositionNormalTexture(frontLeft, new Vector3(-1, 1, 0), frontLeftUV);

            vertices[2] = new VertexPositionNormalTexture(backLeft, new Vector3(-1, 1, 0), backLeftUV);
            #endregion

            #region Front Face
            vertices[3] = new VertexPositionNormalTexture(top, new Vector3(0, 1, 1), topUV);

            //Front right
            vertices[4] = new VertexPositionNormalTexture(frontRight, new Vector3(0, 1, 1), new Vector2(1, 1));

            //Front left
            vertices[5] = new VertexPositionNormalTexture(frontLeft, new Vector3(0, 1, 1), new Vector2(0, 1));
            #endregion

            #region Right Side
            #endregion

            //Create static method - getVerticesOfLitPyramid
            //Put result into primitive object
            //Call primitive object multiple times
            this.vertexDataPyramid = new VertexData<VertexPositionNormalTexture>(vertices, PrimitiveType.TriangleList, 1);
        }

        private void InitializeWireframe() {

            //Setup vertices
            VertexPositionColor[] vertices = new VertexPositionColor[4];
            vertices[0] = new VertexPositionColor(new Vector3(-1, 0, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.Blue);
            vertices[2] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Green);
            vertices[3] = new VertexPositionColor(new Vector3(0, -2, 0), Color.Yellow);

            //Setup buffer
            VertexBuffer vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);

            //Setup vertex data
            this.vertexData = new BufferedVertexData<VertexPositionColor>(graphics.GraphicsDevice, vertices, vertexBuffer, PrimitiveType.LineList, 2);
        }

        private void InitializeUnlitCircle() {

            VertexPositionColor[] vertices = VertexFactory.GetCircleVertices(5, 3, out int primitiveCount, OrientationType.YZAxis);
            this.vertexDataCircle = new VertexData<VertexPositionColor>(vertices, PrimitiveType.LineStrip, primitiveCount);
        }

        //1. Create a BasicEffect (i.e. GPU shader code) to allow us to set W, V, P and render a primitive using the relevant properties (e.g. position, UV, color, texture)
        private void InitializeEffects() {

            //Wireframe
            this.effect = new BasicEffect(graphics.GraphicsDevice) {
                VertexColorEnabled = true
            };

            //Lit pyramid
            this.litEffect = new BasicEffect(graphics.GraphicsDevice) {
                PreferPerPixelLighting = true,
                VertexColorEnabled = false,
                TextureEnabled = true,
                Texture = Content.Load<Texture2D>("ml"),
                DiffuseColor = Color.White.ToVector3(),
                Alpha = 1
            };

            this.litEffect.EnableDefaultLighting();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState ksState = Keyboard.GetState();
            if (ksState.IsKeyDown(Keys.W))
            {
                this.camera3D.translateBy(this.camera3D.Look * this.moveSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (ksState.IsKeyDown(Keys.S))
            {
                this.camera3D.translateBy(-this.camera3D.Look * this.moveSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }

            if (ksState.IsKeyDown(Keys.A))
            {
                this.camera3D.translateBy(-this.camera3D.Right * this.moveSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (ksState.IsKeyDown(Keys.D))
            {
                this.camera3D.translateBy(this.camera3D.Right * this.moveSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }

            if (ksState.IsKeyDown(Keys.Q))
            {
                this.camera3D.rotateByDelta(Vector3.UnitY * this.rotationSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (ksState.IsKeyDown(Keys.E))
            {
                this.camera3D.rotateByDelta(-Vector3.UnitY * this.rotationSpeed * gameTime.ElapsedGameTime.Milliseconds);
            }

            if (ksState.IsKeyDown(Keys.R))
            {
                this.camera3D.reset();
            }

            System.Diagnostics.Debug.WriteLine("Position: " + MathUtility.Round(this.camera3D.Position, 1) + ", Look: " + MathUtility.Round(this.camera3D.Look, 1));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            DrawVertices(gameTime, this.effect);
            base.Draw(gameTime);
        }

        //4. Set W, V, P, apply W, V, P and call a draw on the primitives in the IVertexType array
        private void DrawVertices(GameTime gameTime, BasicEffect effect)
        {
            //Specify where the object is (World) and where the camera is (V, P)
            effect.World = Matrix.Identity;
            effect.View = this.camera3D.View;
            effect.Projection = this.camera3D.Projection;

            //Call Apply() to say to the GFX card - "use these W, V, P variable values"
            effect.CurrentTechnique.Passes[0].Apply();

            //this.vertexData.Draw(gameTime, this.effect);

            //this.vertexDataPyramid.Draw(gameTime, effect);

            this.vertexDataCircle.Draw(gameTime, effect);

            //Here we are actually passing the vertices to GPU VRAM
            //graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //    PrimitiveType.LineList,
            //    this.vertices,
            //    0,
            //    2
            //);
        }
    }
}
