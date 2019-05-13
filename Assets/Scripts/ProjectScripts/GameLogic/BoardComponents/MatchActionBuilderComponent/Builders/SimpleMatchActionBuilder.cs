using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;

public class SimpleMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public const int AMOUNT_BONUS = 5;
    
    public List<int> GetKeys()
    {
        return new List<int>();
    }

    private bool UseBoost(MatchDefinitionComponent definition, List<BoardPosition> matchField)
    {
        return matchField.FindIndex(boardPosition => definition.Context.GetPieceAt(boardPosition).PieceType == PieceType.Boost_CR.Id) != -1;
    }
    
    public bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next)
    {
        next = definition.GetNext(pieceType, false);
        
        if (next == PieceType.None.Id
            || UseBoost(definition, matchField) && definition.GetLast(pieceType) == PieceType.Boost_CR.Id) return false;
        
        var amountForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        return amountForMatchDefault != -1 && matchField.Count >= amountForMatchDefault;
    }
    
    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;

        var useBoost = UseBoost(definition, matchField);

        if (useBoost && definition.GetLast(pieceType) == PieceType.Boost_CR.Id) return null;
        
        var nextPieces = Calculation(pieceType, nextType, definition.GetPieceCountForMatch(pieceType), matchField.Count, useBoost, out var ignoreBoost);

        if (nextPieces.Count == 0) return null;
        
        nextPieces.Sort();
        SendAnalytics(nextPieces.FindAll(id => id == nextType), nextType, useBoost && ignoreBoost == false);
        
        var matchDescription = new MatchDescription
        {
            SourcePieceType = pieceType,
            MatchedPiecesCount = matchField.Count,
            CreatedPieceType = nextType,
        };
        
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, matchDescription);

        bool isEffective = matchField.Count >= 15 || (matchField.Count > 0 && matchField.Count % 5 == 0);
        GameDataService.Current.UserProfile.BaseInformation.MatchesCounter.AddMatch(isEffective);
        
        // collect and purchase rewards before action
        return CreateAction(nextPieces, matchField, position, pieceType, ignoreBoost);
    }

    private List<int> Calculation(int currentType, int nextType, int amountDefault, int amountCurrent, bool useBoost, out bool ignoreBoost)
    {
        ignoreBoost = false;
        
        var nextPieces = new List<int>();
        
        if (amountDefault == -1 || amountCurrent < amountDefault) return nextPieces;
        
        if (amountCurrent < AMOUNT_BONUS)
        {
            nextPieces = Add(1, nextType, nextPieces);

            if (amountCurrent == AMOUNT_BONUS - 1)
            {
                nextPieces = Add(1, useBoost ? PieceType.Boost_CR.Id : currentType, nextPieces);
                ignoreBoost = useBoost;
            }
            
            return nextPieces;
        }

        var amount = amountCurrent / AMOUNT_BONUS * 2;
        var remainder = amountCurrent % AMOUNT_BONUS;

        switch (remainder)
        {
            case 0:
            case 1:
                nextPieces = Add(amount, nextType, nextPieces);
                break;
            case 2:
                nextPieces = Add(amount, nextType, nextPieces);
                nextPieces = Add(1, useBoost ? PieceType.Boost_CR.Id : currentType, nextPieces);
                ignoreBoost = useBoost;
                break;
            case 3:
            case 4:
                amount += 1;
                nextPieces = Add(amount, nextType, nextPieces);
                break;
        }
        
        return nextPieces;
    }
    
    private List<int> Add(int count, int piece, List<int> pieces)
    {
        for (var i = 0; i < count; i++)
        {
            pieces.Add(piece);
        }

        return pieces;
    }
    
    private void SendAnalytics(List<int> data, int next, bool useBoost)
    {
        var boosters = new Dictionary<int, CurrencyPair>
        {
            {PieceType.Boost_CR.Id, new CurrencyPair{Currency = PieceType.Boost_CR.Abbreviations[0]}},
            {PieceType.Boost_WR.Id, new CurrencyPair{Currency = PieceType.Boost_WR.Abbreviations[0]}}
        };
        
        foreach (var id in data)
        {
            if (boosters.TryGetValue(id, out var pair) == false) continue;

            pair.Amount += 1;
        }

        var reason = PieceType.Parse(next);
        var collect = boosters.Values.ToList().FindAll(pair => pair.Amount > 0);
        var spend = useBoost
            ? new List<CurrencyPair> {new CurrencyPair {Currency = PieceType.Boost_CR.Abbreviations[0], Amount = 1}}
            : null; 
        
        if (collect.Count == 0 && spend == null) return;
        
        Analytics.SendPurchase("board_merge", reason, spend, collect, false, false);
    }
}