using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GDApp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private ObjectManager object3DManager;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private Integer2 resolution;
        private Integer2 screenCentre;
        private InputManagerParameters inputManagerParameters;
        private CameraManager cameraManager;
        private ContentDictionary<Model> modelDictionary;
        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SpriteFont> fontDictionary;
        private Dictionary<string, RailParameters> railDictionary;
        private Dictionary<string, Track3D> track3DDictionary;
        private Dictionary<string, EffectParameters> effectDictionary;
        private Dictionary<string, IVertexData> vertexDictionary;
        private Dictionary<string, DrawnActor3D> objectArchetypeDictionary;
        private EventDispatcher eventDispatcher;
        private PickingManager pickingManager;
        private SoundManager soundManager;
        private MyMenuManager menuManager;

        private CameraLayoutType cameraLayoutType;
        private ScreenLayoutType screenLayoutType;
        private UIManager uiManager;
        private PlayerCollidablePrimitiveObject drivableModelObject;
        #endregion

        #region Constructors
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialization
        protected override void Initialize()
        {
            //set the title
            Window.Title = "Tomas Game";

            //note - consider what settings CameraLayoutType.Single and ScreenLayoutType to > 1 means.
            //set camera layout - single or multi
            this.cameraLayoutType = CameraLayoutType.Multi;
            //set screen layout
            this.screenLayoutType = ScreenLayoutType.ThirdPerson;

            #region Assets & Dictionaries
            InitializeDictionaries();
            #endregion

            #region Graphics Related
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.resolution = ScreenUtility.XVGA;
            this.screenCentre = this.resolution / 2;
            InitializeGraphics();
            InitializeEffects();
            #endregion

            #region Event Handling
            //add the component to handle all system events
            this.eventDispatcher = new EventDispatcher(this, 20);
            Components.Add(this.eventDispatcher);
            #endregion

            #region Assets
            LoadAssets();
            #endregion

            #region Initialize Managers
            InitializeManagers();
            #endregion

            #region Load Game
            //load game happens before cameras are loaded because we may add a third person camera that needs a reference to a loaded Actor
            int worldScale = 1250;
            int gameLevel = 1;
            LoadGame(worldScale, gameLevel);
            #endregion

            #region Cameras
            InitializeCameras();
            #endregion

            #region Menu & UI
            InitializeMenu();
            //since debug needs sprite batch then call here
            InitializeUI();
            #endregion

#if DEBUG
            InitializeDebug(true);
            bool bShowCDCRSurfaces = true;
            bool bShowZones = true;
            InitializeDebugCollisionSkinInfo(bShowCDCRSurfaces, bShowZones);
#endif

            //Publish Start Event(s)
            StartGame();

            base.Initialize();
        }

        private void StartGame()
        {
            //will be received by the menu manager and screen manager and set the menu to be shown and game to be paused
            EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));

            //publish an event to set the camera
            object[] additionalEventParamsB = { AppData.CameraIDCollidableFirstPerson };
            EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive, EventCategoryType.Camera, additionalEventParamsB));            
            //or we could also just use the line below, but why not use our event dispatcher?
            //this.cameraManager.SetActiveCamera(x => x.ID.Equals("collidable first person camera 1"));
        }

        private void InitializeManagers()
        {
            //Keyboard
            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);

            //Mouse
            bool bMouseVisible = true;
            this.mouseManager = new MouseManager(this, bMouseVisible);
            this.mouseManager.SetPosition(this.screenCentre);
            Components.Add(this.mouseManager);

            //bundle together for easy passing
            this.inputManagerParameters = new InputManagerParameters(this.mouseManager, this.keyboardManager);

            //this is a list that updates all cameras
            this.cameraManager = new CameraManager(this, 5, this.eventDispatcher, StatusType.Off);
            Components.Add(this.cameraManager);

            //Object3D
            this.object3DManager = new ObjectManager(this, this.cameraManager,
                this.eventDispatcher, StatusType.Off, this.cameraLayoutType);
            this.object3DManager.DrawOrder = 1;
            Components.Add(this.object3DManager);

           
            //Sound
            this.soundManager = new SoundManager(this, this.eventDispatcher, StatusType.Update, "Content/Assets/Audio/", "Demo2DSound.xgs", "WaveBank1.xwb", "SoundBank1.xsb");
            Components.Add(this.soundManager);

            //Menu
            this.menuManager = new MyMenuManager(this, this.inputManagerParameters,
                this.cameraManager, this.spriteBatch, this.eventDispatcher,
                StatusType.Drawn | StatusType.Update);
            this.menuManager.DrawOrder = 2;
            Components.Add(this.menuManager);

            //ui (e.g. reticule, inventory, progress)
            this.uiManager = new UIManager(this, this.spriteBatch, this.eventDispatcher, 10, StatusType.Off);
            this.uiManager.DrawOrder = 3;
            Components.Add(this.uiManager);

            //picking
            this.pickingManager = new PickingManager(this, this.eventDispatcher, StatusType.Update,
               this.inputManagerParameters, this.cameraManager, this.object3DManager, PickingBehaviourType.PickAndRemove);
            Components.Add(this.pickingManager);
        }

        private void InitializeUI()
        {
            InitializeUIMouse();
            InitializeUIProgress();
        }

        private void InitializeUIProgress()
        {
            float separation = 20; //spacing between progress bars

            Transform2D transform = null;
            Texture2D texture = null;
            UITextureObject textureObject = null;
            Vector2 position = Vector2.Zero;
            Vector2 scale = Vector2.Zero;
            float verticalOffset = 20;
            int startValue;

            texture = this.textureDictionary["progress_gradient"];
            scale = new Vector2(1, 0.75f);

            #region Player 1 Progress Bar
            position = new Vector2(graphics.PreferredBackBufferWidth / 2.0f - texture.Width * scale.X - separation, verticalOffset);
            transform = new Transform2D(position, 0, scale,
                Vector2.Zero, /*new Vector2(texture.Width/2.0f, texture.Height/2.0f),*/
                new Integer2(texture.Width, texture.Height));

            textureObject = new UITextureObject(AppData.PlayerOneProgressID,
                    ActorType.UITexture,
                    StatusType.Drawn | StatusType.Update,
                    transform, Color.Green,
                    SpriteEffects.None,
                    1,
                    texture);

            //add a controller which listens for pickupeventdata send when the player (or red box) collects the box on the left
            startValue = 0; //just a random number between 0 and max to demonstrate we can set initial progress value
            textureObject.AttachController(
                new UIProgressController(AppData.PlayerOneProgressControllerID, 
                ControllerType.UIProgress, startValue, 10, this.eventDispatcher));

            textureObject.AttachController(
                new UIProgressIncrementController("bla",
                ControllerType.UIProgressIncrement,
                PlayStatusType.Play,
                AppData.PlayerOneProgressControllerID, //send an event to this controller ID
                1000, //1 sec between update
                1)); //add 1 every 1 sec

            this.uiManager.Add(textureObject);
            #endregion


            #region Player 2 Progress Bar
            position = new Vector2(graphics.PreferredBackBufferWidth / 2.0f + separation, verticalOffset);
            transform = new Transform2D(position, 0, scale, Vector2.Zero, new Integer2(texture.Width, texture.Height));

            textureObject = new UITextureObject(AppData.PlayerTwoProgressID,
                    ActorType.UITexture,
                    StatusType.Drawn | StatusType.Update,
                    transform,
                    Color.Red,
                    SpriteEffects.None,
                    1,
                    texture);

            //add a controller which listens for pickupeventdata send when the player (or red box) collects the box on the left
            startValue = 4; //just a random number between 0 and max to demonstrate we can set initial progress value
            textureObject.AttachController(new UIProgressController(AppData.PlayerTwoProgressControllerID, ControllerType.UIProgress, startValue, 10, this.eventDispatcher));
            this.uiManager.Add(textureObject);
            #endregion
        }

        private void InitializeUIMouse()
        {
            Texture2D texture = this.textureDictionary["reticuleDefault"];
            //show complete texture
            Microsoft.Xna.Framework.Rectangle sourceRectangle 
       = new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height);

            //listens for object picking events from the object picking manager
            UIPickingMouseObject myUIMouseObject = 
                new UIPickingMouseObject("picking mouseObject",
                ActorType.UITexture,
                new Transform2D(Vector2.One),
                this.fontDictionary["mouse"],
                "",
                new Vector2(0, 40),
                texture,
                this.mouseManager,
                this.eventDispatcher);
                this.uiManager.Add(myUIMouseObject);
        }
        #endregion

        #region Load Game Content
        //load the contents for the level specified
        private void LoadGame(int worldScale, int gameLevel)
        {
            //remove anything from the last time LoadGame() may have been called
            this.object3DManager.Clear();
      
            if (gameLevel == 1)
            {
                //non-collidable
                InitializeSkyBox(worldScale);
                //InitializeNonCollidableGround(worldScale);
                //InitializeNonCollidableProps();

                //collidable
                InitializeCollidableProps();
                //collidable and drivable player
                InitializeCollidablePlayer();
                //demo of loading from a level image
                //LoadObjectsFromImageFile("level1", 2, 2, 2.5f, new Vector3(-100, 0, 0));
                InitializeLevelOnePath();
                InitializeLevelOneWalls();
                //collidable zones
                //InitializeCollidableZones();

            }
            else if (gameLevel == 2)
            {
                //add different things for your next level
            }
        }

        #region Non-Collidable Primitive Objects
        private void InitializeSkyBox(int worldScale)
        {
            PrimitiveObject archTexturedPrimitiveObject = null, cloneTexturedPrimitiveObject = null;

            #region Archetype
            //we need to do an "as" typecast since the dictionary holds DrawnActor3D types
            archTexturedPrimitiveObject = this.objectArchetypeDictionary[AppData.UnlitTexturedQuadArchetypeID] as PrimitiveObject;
            archTexturedPrimitiveObject.Transform.Scale *= worldScale;
            #endregion
            //demonstrates how we can simply clone an archetypal primitive object and re-use by re-cloning
            #region Skybox
            //back
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.ID = "skybox_back";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_back"];
            this.object3DManager.Add(cloneTexturedPrimitiveObject);

            //left
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.ID = "skybox_left";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(0, 90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_left"];
            this.object3DManager.Add(cloneTexturedPrimitiveObject);

            //right
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.ID = "skybox_right";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(0, -90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_right"];
            this.object3DManager.Add(cloneTexturedPrimitiveObject);

            //front
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.ID = "skybox_front";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, 0, worldScale / 2.0f);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(0, 180, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_front"];
            this.object3DManager.Add(cloneTexturedPrimitiveObject);

            //top
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.ID = "skybox_sky";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, worldScale / 2.0f, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(90, -90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_sky"];
            this.object3DManager.Add(cloneTexturedPrimitiveObject);
            #endregion
        }

        private void InitializeNonCollidableGround(int worldScale)
        {
            Transform3D transform = new Transform3D(new Vector3(0, 0, 0), new Vector3(-90, 0, 0), worldScale * Vector3.One,
              Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["grass1"];

            PrimitiveObject primitiveObject = new PrimitiveObject("ground", ActorType.Helper,
                    transform,
                    effectParameters,
                    StatusType.Drawn | StatusType.Update,
                    this.vertexDictionary[AppData.UnlitTexturedQuadVertexDataID]);

            this.object3DManager.Add(primitiveObject);
        }

        private void InitializeLevelOnePath()
        {
            CollidablePrimitiveObject collidablePrimitiveObject = null;
            EffectParameters effectParameters = this.effectDictionary[AppData.LitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["LightBlue"];
            Transform3D transform = new Transform3D(new Vector3(10, -3, 40), new Vector3(50, 8, 40));
            BoxCollisionPrimitive collisionPrimitive = new BoxCollisionPrimitive();

            //Ground
            collidablePrimitiveObject = new CollidablePrimitiveObject("collidable lit cube ",
                    //this is important as it will determine how we filter collisions in our collidable player CDCR code
                    ActorType.CollidableGround,
                    transform,
                    effectParameters,
                    StatusType.Drawn | StatusType.Update,
                    this.vertexDictionary[AppData.LitCube],
                    collisionPrimitive, this.object3DManager);

            this.object3DManager.Add(collidablePrimitiveObject);

           


        }

        private void InitializeLevelOneWalls()
        {
            #region Common Attributes
            CollidablePrimitiveObject collidablePrimitiveObject = null,clonedItem = null;
            EffectParameters effectParameters = this.effectDictionary[AppData.LitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["White"];
            BoxCollisionPrimitive collisionPrimitive = null;
            Transform3D transform = null;
            #endregion
            #region Back Wall
            collisionPrimitive = new BoxCollisionPrimitive();
            transform = new Transform3D(new Vector3(-12.5f, 2, 40), new Vector3(5, 2, 30));
            collidablePrimitiveObject = collidablePrimitiveObject = new CollidablePrimitiveObject("Wall-1 ",
            ActorType.CollidableWall,
            transform,
            effectParameters,
            StatusType.Drawn | StatusType.Update,
            this.vertexDictionary[AppData.LitCube],
            collisionPrimitive, this.object3DManager);

            this.object3DManager.Add(collidablePrimitiveObject);
            #endregion

            #region SideWalls
            collisionPrimitive = new BoxCollisionPrimitive();
            transform = new Transform3D(new Vector3(10, 2, 22.5f), new Vector3(50, 2, 5));

            collidablePrimitiveObject = collidablePrimitiveObject = new CollidablePrimitiveObject("Wall-2 ",
            ActorType.CollidableWall,
            transform,
            effectParameters,
            StatusType.Drawn | StatusType.Update,
            this.vertexDictionary[AppData.LitCube],
            collisionPrimitive, this.object3DManager);
            this.object3DManager.Add(collidablePrimitiveObject);


            clonedItem = collidablePrimitiveObject.Clone() as CollidablePrimitiveObject;
            clonedItem.Transform.Translation += new Vector3(0,0,35);
            this.object3DManager.Add(clonedItem);

            #endregion
        }
        private void InitializeNonCollidableProps()
        {
            PrimitiveObject primitiveObject = null;
            Transform3D transform = null;

            #region Add wireframe origin helper
            transform = new Transform3D(new Vector3(0, 10, 0), Vector3.Zero, 10 * Vector3.One,
                Vector3.UnitZ, Vector3.UnitY);

            primitiveObject = new PrimitiveObject("origin1", ActorType.Helper,
                    transform,
                    this.effectDictionary[AppData.UnlitWireframeEffectID].Clone() as EffectParameters,
                    StatusType.Drawn | StatusType.Update,
                    this.vertexDictionary[AppData.WireframeOriginHelperVertexDataID]);

            this.object3DManager.Add(primitiveObject);
            #endregion

            #region Add Coloured Triangles
            //wireframe triangle
            transform = new Transform3D(new Vector3(20, 10, -10), Vector3.Zero, 4 * new Vector3(2, 3, 1),
                    Vector3.UnitZ, Vector3.UnitY);

            primitiveObject = new PrimitiveObject("triangle1", ActorType.Decorator,
                    transform,
                    this.effectDictionary[AppData.UnlitWireframeEffectID].Clone() as EffectParameters,
                    StatusType.Drawn | StatusType.Update,
                    this.vertexDictionary[AppData.WireframeTriangleVertexDataID]);

            primitiveObject.AttachController(new RotationController("rotControl1", ControllerType.Rotation,
                            0.1875f * Vector3.UnitY));

            this.object3DManager.Add(primitiveObject);


            //set transform
            transform = new Transform3D(new Vector3(0, 20, 0), new Vector3(1, 8, 1));

            //make the triangle object
            primitiveObject = new PrimitiveObject("1st triangle", ActorType.Decorator,
                transform,
                //notice we use the right effect for the type e.g. wireframe, textures, lit textured
                this.effectDictionary[AppData.UnlitWireframeEffectID].Clone() as EffectParameters,
                //if an object doesnt need to be updated i.e. no controller then we dont need to update!
                StatusType.Drawn,
                //get the vertex data from the dictionary 
                this.vertexDictionary[AppData.WireframeTriangleVertexDataID]);

            //change some properties - because we can!
            primitiveObject.EffectParameters.Alpha = 0.25f;
            //add
            this.object3DManager.Add(primitiveObject);
            #endregion

            #region Add Textured Quads
            for (int i = 5; i <= 25; i+= 5)
            {
                //set transform
                transform = new Transform3D(new Vector3(-10, i, 0), 3 * Vector3.One);
                primitiveObject = new PrimitiveObject("tex quad ", ActorType.Decorator,
                   transform,
                   //notice we use the right effect for the type e.g. wireframe, textures, lit textured
                   this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters,
                   //if an object doesnt need to be updated i.e. no controller then we dont need to update!
                   StatusType.Drawn,
                   //get the vertex data from the dictionary 
                   this.vertexDictionary[AppData.UnlitTexturedQuadVertexDataID]);

                //change some properties - because we can!
                primitiveObject.EffectParameters.DiffuseColor = Color.Pink;
                primitiveObject.EffectParameters.Alpha = 0.5f;
                primitiveObject.EffectParameters.Texture = this.textureDictionary["ml"];
                this.object3DManager.Add(primitiveObject);
            }
            #endregion

            #region Add Circle
            transform = new Transform3D(new Vector3(-20, 15, 0), new Vector3(2, 4, 1));

            primitiveObject = new PrimitiveObject("1st circle", ActorType.Decorator,
                    transform,
                    //notice we use the right effect for the type e.g. wireframe, textures, lit textured
                   this.effectDictionary[AppData.UnlitWireframeEffectID].Clone() as EffectParameters,
                   //set Update becuase we are going to add a controller!
                   StatusType.Drawn | StatusType.Update,
                   //get the vertex data from the dictionary 
                   this.vertexDictionary[AppData.WireframeCircleVertexDataID]);

            //why not add a controller!?
            primitiveObject.AttachController(new RotationController("rotControl1", ControllerType.Rotation,
                            0.1875f * new Vector3(1, 0, 0)));

            this.object3DManager.Add(primitiveObject);
            #endregion
        }

        private void LoadObjectsFromImageFile(string fileName, float scaleX, float scaleZ, float height, Vector3 offset)
        {
            LevelLoader levelLoader = new LevelLoader(this.objectArchetypeDictionary, this.textureDictionary);
            List<DrawnActor3D> actorList = levelLoader.Load(this.textureDictionary[fileName],
               scaleX, scaleZ, height, offset);

            this.object3DManager.Add(actorList);
        }
        #endregion

        #region Collidable Primitive Objects
        private void InitializeCollidableProps()
        {
            CollidablePrimitiveObject texturedPrimitiveObject = null;
            Transform3D transform = null;

           


            for (int i = 1; i < 10; i++)
            {
                transform = new Transform3D(new Vector3(i * 10 + 10, 4 /*i.e. half the scale of 8*/, 10), new Vector3(6, 8, 6));

                //a unique copy of the effect for each box in case we want different color, texture, alpha
                EffectParameters effectParameters = this.effectDictionary[AppData.UnlitWireframeEffectID].Clone() as EffectParameters;
                //effectParameters.Texture = this.textureDictionary["crate1"];
                effectParameters.DiffuseColor = Color.White;
                effectParameters.Alpha = 1;

                //make the collision primitive - changed slightly to no longer need transform
                BoxCollisionPrimitive collisionPrimitive = new BoxCollisionPrimitive();

                //make a collidable object and pass in the primitive
                texturedPrimitiveObject = new CollidablePrimitiveObject("collidable lit cube " + i,
                    //this is important as it will determine how we filter collisions in our collidable player CDCR code
                    ActorType.CollidableArchitecture,
                    transform,
                    effectParameters,
                    StatusType.Drawn | StatusType.Update,
                    this.vertexDictionary[AppData.LitTexturedDiamondVertexDataID],
                    collisionPrimitive, this.object3DManager);

                if (i > 3) //attach controllers but not on all of the boxes
                {
                    //if we want to make the boxes move (or do something else) then just attach a controller
                    texturedPrimitiveObject.AttachController(new TranslationSineLerpController("transControl1", ControllerType.SineTranslation,
                        Vector3.UnitY, //displacement vector 
                        new TrigonometricParameters(20, //amplitude multipler on displacement 
                        0.1f,  //frequency of the sine curve
                        90 * i))); //notice how the phase offset of 90 degrees offsets each object's translation along the sine curve

                    texturedPrimitiveObject.AttachController(new ColorSineLerpController("colorControl1", ControllerType.SineColor,
                        Color.Red, Color.Green, new TrigonometricParameters(1, 0.1f)));
                }

                this.object3DManager.Add(texturedPrimitiveObject);
            }
        }

        //adds a drivable player that can collide against collidable objects and zones
        private void InitializeCollidablePlayer()
        {
            //set the position
            Transform3D transform = new Transform3D(new Vector3(-5, 2.8f, 40), Vector3.Zero, new Vector3(3, 3, 3), Vector3.UnitX, Vector3.UnitY);
 
            //load up the particular texture, color, alpha for the player
            EffectParameters effectParameters = this.effectDictionary[AppData.LitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["Purple"];

            //make a CDCR surface - sphere or box, its up to you - you dont need to pass transform to either primitive anymore
            ICollisionPrimitive collisionPrimitive = new BoxCollisionPrimitive();

            this.drivableModelObject
                = new PlayerCollidablePrimitiveObject("PLAYER",
                    //this is important as it will determine how we filter collisions in our collidable player CDCR code
                    ActorType.CollidablePlayer,
                    transform,
                    effectParameters,
                    StatusType.Drawn | StatusType.Update,
                    this.vertexDictionary[AppData.LitTexturedCubeVertexDataID],
                    collisionPrimitive, this.object3DManager,
                    AppData.PlayerOneMoveKeys, AppData.PlayerMoveSpeed, AppData.PlayerRotationSpeed,
                    this.keyboardManager);

            this.object3DManager.Add(this.drivableModelObject);

        }
        #endregion

        #region Collidable Zone Objects
        private void InitializeCollidableZones()
        {
            Transform3D transform = null;
            SimpleZoneObject simpleZoneObject = null;
            ICollisionPrimitive collisionPrimitive = null;

            transform = new Transform3D(new Vector3(-20, 8, 40), 8 * Vector3.One);

            //we can have a sphere or a box - its entirely up to the developer
            // collisionPrimitive = new SphereCollisionPrimitive(transform, 2);

            //make the collision primitive - changed slightly to no longer need transform
            collisionPrimitive = new BoxCollisionPrimitive();

            simpleZoneObject = new SimpleZoneObject("camera trigger zone 1", ActorType.CollidableZone, transform,
                StatusType.Drawn | StatusType.Update, collisionPrimitive);

            this.object3DManager.Add(simpleZoneObject);

        }
        #endregion
        #endregion

        private void InitializeMenu()
        {
            Transform2D transform = null;
            Texture2D texture = null;
            Vector2 position = Vector2.Zero;
            UIButtonObject uiButtonObject = null, clone = null;
            string sceneID = "", buttonID = "", buttonText = "";
            int verticalBtnSeparation = 50;

            #region Main Menu
            sceneID = AppData.MenuMainID;

            //retrieve the background texture
            texture = this.textureDictionary["mainmenu"];
            //scale the texture to fit the entire screen
            Vector2 scale = new Vector2((float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height);
            transform = new Transform2D(scale);

            this.menuManager.Add(sceneID, new UITextureObject("mainmenuTexture", ActorType.UITexture,
                StatusType.Drawn, //notice we dont need to update a static texture
                transform, Color.White, SpriteEffects.None,
                1, //depth is 1 so its always sorted to the back of other menu elements
                texture));

            //add start button
            buttonID = "startbtn";
            buttonText = "Start";
            position = new Vector2(graphics.PreferredBackBufferWidth / 2.0f, 200);
            texture = this.textureDictionary["genericbtn"];
            transform = new Transform2D(position,
                0, new Vector2(1.8f, 0.6f),
                new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), new Integer2(texture.Width, texture.Height));

            uiButtonObject = new UIButtonObject(buttonID, ActorType.UIButton, StatusType.Update | StatusType.Drawn,
                transform, Color.LightPink, SpriteEffects.None, 0.1f, texture, buttonText,
                this.fontDictionary["menu"],
                Color.DarkGray, new Vector2(0, 2));
            this.menuManager.Add(sceneID, uiButtonObject);

            //add audio button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "audiobtn";
            clone.Text = "Audio";
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, verticalBtnSeparation);
            //change the texture blend color
            clone.Color = Color.LightGreen;
            this.menuManager.Add(sceneID, clone);

            //add controls button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "controlsbtn";
            clone.Text = "Controls";
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);
            //change the texture blend color
            clone.Color = Color.LightBlue;
            this.menuManager.Add(sceneID, clone);

            //add exit button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "exitbtn";
            clone.Text = "Exit";
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);
            //change the texture blend color
            clone.Color = Color.LightYellow;
            //store the original color since if we modify with a controller and need to reset
            clone.OriginalColor = clone.Color;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Audio Menu
            sceneID = AppData.MenuAudioID;

            //retrieve the audio menu background texture
            texture = this.textureDictionary["audiomenu"];
            //scale the texture to fit the entire screen
            scale = new Vector2((float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height);
            transform = new Transform2D(scale);
            this.menuManager.Add(sceneID, new UITextureObject("audiomenuTexture", 
                ActorType.UITexture,
                StatusType.Drawn, //notice we dont need to update a static texture
                transform, Color.White, SpriteEffects.None,
                1, //depth is 1 so its always sorted to the back of other menu elements
                texture));


            //add volume up button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            clone.ID = "volumeUpbtn";
            clone.Text = "Volume Up";
            //change the texture blend color
            clone.Color = Color.LightPink;
            this.menuManager.Add(sceneID, clone);

            //add volume down button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, verticalBtnSeparation);
            clone.ID = "volumeDownbtn";
            clone.Text = "Volume Down";
            //change the texture blend color
            clone.Color = Color.LightGreen;
            this.menuManager.Add(sceneID, clone);

            //add volume mute button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);
            clone.ID = "volumeMutebtn";
            clone.Text = "Volume Mute";
            //change the texture blend color
            clone.Color = Color.LightBlue;
            this.menuManager.Add(sceneID, clone);

            //add volume mute button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);
            clone.ID = "volumeUnMutebtn";
            clone.Text = "Volume Un-mute";
            //change the texture blend color
            clone.Color = Color.LightSalmon;
            this.menuManager.Add(sceneID, clone);

            //add back button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 4 * verticalBtnSeparation);
            clone.ID = "backbtn";
            clone.Text = "Back";
            //change the texture blend color
            clone.Color = Color.LightYellow;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Controls Menu
            sceneID = AppData.MenuControlsID;

            //retrieve the controls menu background texture
            texture = this.textureDictionary["controlsmenu"];
            //scale the texture to fit the entire screen
            scale = new Vector2((float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height);
            transform = new Transform2D(scale);
            this.menuManager.Add(sceneID, new UITextureObject("controlsmenuTexture", ActorType.UITexture,
                StatusType.Drawn, //notice we dont need to update a static texture
                transform, Color.White, SpriteEffects.None,
                1, //depth is 1 so its always sorted to the back of other menu elements
                texture));

            //add back button - clone the audio button then just reset texture, ids etc in all the clones
            clone = (UIButtonObject)uiButtonObject.Clone();
            //move down on Y-axis for next button
            clone.Transform.Translation += new Vector2(0, 9 * verticalBtnSeparation);
            clone.ID = "backbtn";
            clone.Text = "Back";
            //change the texture blend color
            clone.Color = Color.LightYellow;
            this.menuManager.Add(sceneID, clone);
            #endregion
        }

        private void InitializeDictionaries()
        {
            //textures, models, fonts
            this.modelDictionary = new ContentDictionary<Model>("model dictionary", this.Content);
            this.textureDictionary = new ContentDictionary<Texture2D>("texture dictionary", this.Content);
            this.fontDictionary = new ContentDictionary<SpriteFont>("font dictionary", this.Content);

            //rail, transform3Dcurve               
            this.railDictionary = new Dictionary<string, RailParameters>();
            this.track3DDictionary = new Dictionary<string, Track3D>();

            //effect parameters
            this.effectDictionary = new Dictionary<string, EffectParameters>();

            //vertices
            this.vertexDictionary = new Dictionary<string, IVertexData>();

            //object archetypes that we can clone
            this.objectArchetypeDictionary = new Dictionary<string, DrawnActor3D>();
        }

#if DEBUG
        private void InitializeDebug(bool bEnabled)
        {
            if (bEnabled)
            {
                DebugDrawer debugDrawer = new DebugDrawer(this, this.cameraManager,
                    this.eventDispatcher,
                    StatusType.Off,
                    this.cameraLayoutType,
                    this.spriteBatch, this.fontDictionary["debugFont"], new Vector2(20, 20), Color.White);

                debugDrawer.DrawOrder = 4;
                Components.Add(debugDrawer);
            }
        }
        private void InitializeDebugCollisionSkinInfo(bool bShowCDCRSurfaces, bool bShowZones)
        {
            //draws CDCR surfaces for boxes and spheres
            PrimitiveDebugDrawer primitiveDebugDrawer = new PrimitiveDebugDrawer(this, bShowCDCRSurfaces, bShowZones, 
                this.cameraManager, this.object3DManager, this.eventDispatcher, StatusType.Drawn | StatusType.Update);
            primitiveDebugDrawer.DrawOrder = 5;
            Components.Add(primitiveDebugDrawer);

            //set color for the bounding boxes
            BoundingBoxDrawer.boundingBoxColor = Color.White;
        }
#endif

        #region Assets

        private void LoadAssets()
        {
            LoadTextures();
            LoadFonts();
            LoadRails();
            LoadTracks();

            LoadStandardVertices();
            LoadBillboardVertices();
            LoadArchetypePrimitivesToDictionary();
        }

        //uses BufferedVertexData to create a container for the vertices. you can also use plain VertexData but
        //it doesnt have the advantage of moving the vertices ONLY ONCE onto VRAM on the GFX card
        private void LoadStandardVertices()
        {
            PrimitiveType primitiveType;
            int primitiveCount;

            #region Factory Based Approach
            #region Textured Quad
            this.vertexDictionary.Add(AppData.UnlitTexturedQuadVertexDataID,
                new VertexData<VertexPositionColorTexture>(
                VertexFactory.GetTextureQuadVertices(out primitiveType, out primitiveCount),
                primitiveType, primitiveCount));
            #endregion

            #region Wireframe Circle
  
            this.vertexDictionary.Add(AppData.WireframeCircleVertexDataID, new BufferedVertexData<VertexPositionColor>(
            graphics.GraphicsDevice, VertexFactory.GetCircleVertices(2, 10, out primitiveType, out primitiveCount, OrientationType.XYAxis),
                PrimitiveType.LineStrip, primitiveCount));
            #endregion

            #region Lit Textured Cube
            this.vertexDictionary.Add(AppData.LitTexturedCubeVertexDataID, 
                new BufferedVertexData<VertexPositionNormalTexture>(graphics.GraphicsDevice, VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount),
               primitiveType, primitiveCount));

            this.vertexDictionary.Add(AppData.LitCube,
                new BufferedVertexData<VertexPositionNormalTexture>(graphics.GraphicsDevice, VertexFactory.GetVerticesPositionNormalTexturedCube(1, out primitiveType, out primitiveCount),
               primitiveType, primitiveCount));
            #endregion
            #endregion

            #region Old User Defines Vertices Approach
            VertexPositionColor[] verticesPositionColor = null;

            #region Textured Cube
            this.vertexDictionary.Add(AppData.UnlitTexturedCubeVertexDataID,
                new BufferedVertexData<VertexPositionColorTexture>(graphics.GraphicsDevice, VertexFactory.GetVerticesPositionTexturedCube(1, out primitiveType, out primitiveCount),
                primitiveType, primitiveCount));
            #endregion

            #region Wireframe Origin Helper
            verticesPositionColor = new VertexPositionColor[20];

            //x-axis
            verticesPositionColor[0] = new VertexPositionColor(-Vector3.UnitX, Color.DarkRed);
            verticesPositionColor[1] = new VertexPositionColor(Vector3.UnitX, Color.DarkRed);

            //y-axis
            verticesPositionColor[2] = new VertexPositionColor(-Vector3.UnitY, Color.DarkGreen);
            verticesPositionColor[3] = new VertexPositionColor(Vector3.UnitY, Color.DarkGreen);

            //z-axis
            verticesPositionColor[4] = new VertexPositionColor(-Vector3.UnitZ, Color.DarkBlue);
            verticesPositionColor[5] = new VertexPositionColor(Vector3.UnitZ, Color.DarkBlue);

            //to do - x-text , y-text, z-text
            //x label
            verticesPositionColor[6] = new VertexPositionColor(new Vector3(1.1f, 0.1f, 0), Color.DarkRed);
            verticesPositionColor[7] = new VertexPositionColor(new Vector3(1.3f, -0.1f, 0), Color.DarkRed);
            verticesPositionColor[8] = new VertexPositionColor(new Vector3(1.3f, 0.1f, 0), Color.DarkRed);
            verticesPositionColor[9] = new VertexPositionColor(new Vector3(1.1f, -0.1f, 0), Color.DarkRed);


            //y label
            verticesPositionColor[10] = new VertexPositionColor(new Vector3(-0.1f, 1.3f, 0), Color.DarkGreen);
            verticesPositionColor[11] = new VertexPositionColor(new Vector3(0, 1.2f, 0), Color.DarkGreen);
            verticesPositionColor[12] = new VertexPositionColor(new Vector3(0.1f, 1.3f, 0), Color.DarkGreen);
            verticesPositionColor[13] = new VertexPositionColor(new Vector3(-0.1f, 1.1f, 0), Color.DarkGreen);

            //z label
            verticesPositionColor[14] = new VertexPositionColor(new Vector3(0, 0.1f, 1.1f), Color.DarkBlue);
            verticesPositionColor[15] = new VertexPositionColor(new Vector3(0, 0.1f, 1.3f), Color.DarkBlue);
            verticesPositionColor[16] = new VertexPositionColor(new Vector3(0, 0.1f, 1.1f), Color.DarkBlue);
            verticesPositionColor[17] = new VertexPositionColor(new Vector3(0, -0.1f, 1.3f), Color.DarkBlue);
            verticesPositionColor[18] = new VertexPositionColor(new Vector3(0, -0.1f, 1.3f), Color.DarkBlue);
            verticesPositionColor[19] = new VertexPositionColor(new Vector3(0, -0.1f, 1.1f), Color.DarkBlue);

            this.vertexDictionary.Add(AppData.WireframeOriginHelperVertexDataID, new BufferedVertexData<VertexPositionColor>(graphics.GraphicsDevice, verticesPositionColor, Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList, 10));
            #endregion

            #region Wireframe Triangle
            verticesPositionColor = new VertexPositionColor[3];
            verticesPositionColor[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            verticesPositionColor[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.Green);
            verticesPositionColor[2] = new VertexPositionColor(new Vector3(-1, 0, 0), Color.Blue);
            this.vertexDictionary.Add(AppData.WireframeTriangleVertexDataID, new VertexData<VertexPositionColor>(verticesPositionColor, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 1));
            #endregion

            #region DIAMOND
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 8;
            short[] indices = new short[24];
            VertexPositionColor[] vertices = new VertexPositionColor[6];

            //Square
            vertices[0] = new VertexPositionColor(new Vector3(-1, 0, 1), Color.White);
            vertices[1] = new VertexPositionColor(new Vector3(-1, 0, -1), Color.Yellow);
            vertices[2] = new VertexPositionColor(new Vector3(1, 0, -1), Color.Green);
            vertices[3] = new VertexPositionColor(new Vector3(1, 0, 1), Color.Purple);

            //Top
            vertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
            //Bottom
            vertices[5] = new VertexPositionColor(new Vector3(0, -1, 0), Color.Red);

            //Used to turn the vertices into the diffrent triangles
            //first triangle on left
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 4;

            //2nd triangle on the right
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 4;

            //3nd triangle on the right
            indices[6] = 2;
            indices[7] = 3;
            indices[8] = 4;

            //4th triangle on the center right
            indices[9] = 3;
            indices[10] = 0;
            indices[11] = 4;

            indices[12] = 0;
            indices[13] = 5;
            indices[14] = 1;

            //2nd triangle on the right
            indices[15] = 1;
            indices[16] = 5;
            indices[17] = 3;

            //3nd triangle on the right
            indices[18] = 2;
            indices[19] = 5;
            indices[20] = 3;

            //4th triangle on the center right
            indices[21] = 3;
            indices[22] = 5;
            indices[23] = 0;

            BufferedIndexedVertexData<VertexPositionColor> vertexData =
                new BufferedIndexedVertexData<VertexPositionColor>(graphics.GraphicsDevice, vertices,
                indices, primitiveType, primitiveCount);

            this.vertexDictionary.Add(AppData.LitTexturedDiamondVertexDataID, vertexData);
            #endregion

            #endregion
        }

        private void LoadArchetypePrimitivesToDictionary()
        {
            Transform3D transform = null;
            PrimitiveObject primitiveObject = null;
            EffectParameters effectParameters = null;

            #region Textured Quad Archetype
            //remember we clone because each cube MAY need separate texture, alpha and diffuse color
            effectParameters = this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["white"];
            effectParameters.DiffuseColor = Color.White;
            effectParameters.Alpha = 1;

            transform = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);
            primitiveObject = new PrimitiveObject(AppData.UnlitTexturedQuadArchetypeID, ActorType.Decorator,
                     transform, 
                     effectParameters, 
                     StatusType.Drawn | StatusType.Update,
                     this.vertexDictionary[AppData.UnlitTexturedQuadVertexDataID]); //or  we can leave texture null since we will replace it later

            this.objectArchetypeDictionary.Add(AppData.UnlitTexturedQuadArchetypeID, primitiveObject);
            #endregion

            #region Unlit Collidable Cube

            //remember we clone because each cube MAY need separate texture, alpha and diffuse color
            effectParameters = this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["white"];
            effectParameters.DiffuseColor = Color.White;
            effectParameters.Alpha = 1;

            transform = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            //make the collision primitive - changed slightly to no longer need transform
            BoxCollisionPrimitive collisionPrimitive = new BoxCollisionPrimitive();

            //make a collidable object and pass in the primitive
            primitiveObject = new CollidablePrimitiveObject("collidable unlit cube",
                //this is important as it will determine how we filter collisions in our collidable player CDCR code
                ActorType.CollidablePickup,
                transform,
                effectParameters,
                StatusType.Drawn | StatusType.Update,
                this.vertexDictionary[AppData.UnlitTexturedCubeVertexDataID],
                collisionPrimitive, this.object3DManager);

            this.objectArchetypeDictionary.Add(AppData.UnlitTexturedCubeArchetypeID, primitiveObject);
            #endregion

            //add all the primitive archetypes that your game needs here, then you can just fetch and clone later e.g. in the LevelLoader

        }

        private void LoadBillboardVertices()
        {
            PrimitiveType primitiveType;
            int primitiveCount;
            IVertexData vertexData = null;

            #region Billboard Quad - we must use this type when creating billboards
            // get vertices for textured billboard
            VertexBillboard[] verticesBillboard = VertexFactory.GetVertexBillboard(1, out primitiveType, out primitiveCount);

            //make a vertex data object to store and draw the vertices
            vertexData = new BufferedVertexData<VertexBillboard>(this.graphics.GraphicsDevice, verticesBillboard, primitiveType, primitiveCount);

            //add to the dictionary for use by things like billboards - see InitializeBillboards()
            this.vertexDictionary.Add(AppData.UnlitTexturedBillboardVertexDataID, vertexData);
            #endregion
        }

        private void LoadFonts()
        {
            this.fontDictionary.Load("hudFont", "Assets/Fonts/hudFont");
            this.fontDictionary.Load("menu", "Assets/Fonts/menu");
            this.fontDictionary.Load("Assets/Fonts/mouse");
#if DEBUG
            this.fontDictionary.Load("debugFont", "Assets/Debug/Fonts/debugFont");
#endif
        }

        private void LoadTextures()
        {
            //used for archetypes
            this.textureDictionary.Load("Assets/Textures/white");

            //animated
            this.textureDictionary.Load("Assets/Textures/Animated/alarm");

            //ui
            this.textureDictionary.Load("Assets/Textures/UI/HUD/reticuleDefault");

            //environment
            this.textureDictionary.Load("Assets/Textures/Props/Crates/crate1"); //demo use of the shorter form of Load() that generates key from asset name
            this.textureDictionary.Load("Assets/Textures/Props/Crates/crate2");
            this.textureDictionary.Load("Assets/Textures/Foliage/Ground/grass1");
            this.textureDictionary.Load("skybox_back", "Assets/Textures/Skybox/back");
            this.textureDictionary.Load("skybox_left", "Assets/Textures/Skybox/left");
            this.textureDictionary.Load("skybox_right", "Assets/Textures/Skybox/right");
            this.textureDictionary.Load("skybox_sky", "Assets/Textures/Skybox/sky");
            this.textureDictionary.Load("skybox_front", "Assets/Textures/Skybox/front");
            this.textureDictionary.Load("Assets/Textures/Foliage/Trees/tree2");

            //dual texture demo
            //this.textureDictionary.Load("Assets/Textures/Foliage/Ground/grass_midlevel");
            //this.textureDictionary.Load("Assets/Textures/Foliage/Ground/grass_highlevel");

            //menu - buttons
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Buttons/genericbtn");

            //menu - backgrounds
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/mainmenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/audiomenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/controlsmenu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/exitmenuwithtrans");

            //ui (or hud) elements
            this.textureDictionary.Load("Assets/Textures/UI/HUD/reticuleDefault");
            this.textureDictionary.Load("Assets/Textures/UI/HUD/progress_gradient");

            //architecture
            this.textureDictionary.Load("Assets/Textures/Architecture/Buildings/house-low-texture");
            this.textureDictionary.Load("Assets/Textures/Architecture/Walls/wall");

            //dual texture demo - see Main::InitializeCollidableGround()
            this.textureDictionary.Load("Assets/Debug/Textures/checkerboard_greywhite");

            //debug
            this.textureDictionary.Load("Assets/Debug/Textures/checkerboard");
            this.textureDictionary.Load("Assets/Debug/Textures/ml");
            this.textureDictionary.Load("Assets/Debug/Textures/checkerboard");


            //Colors
            this.textureDictionary.Load("Assets/Textures/Colors/Blue");
            this.textureDictionary.Load("Assets/Textures/Colors/Purple");
            this.textureDictionary.Load("Assets/Textures/Colors/White");
            this.textureDictionary.Load("Assets/Textures/Colors/LightBlue");

            #region billboards
            this.textureDictionary.Load("Assets/Textures/Billboards/billboardtexture");
            this.textureDictionary.Load("Assets/Textures/Billboards/snow1");
            this.textureDictionary.Load("Assets/Textures/Billboards/chevron1");
            this.textureDictionary.Load("Assets/Textures/Billboards/chevron2");
            this.textureDictionary.Load("Assets/Textures/Billboards/alarm1");
            this.textureDictionary.Load("Assets/Textures/Billboards/alarm2");
            this.textureDictionary.Load("Assets/Textures/Props/tv");
            #endregion

            #region Levels
            this.textureDictionary.Load("Assets/Textures/Level/level1");

        //    this.textureDictionary.Load("level1", "Assets/Textures/Level/level_test");
            #endregion


        }

        private void LoadRails()
        {
            RailParameters railParameters = null;

            //create a simple rail that gains height as the target moves on +ve X-axis - try different rail vectors
            railParameters = new RailParameters("battlefield 1", new Vector3(0, 10, 80), new Vector3(50, 50, 80));
            this.railDictionary.Add(railParameters.ID, railParameters);

            //add more rails here...
            railParameters = new RailParameters("battlefield 2", new Vector3(-50, 20, 40), new Vector3(50, 80, 100));
            this.railDictionary.Add(railParameters.ID, railParameters);
        }

        private void LoadTracks()
        {
            Track3D track3D = null;

            //starts away from origin, moves forward and rises, then ends closer to origin and looking down from a height
            track3D = new Track3D(CurveLoopType.Oscillate);
            track3D.Add(new Vector3(0, 10, 200), -Vector3.UnitZ, Vector3.UnitY, 0);
            track3D.Add(new Vector3(0, 20, 150), -Vector3.UnitZ, Vector3.UnitY, 2);
            track3D.Add(new Vector3(0, 40, 100), -Vector3.UnitZ, Vector3.UnitY, 4);

            //set so that the camera looks down at the origin at the end of the curve
            Vector3 finalPosition = new Vector3(0, 80, 50);
            Vector3 finalLook = Vector3.Normalize(Vector3.Zero - finalPosition);

            track3D.Add(finalPosition, finalLook, Vector3.UnitY, 6);
            this.track3DDictionary.Add("push forward 1", track3D);

            //add more transform3D curves here...
        }

        #endregion

        #region Graphics & Effects
        private void InitializeEffects()
        {
            BasicEffect basicEffect = null;
            EffectParameters effectParameters = null;

            //used for UNLIT wireframe primitives
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.UnlitWireframeEffectID, effectParameters);

            //used for UNLIT textured solid primitives
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;
            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.UnlitTexturedEffectID, effectParameters);

            //used for LIT textured solid primitives
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.TextureEnabled = true;
            basicEffect.LightingEnabled = true;
            basicEffect.EnableDefaultLighting();
            basicEffect.PreferPerPixelLighting = true;
            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.LitTexturedEffectID, effectParameters);

            //used for UNLIT billboards i.e. cylindrical, spherical, normal, animated, scrolling
            Effect billboardEffect = Content.Load<Effect>("Assets/Effects/Billboard");
            effectParameters = new EffectParameters(billboardEffect);
            this.effectDictionary.Add(AppData.UnlitBillboardsEffectID, effectParameters);

        }
        private void InitializeGraphics()
        {
            this.graphics.PreferredBackBufferWidth = resolution.X;
            this.graphics.PreferredBackBufferHeight = resolution.Y;

            //solves the skybox border problem
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            this.graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            //enable alpha transparency - see ColorParameters
            this.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.graphics.ApplyChanges();
        }
        #endregion

        #region Cameras
        private void InitializeCameras()
        {
            Viewport viewport = new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            float aspectRatio = (float)this.resolution.X / this.resolution.Y;
            ProjectionParameters projectionParameters 
                = new ProjectionParameters(MathHelper.PiOver4, aspectRatio, 1, 4000);

            AddThirdPersonCamera(AppData.CameraIDThirdPerson, viewport, projectionParameters);
        }
        private void AddCollidableFirstPersonCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
    
        }
        private void AddMainAndPipCamera(Viewport viewport, ProjectionParameters projectionParameters)
        {
            Camera3D camera3D = null;
            Transform3D transform = null;

            //security camera
            transform = new Transform3D(new Vector3(0, 40, 0),
                Vector3.Zero, Vector3.One, -Vector3.UnitY, Vector3.UnitZ);

            int width = 240;
            int height = 180;
            int xPos = this.resolution.X - width - 10;
            Viewport pipViewport = new Viewport(xPos, 10, width, height);

            camera3D = new Camera3D("sc1", ActorType.Camera, transform,
                projectionParameters, pipViewport,
                0f, StatusType.Update);


            camera3D.AttachController(new SecurityCameraController("scc1", ControllerType.Security, 15, 2, Vector3.UnitX));

            this.cameraManager.Add(camera3D);

            //1st person
            transform = new Transform3D(
                 new Vector3(0, 10, 100), Vector3.Zero,
                 Vector3.One, -Vector3.UnitZ, Vector3.UnitY);

            camera3D = new Camera3D("fpc1", ActorType.Camera, transform,
                projectionParameters, viewport,
                1f, StatusType.Update);

            camera3D.AttachController(new FirstPersonCameraController(
              "fpcc1", ControllerType.FirstPerson,
              AppData.CameraMoveKeys, AppData.CameraMoveSpeed,
              AppData.CameraStrafeSpeed, AppData.CameraRotationSpeed, this.inputManagerParameters, this.screenCentre));

            //put controller later!
            this.cameraManager.Add(camera3D);
        }
        private void AddTrack3DCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            //doesnt matter where the camera starts because we reset immediately inside the Transform3DCurveController
            Transform3D transform = Transform3D.Zero; 

            Camera3D camera3D = new Camera3D(id, 
                ActorType.Camera, transform,
                projectionParameters, viewport,
                0f, StatusType.Update);

            camera3D.AttachController(new Track3DController("tcc1", ControllerType.Track,
                this.track3DDictionary["push forward 1"], PlayStatusType.Play));

            this.cameraManager.Add(camera3D);
        }  
        private void AddRailCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            //doesnt matter where the camera starts because we reset immediately inside the RailController
            Transform3D transform = Transform3D.Zero;

            Camera3D camera3D = new Camera3D(id,
                ActorType.Camera, transform,
                ProjectionParameters.StandardMediumFiveThree, viewport,
                0f, StatusType.Update);

            camera3D.AttachController(new RailController("rc1", ControllerType.Rail,
                this.drivableModelObject, this.railDictionary["battlefield 1"]));

            this.cameraManager.Add(camera3D);

        }
        private void AddThirdPersonCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            Transform3D transform = Transform3D.Zero;
            //transform = new Transform3D(new Vector3(0, 0, 0), Vector3.Zero, Vector3.Zero);
            Camera3D camera3D = new Camera3D(id,
                ActorType.Camera, transform,
                projectionParameters, viewport,
                0f, StatusType.Update);

            camera3D.AttachController(new ThirdPersonController("tpcc1", ControllerType.ThirdPerson,
                this.drivableModelObject, AppData.CameraThirdPersonDistance,
                AppData.CameraThirdPersonScrollSpeedDistanceMultiplier,
                AppData.CameraThirdPersonElevationAngleInDegrees, AppData.CameraThirdPersonAngleInDegrees,
                AppData.CameraThirdPersonScrollSpeedElevationMultiplier,
                LerpSpeed.Slow, LerpSpeed.VerySlow, this.inputManagerParameters));

            this.cameraManager.Add(camera3D);

        }
        private void AddSecurityCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            Transform3D transform = new Transform3D(new Vector3(50, 10, 10), Vector3.Zero, Vector3.Zero, -Vector3.UnitX, Vector3.UnitY);

            Camera3D camera3D = new Camera3D(id,
                ActorType.Camera, transform,
                projectionParameters, viewport,
                0f, StatusType.Update);

            camera3D.AttachController(new SecurityCameraController("scc1", ControllerType.Security, 15, 2, Vector3.UnitX));

            this.cameraManager.Add(camera3D);

        }
        private void AddFlightCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            Transform3D transform = new Transform3D(new Vector3(0, 10, 30), Vector3.Zero, Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);

            Camera3D camera3D = new Camera3D(id,
                ActorType.Camera, transform,
                projectionParameters, viewport,
                0f, StatusType.Update);

            camera3D.AttachController(new FlightCameraController("flight camera controller 1", 
                ControllerType.Flight, AppData.CameraMoveKeys_Alt1, AppData.CameraMoveSpeed,
                AppData.CameraStrafeSpeed, AppData.CameraRotationSpeed, this.inputManagerParameters, this.screenCentre));

            this.cameraManager.Add(camera3D);
        }
        private void AddFirstPersonCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            Transform3D transform = new Transform3D(new Vector3(0, 10, 80), Vector3.Zero, Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);

            Camera3D camera3D = new Camera3D(id,
                ActorType.Camera, transform,
                projectionParameters, viewport,
                0f, StatusType.Update);

            camera3D.AttachController(new FirstPersonCameraController(
                "fpcc1", ControllerType.FirstPerson,
                AppData.CameraMoveKeys, AppData.CameraMoveSpeed, 
                AppData.CameraStrafeSpeed, AppData.CameraRotationSpeed, this.inputManagerParameters, this.screenCentre));

            this.cameraManager.Add(camera3D);

        }
        #endregion

        #region Load/Unload, Draw, Update
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //// Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            ////since debug needs sprite batch then call here
            //InitializeDebug(true);
        }
 
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.modelDictionary.Dispose();
            this.fontDictionary.Dispose();
            this.textureDictionary.Dispose();

            //only C# dictionary so no Dispose() method to call
            this.railDictionary.Clear();
            this.track3DDictionary.Clear();
            this.objectArchetypeDictionary.Clear();
            this.vertexDictionary.Clear();

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
            if(this.keyboardManager.IsKeyDown(Keys.P))
            {

            }
            ToggleMenu();

#if DEBUG
            ToggleDebugInfo();
            DemoSetControllerPlayStatus();
            DemoSoundManager();
            DemoCycleCamera();
            DemoUIProgressUpdate();
            DemoUIAddRemoveObject();
#endif
            base.Update(gameTime);
        }

        private void DemoUIAddRemoveObject()
        {
            if(this.keyboardManager.IsFirstKeyPress(Keys.F5))
            {
                string strText = "You win!!!!";
                SpriteFont strFont = this.fontDictionary["menu"];
                Vector2 strDim = strFont.MeasureString(strText);
                strDim /= 2.0f;

                Transform2D transform
                    = new Transform2D(
                        (Vector2)this.screenCentre,
                        0, new Vector2(1, 1), 
                        strDim,     //Vector2.Zero,
                        new Integer2(1, 1));

                UITextObject newTextObject
                    = new UITextObject("win msg",
                    ActorType.UIText,
                    StatusType.Drawn | StatusType.Update,
                    transform,
                    Color.Red,
                    SpriteEffects.None,
                    0,
                    strText,
                    strFont);

                newTextObject.AttachController(new
                    UIRotationScaleExpireController("rslc1",
                    ControllerType.UIRotationLerp, 45, 0.5f, 5000, 1.01f));

                EventDispatcher.Publish(new EventData(
                    "",
                    newTextObject, //handle to "win!"
                    EventActionType.OnAddActor2D,
                    EventCategoryType.SystemAdd));
            }
            else if(this.keyboardManager.IsFirstKeyPress(Keys.F6))
            {
                EventDispatcher.Publish(new EventData(
                    "win msg",
                    null,
                    EventActionType.OnRemoveActor2D,
                    EventCategoryType.SystemRemove));
            }



        }

#if DEBUG
        private void DemoUIProgressUpdate()
        {
            //testing event generation for UIProgressController
            if (this.keyboardManager.IsFirstKeyPress(Keys.F9))
            {
                //increase the left progress controller by 2
                object[] additionalEventParams = {
                    AppData.PlayerOneProgressControllerID, -1 };
                EventDispatcher.Publish(new EventData(
                    EventActionType.OnHealthDelta, 
                    EventCategoryType.Player, additionalEventParams));
            }
            else if (this.keyboardManager.IsFirstKeyPress(Keys.F10))
            {
                //increase the left progress controller by 2
                object[] additionalEventParams = {
                    AppData.PlayerOneProgressControllerID, 1 };
                EventDispatcher.Publish(new EventData(EventActionType.OnHealthDelta, EventCategoryType.Player, additionalEventParams));
            }

            if (this.keyboardManager.IsFirstKeyPress(Keys.F11))
            {
                //increase the left progress controller by 2
                object[] additionalEventParams = { AppData.PlayerTwoProgressControllerID, -2 };
                EventDispatcher.Publish(new EventData(EventActionType.OnHealthDelta, EventCategoryType.Player, additionalEventParams));
            }
            else if (this.keyboardManager.IsFirstKeyPress(Keys.F12))
            {
                //increase the left progress controller by 2
                object[] additionalEventParams = { AppData.PlayerTwoProgressControllerID, 2 };
                EventDispatcher.Publish(new EventData(EventActionType.OnHealthDelta, EventCategoryType.Player, additionalEventParams));
            }
        }
        private void DemoCycleCamera()
        {
            if (this.keyboardManager.IsFirstKeyPress(AppData.CycleCameraKey))
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnCameraCycle, EventCategoryType.Camera));
            }
        }
        private void ToggleDebugInfo()
        {
            if (this.keyboardManager.IsFirstKeyPress(AppData.DebugInfoShowHideKey))
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnToggle, EventCategoryType.Debug));
            }
        }
        private void DemoSoundManager()
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.B))
            {
                //add event to play mouse click
                object[] additionalParameters = { "boing" };
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, additionalParameters));
            }
        }
        private void DemoSetControllerPlayStatus()
        {
            Actor3D torusActor = this.object3DManager.Find(actor => actor.ID.Equals("torus 1"));
            if (torusActor != null && this.keyboardManager.IsFirstKeyPress(Keys.O))
            {
                torusActor.SetControllerPlayStatus(PlayStatusType.Pause, controller => controller.GetControllerType() == ControllerType.Rotation);
            }
            else if (torusActor != null && this.keyboardManager.IsFirstKeyPress(Keys.P))
            {
                torusActor.SetControllerPlayStatus(PlayStatusType.Play, controller => controller.GetControllerType() == ControllerType.Rotation);
            }
        }
#endif

        private void ToggleMenu()
        {
            if (this.keyboardManager.IsFirstKeyPress(AppData.MenuShowHideKey))
            {
                if (this.menuManager.IsVisible)
                    EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
                else
                    EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
        #endregion
    }
}

