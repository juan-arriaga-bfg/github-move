using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IVIPIslandLogicComponent
{
    VIPIslandLogicComponent VIPIslandLogic { get; }
}

public partial class BoardLogicComponent : IVIPIslandLogicComponent
{
    protected VIPIslandLogicComponent vipIslandLogic;
    public VIPIslandLogicComponent VIPIslandLogic => vipIslandLogic ?? (vipIslandLogic = GetComponent<VIPIslandLogicComponent>(VIPIslandLogicComponent.ComponentGuid));
}

public enum VIPIslandState
{
    Fog,
    Broken,
    Paid
}

public class VIPIslandLogicComponent : ECSEntity, ITouchableBoardObjectLogic
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private BoardLogicComponent context;

    public bool IsDraggable => false;
    
    private readonly Dictionary<ViewType, BoardElementView> views = new Dictionary<ViewType, BoardElementView>();
    
    private readonly BoardPosition boardPosition = new BoardPosition(10, 13, BoardLayer.Piece.Layer);
    
    public static readonly List<BoardPosition> IslandPositions = new List<BoardPosition>
    {
        new BoardPosition(7,11),
        new BoardPosition(8,11),
        new BoardPosition(6,12),
        new BoardPosition(7,12),
        new BoardPosition(8,12),
        new BoardPosition(9,12),
        new BoardPosition(6,13),
        new BoardPosition(7,13),
        new BoardPosition(8,13),
        new BoardPosition(9,13),
        new BoardPosition(6,14),
        new BoardPosition(7,14),
        new BoardPosition(8,14),
        new BoardPosition(9,14),
        new BoardPosition(7,15),
        new BoardPosition(8,15)
    };

    public List<BoardPosition> Island;
    
    private Vector3 localPosition;

    private bool isClick;

    private CurrencyPair price;

    public VIPIslandState State;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardLogicComponent;
        price = new CurrencyPair
        {
            Currency = Currency.Crystals.Name,
            Amount = GameDataService.Current.ConstantsManager.PremiumIslandPrice
        };
        Island = IslandPositions.Select(pos => pos.SetZ(BoardLayer.Piece.Layer)).ToList();
    }

    public void Init()
    {
        var save = ProfileService.Current.FieldDef;

        if (save != null) State = (VIPIslandState) save.VIPIslandState;
        else State = VIPIslandState.Fog;
        
        localPosition = context.Context.BoardDef.GetPiecePosition(boardPosition.X, boardPosition.Y);
        context.LockCells(Island, this);
        
        CreateView(ViewType.Coast, boardPosition);
        UpdateView(State, false);
    }
    
    public bool OnDragStart(BoardElementView view)
    {
        return Check(view);
    }

    public bool OnDragEnd(BoardElementView view)
    {
        return Check(view);
    }

    public bool Check(BoardElementView view)
    {
        foreach (var item in views.Values)
        {
            if (item != view) continue;
        
            return true;
        }
        
        return false;
    }

    public bool OnClick(BoardElementView view)
    {
        var isCheck = Check(view);

        if (isCheck == false || isClick) return isCheck;

        isClick = true;
        
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.island.title", "window.island.title");
        model.Message = LocalizationService.Get("window.island.message", "window.island.message");
        model.Prefab = "VIPIsland";
        model.AcceptLabel = string.Format(LocalizationService.Get("common.button.buy", "common.button.buy"), price.ToStringIcon());
        
        model.IsBuy = true;
        model.IsTopMessage = true;
        model.IsShine = true;
        model.ButtonSize = 280;

        model.OnAccept = Purchase;
        model.OnClose = () => { isClick = false; };
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
        
        return true;
    }

    public void HintClick()
    {
        var pieces = GameDataService.Current.FieldManager.IslandPieces;
        var result = new List<int>();

        foreach (var pair in pieces)
        {
            var def = PieceType.GetDefById(pair.Key);

            if (def.Filter.Has(PieceTypeFilter.Chest) == false) continue;

            for (var i = 0; i < pair.Value.Count; i++)
            {
                result.Add(pair.Key);
            }
        }
        
        UILootBoxWindowController.OpenProbabilityWindow(result);
    }

    private void Purchase()
    {
        CurrencyHelper.Purchase(Currency.Island.Name, 1, price, success =>
        {
            if (success == false)
            {
                isClick = false;
                return;
            }

            UpdateView(VIPIslandState.Paid, true);
        });
    }

    public void UpdateView(VIPIslandState state, bool animation)
    {
        switch (state)
        {
            case VIPIslandState.Fog:
                CreateView(ViewType.FogBridge, boardPosition.Down);
                CreateView(ViewType.IslandFog, boardPosition);
                break;
            case VIPIslandState.Broken:
                RemoveView(ViewType.FogBridge);
                CreateView(ViewType.BrokenBridge, boardPosition);
                CreateView(ViewType.Airbaloon, boardPosition, animation);
                
                if (animation == false) break;
                
                (views[ViewType.IslandFog] as AnimatedBoardElementView).PlayHide();
                
                ParticleView.Show(R.FogProgressCompleteParticle, new BoardPosition(8,13,BoardLayer.FX.Layer));
                break;
            case VIPIslandState.Paid:
                context.UnlockCells(Island, this);
                RemoveView(ViewType.BrokenBridge);
                CreateView(ViewType.Bridge, boardPosition, animation);
                
                if (animation == false) break;
                
                (views[ViewType.Airbaloon] as AnimatedBoardElementView).PlayHide(2f);
                
                ParticleView.Show(R.FogProgressCompleteParticle, new BoardPosition(8,13,BoardLayer.FX.Layer));
                break;
        }

        State = state;
        
        UpdateLockState();
    }

    public void SpawnPieces()
    {
        var pieces = GameDataService.Current.FieldManager.IslandPieces;
        foreach (var piece in pieces)
        {
            context.Context.ActionExecutor.AddAction(new FillBoardAction
            {
                Piece = piece.Key,
                Positions = piece.Value,
                OnComplete = (board) => UpdateLockState() 
            });
        }
    }

    public void UpdateLockState()
    {
        var isLocked = State == VIPIslandState.Broken;
        foreach (var position in Island)
        {
            var view = context.Context.RendererContext.GetElementAt(position) as PieceBoardElementView;
            if (view != null)
            {
                view.ToggleLockView(isLocked);
                context.PieceFlyer?.FlyToCodex(view.Piece, position.X, position.Y, Currency.Codex.Name);
            }
        }
    }

    private void CreateView(ViewType id, BoardPosition position, bool animation = false)
    {
        var view = context.Context.RendererContext.CreateBoardElement<BoardElementView>((int)id);
        var animView = view as AnimatedBoardElementView;
        
        view.CachedTransform.localPosition = localPosition;
        view.Init(context.Context.RendererContext);
        view.SyncRendererLayers(position);

        if (animation) animView.PlayShow();
        else if (animView != null) animView.PlayIdle();
        
        views.Add(id, view);
    }

    private void RemoveView(ViewType id)
    {
        if (views.TryGetValue(id, out var view) == false) return;

        views.Remove(id);
        context.Context.RendererContext.DestroyElement(view);
    }
}
