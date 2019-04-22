using System.Collections.Generic;

public class OrderDef
{
    public string Uid;
    public int Level = -1;
    public int Delay;
    
    public List<CurrencyPair> Rewards
    {
        get
        {
            var rewards = new List<CurrencyPair>(NormalRewards);

            if (EventRewards != null && GameDataService.Current.EventManager.IsStarted(EventName.OrderSoftLaunch)) rewards.AddRange(EventRewards);

            return rewards;
        }
    }

    public List<CurrencyPair> NormalRewards;
    public List<CurrencyPair> EventRewards;
    
    public List<CurrencyPair> Prices;
    
    private bool isUnlocked;

    public bool IsUnlocked()
    {
        if (isUnlocked) return true;
        
        var board = BoardService.Current?.FirstBoard;

        if (board == null) return false;
        
        foreach (var price in Prices)
        {
            var id = PieceType.Parse(price.Currency);
            var prev = board.BoardLogic.MatchDefinition.GetPrevious(id);

            if (GameDataService.Current.CodexManager.IsPieceUnlocked(prev)) continue;

            return false;
        }

        isUnlocked = true;
        
        return true;
    }
}