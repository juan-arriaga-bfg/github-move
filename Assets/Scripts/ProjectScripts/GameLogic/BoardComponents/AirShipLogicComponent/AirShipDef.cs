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
        
        foreach (var pair in payload)
        {
            var pieceId = pair.Key;
            var count = pair.Value;
            
            for (var i = 0; i < count; i++)
            {
                var id = pieceId;
                SortedPayload.Add(id);
            }
        }
        
        // todo: Fix sorting
        SortedPayload.Sort((id1, id2) => id2 - id1);
    }
}