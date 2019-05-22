using System;
using System.Collections.Generic;

public class EjectionPieceAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition? From;
	public Func<BoardPosition> GetFrom;
	public List<BoardPosition> To;
	
	public Dictionary<int, int> Pieces { get; set; }
	
	public Action OnComplete { get; set; }
    
    public Action<List<BoardPosition>> OnSuccess;
	
	public Func<int, string> AnimationResourceSearch;

	public bool PerformAction(BoardController gameBoardController)
	{
		if (From == null) From = GetFrom?.Invoke();
		if (From == null) return false;

		var logic = gameBoardController.BoardLogic;
		var pieces = new Dictionary<BoardPosition, Piece>();
		
		foreach (var pair in Pieces)
		{
			var field = To ?? new List<BoardPosition>();

			if (logic.EmptyCellsFinder.CheckInFrontOrFindRandomNear(From.Value, field, pair.Value) == false) break;
			
			foreach (var pos in field)
			{
				var piece = gameBoardController.CreatePieceFromType(pair.Key);

				if (logic.AddPieceToBoard(pos.X, pos.Y, piece) == false) continue;
				
				pieces.Add(pos, piece);
				logic.LockCell(pos, this);
			}
		}
		
		logic.LockCell(From.Value, this);
		
		if (AnimationResourceSearch == null) AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnMultiSpawn);
		
		var animation = new ReproductionPieceAnimation
		{
			From = From.Value,
			Pieces = pieces,
			AnimationResourceSearch = AnimationResourceSearch
		};
		
		var result = new List<BoardPosition>();
		    
		foreach (var piece in pieces)
		{
			result.Add(piece.Value.Multicellular?.GetTopPosition ?? piece.Value.CachedPosition);
		}
			
		OnSuccess?.Invoke(result);

		animation.OnCompleteEvent += (_) =>
		{
			logic.UnlockCell(From.Value, this);

			foreach (var pair in pieces)
			{
				logic.UnlockCell(pair.Key, this);
			}

			OnComplete?.Invoke();
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	}
}