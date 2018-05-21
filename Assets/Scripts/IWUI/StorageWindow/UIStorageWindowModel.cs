using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStorageWindowModel : IWWindowModel
{
    public string Title
    {
        get { return "Storage"; }
    }

    public string Message
    {
        get { return "Tap a product, to sell them!"; }
    }

    public float Progress
    {
        get { return Mathf.Clamp01(Current / (float) Max); }
    }
    
    public string Capacity
    {
        get { return string.Format("{0}/{1}", Current, Max); }
    }

    private int Current
    {
        get { return CurrencyHellper.GetCountByTag(CurrencyTag.All); }
    }

    private int Max
    {
        get { return 50 + ProfileService.Current.GetStorageItem(Currency.LevelStorage.Name).Amount * 20; }
    }

    public List<string> Tabs
    {
        get
        {
            var tabs = new List<string>();

            foreach (var value in Enum.GetValues(typeof(CurrencyTag)))
            {
                tabs.Add(value.ToString());
            }
            
            return tabs;
        }
    }
    
    public bool Upgrade()
    {
        var board = BoardService.Current.GetBoardById(0);
        var piece = board.BoardLogic.GetPieceAt(GameDataService.Current.PiecesManager.StoragePosition);
        var reaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(reaction == null) return false;
        
        var menu = reaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menu == null) return false;

        var upgrade = menu.GetDefinition<TouchReactionDefinitionUpgrade>();
        
        if(upgrade == null) return false;
        
        return upgrade.Make(piece.CachedPosition, piece);
    }
}
