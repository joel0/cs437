using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1B {
    class Torpedo : DrawableGameComponent {
        readonly SpaceDockerGame mGame;
        Model mModel;
        readonly Entity mPhysicsEntity;

        public Torpedo(SpaceDockerGame game, Vector3 position, Vector3 velocity) : base(game) {
            mGame = game;
            mPhysicsEntity = new BEPUphysics.Entities.Prefabs.Sphere(MathConverter.Convert(position), 30, 1)
            {
                LinearDamping = 0,
                LinearVelocity = MathConverter.Convert(velocity),
                Tag = this
            };
            mGame.Space.Add(mPhysicsEntity);

            mPhysicsEntity.CollisionInformation.Events.DetectingInitialCollision += OnCollisionDetected;
        }

        private void OnCollisionDetected(EntityCollidable sender, Collidable other, CollidablePairHandler pair) {
            // Collision with asteroid.
            if ((other as ConvexCollidable).Entity.Tag is Asteroid asteroid) {
                // Delete the asteroid and torpedo
                if (mGame.Components.Remove(this)) {
                    mGame.Space.Remove(pair.EntityA);
                    mGame.Space.Remove(pair.EntityB);
                    mGame.Components.Remove(asteroid);
                }
            }
            // Collision with mothership.
            if ((other as ConvexCollidable).Entity.Tag is Mothership mothership) {
                // Delete the torpedo
                mGame.Components.Remove(this);
                mGame.Space.Remove(((ConvexCollidable)other).Entity);
            }
            // Collision with mothership's goal.
            if ((other as ConvexCollidable).Entity.Tag is MothershipGoal mothershipGoal) {
                // Delete the torpedo
                mGame.Components.Remove(this);
                mGame.Space.Remove(((ConvexCollidable)other).Entity);
            }
        }

        protected override void LoadContent() {
            mModel = Game.Content.Load<Model>("Models/torpedo");
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
