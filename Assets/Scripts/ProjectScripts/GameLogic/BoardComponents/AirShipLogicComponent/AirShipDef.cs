using System.Collections.Generic;

public class AirShipDef
{
    public int Id;
    public AirShipView View;
    
    public List<int> SortedPayload { get; private set; }

    private Dictionary<int, int> payload;
    public Dictionary<int, int> Payload
    {
        get => payload;
        set
        {
            payload = value;
            NormalizeAndSortPayload();
        }
    }

    private void NormalizeAndSortPayload()
    {
        SortedPayload = new List<int>();

        var data = GameDataService.Current.PiecesManager;
        var scores = new List<KeyValuePair<int, int>>();
        
        foreach (var pair in payload)
        {
            var def = data.GetPieceDef(pair.Key);
            var experience = def?.CreateRewards?.Find(reward => reward.Currency == Currency.Experience.Name);
            
            scores.Add(new KeyValuePair<int, int>(pair.Key, experience?.Amount ?? 0));
        }

        scores.Sort((a, b) => -a.Value.CompareTo(b.Value));

        foreach (var score in scores)
        {
            if (payload.TryGetValue(score.Key, out var amount) == false) continue;
            
            for (var i = 0; i < amount; i++)
            {
                SortedPayload.Add(score.Key);
            }
        }
    }
}