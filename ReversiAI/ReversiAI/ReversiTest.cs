using System;
using System.Collections.Generic;

using ReversiTournament;

// Requires ReferenceImplementation.dll; my AI uses minimax with heuristic
//   weights from play-othello.appspot.com/files/Othello.pdf

// This is a simple example to show you how the interface will be used.  The tournament code will be different, but
//   it will call your code in a similar way.  It will have some extra logic to handle win conditions, of course,
//   and won't be capped at 20 turns.

// Recall the we're using this nonstandard rule: if it is a player's turn to take a move
//   but no moves are available, that player loses the game (doesn't apply when the board is full).
//   Since the simple example below doesn't handle win conditions, it will instead throw an exception
//   if it reaches a state with no outgoing moves

// Also, remember that the row and column indices in Move start at 0!

public class ReversiTest
{
	public static void Main()
	{
		ReversiMoves blackAgent = new ReferenceAI(3); //my AI with depth 3 lookahead
		ReversiMoves whiteAgent = new JoelAI();

		blackAgent.Reset(ReversiCommon.TokenColor.BLACK, ReversiCommon.TokenColor.BLACK);
		whiteAgent.Reset(ReversiCommon.TokenColor.WHITE, ReversiCommon.TokenColor.BLACK);

		Console.WriteLine(blackAgent);

		// take the first 20 turns
		for(int i = 0; i < 20; i += 2) // we increment by 2 because each iteration is a black and a white turn
		{
			Move blackMove = blackAgent.GetMove();

			blackAgent.MakeMove(blackMove); // remember to update both agents!
			whiteAgent.MakeMove(blackMove);

            Console.WriteLine(((ReferenceAI)blackAgent).CurrentStateString());

            Move whiteMove = whiteAgent.GetMove();

			blackAgent.MakeMove(whiteMove);
			whiteAgent.MakeMove(whiteMove);

            Console.WriteLine(((JoelAI)whiteAgent).CurrentStateString());
		}

        Console.ReadKey();
	}
}