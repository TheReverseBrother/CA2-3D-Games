/*
Function: 		Stores common hard-coded variable values used within the game e.g. key mappings, mouse sensitivity
Author: 		NMCG
Version:		1.0
Date Updated:	5/10/17
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace GDLibrary
{
    public sealed class LerpSpeed
    {
        private static readonly float SpeedMultiplier = 2;
        public static readonly float VerySlow = 0.008f; 
        public static readonly float Slow = SpeedMultiplier * VerySlow;
        public static readonly float Medium = SpeedMultiplier * Slow;
        public static readonly float Fast = SpeedMultiplier * Medium;
        public static readonly float VeryFast = SpeedMultiplier * Fast;
    }

    public sealed class AppData
    {
        #region Keys
        public static readonly Keys CycleCameraKey = Keys.C;
        public static readonly Keys DebugInfoShowHideKey = Keys.T;
        public static readonly Keys[] PlayerOneMoveKeys = { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.C, Keys.Q, Keys.E };

        public static readonly Keys[] ObjectMoveKeys = {
            Keys.NumPad8, Keys.NumPad5,  //forward, backward
            Keys.NumPad4, Keys.NumPad6,  //rotate left, rotate right
            Keys.NumPad1, Keys.NumPad3   //strafe left, strafe right
        };
        public static readonly Keys[] CameraMoveKeys = { Keys.T, Keys.G, Keys.F, Keys.H, Keys.R, Keys.Y, Keys.V, Keys.B };
        public static readonly Keys[] CameraMoveKeys_Alt1 = { Keys.I, Keys.K, Keys.J, Keys.L };
        public static readonly Keys MenuShowHideKey = Keys.Escape;
        #endregion

        #region Common
        public static int IndexMoveForward = 0;
        public static int IndexMoveBackward = 1;
        public static int IndexRotateLeft = 2;
        public static int IndexRotateRight = 3;
        public static int IndexMoveJump = 4;
        public static int IndexMoveCrouch = 5;
        public static int IndexStrafeLeft = 6;
        public static int IndexStrafeRight = 7;

        #endregion

        #region Car
        public static readonly float CarRotationSpeed = 0.1f;
        public static readonly float CarMoveSpeed = 0.09f;
        public static readonly float CarStrafeSpeed = 0.7f * CarMoveSpeed;
        #endregion

        #region Camera
        public static readonly string CameraIDCollidableFirstPerson = "collidable first";
        public static readonly string CameraIDFirstPerson = "first";
        public static readonly string CameraIDThirdPerson = "third";
        public static readonly string CameraIDFlight = "flight";
        public static readonly string CameraIDRail = "rail";
        public static readonly string CameraIDTrack = "track";
        public static readonly string CameraIDSecurity = "security";


        public static readonly float CameraRotationSpeed = 0.01f;
        public static readonly float CameraMoveSpeed = 0.075f;
        public static readonly float CameraStrafeSpeed = 0.7f * CameraMoveSpeed;
    

        //3rd person specific
        public static readonly float CameraThirdPersonScrollSpeedDistanceMultiplier = 0.00125f;
        public static readonly float CameraThirdPersonScrollSpeedElevationMultiplier = 0.001f;
        public static readonly float CameraThirdPersonDistance = 90;
        public static readonly float CameraThirdPersonElevationAngleInDegrees = 30;

        //security camera
        public static readonly Vector3 SecurityCameraRotationAxisYaw = Vector3.UnitX;
        public static readonly Vector3 SecurityCameraRotationAxisPitch = Vector3.UnitY;
        public static readonly Vector3 SecurityCameraRotationAxisRoll = Vector3.UnitZ;

        #endregion

        #region UI
        public static readonly string PlayerOneProgressID = "p1 progress";
        public static readonly string PlayerTwoProgressID = "p2 progress";
        public static readonly string PlayerOneProgressControllerID = PlayerOneProgressID + " ctrllr";
        public static readonly string PlayerTwoProgressControllerID = PlayerTwoProgressID + " ctrllr";
        #endregion

        #region Menu
        public static readonly string MenuMainID = "main";
        public static readonly string MenuOptionsID = "options";
        public static readonly string MenuControlsID = "controls";
        public static readonly string MenuAudioID = "audio";
        #endregion

        #region JigLibX
        public static readonly Vector3 Gravity = -10 * Vector3.UnitY;
        public static readonly Vector3 BigGravity = 5 * Gravity;

        //JigLib related collidable camera properties
        public static readonly float CollidableCameraJumpHeight = 12;
        public static readonly float CollidableCameraMoveSpeed = 0.6f;
        public static readonly float CollidableCameraStrafeSpeed = 0.6f * CollidableCameraMoveSpeed;
        public static readonly float CollidableCameraCapsuleRadius = 2;
        public static readonly float CollidableCameraViewHeight = 8; //how tall is the first person player?
        public static readonly float CollidableCameraMass = 10;

        //always ensure that we start picking OUTSIDE the collidable first person camera radius - otherwise we will always pick ourself!
        public static readonly float PickStartDistance = CollidableCameraCapsuleRadius * 2f;
        public static readonly float PickEndDistance = 1000; //can be related to camera far clip plane radius but should be limited to typical level max diameter
        public static readonly bool EnablePickAndPlace = true;

        #endregion

        #region Object archetype ids used when we want to clone and make another quad, triangle, cube etc
        public static readonly string UnlitTexturedQuadArchetypeID = "unlit tex quad archetype";
        public static readonly string UnlitTexturedCubeArchetypeID = "unlit tex cube archetype";
        #endregion

        #region Primitive ids used by vertexData dictionary
        public static readonly string WireframeOriginHelperVertexDataID = "wire origin vertexdata";
        public static readonly string WireframeTriangleVertexDataID = "wire triangle vertexdata";
        public static readonly string WireframeCircleVertexDataID = "wire circle vertexdata";
        public static readonly string UnlitTexturedQuadVertexDataID = "unlit tex quad vertexdata";
        public static readonly string UnlitTexturedCubeVertexDataID = "unlit tex cube vertexdata";
        public static readonly string UnlitTexturedBillboardVertexDataID = "unlit tex billboard vertexdata";
        public static readonly string LitTexturedCubeVertexDataID = "lit tex cube vertexdata";
        #endregion

        #region Effect parameter ids used by the effect dictionary
        public static readonly string UnlitWireframeEffectID = "wireframe unlit effect";
        public static readonly string UnlitTexturedEffectID = "textured unlit effect";
        public static readonly string LitTexturedEffectID = "textured lit effect";
        public static readonly string UnlitBillboardsEffectID = "unlit billboards effect";
        #endregion

        #region Player
        public static readonly string PlayerOneID = "player1";
        public static readonly string PlayerTwoID = "player2";
        public static readonly float PlayerMoveSpeed = 0.125f;
        public static readonly float PlayerStrafeSpeed = 0.7f * PlayerMoveSpeed;
        public static readonly float PlayerRotationSpeed = 0.08f;
        public static readonly float PlayerRadius = 1.5f;
        public static readonly float PlayerHeight = 4.5f;
        public static readonly float PlayerMass = 5;
        public static readonly float PlayerJumpHeight = 30;

        public static readonly float SquirrelPlayerMoveSpeed = 0.4f;
        public static readonly float SquirrelPlayerRotationSpeed = 0.2f;
        public static readonly Keys[] SquirrelPlayerMoveKeys = { Keys.NumPad8, Keys.NumPad5, Keys.NumPad4, Keys.NumPad6, Keys.NumPad7, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3 };

        #endregion

        #region Mouse
        //defines how much the mouse has to move in pixels before a movement is registered - see MouseManager::HasMoved()
        public static readonly float MouseSensitivity = 1;

        #endregion
    }
}
