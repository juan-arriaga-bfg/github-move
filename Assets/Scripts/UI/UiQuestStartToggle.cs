using UnityEngine;
using UnityEngine.UI;

public class UiQuestStartToggle : MonoBehaviour
{
    [SerializeField] private GameObject icon;
    [SerializeField] private GameObject iconHero;
    [SerializeField] private GameObject button;
    
    public string heroName;
    
    private Obstacle obstacle;
    
    public void Init(Obstacle obstacle)
    {
        this.obstacle = obstacle;
        
        var hero = GameDataService.Current.HeroesManager.GetHero(heroName);
        
        if(hero == null) return;
        
        var InAdventure = hero.InAdventure == obstacle.GetUid();
        
        icon.SetActive(!InAdventure);
        iconHero.SetActive(InAdventure);
        button.SetActive(!InAdventure);
    }

    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UITavernWindowModel>(UIWindowType.TavernWindow);

        model.Obstacle = obstacle;
        
        UIService.Get.ShowWindow(UIWindowType.TavernWindow);
    }

    public void HeroInHome()
    {
        var hero = GameDataService.Current.HeroesManager.GetHero(heroName);

        if (hero != null && hero.InAdventure == obstacle.GetUid()) hero.InAdventure = -1;
    }
    
    public bool InAdventure()
    {
        return !gameObject.activeSelf || iconHero.activeSelf;
    }
}