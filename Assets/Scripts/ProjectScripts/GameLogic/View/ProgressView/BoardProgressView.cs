using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardProgressView : UIBoardView
{
    [SerializeField] private Image progress;
    [SerializeField] private Image light;
    
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    
    [SerializeField] private GameObject dot;
    [SerializeField] private RectTransform sizeRect;

    private List<GameObject> dots = new List<GameObject>();
    
    public override Vector3 Ofset => new Vector3(0, 1.5f);

    protected override ViewType Id => ViewType.Progress;
    private StorageLifeComponent data;

    public void SetTarget(StorageLifeComponent target)
    {
        data = target;
        
        for (var i = 1; i < data.HP; i++)
        {
            var dt = Instantiate(dot, dot.transform.parent);
            dots.Add(dt);
        }

        layoutGroup.spacing = sizeRect.sizeDelta.x / data.HP;
        
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
        
        progress.fillAmount = data.GetProgressNext;
        light.fillAmount = data.GetProgress;
    }
}