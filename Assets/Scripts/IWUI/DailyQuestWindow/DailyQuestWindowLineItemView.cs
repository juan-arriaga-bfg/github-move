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

    public void UpdateUi(State state)
    {
        CacheResources();
        
        empty.SetActive(state == State.Empty); 
        check.SetActive(state == State.Check); 
        fill.SetActive(state == State.Fill); 
    }
}