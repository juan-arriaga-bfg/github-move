using UnityEngine;
using UnityEngine.UI;

public class OfferButton : MonoBehaviour
{
    [SerializeField] private NSText timer;
    [SerializeField] private Image icon;
    
    private void OnEnable()
    {
        BoardService.Current.FirstBoard.MarketLogic.OfferTimer.OnTimeChanged += UpdateTimer;
        UpdateTimer();
    }

    private void OnDisable()
    {
        BoardService.Current.FirstBoard.MarketLogic.OfferTimer.OnTimeChanged -= UpdateTimer;
    }

    private void UpdateTimer()
    {
        timer.Text = BoardService.Current.FirstBoard.MarketLogic.OfferTimer.CompleteTime.GetTimeLeftText(true, false, null, false);
    }
}
