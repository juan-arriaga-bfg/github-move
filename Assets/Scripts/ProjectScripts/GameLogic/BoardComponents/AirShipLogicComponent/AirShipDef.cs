using System.Collections.Generic;
using System.Linq;

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

        var definition = BoardService.Current.FirstBoard.BoardLogic.MatchDefinition;
        var branches = new Dictionary<int, List<int>>();
        
        foreach (var pair in payload)
        {
            var key = definition.GetFirst(pair.Key);

            if (branches.TryGetValue(key, out var branch) == false)
            {
                branch = new List<int>();
                branches.Add(key, branch);
            }
            
            for (var i = 0; i < pair.Value; i++)
            {
                branch.Add(pair.Key);
            }
        }

        var keys = branches.Keys.ToList();
        keys.Sort((a, b) => a.CompareTo(b));

        foreach (var key in keys)
        {
            var pieces = branches[key];
            
            pieces.Sort((a, b) => -a.CompareTo(b));
            SortedPayload.AddRange(pieces);
        }
    }
}