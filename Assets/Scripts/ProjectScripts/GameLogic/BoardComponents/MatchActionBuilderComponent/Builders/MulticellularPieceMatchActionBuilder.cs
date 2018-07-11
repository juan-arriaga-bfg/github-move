using System.Collections.Generic;
using DG.Tweening;

public class MulticellularPieceMatchActionBuilder : IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.Castle1.Id,
            PieceType.Castle2.Id,
            PieceType.Castle3.Id,
            PieceType.Castle4.Id,
            PieceType.Castle5.Id,
            PieceType.Castle6.Id,
            PieceType.Castle7.Id,
            PieceType.Castle8.Id,
            PieceType.Castle9.Id
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