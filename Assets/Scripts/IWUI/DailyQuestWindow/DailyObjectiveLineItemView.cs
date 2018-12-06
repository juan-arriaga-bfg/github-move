using UnityEngine;

public class DailyObjectiveLineItemView : IWBaseMonoBehaviour
{
    public enum State
    {
        Unknown,
        Empty,
        Check,
        Fill
    }
    
    [SerializeField] private GameObject empty;
    [SerializeField] private GameObject check;
    [SerializeField] private GameObject fill;
    
    public void Init(State state)
    {
        empty.SetActive(state == State.Empty); 
        check.SetActive(state == State.Check); 
        fill.SetActive(state == State.Fill); 
    }
}