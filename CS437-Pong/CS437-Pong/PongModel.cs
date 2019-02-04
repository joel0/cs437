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
        const int padding = 20;
        const int paddleScaleHeight = 8;
        const int paddleScaleWidth = 60;
        public float player1Offset = 0;
        float player2Offset = 0;

        public void getPlayer1Rect(Viewport vp, ref Rectangle r)
        {
            r.Height = vp.Height / paddleScaleHeight;
            r.Width = vp.Width / paddleScaleWidth;

            r.X = vp.Width - r.Width - padding;
            r.Y = padding + (int) player1Offset;
        }
    }
}
