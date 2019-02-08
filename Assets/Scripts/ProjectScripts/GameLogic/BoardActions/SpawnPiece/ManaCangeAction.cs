using System.Collections.Generic;
using System.Linq;

public class ManaCangeAction : IBoardAction
{
	public static readonly int ComponentGuid = ECSManager.GetNextGuid();
	public virtual int Guid => ComponentGuid;
	
	public BoardPosition Target;
	public BoardPosition Old;
	public BoardPosition From;
	public Dictionary<int, int> Pieces { get; set; }

	public bool PerformAction(BoardController gameBoardController)
	{
		var amount = Pieces.Sum(pair => pair.Value);
		var pieces = new Dictionary<BoardPosition, Piece>();
		var field = new List<BoardPosition>();

		gameBoardController.BoardLogic.RemovePieceAt(Old);

		if (gameBoardController.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(From, field, amount) == false) return false;
		
		foreach (var pair in Pieces)
		{
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

		var collapse = new CollapsePieceToAnimation
		{
			To = Target,
			Positions = new List<BoardPosition>{Old},
			AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnDestroyFromBoard)
		};
		
		var animation = new ReproductionPieceAnimation
		{
			From = From,
			Pieces = pieces,
			AnimationResourceSearch = piece => AnimationOverrideDataService.Current.FindAnimation(piece, def => def.OnMultiSpawn)
		};

		animation.OnCompleteEvent += (_) =>
		{
			foreach (var pair in pieces)
			{
				gameBoardController.BoardLogic.UnlockCell(pair.Key, this);
			}
		};
		
		gameBoardController.RendererContext.AddAnimationToQueue(animation);
		gameBoardController.RendererContext.AddAnimationToQueue(collapse);
		

		return true;
	}
}