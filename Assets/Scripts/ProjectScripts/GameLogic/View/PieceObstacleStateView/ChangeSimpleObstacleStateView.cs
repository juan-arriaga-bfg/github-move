using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSimpleObstacleStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;
    
    [SerializeField] private Image progress;
    [SerializeField] private Image light;
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

        DOTween.Sequence().SetId(light).SetLoops(int.MaxValue)
            .Append(light.DOFade(0.5f, 0.3f))
            .Append(light.DOFade(1f, 0.3f));
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();

        DOTween.Kill(light);
        
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
        
        DOTween.Sequence()
            .AppendInterval(0.01f)
            .AppendCallback(() => Change(simpleObstacle.isOpen));
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
        DOTween.Sequence()
            .AppendInterval(0.01f)
            .AppendCallback(() => Change(simpleObstacle.isOpen));
    }

    private void SetProgress()
    {
        if (progress == null) return;
        
        progress.fillAmount = simpleObstacle.GetProgressFake;
        light.fillAmount = simpleObstacle.GetProgress;
    }
}