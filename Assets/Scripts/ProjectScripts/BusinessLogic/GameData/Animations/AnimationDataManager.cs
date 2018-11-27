using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDataManager
{
    private static Dictionary<int, AnimationDef> animDefs = Build();

    private static void AddPieceChain(Dictionary<int, AnimationDef> defDict, int start, int end,
        AnimationDef definition)
    {
        for (int i = start; i <= end; i++)
        {
            defDict[i] = definition;
        }
    }

    private static void AddPiece(Dictionary<int, AnimationDef> defDict, int piece,  AnimationDef definition)
    {
        defDict[piece] = definition;
    }

    private static void AddByFilter(Dictionary<int, AnimationDef> defDict, PieceTypeFilter filter,
        AnimationDef definition)
    {
        var piecesId = PieceType.GetIdsByFilter(filter);
        foreach (var id in piecesId)
        {
            Debug.LogWarning($"{PieceType.GetDefById(id).Abbreviations[0]}");
            AddPiece(defDict, id, definition);
        }
    }
    
    private static Dictionary<int, AnimationDef> Build()
    {
        var defs = new Dictionary<int, AnimationDef>();
        
//        AddByFilter(defs, PieceTypeFilter.Simple, new AnimationDef
//        {
//            OnFogSpawn = R.SpawnScalePieceAnimation,
//            OnMergeSpawn = R.SpawnScalePieceAnimation,
//            OnMultiSpawn = R.SpawnScalePieceAnimation,
//            OnPurchaseSpawn = R.SpawnScalePieceAnimation,
//        });

        AddByFilter(defs, PieceTypeFilter.Chest, new AnimationDef
        {
//            OnFogSpawn = R.SpawnScalePieceAnimation,
//            OnMergeSpawn = R.SpawnScalePieceAnimation,
//            OnMultiSpawn = R.SpawnScalePieceAnimation,
            OnPurchaseSpawn = R.SpawnScalePieceAnimation,
        });
        
        return defs;
    }
    
    public static AnimationDef GetDefinition(int pieceType)
    {
        if (animDefs.ContainsKey(pieceType))
            return animDefs[pieceType];
        return null;
    }

    public static string FindAnimation(int pieceType, Func<AnimationDef, string> onDefExist)
    {
        var animationDefinition = GetDefinition(pieceType);
        var animationResource = "";
        if (animationDefinition != null)
            animationResource = onDefExist(animationDefinition);

        return animationResource;
    }
}