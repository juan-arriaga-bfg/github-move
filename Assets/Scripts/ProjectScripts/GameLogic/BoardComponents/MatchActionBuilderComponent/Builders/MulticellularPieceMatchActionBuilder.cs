using System.Collections.Generic;
using DG.Tweening;

public class MulticellularPieceMatchActionBuilder : IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.Mine1.Id,
            PieceType.Mine2.Id,
            PieceType.Mine3.Id,
            PieceType.Mine4.Id,
            PieceType.Mine5.Id,
            PieceType.Mine6.Id,
            PieceType.Mine7.Id,
            
            PieceType.Sawmill1.Id,
            PieceType.Sawmill2.Id,
            PieceType.Sawmill3.Id,
            PieceType.Sawmill4.Id,
            PieceType.Sawmill5.Id,
            PieceType.Sawmill6.Id,
            PieceType.Sawmill7.Id,
            
            PieceType.Sheepfold1.Id,
            PieceType.Sheepfold2.Id,
            PieceType.Sheepfold3.Id,
            PieceType.Sheepfold4.Id,
            PieceType.Sheepfold5.Id,
            PieceType.Sheepfold6.Id,
            PieceType.Sheepfold7.Id,
            
            PieceType.Castle1.Id,
            PieceType.Castle2.Id,
            PieceType.Castle3.Id,
            PieceType.Castle4.Id,
            PieceType.Castle5.Id,
            PieceType.Castle6.Id,
            PieceType.Castle7.Id,
            PieceType.Castle8.Id,
            PieceType.Castle9.Id,
            
            PieceType.Market1.Id,
            PieceType.Market2.Id,
            PieceType.Market3.Id,
            PieceType.Market4.Id,
            PieceType.Market5.Id,
            PieceType.Market6.Id,
            PieceType.Market7.Id,
            PieceType.Market8.Id,
            PieceType.Market9.Id,
            
            PieceType.Storage1.Id,
            PieceType.Storage2.Id,
            PieceType.Storage3.Id,
            PieceType.Storage4.Id,
            PieceType.Storage5.Id,
            PieceType.Storage6.Id,
            PieceType.Storage7.Id,
            PieceType.Storage8.Id,
            PieceType.Storage9.Id,
            
            PieceType.Factory1.Id,
            PieceType.Factory2.Id,
            PieceType.Factory3.Id,
            PieceType.Factory4.Id,
            PieceType.Factory5.Id,
            PieceType.Factory6.Id,
            PieceType.Factory7.Id,
            PieceType.Factory8.Id,
            PieceType.Factory9.Id,
        };
    }

    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = 1;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatchDefault == -1 || countForMatch < countForMatchDefault) return null;
        
        var nextAction = new SpawnPieceAtAction
        {
            IsCheckMatch = false,
            At = position,
            PieceTypeId = nextType,
            OnSuccessEvent = pos => SpawnReward(pos, nextType)
        };
        
        return new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            OnCompleteAction = nextAction
        };
    }
    
    private void SpawnReward(BoardPosition position, int pieceType)
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