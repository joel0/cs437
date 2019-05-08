using BEPUphysics.Entities;
using ConversionHelper;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1B {
    class MothershipGoal : GameComponent {
        readonly SpaceDockerGame mGame;
        readonly Entity mPhysicsEntity;

        public MothershipGoal(SpaceDockerGame game, Vector3 position) : base(game) {
            mGame = game;
            mPhysicsEntity = new BEPUphysics.Entities.Prefabs.Box(
                MathConverter.Convert(position
                    + Vector3.Backward * 10000
                    + Vector3.Up * 1300),
                7000, 3800, 500)
            {
                Tag = this
            };
            mGame.Space.Add(mPhysicsEntity);
        }
    }
}
