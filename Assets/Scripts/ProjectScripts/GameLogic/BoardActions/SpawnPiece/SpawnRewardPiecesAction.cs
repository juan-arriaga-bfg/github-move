using System;
using System.Collections.Generic;

public class SpawnRewardPiecesAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;

	public BoardPosition? From;
	public Func<BoardPosition> GetFrom;
	
	public bool EnabledTopHighlight = false;
	public bool EnabledBottomHighlight = false;
	
	public Dictionary<int, int> Pieces { get; set; }
	
	public Action OnComplete { get; set; }
    
    public Action<List<BoardPosition>> OnSuccess;
	
	public Func<int, string> AnimationResourceSearch;

	public bool PerformAction(BoardController gameBoardController)
	{
		if (From == null) From = GetFrom?.Invoke();
		if (From == null) return false;
		
		var pieces = new Dictionary<BoardPosition, Piece>();
		
		foreach (var pair in Pieces)
		{
			var field = new List<BoardPosition>();
        
			if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From.Value, field, pair.Value) == false)
			{
				break;
			}
			
			foreach (var pos in field)
			{
				var piece = gameBoardController.CreatePieceFromType(pair.Key);

				if (gameBoardController.BoardLogic.AddPieceToBoard(pos.X, pos.Y, piece) == false)
				{
					continue;
				}
			
				pieces.Add(pos, piece);
				gameBoardController.BoardLogic.LockCell(pos, this);
			}
		}
		
		gameBoardController.BoardLogic.LockCell(From.Value, this);
		if (AnimationResourceSearch == null)
			AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnMultiSpawn);
		var animation = new SpawnRewardPiecesAnimation()
		{
			Pieces = pieces,
			AnimationResourceSearch = AnimationResourceSearch,
			EnabledTopHighlight = EnabledTopHighlight,
			EnabledBottomHighlight =  EnabledBottomHighlight
		};

		animation.OnCompleteEvent += (_) =>
		{
			gameBoardController.BoardLogic.UnlockCell(From.Value, this);

			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}

			OnComplete?.Invoke();
		    
		    var result = new List<BoardPosition>();
		    
		    foreach (var piece in pieces)
		    {
		        result.Add(piece.Value.Multicellular?.GetTopPosition ?? piece.Value.CachedPosition);
		    }
			
		    OnSuccess?.Invoke(result);
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);

		return true;
	} 
	
	
}