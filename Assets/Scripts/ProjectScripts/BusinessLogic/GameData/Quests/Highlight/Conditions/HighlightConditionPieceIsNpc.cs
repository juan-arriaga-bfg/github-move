using IW;

public class HighlightConditionPieceIsNpc: ITaskHighlightCondition
{
    public bool Check(TaskEntity task)
    {
        var pieceTask = task as IHavePieceId;
        if (pieceTask == null)
        {
            Logger.LogError($"[HighlightConditionPieceIsNpc] => Task {task.GetType()} is not IHavePieceId");
            return false;
        }

        var chain = GameDataService.Current.MatchDefinition.GetChain(pieceTask.PieceId);
        for (int i = chain.Count - 1; i >= 0; i--)
        {
            PieceTypeDef def = PieceType.GetDefById(chain[i]);
            if (def.Filter.Has(PieceTypeFilter.Character))
            {
                return true;
            }
        }

        return false;
    }
}