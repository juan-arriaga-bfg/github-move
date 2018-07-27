using System.Collections.Generic;
using UnityEngine;

public class UpgradeComponent : IECSComponent, IPieceBoardObserver, IResourceCarrierView
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid { get { return ComponentGuid; } }

    public RectTransform GetAnchorRect()
    {
        throw new System.NotImplementedException();
    }
    
    public Vector3 GetAcnhorPosition()
    {
        throw new System.NotImplementedException();
    }

    public Camera RenderCamera { get; private set; }
    public List<IResourceCarrier> Carriers { get; private set; }
    
    private Piece thisContext;
    private CurrencyPair price;
    private StorageItem storageItem;
    private ViewDefinitionComponent viewDef;

    private bool isShow;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
        
        var def = GameDataService.Current.PiecesManager.GetPieceDefOrDefault(thisContext.PieceType);

        var isLast = thisContext.Context.BoardLogic.MatchDefinition.GetNext(thisContext.PieceType) == PieceType.None.Id;

        if (def == null || isLast) return;
        
        price = def.UpgradePrices[0];
        
        viewDef = thisContext.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
        
        if (viewDef == null) return;
        
        storageItem = ProfileService.Current.GetStorageItem(GetResourceId());
    }
    
    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if(viewDef == null) return;
        
        ResourcesViewManager.Instance.RegisterView(this);
    }

    public void OnMovedFromTo(BoardPosition from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        if(viewDef == null) return;
        
        ResourcesViewManager.Instance.UnRegisterView(this);
    }
    
    public Vector2 GetScreenPosition()
    {
        return thisContext.Context.BoardDef.GetSectorCenterWorldPosition(
            thisContext.CachedPosition.X,
            thisContext.CachedPosition.Y,
            thisContext.CachedPosition.Z);
    }
    
    public void RegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UnRegisterCarrier(IResourceCarrier carrier)
    {
    }
    
    public void UpdateResource(int offset)
    {
        if (storageItem.Amount < price.Amount)
        {
            if (!isShow) return;
            
            viewDef.AddView(ViewType.SimpleUpgrade).Change(false);
            isShow = false;
            
            return;
        }
        
        thisContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        viewDef.AddView(ViewType.SimpleUpgrade).Change(true);
        isShow = true;
    }
    
    public string GetResourceId()
    {
        return price.Currency;
    }
}