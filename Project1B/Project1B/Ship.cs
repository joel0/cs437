using BEPUphysics.BroadPhaseEntries;
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
        public int Health { get; private set; } = HEALTH_MAX;
        public float Fuel { get; private set; } = FUEL_MAX;
        public int TorpedoesAvail = TORPEDO_MAX_AVAIL;
        Vector2 mTorpedoAim = Vector2.Zero;
        const int ASTEROID_DAMAGE = 25;
        const int TORPEDO_SPEED = 10000;
        const float FUEL_ROTATE = 0.02f;
        const float FUEL_THROTTLE = 0.002f;
        const int FUEL_MAX = 100;
        const int HEALTH_MAX = 100;
        public const int TORPEDO_MAX_AVAIL = 10;
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
            ConvexCollidable o = other as ConvexCollidable;
            // Collision with asteroid.
            if (o?.Entity.Tag is Asteroid asteroid) {
                Health = Math.Max(Health - ASTEROID_DAMAGE, 0);
                if (Health == 0) {
                    // Game over. Delete the player
                    Lose();
                } else {
                    // Stil alive.  Delete the asteroid
                    mGame.Components.Remove(asteroid);
                    mGame.Space.Remove(o.Entity);
                }
            }
            // Collision with mothership
            if (o?.Entity.Tag is Mothership mothership) {
                // Game over. Delete the player
                Lose();
            }
            // Collision with mothership goal.
            if ((other as ConvexCollidable).Entity.Tag is MothershipGoal mothershipGoal) {
                if (IsSafeDockingSpeed()) {
                    Win();
                } else {
                    Lose();
                }
            }
            // Collision with powerup
            if (o?.Entity.Tag is Powerup powerup) {
                // Delete the powerup
                mGame.Components.Remove(powerup);
                mGame.Space.Remove(o.Entity);
                Fuel = FUEL_MAX;
                Health = HEALTH_MAX;
                TorpedoesAvail = TORPEDO_MAX_AVAIL;
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

                if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed &&
                    mPreviousGPState?.Buttons.B == ButtonState.Released) {
                    DoWormhole();
                }

                if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed) {
                    mTorpedoAim.Y -= 0.4f;
                }
                if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed) {
                    mTorpedoAim.Y += 0.4f;
                }
                if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) {
                    mTorpedoAim.X -= 0.4f;
                }
                if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) {
                    mTorpedoAim.X += 0.4f;
                }
                if (mTorpedoAim.X > 20) { mTorpedoAim.X = 20; }
                if (mTorpedoAim.X < -20) { mTorpedoAim.X = -20; }
                if (mTorpedoAim.Y > 20) { mTorpedoAim.Y = 20; }
                if (mTorpedoAim.Y < -20) { mTorpedoAim.Y = -20; }

                mPreviousGPState = GamePad.GetState(PlayerIndex.One);
            }

            Fuel -= Math.Abs(yaw) * FUEL_ROTATE
                  + Math.Abs(pitch) * FUEL_ROTATE
                  + Math.Abs(roll) * FUEL_ROTATE
                  + Math.Abs(throttle) * FUEL_THROTTLE;
            if (Fuel <= 0) {
                Lose();
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

        public void DrawHUD(SpriteBatch spriteBatch, Texture2D texture) {
            float xPercent = mTorpedoAim.X / 40;
            float yPercent = mTorpedoAim.Y / 40;
            spriteBatch.Draw(texture, new Rectangle(10, 370, 100, 100), Color.Blue);
            spriteBatch.Draw(texture, new Rectangle(
                (int)(10 + (100 - 6) * (xPercent + 0.5)),
                (int)(370 + (100 - 6) * (yPercent + 0.5)), 6, 6), Color.White);
        }

        void FireTorpedo() {
            if (TorpedoesAvail > 0) {
                TorpedoesAvail--;

                Torpedo newTorpedo;
                BEPUutilities.Vector3 position;
                BEPUutilities.Vector3 velocity;

                position = mPhysicsEntity.WorldTransform.Translation
                         + mPhysicsEntity.WorldTransform.Forward * 1000;
                velocity = mPhysicsEntity.LinearVelocity
                         + mPhysicsEntity.WorldTransform.Forward * TORPEDO_SPEED
                         + mPhysicsEntity.WorldTransform.Right * mTorpedoAim.X * TORPEDO_SPEED / 100
                         + mPhysicsEntity.WorldTransform.Down * mTorpedoAim.Y * TORPEDO_SPEED / 100;

                newTorpedo = new Torpedo(mGame, MathConverter.Convert(position), MathConverter.Convert(velocity));
                mGame.Components.Add(newTorpedo);
            }
        }

        private void Lose() {
            mGame.Components.Remove(this);
            mGame.Space.Remove(mPhysicsEntity);
            mGame.IsGameOver = true;
        }

        private void Win() {
            mGame.Components.Remove(this);
            mGame.Space.Remove(mPhysicsEntity);
            mGame.IsGameOver = true;
            mGame.IsWin = true;
        }

        private bool IsSafeDockingSpeed() {
            return mPhysicsEntity.LinearVelocity.Length() < 2000
                && mPhysicsEntity.AngularVelocity.Length() < 1;
        }

        private void DoWormhole() {
            Random rng = new Random();
            Fuel -= 15;
            mPhysicsEntity.LinearVelocity = new BEPUutilities.Vector3(0, 0, 10f);
            mPhysicsEntity.AngularVelocity = BEPUutilities.Vector3.Zero;
            mPhysicsEntity.Position = new BEPUutilities.Vector3(
                (float)(SpaceDockerGame.PLAY_AREA_SIZE * (rng.NextDouble() - 0.5)),
                (float)(SpaceDockerGame.PLAY_AREA_SIZE * (rng.NextDouble() - 0.5)),
                (float)(SpaceDockerGame.PLAY_AREA_SIZE * (rng.NextDouble() - 0.5)));
        }
    }
}
