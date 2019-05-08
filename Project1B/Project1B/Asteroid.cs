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
    class Asteroid : DrawableGameComponent {
        static Model mModel = null;
        readonly SpaceDockerGame mGame;
        readonly Entity mPhysicsEntity;

        public Asteroid(SpaceDockerGame game, Vector3 position, Vector3 speed, Vector3 angularSpeed) : base(game) {
            mGame = game;
            // Magic numbers for capsule length and radius correspond to model dimensions
            mPhysicsEntity = new BEPUphysics.Entities.Prefabs.Capsule(MathConverter.Convert(position), 1300, 400, 1)
            {
                AngularDamping = 0,
                LinearDamping = 0,
                LinearVelocity = MathConverter.Convert(speed),
                AngularVelocity = MathConverter.Convert(angularSpeed),
                Tag = this
            };
            mGame.Space.Add(mPhysicsEntity);

            mPhysicsEntity.CollisionInformation.Events.DetectingInitialCollision += OnCollisionDetected;
        }

        private void OnCollisionDetected(EntityCollidable sender, Collidable other, CollidablePairHandler pair) {
            // Collision with other asteroid.
            if ((other as ConvexCollidable).Entity.Tag is Asteroid otherAsteroid) {
                // Delete both asteroids
                mGame.Components.Remove(this);
                mGame.Components.Remove(otherAsteroid);
                try {
                    mGame.Space.Remove(pair.EntityA);
                    mGame.Space.Remove(pair.EntityB);
                } catch { /* Do nothing */ }
            }
            // Collision with mothership.
            if ((other as ConvexCollidable).Entity.Tag is Mothership mothership) {
                // Delete the asteroid
                mGame.Components.Remove(this);
                try {
                    mGame.Space.Remove(((ConvexCollidable)other).Entity);
                } catch { /* Do nothing */ }
            }
            // Collision with mothership's goal.
            if ((other as ConvexCollidable).Entity.Tag is MothershipGoal mothershipGoal) {
                // Delete the asteroid
                mGame.Components.Remove(this);
                try {
                mGame.Space.Remove(((ConvexCollidable)other).Entity);
                } catch { /* Do nothing */ }
            }
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
