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
        
        if (string.IsNullOrEmpty(currencyTask.Currency))
        {
            return false;
        }

        if (currencyTask.Currency == Currency.Coins.Name)
        {
            return HighlightSoft(currencyTask);
        }  
        
        if (currencyTask.Currency == Currency.Crystals.Name)
        {
            return new HighlightTaskNotImplemented().Highlight(currencyTask);
        }
        
        if (currencyTask.Currency == Currency.Mana.Name)
        {
            return HighlightOrdersButton(currencyTask);
        }

        Debug.LogError($"[HighlightTaskCurrencyCollect] => Unsupported Currency type: '{currencyTask.Currency}'");
        
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
            if (HighlightTaskPointToPiece.FindAndPointToPiece(id))
            {
                return true;
            }
        }

        return false;
    }
}