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
        AddByFilter(defDict, filter, PieceTypeFilter.Default, definition);
    }
    
    private void AddByFilter(Dictionary<int, AnimationDef> defDict, PieceTypeFilter filter, PieceTypeFilter exclude,
        AnimationDef definition)
    {
        var piecesId = PieceType.GetIdsByFilter(filter, exclude);
        foreach (var id in piecesId)
        {
            AddPiece(defDict, id, definition);
        }
    }

    private void AddByManyFilters(Dictionary<int, AnimationDef> defDict, List<PieceTypeFilter> filters,
        AnimationDef definition)
    {
        if (filters == null)
        {
            return;
        }
        
        var piecesId = PieceType.GetAllIds();
        foreach (var id in piecesId)
        {
            var def = PieceType.GetDefById(id);
            var isValid = true;
            foreach (var filter in filters)
            {
                if (def.Filter.Has(filter) == false)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                AddPiece(defDict, id, definition);    
            }
        }
    }
    
    private Dictionary<int, AnimationDef> Build()
    {
        var defs = new Dictionary<int, AnimationDef>();

        AddPieceChain(defs, PieceType.Soft1.Id, PieceType.Soft8.Id, new AnimationDef()
        {
            OnDestroyFromBoard = R.DestroySoftCurrencyAnimation
        });
        
        AddPieceChain(defs, PieceType.Hard1.Id, PieceType.Hard6.Id, new AnimationDef()
        {
            OnDestroyFromBoard = R.DestroyHardCurrencyAnimation
        });
        
        AddPieceChain(defs, PieceType.Token1.Id, PieceType.Token3.Id, new AnimationDef()
        {
            OnDestroyFromBoard = R.DestroyTokenCurrencyAnimation
        });
        
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
        
        AddByFilter(defs, PieceTypeFilter.OrderPiece, new AnimationDef
        {
            OnDestroyFromBoard = R.DestroyOrderAnimation
        });

        AddPiece(defs, PieceType.NPC_Gnome.Id, new AnimationDef
        {
            OnDestroyFromBoard = R.DestroyWorkerAnimation
        });
        
        AddByFilter(defs, PieceTypeFilter.Mine, PieceTypeFilter.Fake, new AnimationDef
        {
            OnMergeSpawn = R.SpawnNormalMineAnimation
        });
        
        AddByManyFilters(defs, new List<PieceTypeFilter> {PieceTypeFilter.Mine, PieceTypeFilter.Fake}, new AnimationDef
        {
            OnMergeSpawn = R.SpawnFakeMineAnimation
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