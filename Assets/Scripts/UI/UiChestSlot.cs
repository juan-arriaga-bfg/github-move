using UnityEngine;
using UnityEngine.UI;

public class UiChestSlot : MonoBehaviour
{
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText timerLabel;
    
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    
    [SerializeField] private GameObject currencyIcon;
    [SerializeField] private GameObject timerGo;
    [SerializeField] private GameObject chestGo;
    [SerializeField] private GameObject buttonGo;
    
    public void Init(Chest chest)
    {
        if (chest == null)
        {
            timerGo.SetActive(false);
            chestGo.SetActive(false);
            buttonGo.SetActive(false);
            
            return;
        }
        
        var state = chest.State;
        
        chestGo.SetActive(true);
        timerGo.SetActive(state == ChestState.InProgress);
        buttonGo.SetActive(true);
        currencyIcon.SetActive(state == ChestState.InProgress);
        
        top.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, state == ChestState.Open ? 45 : 15);
        
        var id = chest.GetSkin();
        
        top.sprite = IconService.Current.GetSpriteById(id + "_2");
        bottom.sprite = IconService.Current.GetSpriteById(id + "_1");
    }
}