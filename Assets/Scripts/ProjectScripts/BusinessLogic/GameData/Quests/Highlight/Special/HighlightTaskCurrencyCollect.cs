using Debug = IW.Logger;
using UnityEngine;

public class HighlightTaskCurrencyCollect : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        TaskCurrencyCollectEntity currencyTask = task as TaskCurrencyCollectEntity;
        if (currencyTask == null)
        {
            Debug.LogError("[HighlightTaskCurrencyCollect] => task is not TaskCurrencyCollectEntity");
            return false;
        }
        
        if (string.IsNullOrEmpty(currencyTask.CurrencyName))
        {
            return false;
        }

        if (currencyTask.CurrencyName == Currency.Coins.Name)
        {
            return HighlightSoft(currencyTask);
        }  
        
        if (currencyTask.CurrencyName == Currency.Crystals.Name)
        {
            return new HighlightTaskPointToMarketButton().Highlight(task);
        }
        
        if (currencyTask.CurrencyName == Currency.Mana.Name)
        {
            return HighlightOrdersButton(currencyTask);
        }

        Debug.LogError($"[HighlightTaskCurrencyCollect] => Unsupported Currency type: '{currencyTask.CurrencyName}'");
        
        return false;
    }

    private bool HighlightSoft(TaskCurrencyCollectEntity task)
    {
        if (HighlightChain(PieceType.Soft1.Id))
        {
            return true;
        }

        if (HighlightOrdersButton(task))
        {
            return true;
        }

        return false;
    }

    private bool HighlightOrdersButton(TaskCurrencyCollectEntity task)
    {
        return new HighlightTaskPointToOrdersButton().Highlight(task);
    }

    private bool HighlightChain(int pieceId)
    {
        var board = BoardService.Current.FirstBoard;
        var matchDef = board.BoardLogic.GetComponent<MatchDefinitionComponent>(MatchDefinitionComponent.ComponentGuid);

        var chain = matchDef.GetChain(pieceId);

        foreach (var id in chain)
        {
            if (HighlightTaskPointToPieceHelper.FindAndPointToRandomPiece(id))
            {
                return true;
            }
        }

        return false;
    }
}