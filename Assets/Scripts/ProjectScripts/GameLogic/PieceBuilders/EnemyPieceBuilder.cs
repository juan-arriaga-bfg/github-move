public class EnemyPieceBuilder : GenericPieceBuilder
{
    public override Piece Build(int pieceType, BoardController context)
    {
        var piece = base.Build(pieceType, context);

        var enemy = GameDataService.Current.GetEnemy(GameDataService.Current.EnemyIndex - 1);
        
        piece.RegisterComponent(new LivePieceComponent {HitPoints = enemy.HP, MaxHitPoints = enemy.HP});
        
        piece.RegisterComponent(new TouchReactionComponent()
            .RegisterComponent(new TouchReactionDefinitionFightEnd())
            .RegisterComponent(new TouchReactonConditionFight()));
		
        return piece;
    }
}