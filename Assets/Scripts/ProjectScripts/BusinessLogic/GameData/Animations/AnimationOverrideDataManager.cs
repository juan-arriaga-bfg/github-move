using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrideDataManager
{
    private Dictionary<int, AnimationDef> animDefs;

    public AnimationOverrideDataManager()
    {
        animDefs = Build();
    }

    private void AddPieceChain(Dictionary<int, AnimationDef> defDict, int start, int end,
        AnimationDef definition)
    {
        for (int i = start; i <= end; i++)
        {
            defDict[i] = definition;
        }
    }

    private void AddPiece(Dictionary<int, AnimationDef> defDict, int piece,  AnimationDef definition)
    {
        defDict[piece] = definition;
    }

    private void AddByFilter(Dictionary<int, AnimationDef> defDict, PieceTypeFilter filter,
        AnimationDef definition)
    {
        var piecesId = PieceType.GetIdsByFilter(filter);
        foreach (var id in piecesId)
        {
            AddPiece(defDict, id, definition);
        }
    }
    
    private Dictionary<int, AnimationDef> Build()
    {
        var defs = new Dictionary<int, AnimationDef>();

        AddByFilter(defs, PieceTypeFilter.Obstacle, new AnimationDef
        {
            OnDestroyFromBoard = R.DestroyObstacleAnimation
        });
        
        AddByFilter(defs, PieceTypeFilter.ProductionField, new AnimationDef
        {
            OnDestroyFromBoard = R.DestroyProductionAnimation
        });
        
        AddByFilter(defs, PieceTypeFilter.Chest, new AnimationDef
        {
            OnDestroyFromBoard = R.DestroyChestAnimation
        });

        AddPiece(defs, PieceType.NPC_Gnome.Id, new AnimationDef
        {
            OnDestroyFromBoard = R.DestroyWorkerAnimation
        });
        
        return defs;
    }
    
    public AnimationDef GetDefinition(int pieceType)
    {
        if (animDefs.ContainsKey(pieceType))
            return animDefs[pieceType];
        return null;
    }

    public string FindAnimation(int pieceType, Func<AnimationDef, string> onDefExist)
    {
        var animationDefinition = GetDefinition(pieceType);
        var animationResource = "";
        if (animationDefinition != null)
            animationResource = onDefExist(animationDefinition);
        return animationResource;
    }
}