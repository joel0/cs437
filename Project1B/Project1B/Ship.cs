using BEPUphysics.Entities;
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
        }

        protected override void LoadContent() {
            mShipModel = Game.Content.Load<Model>("Models/p1_wedge");
        }

        public override void Update(GameTime gameTime) {
            float yaw = MathHelper.ToRadians(0);
            float pitch = MathHelper.ToRadians(0);
            float roll = MathHelper.ToRadians(0);
            float throttle = 0;
            BEPUutilities.Vector3 linearForce = BEPUutilities.Vector3.Zero;
            BEPUutilities.Vector3 angularForce = BEPUutilities.Vector3.Zero;

            // PROCESS INPUT
            if (GamePad.GetState(PlayerIndex.One).IsConnected) {
                yaw -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                pitch -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                roll -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;

                throttle = (GamePad.GetState(PlayerIndex.One).Triggers.Right - GamePad.GetState(PlayerIndex.One).Triggers.Left) * 50f;
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
    }
}
