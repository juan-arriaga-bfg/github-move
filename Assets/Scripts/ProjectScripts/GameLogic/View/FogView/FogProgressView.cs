using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FogProgressView : UIBoardView
{
    [SerializeField] private NSText label;
    [SerializeField] private RectTransform line;
    [SerializeField] private RectTransform lineFake;
    [SerializeField] private float barWidth;

    private Image light;
    private const float delay = 0.5f;
    
    protected override ViewType Id => ViewType.FogProgress;

    private FogObserver fog;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
		
        Priority = defaultPriority = 0;
        fog = Context.GetComponent<FogObserver>(FogObserver.ComponentGuid);
        
        if(fog == null) return;

        light = lineFake.GetComponent<Image>();

        SetText(fog.Credit.Amount);
        line.sizeDelta = lineFake.sizeDelta = new Vector2(Progress(line, fog.Def.Condition.Amount, fog.Credit.Amount, out var time), line.sizeDelta.y);
        
        DOTween.Kill(light);

        DOTween.Sequence().SetId(light).SetLoops(int.MaxValue)
            .Append(light.DOFade(0.5f, 0.3f))
            .Append(light.DOFade(1f, 0.3f));
    }
    
    public override void ResetViewOnDestroy()
    {
        DOTween.Kill(line);
        DOTween.Kill(lineFake);
        DOTween.Kill(light);
        
        base.ResetViewOnDestroy();
    }

    public void UpdateProgress(Action onComplete)
    {
        DOTween.Kill(line);
        
        line
            .DOSizeDelta(new Vector2(Progress(line, fog.Def.Condition.Amount, fog.Credit.Amount, out var time), line.sizeDelta.y), time)
            .SetId(line)
            .SetEase(Ease.Linear)
            .OnUpdate(() => SetText((int)(fog.Def.Condition.Amount * (line.sizeDelta.x / barWidth))))
            .OnComplete(() =>
            {
                SetText(fog.Credit.Amount);
                onComplete?.Invoke();
            });
    }
    
    public void UpdateFakeProgress(int value)
    {
        DOTween.Kill(lineFake);
        
        value = Mathf.Clamp(fog.Credit.Amount + value, fog.Credit.Amount, fog.Def.Condition.Amount);
        
        lineFake
            .DOSizeDelta(new Vector2(Progress(lineFake, fog.Def.Condition.Amount, value, out var time), lineFake.sizeDelta.y), time * 0.5f)
            .SetId(lineFake)
            .SetEase(Ease.Linear);
    }
    
    private float Progress(RectTransform target, float required, float current, out float time)
    {
        var progress = barWidth*(current/required);
        var offset = progress - target.sizeDelta.x;
        
        time = Mathf.Abs(delay * (offset / barWidth));
        
        return progress;
    }

    private void SetText(int value)
    {
        label.Text = $"{value}/{fog.Def.Condition.Amount}";
    }
}