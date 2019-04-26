using System;
using System.Collections.Generic;

namespace ReversiTournament
{
	public class ReferenceAI : ReversiMoves
	{
		ReferenceImplementation ri;

		public ReferenceAI(int level)
		{
			ri = new ReferenceImplementation(level);
		}

		public void Reset(ReversiCommon.TokenColor yourColor, ReversiCommon.TokenColor firstPlayerColor)
		{
			ri.Reset(firstPlayerColor == ReversiCommon.TokenColor.BLACK);
		}

		public Move GetMove()
		{
			Tuple<int, int> move = ri.GetMove();
			return new Move(move.Item2, move.Item1);
		}

		public void MakeMove(Move move)
		{
			ri.MakeMove(move.Col, move.Row);
		}

		public string CurrentStateString()
		{
			return ri.CurrentStateString();
		}
	}
}