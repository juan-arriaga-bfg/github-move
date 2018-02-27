public class EnemyPieceBuilder : GenericPieceBuilder
{
    public int HitPoints;
    
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);
        
        piece.RegisterComponent(new LivePieceComponent {HitPoints = this.HitPoints});
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionFightEnd())
            .RegisterComponent(new TouchReactonConditionFight()));
		
        return piece;
    }
}