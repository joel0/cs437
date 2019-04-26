using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiTournament {
    class JoelAI : ReversiMoves {
        BoardToken[,] mBoard = new BoardToken[8, 8];
        int mCountBlackPieces;
        int mCountWhitePieces;
        ReversiCommon.TokenColor mMyColor;
        ReversiCommon.TokenColor mCurrentPlayer;
        ReversiCommon.TokenColor OpponentColor {
            get {
                if (mMyColor == ReversiCommon.TokenColor.BLACK) {
                    return ReversiCommon.TokenColor.WHITE;
                }
                return ReversiCommon.TokenColor.BLACK;
            }
        }

        public void Reset(ReversiCommon.TokenColor yourColor, ReversiCommon.TokenColor firstPlayerColor) {
            mBoard = new BoardToken[8, 8];
            mBoard[3, 3] = BoardToken.WHITE;
            mBoard[4, 4] = BoardToken.WHITE;
            mBoard[4, 3] = BoardToken.BLACK;
            mBoard[3, 4] = BoardToken.BLACK;
            mCountBlackPieces = 2;
            mCountWhitePieces = 2;
            mMyColor = yourColor;
            mCurrentPlayer = firstPlayerColor;
        }

        public void MakeMove(Move move) {
            ClaimLine(mCurrentPlayer, move.Row, move.Col, -1, 1);  // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 0, 1);   // | Right side
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 1, 1);   // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 1, 0);   // - Bottom
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 1, -1);  // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, 0, -1);  // | Left side
            ClaimLine(mCurrentPlayer, move.Row, move.Col, -1, -1); // +
            ClaimLine(mCurrentPlayer, move.Row, move.Col, -1, 0);  // - Top
            mBoard[move.Col, move.Row] = BoardTokenFromTokenColor(mCurrentPlayer);
            IncreaseCount(mCurrentPlayer);
            mCurrentPlayer = InvertColor(mCurrentPlayer);
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
                    mBoard[col, row] = activeColor;
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
            if (row < 0 || row > 7 || col < 0 || col > 7 || mBoard[col, row] != opponentColor) {
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
            } while (mBoard[col, row] == opponentColor);

            // If this line ends with the active player's piece, it's valid.
            return mBoard[col, row] == activeColor;
        }
        // Function that strips the ref keywords
        bool CanClaimLine(ReversiCommon.TokenColor player, int row, int col, int rowDir, int colDir) {
            return CanClaimLine(player, ref row, ref col, rowDir, colDir);
        }

            public Move GetMove() {
            for (int row = 0; row < 8; row++) {
                for (int col = 0; col < 8; col++) {
                    if (IsValidMove(mCurrentPlayer, row, col)) {
                        return new Move(row, col);
                    }
                }
            }
            return null;
        }

        bool IsValidMove(ReversiCommon.TokenColor activePlayer, int row, int col) {
            if (mBoard[col, row] != BoardToken.NONE) {
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


        public string CurrentStateString() {
            StringBuilder boardStr = new StringBuilder();
            boardStr.AppendLine("JoelAI");
            boardStr.AppendLine("  0 1 2 3 4 5 6 7");
            for (int row = 0; row < 8; row++) {
                boardStr.Append(row);
                for (int col = 0; col < 8; col++) {
                    boardStr.Append(" " + BoardTokenToChar(mBoard[col, row]));
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
        enum BoardToken {
            NONE,
            BLACK,
            WHITE
        }
    }
}
