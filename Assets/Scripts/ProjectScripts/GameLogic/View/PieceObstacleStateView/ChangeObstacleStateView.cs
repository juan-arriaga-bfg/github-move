using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChangeObstacleStateView : UIBoardView, IBoardEventListener
{
    [SerializeField] private NSText message;
    [SerializeField] private NSText price;
    
    [SerializeField] private Image progress;
    [SerializeField] private Image light;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private GameObject dot;
    [SerializeField] private GameObject progressbar;

    private ObstacleLifeComponent life;
    
    private List<GameObject> dots = new List<GameObject>();
    
    public override Vector3 Ofset
    {
        get { return new Vector3(0, 1.5f); }
    }
    
    protected override ViewType Id
    {
        get { return ViewType.SimpleObstacle; }
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        life = piece.GetComponent<ObstacleLifeComponent>(ObstacleLifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceMenu);
        
        for (var i = 1; i < life.HP; i++)
        {
            var dt = Instantiate(dot, dot.transform.parent);
            dots.Add(dt);
        }

        layoutGroup.spacing = 110 / life.HP;
        
        progressbar.SetActive(life.HP > 1);
        
        dot.SetActive(false);
        
        DOTween.Kill(light);

        DOTween.Sequence().SetId(light).SetLoops(int.MaxValue)
            .Append(light.DOFade(0.5f, 0.3f))
            .Append(light.DOFade(1f, 0.3f));
    }
    
    public override void ResetViewOnDestroy()
    {
        DOTween.Kill(light);

        light.DOFade(1f, 0f);
        
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceMenu);

        foreach (var dt in dots)
        {
            Destroy(dt);
        }
        
        dots = new List<GameObject>();
        dot.SetActive(true);
        
        base.ResetViewOnDestroy();
    }

    public override void UpdateVisibility(bool isVisible)
    {
        base.UpdateVisibility(isVisible);

        if (IsShow == false) return;
        
        price.Text = life.Price.ToStringIcon();
        
        progress.fillAmount = life.GetProgressNext;
        light.fillAmount = life.GetProgress;
    }
    
    public void Clear()
    {
        if (life.Damage(Reopen))
        {
            Change(false);
        }
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceMenu || context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition)) return;
		
        Change(false);
    }

    private void Reopen()
    {
        var viewDef = Context.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);
            
        if (viewDef == null) return;
        
        var view = viewDef.AddView(ViewType.SimpleObstacle);
        
        view.Change(true);
    }
}