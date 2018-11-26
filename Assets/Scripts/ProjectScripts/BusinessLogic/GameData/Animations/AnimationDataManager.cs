using System.Collections.Generic;

public class AnimationDataManager
{
    private static Dictionary<int, AnimationDef> animDefs = Build();

    private static void AddPieceChain(Dictionary<int, AnimationDef> defDict, int start, int end,
        AnimationDef definition)
    {
        for (int i = start; i <= end; i++)
        {
            defDict.Add(i, definition);
        }
    }

    private static void AddPiece(Dictionary<int, AnimationDef> defDict, int piece,  AnimationDef definition)
    {
        defDict.Add(piece, definition);
    }
    
    private static Dictionary<int, AnimationDef> Build()
    {
        var defs = new Dictionary<int, AnimationDef>();
        
        AddPieceChain(defs, PieceType.A1.Id, PieceType.A9.Id, new AnimationDef {OnFogSpawn = R.SpawnScalePieceAnimation});

        return defs;
    }
    
    public static AnimationDef GetDefinition(int pieceType)
    {
        if (animDefs.ContainsKey(pieceType))
            return animDefs[pieceType];
        return null;
    }
}