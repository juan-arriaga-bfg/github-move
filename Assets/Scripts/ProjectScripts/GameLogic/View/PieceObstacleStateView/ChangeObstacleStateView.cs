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
    
    [SerializeField] private RectTransform sizeTarget;

    private StorageLifeComponent life;
    
    private List<GameObject> dots = new List<GameObject>();
    
    public override Vector3 Ofset => new Vector3(0, 1.5f);

    protected override ViewType Id => ViewType.ObstacleState;

    public override void Init(Piece piece)
    {
        base.Init(piece);
        
        life = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        
        if(life == null) return;
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ClosePieceUI);
        
        for (var i = 1; i < life.HP; i++)
        {
            var dt = Instantiate(dot, dot.transform.parent);
            dots.Add(dt);
        }

        layoutGroup.spacing = sizeTarget.sizeDelta.x / life.HP;
        
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
        
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ClosePieceUI);

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
        
        message.Text = $"{life.Message}{(life.Energy.Amount == 0 ? "" : " " + life.Energy.ToStringIcon())}";
        price.Text = $"Send<sprite name={life.Worker.Currency}>";
        
        progress.fillAmount = life.GetProgressNext;
        light.fillAmount = life.GetProgress;
    }
    
    public void Clear()
    {
        if (life.Damage()) Change(false);
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (code != GameEventsCodes.ClosePieceUI || context is BoardPosition && ((BoardPosition) context).Equals(Context.CachedPosition)) return;
		
        Change(false);
    }
}