using System.Collections.Generic;
using UnityEngine;

public class PieceMenuView : UIBoardView, IBoardEventListener
{
    [SerializeField] private GameObject pattern;
    
    private List<GameObject> btns = new List<GameObject>();
    
    protected override ViewType Id => ViewType.Menu;

    public override Vector3 Ofset => new Vector3(0, 0.7f);

    public override void SetOfset()
    {
        CachedTransform.localPosition = controller.GetViewPositionTop(multiSize) + Ofset;
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);

        Priority = defaultPriority = 20;
        
        var menuDef = Context.TouchReaction?.GetComponent<TouchReactionDefinitionMenu>(TouchReactionDefinitionMenu.ComponentGuid);
        
        if(menuDef == null) return;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
        
        foreach (var def in menuDef.Definitions)
        {
            var btn = Instantiate(pattern, pattern.transform.parent);
            
            btn.GetComponent<PieceMenuItem>().Decoration(def, Context);
            btns.Add(btn);
        }
        
        pattern.SetActive(false);
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);

        foreach (var btn in btns)
        {
            Destroy(btn);
        }
        
        btns = new List<GameObject>();
        
        pattern.SetActive(true);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI || context is Piece && (Piece)context == Context) return;
        
        Change(false);
    }
}