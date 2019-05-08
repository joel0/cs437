﻿using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1B {
    class Ship : DrawableGameComponent {
        readonly SpaceDockerGame mGame;
        Model mShipModel;
        readonly Entity mPhysicsEntity;
        public int Health { get; private set; } = 100;
        public float Fuel { get; private set; } = 100;
        const int ASTEROID_DAMAGE = 25;
        const int TORPEDO_SPEED = 10000;
        const float FUEL_ROTATE = 0.05f;
        const float FUEL_THROTTLE = 0.005f;
        GamePadState? mPreviousGPState = null;

        public Matrix WorldMatrix {
            get {
                return MathConverter.Convert(mPhysicsEntity.WorldTransform);
            }
        }
        public Matrix OrientationMatrix {
            get {
                return MathConverter.Convert(mPhysicsEntity.OrientationMatrix);
            }
        }

        public Ship(SpaceDockerGame game, Vector3 position) : base(game) {
            mGame = game;
            mPhysicsEntity = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(position), 900, 1)
            {
                AngularDamping = 0,
                LinearDamping = 0,
                LinearVelocity = new BEPUutilities.Vector3(0, 0, 10f) // This hack might prevent BEPU from freezing
            };
            mGame.Space.Add(mPhysicsEntity);

            mPhysicsEntity.CollisionInformation.Events.DetectingInitialCollision += OnCollisionDetected;
        }

        private void OnCollisionDetected(EntityCollidable sender, Collidable other, CollidablePairHandler pair) {
            // Collision with asteroid.
            if ((other as ConvexCollidable).Entity.Tag is Asteroid asteroid) {
                Health = Math.Max(Health - ASTEROID_DAMAGE, 0);
                if (Health == 0) {
                    // Game over. Delete the player
                    mGame.Components.Remove(this);
                    mGame.Space.Remove(sender.Entity);
                    mGame.IsGameOver = true;
                } else {
                    // Stil alive.  Delete the asteroid
                    mGame.Components.Remove(asteroid);
                    mGame.Space.Remove(((ConvexCollidable)other).Entity);
                }
            }
        }

        protected override void LoadContent() {
            mShipModel = Game.Content.Load<Model>("Models/p1_wedge");
        }

        public override void Update(GameTime gameTime) {
            float yaw = 0;
            float pitch = 0;
            float roll = 0;
            float throttle = 0;
            BEPUutilities.Vector3 linearForce = BEPUutilities.Vector3.Zero;
            BEPUutilities.Vector3 angularForce = BEPUutilities.Vector3.Zero;

            // PROCESS INPUT
            if (!mGame.IsGameOver && GamePad.GetState(PlayerIndex.One).IsConnected) {
                yaw -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                pitch -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                roll -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;

                throttle = (GamePad.GetState(PlayerIndex.One).Triggers.Right - GamePad.GetState(PlayerIndex.One).Triggers.Left) * 50f;

                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed &&
                    mPreviousGPState?.Buttons.A == ButtonState.Released) {
                    FireTorpedo();
                }
                mPreviousGPState = GamePad.GetState(PlayerIndex.One);
            }

            Fuel -= Math.Abs(yaw) * FUEL_ROTATE
                  + Math.Abs(pitch) * FUEL_ROTATE
                  + Math.Abs(roll) * FUEL_ROTATE
                  + Math.Abs(throttle) * FUEL_THROTTLE;
            if (Fuel <= 0) {
                mGame.IsGameOver = true;
                mGame.Components.Remove(this);
                mGame.Space.Remove(mPhysicsEntity);
            }

            // CALCULATE FORCES BASED ON ORIENTATION
            angularForce = mPhysicsEntity.WorldTransform.Up * yaw * 10000
                         + mPhysicsEntity.WorldTransform.Left * pitch * 10000
                         + mPhysicsEntity.WorldTransform.Backward * roll * 10000;
            linearForce = mPhysicsEntity.WorldTransform.Forward * throttle;

            mPhysicsEntity.ApplyLinearImpulse(ref linearForce);
            mPhysicsEntity.ApplyAngularImpulse(ref angularForce);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            Matrix shipWorldMatrix = MathConverter.Convert(mPhysicsEntity.WorldTransform);

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mShipModel.Bones.Count];
            mShipModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mShipModel.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * shipWorldMatrix;
                    effect.View = mGame.ViewMatrix;
                    effect.Projection = mGame.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        void FireTorpedo() {
            Torpedo newTorpedo;
            BEPUutilities.Vector3 position;
            BEPUutilities.Vector3 velocity;

            position = mPhysicsEntity.WorldTransform.Translation
                     + mPhysicsEntity.WorldTransform.Forward * 1000;
            velocity = mPhysicsEntity.LinearVelocity
                     + mPhysicsEntity.WorldTransform.Forward * TORPEDO_SPEED;

            newTorpedo = new Torpedo(mGame, MathConverter.Convert(position), MathConverter.Convert(velocity));
            mGame.Components.Add(newTorpedo);
        }
    }
}
