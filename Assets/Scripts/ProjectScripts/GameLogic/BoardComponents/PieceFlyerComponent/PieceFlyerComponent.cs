public class PieceFlyerComponent : ECSEntity, ILockerComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private BoardLogicComponent context;
    
    private LockerComponent locker;
    public virtual LockerComponent Locker => locker ?? (locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid));
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardLogicComponent;
    }

    private bool IsAnyActiveQuestAboutPiece(Piece piece)
    {
        var activeQuests = GameDataService.Current.QuestsManager.ActiveQuests;

        for (var questIndex = 0; questIndex < activeQuests.Count; questIndex++)
        {
            var quest = activeQuests[questIndex];
            for (var taskIndex = 0; taskIndex < quest.Tasks.Count; taskIndex++)
            {
                var task      = quest.Tasks[taskIndex];
                var pieceTask = task as IHavePieceId;

                if (pieceTask != null)
                {
                    if (pieceTask.PieceId == piece.PieceType || pieceTask.PieceId == PieceType.None.Id || pieceTask.PieceId == PieceType.Empty.Id )
                    return true;
                }
            }
        }

        return false;
    }
    
    public void FlyToQuest(Piece piece)
    {
        if (Locker.IsLocked || IsAnyActiveQuestAboutPiece(piece) == false) return;
        
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.PieceBuilded, piece.PieceType);
        
        // var flay = ResourcesViewManager.Instance.GetFirstViewById(PieceType.Parse(piece.PieceType));
        // flay?.UpdateResource(1);
    }

    
    public void FlyTo(Piece piece, int x, int y, string target)
    {
        if (Locker.IsLocked || GameDataService.Current.CodexManager.OnPieceBuilded(piece.PieceType) == false) return;

        FlyToTarget(piece, x, y, target);
        GameDataService.Current.CharactersManager.UnlockNewCharacter(piece.PieceType);
    }

    public void FlyToTarget(Piece piece, BoardPosition position, string target)
    {
        FlyToTarget(piece, position.X, position.Y, target);
    }
    
    public void FlyToTarget(Piece piece, int x, int y, string target)
    {
        if (Locker.IsLocked) return;
        
        var currency = PieceType.Parse(piece.PieceType);
        var flay = ResourcesViewManager.Instance.GetFirstViewById(target);
        
        if (flay == null) return;

        var from = context.Context.BoardDef.GetPiecePosition(x, y);
        
        var carriers = ResourcesViewManager.DeliverResource<ResourceCarrierWithObject>
        (
            target,
            1,
            flay.GetAnchorRect(),
            context.Context.BoardDef.ViewCamera.WorldToScreenPoint(from),
            R.ResourceCarrierWithObject
        );
        
        if (carriers.Count != 0) carriers[0].RefreshIcon(currency);
    }

}