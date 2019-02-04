using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS437_Pong
{
    class PongModel
    {
        // ===== OBJECT SCALE TO VIEWPORT =====
        // These values are what the viewport's width and height will be divided by to produce a pixel dimension.
        /// <summary>
        /// Horizontal padding around the play area.
        /// </summary>
        const int paddingHorizontal = 40;
        /// <summary>
        /// Vertical padding around the play area (where the ball will bounce and where paddles cannot go).
        /// </summary>
        const int paddingVertical = 24;
        /// <summary>
        /// The height of a player paddle.
        /// </summary>
        const int paddleScaleHeight = 8;
        /// <summary>
        /// The width of a player paddle.
        /// </summary>
        const int paddleScaleWidth = 60;
        /// <summary>
        /// The height of the ball (square on 15:9 aspect ratio).
        /// </summary>
        const int ballScaleHeight = 18;
        /// <summary>
        /// The widthe of the ball (square in 15:9 aspect ratio).
        /// </summary>
        const int ballScaleWidth = 30;

        // ===== OBJECT CURRENT POSITIONS =====
        /// <summary>
        /// Player 1's vertical position between 0.0 and 1.0 between top and bottom padding.
        /// </summary>
        float player1Pos = 0;
        /// <summary>
        /// Player 2's vertical position between 0.0 and 1.0 between top and bottom padding.
        /// </summary>
        float player2Pos = 0;

        public void getPlayer1Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = scaleToPxV(vp, paddleScaleHeight);
            r.Width = scaleToPxH(vp, paddleScaleWidth);

            // Left side of the paddle is aligned with the left padding of the play area.
            r.X = scaleToPxH(vp, paddingHorizontal);
            // TODO: scale 0.0 to 1.0
            r.Y = scaleToPxV(vp, paddingVertical) + (int)player1Pos;
        }

        public void getPlayer2Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = scaleToPxV(vp, paddleScaleHeight);
            r.Width = scaleToPxH(vp, paddleScaleWidth);

            // Right side of the paddle is aligned with the right padding of the play area.
            r.X = vp.Width - r.Width - scaleToPxH(vp, paddingHorizontal);
            // TODO: scale 0.0 to 1.0
            r.Y = scaleToPxV(vp, paddingVertical) + (int)player2Pos;
        }

        public void getBallRect(Viewport vp, ref Rectangle r)
        {
            r.Height = scaleToPxV(vp, ballScaleHeight);
            r.Width = scaleToPxH(vp, ballScaleWidth);

            // TODO: offset in instavars
            r.X = 100;
            r.Y = 100;
        }

        // ===== SCALE UNIT TRANSLATIONS =====
        /// <summary>
        /// Scales a horizontal scale measurment to number of pixels for the current Viewport.
        /// </summary>
        /// <param name="vp">the Viewport that this measurement is applicable to</param>
        /// <param name="scale">the denominator value for the relative size</param>
        /// <returns>the number of pixels this measurement corresponds to</returns>
        int scaleToPxH(Viewport vp, float scale)
        {
            return (int)(vp.Width / scale);
        }

        /// <summary>
        /// Scales a vertical scale measurment to number of pixels for the current Viewport.
        /// </summary>
        /// <param name="vp">the Viewport that this measurement is applicable to</param>
        /// <param name="scale">the denominator value for the relative size</param>
        /// <returns>the number of pixels this measurement corresponds to</returns>
        int scaleToPxV(Viewport vp, float scale)
        {
            return (int)(vp.Height / scale);
        }
    }
}
