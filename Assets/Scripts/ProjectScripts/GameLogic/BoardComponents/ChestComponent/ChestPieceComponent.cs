using System.Collections.Generic;

public class ChestPieceComponent : IECSComponent, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;
    
    public RewardsStoreComponent Rewards;
    public ChestDef Def;

    private Piece contextPiece;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        
        Def = GameDataService.Current.ChestsManager.GetChest(contextPiece.PieceType);
        
        Rewards = new RewardsStoreComponent
        {
            GetRewards = GetRewards,
            OnComplete = OnOpen,
            IsTargetReplace = true
        };
        
        contextPiece.RegisterComponent(Rewards);
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Rewards.InitInSave(position);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }
    
    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }
    
    private Dictionary<int, int> GetRewards()
    {
        var hard = GameDataService.Current.LevelsManager.GetSequence(Currency.Level.Name);
        var resources = GameDataService.Current.LevelsManager.GetSequence(Currency.Resources.Name);
        var sequence = GameDataService.Current.ChestsManager.GetSequence(Def.Uid);
        
        var character = GameDataService.Current.CharactersManager.GetSequence(
            contextPiece.PieceType == PieceType.CH_NPC.Id
                ? GameDataService.Current.CharactersManager.ChestKey
                : Currency.Character.Name);

        var productionAmount = Def.ProductionAmount.Range();
        var resourcesAmount = Def.ResourcesAmount.Range();
        var charactersAmount = Def.CharactersAmount.Range();
        
        var reward = hard.GetNextDict(productionAmount);
        
        reward = resources.GetNextDict(resourcesAmount, reward);
        reward = sequence.GetNextDict(Def.PieceAmount, reward);
        reward = character.GetNextDict(charactersAmount, reward);
        
        return reward;
    }

    private void OnOpen(bool isComplete)
    {
        if(isComplete) BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.OpenChest, contextPiece);
    }
}