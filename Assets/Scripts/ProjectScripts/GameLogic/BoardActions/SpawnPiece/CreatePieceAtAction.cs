using System;
using UnityEngine;

public class CreatePieceAtAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition At { get; set; }
	
	public int PieceTypeId { get; set; }
	
	public Action OnComplete;

	public string SpawnResource;
	
	public bool PerformAction(BoardController gameBoardController)
	{
		var piece = gameBoardController.CreatePieceFromType(PieceTypeId);
		
		At = new BoardPosition(At.X, At.Y, piece.Layer.Index);
		
		if (At.IsValid == false) return false;
		
		if (gameBoardController.BoardLogic.IsLockedCell(At)) return false;

		if (gameBoardController.BoardLogic.AddPieceToBoard(At.X, At.Y, piece) == false) return false;
		
		gameBoardController.BoardLogic.LockCell(At, this);
		
		var animation = new SpawnPieceAtAnimation
		{
			CreatedPiece = piece,
			At = At,
			AnimationResource = SpawnResource
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(At, this);
			OnComplete?.Invoke();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		
		return true;
	}
}