using UnityEngine;
using UnityEngine.UI;

public class UiQuestStartItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image iconHero;
    [SerializeField] private Image background;
    
    [SerializeField] private NSText btnLabel;
    [SerializeField] private NSText progressLabel;
    
    [SerializeField] private RectTransform progress;
    
    private Obstacle obstacle;
    private Hero hero;
    private HeroAbility ability;
    
    public void Init(Obstacle obstacle, HeroAbility ability)
    {
        this.obstacle = obstacle;
        this.ability = ability;
        
        background.sprite = IconService.Current.GetSpriteById(string.Format("back_{0}", ability.Ability));
        icon.sprite = IconService.Current.GetSpriteById(ability.Ability.ToString());

        hero = GameDataService.Current.HeroesManager.GetHeroes(ability.Ability).Find(h => h.InAdventure == this.obstacle.GetUid());
        
        var currentValue = hero == null ? 0 : hero.CurrentAbilityValue;
        
        btnLabel.Text = string.Format("<size=65>{0}</size>", hero == null ? "+" : "v");
        progressLabel.Text = string.Format("{0}/{1}", currentValue, ability.Value);
        
        progress.sizeDelta = new Vector2(Mathf.Clamp(145 * currentValue / (float) ability.Value, 0, 145), progress.sizeDelta.y);
        
        if(hero != null)
        {
            iconHero.sprite = IconService.Current.GetSpriteById(string.Format("face_{0}", hero.Def.Uid));
            icon.gameObject.SetActive(false);
            iconHero.gameObject.SetActive(true);
            return;
        }
        
        icon.gameObject.SetActive(true);
        iconHero.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UITavernWindowModel>(UIWindowType.TavernWindow);
        
        model.Obstacle = obstacle;
        model.CurrentAbility = ability;
        
        UIService.Get.ShowWindow(UIWindowType.TavernWindow);
    }

    public void HeroGoHome()
    {
        if (hero != null && hero.InAdventure == obstacle.GetUid()) hero.InAdventure = -1;
    }
    
    public bool InAdventure()
    {
        return !gameObject.activeSelf || hero != null && hero.InAdventure == obstacle.GetUid() && hero.CurrentAbilityValue >= ability.Value;
    }
}