using System.Collections.Generic;

public static class UiCharacterData
{
    public const string CharSleepingBeauty = "SleepingBeauty";
    public const string CharPussInBoots = "PussInBoots";
    public const string CharRapunzel = "Rapunzel";
    public const string CharGnomeWorker = "GnomeWorker";
    
    private static readonly Dictionary<string, UICharacterDef> charDefs = new Dictionary<string, UICharacterDef>
    {
        { 
            CharSleepingBeauty, 
            new UICharacterDef
            {
                Name = "conversation.character.SleepingBeauty",
                ColorHex = "#E046F8"
            }
        },
        { 
            CharPussInBoots, 
            new UICharacterDef
            {
                Name = "conversation.character.PussInBoots",
                ColorHex = "#52CBD1"
            }
        },
        { 
            CharRapunzel, 
            new UICharacterDef
            {
                Name = "conversation.character.Rapunzel",
                ColorHex = "#F5BE03"
            }
        },
        { 
            CharGnomeWorker, 
            new UICharacterDef
            {
                Name = "conversation.character.GnomeWorker",
                ColorHex = "#00A0FF"
            }
        },
    };

    public static UICharacterDef GetDef(string charId)
    {
        UICharacterDef def;
        charDefs.TryGetValue(charId, out def);
        return def;
    }
}