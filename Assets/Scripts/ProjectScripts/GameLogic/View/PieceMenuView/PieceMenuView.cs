using System.Collections.Generic;
using UnityEngine;

public class PieceMenuView : UIBoardView, IBoardEventListener
{
    private Dictionary<BoardButtonView, TouchReactionDefinitionComponent> buttons = new Dictionary<BoardButtonView, TouchReactionDefinitionComponent>();

    private BoardButtonView spawnButton;

    private bool isOpen;

    public override int Priority
    {
        get { return 1; }
    }
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, 0.7f); }
    }

    protected override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(touchReaction == null) return;
        
        var menuDef = touchReaction.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menuDef == null) return;

        menuDef.OnClick = OnClick;
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
        
        foreach (var pair in menuDef.Definitions)
        {
            var element = piece.Context.RendererContext.CreateElement((int)ViewType.MenuButton);
            
            
            var btn = element.GetComponent<BoardButtonView>();
            
            element.parent = viewGo.transform;
            element.localPosition = Vector3.zero;
            element.localScale = Vector3.one;
            
            btn.Init(pair.Key, menuDef.GetColor(pair.Value), view => OnButtonClick(view));
            buttons.Add(btn, pair.Value);

            if (pair.Value is TouchReactionDefinitionSpawnInStorage) spawnButton = btn;
        }
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();

        foreach (var buttonsKey in buttons.Keys)
        {
            Context.Context.RendererContext.DestroyElement(buttonsKey.gameObject);
        }
        
        buttons = new Dictionary<BoardButtonView, TouchReactionDefinitionComponent>();
    }

    private void OnClick()
    {
        if (isOpen == false && spawnButton != null && OnButtonClick(spawnButton))
        {
            return;
        }
        
        Context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
        
        isOpen = !isOpen;

        if (isOpen) OpenMenu();
        else CloseMenu();
    }

    private void OpenMenu()
    {
        Change(true);

        var index = 0;
        var angle = (360 / (float) buttons.Count) * (Mathf.PI/180);
        
        foreach (var pair in buttons)
        {
            index++;
            var position = new Vector2(Mathf.Cos(angle  * index), 0.5f + Mathf.Sin(angle * index));
            pair.Key.CachedTransform.localPosition = position;
        }
    }

    private void CloseMenu()
    {
        Change(false);
        
        foreach (var pair in buttons)
        {
            pair.Key.CachedTransform.localPosition = Vector3.zero;
        }
    }

    private bool OnButtonClick(BoardButtonView btn)
    {
        TouchReactionDefinitionComponent definition;
        if(buttons.TryGetValue(btn, out definition) == false) return false;
        
        isOpen = false;
        CloseMenu();
        
        return definition.Make(Context.CachedPosition, Context);
    }

    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceMenu) return;
        if(isOpen == false) return;

        isOpen = false;
        CloseMenu();
    }
}