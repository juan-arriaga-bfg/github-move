using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceTypeDef
{
    public int Id { get; set; }

    public List<string> Abbreviations { get; set; }
}

public static partial class PieceType
{
    public readonly static PieceTypeDef None = new PieceTypeDef {Id = -1, Abbreviations = new List<string>{"None"}};
    
    public readonly static PieceTypeDef Empty = new PieceTypeDef {Id = 0, Abbreviations = new List<string>{"Empty", "---"}};
    
    public readonly static PieceTypeDef Generic = new PieceTypeDef {Id = 1, Abbreviations = new List<string>{"Generic"}};
    
    public readonly static Dictionary<string, int> Abbreviations = new Dictionary<string, int>();

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
