public class DefaultMatchActionBuilder
{
    protected void SpawnReward(BoardPosition position, int pieceType)
    {
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);
        
        if(def?.CreateRewards == null) return;

        AddResourceView.Show(position, def.CreateRewards);
    }
}