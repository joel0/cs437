

namespace ReversiTournament {
    public interface IReversiMoves {
        void Reset(ReversiCommon.TokenColor yourColor, ReversiCommon.TokenColor firstPlayerColor);
        Move GetMove();
        void MakeMove(Move move);
    }

    public class Move {
        public int Row { get; set; }
        public int Col { get; set; }
        public Move(int row, int col) {
            this.Row = row;
            this.Col = col;
        }
    }

    public class ReversiCommon {
        public enum TokenColor {
            BLACK,
            WHITE
        };
    }
}

