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
        const int paddingScaleHorizontal = 40;
        /// <summary>
        /// Vertical padding around the play area (where the ball will bounce and where paddles cannot go).
        /// </summary>
        const int paddingScaleVertical = 24;
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
        public float player1Pos = 0.5f;
        /// <summary>
        /// Player 2's vertical position between 0.0 and 1.0 between top and bottom padding.
        /// </summary>
        public float player2Pos = 0.5f;
        /// <summary>
        /// The ball horizontal position within the play area, with 0.0 and 1.0 as the bounds, but allows out of bounds.
        /// </summary>
        public float ballPosX = 0.5f;
        /// <summary>
        /// The ball vertical position within the play area, bounded by 0.0 and 1.0 as the edges of the play area.
        /// </summary>
        public float ballPosY = 0.5f;

        public void getPlayer1Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = scaleToPxV(vp, paddleScaleHeight);
            r.Width = scaleToPxH(vp, paddleScaleWidth);

            // Left side of the paddle is aligned with the left padding of the play area.
            r.X = scaleToPxH(vp, paddingScaleHorizontal);
            int paddingV = scaleToPxV(vp, paddingScaleVertical);
            // top padding + player movable range, which is vp height minus both paddings and paddle height.
            r.Y = paddingV + (int)(player1Pos * (vp.Height - 2 * paddingV - r.Height));
        }

        public void getPlayer2Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = scaleToPxV(vp, paddleScaleHeight);
            r.Width = scaleToPxH(vp, paddleScaleWidth);

            // Right side of the paddle is aligned with the right padding of the play area.
            r.X = vp.Width - r.Width - scaleToPxH(vp, paddingScaleHorizontal);
            // top padding + player movable range, which is vp height minus both paddings and paddle height.
            int paddingV = scaleToPxV(vp, paddingScaleVertical);
            r.Y = paddingV + (int)(player2Pos * (vp.Height - 2 * paddingV - r.Height));
        }

        public void getBallRect(Viewport vp, ref Rectangle r)
        {
            r.Height = scaleToPxV(vp, ballScaleHeight);
            r.Width = scaleToPxH(vp, ballScaleWidth);

            int paddingH = scaleToPxH(vp, paddingScaleHorizontal);
            int paddingV = scaleToPxV(vp, paddingScaleVertical);
            r.X = paddingH + (int)(ballPosX * (vp.Width - 2 * paddingH - r.Width));
            r.Y = paddingV + (int)(ballPosY * (vp.Height - 2 * paddingV - r.Height));
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
