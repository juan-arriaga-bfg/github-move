using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSimpleObstacleStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;
    
    [SerializeField] private Image progress;
    [SerializeField] private HorizontalLayoutGroup group;
    [SerializeField] private GameObject dot;
    [SerializeField] private GameObject progressbar;
    
    private TouchReactionDefinitionSimpleObstacle simpleObstacle;
    
    private List<GameObject> dots = new List<GameObject>();
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, -0.5f); }
    }

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
        
        if(touchReaction == null) return;
        
        simpleObstacle = touchReaction.GetComponent<TouchReactionDefinitionSimpleObstacle>(TouchReactionDefinitionSimpleObstacle.ComponentGuid);
        
        if(simpleObstacle == null) return;
        
        simpleObstacle.OnClick = OnClick;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
        
        simpleObstacle.Steps = piece.Context.BoardLogic.MatchDefinition.GetIndexInChain(piece.PieceType);

        for (var i = 1; i < simpleObstacle.Steps; i++)
        {
            var dt = Instantiate(dot, dot.transform.parent);
            dots.Add(dt);
        }

        group.spacing = 110 / simpleObstacle.Steps;
        
        progressbar.SetActive(simpleObstacle.Steps > 1);
        
        dot.SetActive(false);
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);

        foreach (var dt in dots)
        {
            Destroy(dt);
        }
        
        dots = new List<GameObject>();
        dot.SetActive(true);
    }
    
    private void OnClick()
    {
        Context.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
        simpleObstacle.isOpen = !simpleObstacle.isOpen;
        
        SetProgress();
        price.Text = string.Format("<sprite name={0}> {1}", simpleObstacle.Price.Currency, simpleObstacle.Price.Amount);

        Change(simpleObstacle.isOpen);
    }

    public void Clear()
    {
        simpleObstacle.Clear(Context);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceMenu|| (context as ChangeSimpleObstacleStateView) == this) return;
        if(simpleObstacle.isOpen == false) return;

        simpleObstacle.isOpen = false;
        Change(false);
    }

    private void SetProgress()
    {
        if (progress != null) progress.fillAmount = simpleObstacle.GetProgress;
    }
}