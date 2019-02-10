﻿using Microsoft.Xna.Framework;
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
        float ballVelocityX = 1.5f;
        float ballVelocityY = 1.5f;

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
            // ----- Update Positions -----
            movePlayers(t);
            moveBall(t);
        }

        void movePlayers(GameTime t)
        {
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

            // ----- Keep players in bounds -----
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
            else if (model.player2Pos > 1)
            {
                model.player2Pos = 1;
            }

            // ----- Check for ball collisions -----
            // Player 1
            //if (model.ballPosX < model.player2Pos)
        }

        void moveBall(GameTime t)
        {
            model.ballPosX += (float)t.ElapsedGameTime.TotalSeconds * ballVelocityX;
            model.ballPosY += (float)t.ElapsedGameTime.TotalSeconds * ballVelocityY;
        }
    }
}
