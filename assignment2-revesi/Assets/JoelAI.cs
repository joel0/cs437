using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiTournament {
    class JoelAI : IReversiMoves {
        readonly int mRecursion;
        public BoardToken[,] Board { get; private set; } = new BoardToken[8, 8];
        int mCountBlackPieces;
        int mCountWhitePieces;
        ReversiCommon.TokenColor mMyColor;
        ReversiCommon.TokenColor mCurrentPlayer;
        MiniMax.TreeNode mPredictRoot = null;
        ReversiCommon.TokenColor OpponentColor {
            get {
                if (mMyColor == ReversiCommon.TokenColor.BLACK) {
                    return ReversiCommon.TokenColor.WHITE;
                }
                return ReversiCommon.TokenColor.BLACK;
            }
        }

        /// <summary>
        /// Intended to be run with recursion depth of 5!
        /// </summary>
        /// <param name="recursionDepth"></param>
        public JoelAI(int recursionDepth) {
            mRecursion = recursionDepth;
        }
        public JoelAI(JoelAI cloneSrc) {
            Board = (BoardToken[,])cloneSrc.Board.Clone();
            mCountBlackPieces = cloneSrc.mCountBlackPieces;
            mCountWhitePieces = cloneSrc.mCountWhitePieces;
            mMyColor = cloneSrc.mMyColor;
            mCurrentPlayer = cloneSrc.mCurrentPlayer;
        }

        public void Reset(ReversiCommon.TokenColor yourColor, ReversiCommon.TokenColor firstPlayerColor) {
            Board = new BoardToken[8, 8];
            Board[3, 3] = BoardToken.WHITE;
            Board[4, 4] = BoardToken.WHITE;
            Board[4, 3] = BoardToken.BLACK;
            Board[3, 4] = BoardToken.BLACK;
            mCountBlackPieces = 2;
            mCountWhitePieces = 2;
            mMyColor = yourColor;
            mCurrentPlayer = firstPlayerColor;
        }

        public void MakeMove(Move move) {
            if (mPredictRoot != null) {
                mPredictRoot = mPredictRoot.childNodes[MiniMax.GetMoveIndex(mPredictRoot, move)];
            }
            ClaimLine(mCurrentPlayer, move.Row, move.Col, -1, 1);  // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 0, 1);   // | Right side
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 1, 1);   // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 1, 0);   // - Bottom
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 1, -1);  // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 0, -1);  // | Left side
            ClaimLine(mCurrentPlayer, move.Row, move.Col, -1, -1); // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, -1, 0);  // - Top
            Board[move.Col, move.Row] = BoardTokenFromTokenColor(mCurrentPlayer);
            IncreaseCount(mCurrentPlayer);
            mCurrentPlayer = InvertColor(mCurrentPlayer);
        }

        public int EvaluationFunction() {
            int score = 0;
            if (mMyColor == ReversiCommon.TokenColor.WHITE) {
                score = mCountWhitePieces - mCountBlackPieces;
            } else {
                score = mCountBlackPieces - mCountWhitePieces;
            }
            int cornerBonus = 0;
            BoardToken myColor = BoardTokenFromTokenColor(mMyColor);
            BoardToken oppColor = BoardTokenFromTokenColor(OpponentColor);
            if (Board[0, 0] == myColor) { cornerBonus += 1; }
            if (Board[0, 7] == myColor) { cornerBonus += 1; }
            if (Board[7, 0] == myColor) { cornerBonus += 1; }
            if (Board[7, 7] == myColor) { cornerBonus += 1; }
            if (Board[0, 0] == oppColor) { cornerBonus -= 1; }
            if (Board[0, 7] == oppColor) { cornerBonus -= 1; }
            if (Board[7, 0] == oppColor) { cornerBonus -= 1; }
            if (Board[7, 7] == oppColor) { cornerBonus -= 1; }
            int deadendPenalty = 0;
            if (GetLegalMoves().Count == 0) {
                if (score > 0) {
                    deadendPenalty = 100;
                } else {
                    deadendPenalty = -100;
                }
            }
            return score + 5 * cornerBonus + deadendPenalty;
        }

        private void ClaimLine(ReversiCommon.TokenColor player, int row, int col, int rowDir, int colDir) {
            int startRow = row; // Store the start location for back traversal.
            int startCol = col;
            BoardToken activeColor = BoardTokenFromTokenColor(player);
            BoardToken opponentColor = InvertColor(activeColor);
            if (CanClaimLine(player, ref row, ref col, rowDir, colDir)) {
                row -= rowDir;
                col -= colDir;
                while (row != startRow || col != startCol) {
                    Board[col, row] = activeColor;
                    IncreaseCount(activeColor);
                    DecreaseCount(opponentColor);
                    row -= rowDir;
                    col -= colDir;
                }
            }
        }

        bool CanClaimLine(ReversiCommon.TokenColor player, ref int row, ref int col, int rowDir, int colDir) {
            BoardToken activeColor = BoardTokenFromTokenColor(player);
            BoardToken opponentColor = InvertColor(activeColor);
            row += rowDir;
            col += colDir;
            if (row < 0 || row > 7 || col < 0 || col > 7 || Board[col, row] != opponentColor) {
                // The neighbour must be an opponent token.
                return false;
            }
            do {
                row += rowDir;
                col += colDir;
                if (row < 0 || row > 7 || col < 0 || col > 7) {
                    // Edge of board encountered.
                    return false;
                }
            } while (Board[col, row] == opponentColor);

            // If this line ends with the active player's piece, it's valid.
            return Board[col, row] == activeColor;
        }
        // Function that strips the ref keywords
        bool CanClaimLine(ReversiCommon.TokenColor player, int row, int col, int rowDir, int colDir) {
            return CanClaimLine(player, ref row, ref col, rowDir, colDir);
        }

        public Move GetMove() {
            if (mPredictRoot == null) {
                mPredictRoot = MiniMax.BuildTree(this, mRecursion, false);
            }
            MiniMax.ExtendTree(mPredictRoot, mRecursion, false);

            int chosenIndex = MiniMax.BestMoveIndex(mPredictRoot);
            Move chosenMove = mPredictRoot.childMoves[chosenIndex];
            // Traverse down the tree
            //mPredictRoot = mPredictRoot.childNodes[chosenIndex];
            return chosenMove;
        }

        public bool IsValidMove(ReversiCommon.TokenColor activePlayer, int row, int col) {
            if (Board[col, row] != BoardToken.NONE) {
                return false;
            }
            return
                CanClaimLine(activePlayer, row, col, -1, 1) ||  // +
                CanClaimLine(activePlayer, row, col, 0, 1) ||   // | Right side
                CanClaimLine(activePlayer, row, col, 1, 1) ||   // +
                CanClaimLine(activePlayer, row, col, 1, 0) ||   // - Bottom
                CanClaimLine(activePlayer, row, col, 1, -1) ||  // +
                CanClaimLine(activePlayer, row, col, 0, -1) ||  // | Left side
                CanClaimLine(activePlayer, row, col, -1, -1) || // +
                CanClaimLine(activePlayer, row, col, -1, 0);    // - Top
        }

        public List<Move> GetLegalMoves() {
            List<Move> legalMoves = new List<Move>(8 * 8);
            for (int row = 0; row < 8; row++) {
                for (int col = 0; col < 8; col++) {
                    if (IsValidMove(mCurrentPlayer, row, col)) {
                        legalMoves.Add(new Move(row, col));
                    }
                }
            }
            return legalMoves;
        }


        public string CurrentStateString() {
            StringBuilder boardStr = new StringBuilder();
            boardStr.AppendLine("JoelAI");
            boardStr.AppendLine("  0 1 2 3 4 5 6 7");
            for (int row = 0; row < 8; row++) {
                boardStr.Append(row);
                for (int col = 0; col < 8; col++) {
                    boardStr.Append(" " + BoardTokenToChar(Board[col, row]));
                }
                boardStr.AppendLine();
            }
            boardStr.AppendLine("White pieces: " + mCountWhitePieces + "  Black pieces: " + mCountBlackPieces);
            return boardStr.ToString();
        }


        void IncreaseCount(BoardToken token) {
            if (token == BoardToken.WHITE) {
                mCountWhitePieces++;
            } else if (token == BoardToken.BLACK) {
                mCountBlackPieces++;
            }
        }
        private void IncreaseCount(ReversiCommon.TokenColor token) {
            if (token == ReversiCommon.TokenColor.WHITE) {
                mCountWhitePieces++;
            } else {
                mCountBlackPieces++;
            }
        }
        void DecreaseCount(BoardToken token) {
            if (token == BoardToken.WHITE) {
                mCountWhitePieces--;
            } else if (token == BoardToken.BLACK) {
                mCountBlackPieces--;
            }
        }
        private void DecreaseCount(ReversiCommon.TokenColor token) {
            if (token == ReversiCommon.TokenColor.WHITE) {
                mCountWhitePieces--;
            } else {
                mCountBlackPieces--;
            }
        }

        static BoardToken InvertColor(BoardToken token) {
            if (token == BoardToken.BLACK) {
                return BoardToken.WHITE;
            }
            if (token == BoardToken.WHITE) {
                return BoardToken.BLACK;
            }
            return BoardToken.NONE;
        }
        static ReversiCommon.TokenColor InvertColor(ReversiCommon.TokenColor token) {
            if (token == ReversiCommon.TokenColor.BLACK) {
                return ReversiCommon.TokenColor.WHITE;
            }
            return ReversiCommon.TokenColor.BLACK;
        }
        static BoardToken BoardTokenFromTokenColor(ReversiCommon.TokenColor color) {
            if (color == ReversiCommon.TokenColor.BLACK) {
                return BoardToken.BLACK;
            }
            return BoardToken.WHITE;
        }
        static ReversiCommon.TokenColor TokenColorFromBoardToken(BoardToken token) {
            if (token == BoardToken.WHITE) {
                return ReversiCommon.TokenColor.WHITE;
            }
            if (token == BoardToken.BLACK) {
                return ReversiCommon.TokenColor.BLACK;
            }
            throw new ArgumentException("Board token must contain a player token.");
        }
        static char BoardTokenToChar(BoardToken token) {
            switch (token) {
                case BoardToken.NONE:
                    return '.';
                case BoardToken.WHITE:
                    return 'O';
                case BoardToken.BLACK:
                    return '#';
            }
            return '-';
        }
        public enum BoardToken {
            NONE,
            BLACK,
            WHITE
        }
    }

    static class MiniMax {
        public static int WorstMoveIndex(TreeNode root) {
            int minIndex = 0;
            int minScore = root.childNodes[0].minmax;
            for (int i = 1; i < root.childNodes.Length; i++) {
                if (root.childNodes[i].minmax < minScore) {
                    minScore = root.childNodes[i].minmax;
                    minIndex = i;
                }
            }
            return minIndex;
        }
        public static int BestMoveIndex(TreeNode root) {
            int maxIndex = 0;
            int maxScore = root.childNodes[0].minmax;
            for (int i = 1; i < root.childNodes.Length; i++) {
                if (root.childNodes[i].minmax > maxScore) {
                    maxScore = root.childNodes[i].minmax;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        public static int GetMoveIndex(TreeNode root, Move move) {
            for (int i = 0; i < root.childMoves.Length; i++) {
                if (root.childMoves[i].Row == move.Row &&
                    root.childMoves[i].Col == move.Col) {
                    return i;
                }
            }
            return -1;
        }

        public static void ExtendTree(TreeNode node, int depth, bool min) {
            if (depth < 1) {
                return;
            }

            int minmaxValue = min ? 2000 : -2000;
            for (int i = 0; i < node.childNodes.Length; i++) {
                if (node.childNodes[i] == null) {
                    JoelAI tempState = new JoelAI(node.state);
                    tempState.MakeMove(node.childMoves[i]);
                    node.childNodes[i] = BuildTree(tempState, depth - 1, !min);
                } else {
                    ExtendTree(node.childNodes[i], depth - 1, !min);
                }

                int score = node.childScores[i];
                if (node.childNodes[i] != null) {
                    score = node.childNodes[i].minmax;
                }
                if ((min && score < minmaxValue) || (!min && score > minmaxValue)) {
                    minmaxValue = score;
                }
            }
            node.minmax = minmaxValue;
        }

        public static TreeNode BuildTree(JoelAI state, int depth, bool min) {
            if (depth < 1) {
                return null;
            }

            TreeNode node = new TreeNode();
            List<Move> legalMoves = state.GetLegalMoves();
            node.childNodes = new TreeNode[legalMoves.Count];
            node.childScores = new int[legalMoves.Count];
            node.childMoves = new Move[legalMoves.Count];
            node.state = state;

            int minmaxValue = min ? 2000 : -2000;
            for (int i = 0; i < legalMoves.Count; i++) {
                JoelAI tempState = new JoelAI(state);
                tempState.MakeMove(legalMoves[i]);
                node.childScores[i] = tempState.EvaluationFunction();
                node.childMoves[i] = legalMoves[i];
                node.childNodes[i] = BuildTree(tempState, depth - 1, !min);

                int score = node.childScores[i];
                if (node.childNodes[i] != null) {
                    score = node.childNodes[i].minmax;
                }
                if ((min && score < minmaxValue) || (!min && score > minmaxValue)) {
                    minmaxValue = score;
                }
            }
            node.minmax = minmaxValue;
            return node;
        }

        public class TreeNode {
            public TreeNode[] childNodes;
            public int[] childScores;
            public Move[] childMoves;
            public JoelAI state;

            public int minmax;
        }
    }
}
