using Godot;
using System;
using System.Collections.Generic;

public partial class Bitboard : Node
{
	public ulong[] whitePieces = { 0, 0, 0, 0, 0, 0 };
	public ulong[] blackPieces = { 0, 0, 0, 0, 0, 0 };


	public ulong GetBlackBitBoard()
	{
		ulong ans = 0;
		foreach (ulong i in blackPieces)
		{
			ans |= i;
		}
		return ans;
	}
	public ulong GetWhiteBitBoard()
	{
		ulong ans = 0;
		foreach (ulong i in whitePieces)
		{
			ans |= i;
		}
		return ans;
	}

	public void ClearBitBoard()
	{
		Array.Clear(whitePieces);
		Array.Clear(blackPieces);

	}
	public void InitBitBoard(string fen)
	{
		ClearBitBoard();
		string[] fen_split = fen.Split(" ");
		foreach (char i in fen_split[0])
		{
			if (i.Equals('/'))
				continue;
			if (Char.IsDigit(i))
			{
				int shiftAmount = int.Parse(i.ToString());
				LeftShift(shiftAmount);
				continue;
			}
			LeftShift(1);
			if (Char.IsUpper(i))
			{
				whitePieces[DataHandlerCs.FenDict[Char.ToLower(i)]] |= 1UL;
			}
			else
			{
				blackPieces[DataHandlerCs.FenDict[i]] |= 1UL;
			}
		}
		GD.Print("Bitboard init successfully");
	}
	private void LeftShift(int shiftAmount)
	{
		for (int piece = 0; piece < blackPieces.Length; piece++)
		{
			blackPieces[piece] <<= shiftAmount;
		}
		for (int piece = 0; piece < whitePieces.Length; piece++)
		{
			whitePieces[piece] <<= shiftAmount;
		}
	}
	
	public void SetBoard(ulong[] Whites, ulong[] Blacks)
	{
		Array.Copy(Whites,whitePieces,Whites.Length);
		Array.Copy(Blacks,blackPieces,Blacks.Length);
	}
	public ulong GetBitBoard()
	{
		return blackPieces[3];
	}

	public void RemovePiece(int location , int pieceType)
	{
		if (pieceType > 5)
		{
			blackPieces[pieceType%6] &= ~(1UL<<location);
		}
		else
		{
			whitePieces[pieceType%6] &= ~(1UL<<location);
		}
	}
	public void AddPiece(int location,int pieceType)
	{
		if (pieceType > 5)
		{
			blackPieces[pieceType%6] |= 1UL<<location;
		}
		else
		{
			whitePieces[pieceType%6] |= (1UL<<location);
		}
	}

	public void MakeMove(DataHandlerCs.Move move,bool isBlack)
	{
		ulong[] fromList = isBlack? blackPieces: whitePieces;
		ulong[] toList = isBlack? whitePieces :  blackPieces;

		ulong fromBit = 1UL << move.From;
		ulong toBit = 1UL << move.To;
		for(int i = 0 ; i < 6 ; i++)
		{
			toList[i] &= ~(toBit);
		}
		for(int i = 0 ; i < 6 ; i++)
		{
			if((fromList[i] & fromBit) != 0)
			{
				fromList[i] &= ~fromBit;
				fromList[i] |= toBit;
			}
		}
	}

	public List<DataHandlerCs.Move> GenerateMoveSet(bool isBlackMove)
	{
		ulong[] searchList;
		ulong selfboard,enemyboard;
		List<DataHandlerCs.Move> moveSet = new();
		GeneratePath pathGenerator = new();
		if (isBlackMove)
		{
			selfboard = GetBlackBitBoard();
			enemyboard = GetWhiteBitBoard();
			searchList = blackPieces;
		}
		else
		{
			selfboard = GetWhiteBitBoard() ;
			enemyboard = GetBlackBitBoard();
			searchList = whitePieces;
		}

		//bishop
		for(int i = 0 ; i<64 ; i++)
		{
			if((searchList[0]&(1UL<<i)) != 0)
			{
				ulong currentMoves = pathGenerator.BishopPath(i,selfboard,enemyboard,isBlackMove);
				for(int j = 0; j< 64; j++)
				{
					if((currentMoves & (1UL << j)) != 0)
					{
						DataHandlerCs.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}

			}
		}
		//King
		for(int i = 0; i<64;i++){
			if((searchList[1] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.KingPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCs.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Knight
		for(int i = 0; i<64;i++){
			if((searchList[2] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.KnightPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCs.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Generate Pawn moveset
		for(int i = 0; i<64;i++){
			if((searchList[3] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.PawnPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCs.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Queen
		for(int i = 0; i<64;i++){
			if((searchList[4] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.QueenPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCs.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Rook
		for(int i = 0; i<64;i++){
			if((searchList[5] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.RookPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCs.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}

		return moveSet;

	}
}
