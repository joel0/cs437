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
            // ----- User Input -----
            // Player 1
            if (Keyboard.GetState().IsKeyDown(KeyMap.W))
            {
                model.player1Pos -= (float)t.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(KeyMap.S))
            {
                model.player1Pos += (float)t.ElapsedGameTime.TotalSeconds;
            }

            // Player 2
            // Note, this is not using KeyMap because it is assumed that the keyboard layout won't change arrow keys.
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                model.player2Pos -= (float)t.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                model.player2Pos += (float)t.ElapsedGameTime.TotalSeconds;
            }

            // ----- Keep everything in bounds -----
            // Player 1
            if (model.player1Pos < 0)
            {
                model.player1Pos = 0;
            }
            else if (model.player1Pos > 1)
            {
                model.player1Pos = 1;
            }
            // Player 2
            if (model.player2Pos < 0)
            {
                model.player2Pos = 0;
            }
            else if (model.player2Pos > 1) {
                model.player2Pos = 1;
            }
        }
    }
}
