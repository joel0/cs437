﻿using BEPUphysics.Entities;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1B {
    class Mothership : DrawableGameComponent {
        readonly SpaceDockerGame mGame;
        Model mModel;
        readonly Entity mPhysicsEntity;
        Matrix mScaleMatrix;

        public Mothership(SpaceDockerGame game, Vector3 position) : base(game) {
            mGame = game;
            mPhysicsEntity = new BEPUphysics.Entities.Prefabs.Box(MathConverter.Convert(position), 20000, 20000, 20000)
            {
                Tag = this
            };
            mGame.Space.Add(mPhysicsEntity);

            new MothershipGoal(game, position);

            mScaleMatrix = Matrix.CreateScale(10);
        }

        protected override void LoadContent() {
            mModel = Game.Content.Load<Model>("Models/mothership");
        }

        public override void Draw(GameTime gameTime) {
            Matrix worldMatrix = MathConverter.Convert(mPhysicsEntity.WorldTransform);
            
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mModel.Bones.Count];
            mModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mModel.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * mScaleMatrix * worldMatrix;
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
