using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1B {
    class Skybox : DrawableGameComponent {
        readonly SpaceDockerGame mGame;
        Model mModel;

        public Skybox(SpaceDockerGame game) : base(game) {
            mGame = game;
        }

        protected override void LoadContent() {
            mModel = Game.Content.Load<Model>("Models/skybox");
        }

        public override void Draw(GameTime gameTime) {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[mModel.Bones.Count];
            mModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in mModel.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(1000f);
                    effect.View = Matrix.Invert(mGame.OrientationMatrix);
                    effect.Projection = mGame.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
