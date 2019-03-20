using System.Collections.Generic;

public partial class PieceTypeDef
{
    private int id;
    
    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public List<string> Abbreviations { get; set; }
}

public static partial class PieceType
{
    public static readonly PieceTypeDef None = new PieceTypeDef {Id = -1, Abbreviations = new List<string>{"None"}};
    public static readonly PieceTypeDef Empty = new PieceTypeDef {Id = 0, Abbreviations = new List<string>{"Empty", "---"}};
    public static readonly PieceTypeDef Generic = new PieceTypeDef {Id = 1, Abbreviations = new List<string>{"Generic"}};
    public static readonly PieceTypeDef LockedEmpty = new PieceTypeDef{Id = 10, Abbreviations = new List<string>{ "LockedEmpty" }, Filter = PieceTypeFilter.Simple};
    
    public static readonly Dictionary<string, int> Abbreviations = new Dictionary<string, int>();

    public static void RegisterType(PieceTypeDef def)
    {
        for (int i = 0; i < def.Abbreviations.Count; i++)
        {
            var abbr = def.Abbreviations[i];
            if (Abbreviations.ContainsKey(abbr) == false)
            {
                Abbreviations.Add(abbr, def.Id);
            }
        }
    }

    public static string Parse(int t)
    {
        string targetAbbr = null;
        foreach (var abbr in Abbreviations)
        {
            if (abbr.Value == t)
            {
                targetAbbr = abbr.Key;
                break;
            }
        }
        return targetAbbr;
    }

    public static int Parse(string label)
    {
        int t;
        if (int.TryParse(label, out t))
        {
            return t;
        }

        if (Abbreviations.ContainsKey(label))
        {
            return Abbreviations[label];
        }

        return None.Id;
    }
}
