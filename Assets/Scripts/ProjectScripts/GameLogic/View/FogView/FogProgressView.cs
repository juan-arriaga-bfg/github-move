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
    [SerializeField] private Transform hintAnchor;
    
    private Image light;
    private const float delay = 0.5f;
    private HintArrowView arrow;
    
    protected override ViewType Id => ViewType.FogProgress;

    private FogObserver fog;
    
    public override void Init(Piece piece)
    {
        base.Init(piece);
		
        Priority = defaultPriority = 0;
        fog = Context.GetComponent<FogObserver>(FogObserver.ComponentGuid);
        
        if(fog == null) return;

        light = lineFake.GetComponent<Image>();

        SetText(fog.AlreadyPaid.Amount);
        line.sizeDelta = lineFake.sizeDelta = new Vector2(Progress(line, fog.Def.Condition.Amount, fog.AlreadyPaid.Amount, out var time), line.sizeDelta.y);
    }

    private void StartHighlight()
    {
        DOTween.Kill(light);
        
        DOTween.Sequence().SetId(light).SetLoops(int.MaxValue)
            .SetEase(Ease.InOutSine)
            .Append(light.DOFade(0.5f, 0.3f))
            .Append(light.DOFade(1f, 0.4f));
    }
    
    public override void ResetViewOnDestroy()
    {
        KillTweens();

        if (arrow != null)
        {
            arrow.Remove(0);
        }
        
        base.ResetViewOnDestroy();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void KillTweens()
    {
        DOTween.Kill(line);
        DOTween.Kill(lineFake);
        DOTween.Kill(light);
    }

    public void UpdateProgress(Action onComplete)
    {
        DOTween.Kill(line);
        
        line
            .DOSizeDelta(new Vector2(Progress(line, fog.Def.Condition.Amount, fog.AlreadyPaid.Amount, out var time), line.sizeDelta.y), time)
            .SetId(line)
            .SetEase(Ease.Linear)
            .OnUpdate(() => SetText((int)(fog.Def.Condition.Amount * (line.sizeDelta.x / barWidth))))
            .OnComplete(() =>
            {
                SetText(fog.AlreadyPaid.Amount);
                onComplete?.Invoke();
            });
    }
    
    public void UpdateFakeProgress(int value)
    {   
        DOTween.Kill(lineFake);
        

        var progressValue = value;
        progressValue = Mathf.Clamp(fog.AlreadyPaid.Amount + progressValue, fog.AlreadyPaid.Amount, fog.Def.Condition.Amount);

        var progress = Progress(lineFake, fog.Def.Condition.Amount, progressValue, out var time);
        lineFake
            .DOSizeDelta(new Vector2(progress, lineFake.sizeDelta.y), time * 0.5f)
            .SetId(lineFake)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if(value > 0) StartHighlight();
            })
            .OnComplete(() =>
            {
                if(value == 0) DOTween.Kill(light);
            });

        if (value > 0)
        {
            AddArrow();
        }
    }

    private void AddArrow()
    {
        if (arrow == null)
        {
            arrow = HintArrowView.Show(hintAnchor, 0, 0, false, false, 0);
            arrow.AddOnRemoveAction(() => { arrow = null; });
        }
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