using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class ChessBot : Node
{

	public int searchCounter = 0;
	public int maxDepth = 0;

	public Bitboard currentboard;

	public DataHandlerCs.Move currentMove = new(-1,-1);
	public List<DataHandlerCs.Move> bestMoves = new();
	public DataHandlerCs DH = new();
	public Random rng = new();

	public void InitBot(Bitboard board)
	{
		currentboard = board;
	}

	public int SearchMoves(bool isBlackMove,int depth,Bitboard searchboard,int alpha = int.MinValue+2,int beta = int.MaxValue )
	{
		searchCounter++;
		if(depth == 0)
		{
			return Evaluate(isBlackMove,searchboard);
		}
		List<DataHandlerCs.Move> moves = searchboard.GenerateMoveSet(isBlackMove);

		if (depth == maxDepth)
			bestMoves.Clear();

		foreach(DataHandlerCs.Move move in moves)
		{
			Bitboard newBoard = new();
			newBoard.SetBoard(searchboard.whitePieces,searchboard.blackPieces);
			newBoard.MakeMove(move,isBlackMove);
			int evaluation = -SearchMoves(!isBlackMove,depth-1,newBoard,-beta,-alpha);

			if(depth == maxDepth)
			{
				if (evaluation > alpha)
				{
					bestMoves.Clear();
					bestMoves.Add(move);
				}
				else if (evaluation == alpha)
				{
					bestMoves.Add(move);
				}
			}

			alpha = Math.Max(evaluation,alpha);
			if (evaluation >= beta)
			{
				return beta;
			}
		}
		return alpha;
	}

	public int[] FindNextMove(bool botIsBlack)
	{
		searchCounter = 0;
		maxDepth = 4;

		SearchMoves(botIsBlack,maxDepth,currentboard);

		// Prefer captures among tied-best moves
		ulong enemyBoard = botIsBlack ? currentboard.GetWhiteBitBoard() : currentboard.GetBlackBitBoard();
		List<DataHandlerCs.Move> captureMoves = new();
		foreach (var m in bestMoves)
		{
			if ((enemyBoard & (1UL << m.To)) != 0)
				captureMoves.Add(m);
		}

		List<DataHandlerCs.Move> pickFrom = captureMoves.Count > 0 ? captureMoves : bestMoves;
		currentMove = pickFrom[rng.Next(pickFrom.Count)];

		int[] nextMove = {currentMove.From,currentMove.To};
		currentboard.MakeMove(currentMove,botIsBlack);

		return nextMove;
	}


	public int Evaluate(bool isBlackMove,Bitboard searchboard)
	{
		int whiteValues = pieceValues(searchboard.whitePieces);
		int blackValues = pieceValues(searchboard.blackPieces);
		int evaluation = whiteValues- blackValues;
		return isBlackMove? -evaluation : evaluation;
	}


	public int pieceValues(ulong[] pieces)
	{
		int totalValue = 0;
		for(int i=0; i < 6; i++)
		{
			totalValue += BitOperations.PopCount(pieces[i])*DH.pieceValues[i];
		}
		return totalValue;
	}
}