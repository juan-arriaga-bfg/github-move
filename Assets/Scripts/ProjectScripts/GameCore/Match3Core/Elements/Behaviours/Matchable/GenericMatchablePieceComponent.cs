public class GenericMatchablePieceComponent : MatchablePieceComponent
{
    public override bool IsMatchable(BoardPosition at)
    {
        int currentLayer = at.Z;
        for (int layer = 0; layer < context.Context.BoardDef.Depth; layer++)
        {
            var point = new BoardPosition(at.X, at.Y, layer);
            var piece = context.Context.BoardLogic.GetPieceAt(point);
            var overrideMatch = piece == null ? null : piece.GetComponent<OverrideMatchPieceComponent>(OverrideMatchPieceComponent.ComponentGuid);
            if (overrideMatch != null)
            {
                bool state = overrideMatch.IsMatchableAtLayer(currentLayer);
                if (state == false)
                {
                    return state;
                }
            }
        }
        return true;
    }

    public override bool OnMatchStart(BoardPosition at)
    {
        return true;
    }

    public override bool OnMatchFinish(BoardPosition at)
    {

        return true;
    }
}