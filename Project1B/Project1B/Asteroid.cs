using BEPUphysics.Entities;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1B {
    class Asteroid : DrawableGameComponent {
        static Model mModel = null;
        readonly SpaceDockerGame mGame;
        readonly Entity mPhysicsEntity;
        Matrix mRotationMatrix = Matrix.CreateTranslation(0, 0, 0);
        Matrix mTranslationMatrix;
        float mVelocityAngleX = 1;
        float mVelocityAngleY = 1;
        float mVelocityAngleZ = 1;
        Vector3 mPosition = new Vector3(0, 0, 0);
        Vector3 mVelocity = new Vector3(0, 0, 1);

        public Asteroid(SpaceDockerGame game, Entity physicsEntity) : base(game) {
            mGame = game;
            mPhysicsEntity = physicsEntity;
        }

        public override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            if (mModel == null) {
                mModel = Game.Content.Load<Model>("Models/astroid");
            }
        }

        public override void Update(GameTime gameTime) {
            mRotationMatrix = Matrix.CreateRotationX((float)(mVelocityAngleX * gameTime.ElapsedGameTime.TotalSeconds))
                            * Matrix.CreateRotationY((float)(mVelocityAngleY * gameTime.ElapsedGameTime.TotalSeconds))
                            * Matrix.CreateRotationZ((float)(mVelocityAngleZ * gameTime.ElapsedGameTime.TotalSeconds))
                            * mRotationMatrix;
            mPosition += mVelocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            mTranslationMatrix = Matrix.CreateTranslation(mPosition);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            Matrix worldMatrix = MathConverter.Convert(BEPUutilities.Matrix.Identity * mPhysicsEntity.WorldTransform);

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mModel.Bones.Count];
            mModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mModel.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * worldMatrix;
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
