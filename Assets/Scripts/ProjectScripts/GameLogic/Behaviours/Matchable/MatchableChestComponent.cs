﻿public class MatchableChestComponent : MatchablePieceComponent
{
    private Chest chest;
    private ChestPieceComponent chestPiece;
    
    public override bool IsMatchable()
    {
        if (base.IsMatchable() == false) return false;
        if (chest != null) return chest.State == ChestState.Close;
        
        chestPiece = Context.GetComponent<ChestPieceComponent>(ChestPieceComponent.ComponentGuid);

        if (chestPiece == null) return false;
            
        chest = chestPiece.Chest;
            
        if (chest == null) return false;

        return chest.State == ChestState.Close;
    }
}