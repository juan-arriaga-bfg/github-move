using DG.Tweening;
using UnityEngine;

public class DailyQuestWindowLineItemView : IWUIWindowViewController
{
    public enum State
    {
        Unknown,
        Empty,
        Check,
        Fill
    }
    
    [IWUIBinding("#LineItemEmpty")] private GameObject empty;
    [IWUIBinding("#LineItemCheck")] private GameObject check;
    [IWUIBinding("#LineItemFill")] private GameObject fill;
    [IWUIBinding("#LineItemFillHighlight")] private GameObject fillHiglhlight;
    
    public void UpdateUi(State state)
    {
        CacheResources();
        
        empty.SetActive(state == State.Empty); 
        check.SetActive(state == State.Check); 
        
        DOTween.Kill(this);
        if (state == State.Fill)
        {
            var interval = 0.5f;
            var sequence = DOTween.Sequence();
            
            fill.SetActive(true);
            fillHiglhlight.SetActive(false);
            sequence.InsertCallback(0.0f, () =>
            {
                fillHiglhlight.SetActive(!fillHiglhlight.activeSelf);
                fill.SetActive(!fill.activeSelf);
            });
            sequence.AppendInterval(interval);
            sequence.SetLoops(-1);
        }
        else
        {
            fill.SetActive(false);
            fillHiglhlight.SetActive(false);
        }
    }
}