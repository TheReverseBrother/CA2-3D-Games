﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GDApp;
using System;

namespace GDLibrary
{
    public class PlayerCollidablePrimitiveObject : CollidablePrimitiveObject
    {
        #region Fields
        private float moveSpeed, rotationSpeed;
        private KeyboardManager keyboardManager;
        private Keys[] moveKeys;
        #endregion

        #region Properties
        #endregion


        public PlayerCollidablePrimitiveObject(string id, ActorType actorType, Transform3D transform,
            EffectParameters effectParameters, StatusType statusType, IVertexData vertexData,
            ICollisionPrimitive collisionPrimitive, ObjectManager objectManager, 
            Keys[] moveKeys, float moveSpeed, float rotationSpeed, KeyboardManager keyboardManager)
            : base(id, actorType, transform,
            effectParameters, statusType, vertexData,
            collisionPrimitive, objectManager)
        {
            this.moveKeys = moveKeys;
            this.moveSpeed = moveSpeed;
            this.rotationSpeed = rotationSpeed;

            //for movement
            this.keyboardManager = keyboardManager;
        }

        public override void Update(GameTime gameTime)
        {
            //read any input and store suggested increments
            HandleInput(gameTime);

            //have we collided with something?
            this.Collidee = CheckCollisions(gameTime);

            //how do we respond to this collidee e.g. pickup?
            HandleCollisionResponse(this.Collidee);

            //if no collision then move - see how we set this.Collidee to null in HandleCollisionResponse() 
            //below when we hit against a zone
            if (this.Collidee == null)
                ApplyInput(gameTime);

            //reset translate and rotate and update primitive
            base.Update(gameTime);
        }

        //this is where you write the application specific CDCR response for your game
        protected override void HandleCollisionResponse(Actor collidee)
        {
            if(collidee is SimpleZoneObject)
            {
                SimpleZoneObject simpleZoneObject = collidee as SimpleZoneObject;

                //do something based on the zone type - see Main::InitializeCollidableZones() for ID
                if (simpleZoneObject.ID.Equals("camera trigger zone 1"))
                {
                    //publish an event e.g sound, health progress
                    object[] additionalParameters = { "boing" };
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, additionalParameters));
                }

                //IMPORTANT - setting this to null means that the ApplyInput() method will get called and the player can move through the zone.
                this.Collidee = null;
            }
            else if(collidee is CollidablePrimitiveObject)
            {
                if(collidee.ActorType == ActorType.CollidablePickup)
                {
                    //Remove The Object Add to Score
                    EventDispatcher.Publish(new EventData(collidee, EventActionType.OnRemoveActor, EventCategoryType.SystemRemove));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "pickup-sound" }));

                }
                else if (collidee.ActorType == ActorType.CollidableLazer)
                {
                    StopMusic();
                    EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
                    EventDispatcher.Publish(new EventData(EventActionType.GameOver,EventCategoryType.PauseAndChangeMenu));
                }
                else if(collidee.ActorType == ActorType.LevelOneFinish)
                {
                    object[] additionalParameters = { 2 };
                    EventDispatcher.Publish(new EventData(EventActionType.LevelOneComplete, EventCategoryType.Level,additionalParameters));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
                    EventDispatcher.Publish(new EventData(EventActionType.LevelOneComplete, EventCategoryType.PauseAndChangeMenu));
                }
                else if (collidee.ActorType == ActorType.LevelTwoFinish)
                {
                    object[] additionalParameters = { 1 };
                    EventDispatcher.Publish(new EventData(EventActionType.LevelOneComplete, EventCategoryType.Level, additionalParameters));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
                    EventDispatcher.Publish(new EventData(EventActionType.GameComplete, EventCategoryType.PauseAndChangeMenu));
                }
                else if (collidee.ActorType == ActorType.CollidableButton)
                {                    
                    EventDispatcher.Publish(new EventData("Change State",collidee,EventActionType.OnRemoveActor, EventCategoryType.Button));
                }
            }
        }
        protected void StopMusic()
        {
            object[] additionalParameters = { "background-track" };

            EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, additionalParameters));
        }

        protected override void HandleInput(GameTime gameTime)
        {
            if (this.keyboardManager.IsKeyDown(this.moveKeys[AppData.IndexMoveForward])) //Forward
            {
                this.Transform.TranslateIncrement
                    = this.Transform.Look * gameTime.ElapsedGameTime.Milliseconds
                            * this.moveSpeed;
            }
            else if (this.keyboardManager.IsKeyDown(this.moveKeys[AppData.IndexMoveBackward])) //Backward
            {
                this.Transform.TranslateIncrement
                   = -this.Transform.Look * gameTime.ElapsedGameTime.Milliseconds
                           * this.moveSpeed;
            }

            //Changed to move left
            if (this.keyboardManager.IsKeyDown(this.moveKeys[AppData.IndexRotateLeft])) //Left
            {
                this.Transform.TranslateIncrement = -this.Transform.Right * gameTime.ElapsedGameTime.Milliseconds
                           * this.moveSpeed;
            }
            else if (this.keyboardManager.IsKeyDown(this.moveKeys[AppData.IndexRotateRight])) //Right
            {
                this.Transform.TranslateIncrement = this.Transform.Right * gameTime.ElapsedGameTime.Milliseconds
                           * this.moveSpeed;
            }
        }
    }
}
