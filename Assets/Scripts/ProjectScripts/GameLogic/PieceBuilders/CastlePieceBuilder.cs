public class CastlePieceBuilder : MulticellularSpawnPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

        if (touchReaction != null)
        {
            var touchMenu = touchReaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);

            if (touchMenu != null)
            {
                touchMenu.RegisterDefinition(new TouchReactionDefinitionOpenHeroesWindow(), "face_Robin");
            }
        }
        
        AddView(piece, ViewType.BoardTimer);
        
        return piece;
    }
}