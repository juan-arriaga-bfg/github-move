using System.Collections.Generic;

public static class UiCharacterData
{
    public const string CharSleepingBeauty = "SleepingBeauty";
    public const string CharPussInBoots = "PussInBoots";
    public const string CharRapunzel = "Rapunzel";
    public const string CharGnomeWorker = "GnomeWorker";
    public const string CharGnomeWorker2 = "GnomeWorker2";
    public const string CharRedHood = "RedHood";
    public const string Mermaid = "Mermaid";
    public const string TigerLilly = "TigerLilly";
    public const string PeterPan = "PeterPan";
    
    private static readonly Dictionary<string, UICharacterDef> charDefs = new Dictionary<string, UICharacterDef>
    {
        { 
            CharSleepingBeauty, 
            new UICharacterDef
            {
                Id = CharSleepingBeauty,
                Name = "conversation.character.SleepingBeauty",
                ColorHex = "#E046F8",
                PieceId = PieceType.NPC_A.Id
            }
        },
        { 
            CharPussInBoots, 
            new UICharacterDef
            {
                Id = CharPussInBoots,
                Name = "conversation.character.PussInBoots",
                ColorHex = "#52CBD1",
                PieceId = PieceType.NPC_B.Id
            }
        },
        { 
            CharRapunzel, 
            new UICharacterDef
            {
                Id = CharRapunzel,
                Name = "conversation.character.Rapunzel",
                ColorHex = "#ffd12f",
                PieceId = PieceType.NPC_C.Id
            }
        },
        { 
            CharRedHood, 
            new UICharacterDef
            {
                Id = CharRedHood,
                Name = "conversation.character.RedHood",
                ColorHex = "#ff5936",
                PieceId = PieceType.NPC_D.Id
            }
        },
        { 
            Mermaid, 
            new UICharacterDef
            {
                Id = Mermaid,
                Name = "conversation.character.Mermaid",
                ColorHex = "#7afff6",
                PieceId = PieceType.NPC_E.Id
            }
        },
        { 
            TigerLilly, 
            new UICharacterDef
            {
                Id = TigerLilly,
                Name = "conversation.character.TigerLily",
                ColorHex = "#ff0033",
                PieceId = PieceType.NPC_F.Id
            }
        },
        { 
            CharGnomeWorker, 
            new UICharacterDef
            {
                Id = CharGnomeWorker,
                Name = "conversation.character.GnomeWorker",
                ColorHex = "#00A0FF"
            }
        },
        { 
            CharGnomeWorker2, 
            new UICharacterDef
            {
                Id = CharGnomeWorker2,
                Name = "conversation.character.GnomeWorker2",
                ColorHex = "#00A0FF",
                ViewName = $"UICharacter{CharGnomeWorker}View" // Use the same view as for CharGnomeWorker #1
            }
        },
        { 
            PeterPan, 
            new UICharacterDef
            {
                Id = PeterPan,
                Name = "conversation.character.PeterPan",
                ColorHex = "#FFFB00",
                PieceId = PieceType.NPC_G.Id
            }
        },
    };

    public static UICharacterDef GetDefByCharId(string charId)
    {
        UICharacterDef def;
        charDefs.TryGetValue(charId, out def);
        return def;
    }

    public static UICharacterDef GetDefByPieceId(int pieceId)
    {
        foreach (var def in charDefs)
        {
            if (def.Value.PieceId == pieceId)
            {
                return def.Value;
            }
        }

        return null;
    }
}