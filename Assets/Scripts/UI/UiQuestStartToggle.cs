using UnityEngine;
using UnityEngine.UI;

public class UiQuestStartToggle : MonoBehaviour
{
    [SerializeField] private GameObject icon;
    [SerializeField] private GameObject iconHero;
    [SerializeField] private GameObject button;
    
    public string heroName;

    private int uid;
    private bool isInit;
    
    public void Init(int uid)
    {
        this.uid = uid;
        
        isInit = true;
        
        icon.SetActive(true);
        iconHero.SetActive(false);
        button.SetActive(true);

        GetComponent<Toggle>().isOn = false;
        
        isInit = false;
    }

    public void Onclick(Toggle toggle)
    {
        var hero = GameDataService.Current.HeroesManager.GetHero(heroName);

        if (hero == null || (hero.InAdventure != -1 && hero.InAdventure != uid))
        {
            if (isInit == false)
            {
                UIMessageWindowController.CreateDefaultMessage("This hero is busy on another quest. Complete that quest to use the hero.");
            }
            
            return;
        }
        
        icon.SetActive(!toggle.isOn);
        iconHero.SetActive(toggle.isOn);
        button.SetActive(!toggle.isOn);
        
        hero.InAdventure = toggle.isOn ? uid : -1;
    }

    public void HeroInHome()
    {
        var hero = GameDataService.Current.HeroesManager.GetHero(heroName);

        if (hero != null && hero.InAdventure == uid) hero.InAdventure = -1;
    }
    
    public bool InAdventure()
    {
        return !gameObject.activeSelf || iconHero.activeSelf;
    }
}