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
        // These values are what the viewport's width and height will be multiplied by to produce a pixel dimension.
        /// <summary>
        /// Horizontal padding around the play area.
        /// </summary>
        const float paddingScaleHorizontal = 1.0f / 40;
        /// <summary>
        /// Vertical padding around the play area (where the ball will bounce and where paddles cannot go).
        /// </summary>
        const float paddingScaleVertical = 1.0f / 24;
        // ===== OBJECT SCALE TO PLAY AREA =====
        /// <summary>
        /// The height of a player paddle.
        /// </summary>
        const float paddlePScaleHeight = 1.0f / 8;
        /// <summary>
        /// The width of a player paddle.
        /// </summary>
        const float paddlePScaleWidth = 1.0f / 60;
        /// <summary>
        /// The height of the ball (square on 15:9 aspect ratio).
        /// </summary>
        const float ballPScaleHeight = 1.0f / 18;
        /// <summary>
        /// The widthe of the ball (square in 15:9 aspect ratio).
        /// </summary>
        const float ballPScaleWidth = 1.0f / 30;

        // ===== GAME STATE =====
        public int player1Score = 0;
        public int player2Score = 0;

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

        // ====== OBJECT VIRTUAL VALUES =====
        public float virtPlayer1Left { get { return 0f; } }
        public float virtPlayer1Right { get { return paddlePScaleWidth; } }
        public float virtPlayer1Top { get { return player1Pos * (1f - paddlePScaleHeight); } }
        public float virtPlayer1Bottom { get { return virtPlayer1Top + paddlePScaleHeight; } }

        public float virtPlayer2Left { get { return 1f - paddlePScaleWidth; } }
        public float virtPlayer2Right { get { return 1f; } }
        public float virtPlayer2Top { get { return player2Pos * (1f - paddlePScaleHeight); } }
        public float virtPlayer2Bottom { get { return virtPlayer2Top + paddlePScaleHeight; } }


        public float virtBallLeft { get { return ballPosX * (1f - ballPScaleWidth); } }
        public float virtBallRight { get { return virtBallLeft + ballPScaleWidth; } }
        public float virtBallTop { get { return ballPosY * (1f - ballPScaleHeight); } }
        public float virtBallBottom { get { return virtBallTop + ballPScaleHeight; } }

        public bool isBallCollidePlayer1 {
            get {
                return virtBallLeft < virtPlayer1Right && virtBallRight > virtPlayer1Left &&
                    virtBallBottom > virtPlayer1Top && virtBallTop < virtPlayer1Bottom;
            }
        }
        public bool isBallCollidePlayer2 {
            get {
                return virtBallLeft < virtPlayer2Right && virtBallRight > virtPlayer2Left &&
                    virtBallBottom > virtPlayer2Top && virtBallTop < virtPlayer2Bottom;
            }
        }

        public void getPlayer1Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = pScaleToPxV(vp, paddlePScaleHeight);
            r.Width = pScaleToPxH(vp, paddlePScaleWidth);

            int paddingH = scaleToPxH(vp, paddingScaleHorizontal);
            int paddingV = scaleToPxV(vp, paddingScaleVertical);
            r.X = paddingH; // Implicit player1left added
            r.Y = paddingV + pScaleToPxV(vp, virtPlayer1Top);
        }

        public void getPlayer2Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = pScaleToPxV(vp, paddlePScaleHeight);
            r.Width = pScaleToPxH(vp, paddlePScaleWidth);

            int paddingH = scaleToPxH(vp, paddingScaleHorizontal);
            int paddingV = scaleToPxV(vp, paddingScaleVertical);
            r.X = paddingH + pScaleToPxH(vp, virtPlayer2Left);
            r.Y = paddingV + pScaleToPxV(vp, virtPlayer2Top);
        }

        public void getBallRect(Viewport vp, ref Rectangle r)
        {
            r.Height = pScaleToPxV(vp, ballPScaleHeight);
            r.Width = pScaleToPxH(vp, ballPScaleWidth);

            int paddingH = scaleToPxH(vp, paddingScaleHorizontal);
            int paddingV = scaleToPxV(vp, paddingScaleVertical);
            r.X = paddingH + pScaleToPxH(vp, virtBallLeft);
            r.Y = paddingV + pScaleToPxV(vp, virtBallTop);
        }

        // ===== SCALE UNIT TRANSLATIONS =====
        /// <summary>
        /// Scales a horizontal scale measurment to number of pixels for the current Viewport.
        /// </summary>
        /// <param name="vp">the Viewport that this measurement is applicable to</param>
        /// <param name="scale">the ratio value for the relative size</param>
        /// <returns>the number of pixels this measurement corresponds to</returns>
        int scaleToPxH(Viewport vp, float scale)
        {
            return (int)(vp.Width * scale);
        }

        int pScaleToPxH(Viewport vp, float scale)
        {
            float playAreaRatio = 1f - 2f * paddingScaleHorizontal;
            return scaleToPxH(vp, playAreaRatio * scale);
        }

        /// <summary>
        /// Scales a vertical scale measurment to number of pixels for the current Viewport.
        /// </summary>
        /// <param name="vp">the Viewport that this measurement is applicable to</param>
        /// <param name="scale">the ratio value for the relative size</param>
        /// <returns>the number of pixels this measurement corresponds to</returns>
        int scaleToPxV(Viewport vp, float scale)
        {
            return (int)(vp.Height * scale);
        }

        int pScaleToPxV(Viewport vp, float scale)
        {
            float playAreaRatio = 1f - 2f * paddingScaleVertical;
            return scaleToPxV(vp, playAreaRatio * scale);
        }
    }
}
