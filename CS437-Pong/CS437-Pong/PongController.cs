using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS437_Pong
{
    class PongController
    {
        PongModel model;

        public PongController(PongModel gameModel)
        {
            model = gameModel;
        }

        /// <summary>
        /// Updates the positions of the objects in the corresponding PongModel based on the time.
        /// </summary>
        /// <param name="t">The amount of time that has passed.</param>
        public void updateModel(GameTime t)
        {
            if (Keyboard.GetState().IsKeyDown(KeyMap.W))
            {
                model.player1Pos += (float)t.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
