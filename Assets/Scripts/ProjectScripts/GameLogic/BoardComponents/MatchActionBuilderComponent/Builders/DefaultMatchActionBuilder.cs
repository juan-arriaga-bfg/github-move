using DG.Tweening;

public class DefaultMatchActionBuilder
{
    protected void SpawnReward(BoardPosition position, int pieceType)
    {
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);
        
        if(def == null || def.CreateRewards == null) return;

        var sequence = DOTween.Sequence();
        
        for (var i = 0; i < def.CreateRewards.Count; i++)
        {
            var reward = def.CreateRewards[i];
            sequence.InsertCallback(0.5f*i, () => AddResourceView.Show(position, reward));
        }
    }
}