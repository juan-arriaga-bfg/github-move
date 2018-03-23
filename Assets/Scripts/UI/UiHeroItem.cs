using System;
using UnityEngine;
using UnityEngine.UI;

public class UiHeroItem : MonoBehaviour
{
    [SerializeField] private NSText nameLabel;
    [SerializeField] private NSText levelLabel;
    [SerializeField] private NSText messageLabel;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private NSText skillLabel;
    [SerializeField] private NSText cardLabel;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private Image heroIcon;
    [SerializeField] private Image heroSmallIcon;
    [SerializeField] private Image skillIcon;
    
    [SerializeField] private RectTransform progress;
    
    [SerializeField] private GameObject progressGo;
    [SerializeField] private GameObject buttonGo;
    
    [SerializeField] private Toggle toggle;

    private Hero hero;
    private Obstacle obstacle;
    private HeroAbility ability;

    private bool isInit;
    private bool isReady;
    
    public void Init(Hero hero, Obstacle obstacle, HeroAbility ability)
    {
        this.hero = hero;
        this.obstacle = obstacle;
        this.ability = ability;
        
        isInit = true;
        
        cardLabel.Text = string.Format("<color=#3D7AA4>{0}</color>", CharacterWindowCardTupe.Rare);

        if (hero == null)
        {
            InitFake();
            skillIcon.gameObject.SetActive(false);
            return;
        }
        
        var iconHero = IconService.Current.GetSpriteById("face_" + hero.Def.Uid);
        
        heroIcon.sprite = iconHero;
        heroSmallIcon.sprite = iconHero;
        
        nameLabel.Text = hero.Def.Uid;
        levelLabel.Text = string.Format("<size=45>{0}</size> <size=35>Lvl</size>", hero.Level + 1);
        
        progressLabel.Text = string.Format("{0}/{1}", hero.CurrentProgress, hero.TotalProgress);
        progress.sizeDelta = new Vector2(Mathf.Clamp(190 * hero.CurrentProgress / (float) hero.TotalProgress, 0, 190), progress.sizeDelta.y);
        
        skillIcon.gameObject.SetActive(true);

        if (obstacle == null)
        {
            InitForTavern();
        }
        else
        {
            InitForQuest();
        }
        
        isInit = false;
    }

    public void OnClick()
    {
        if (obstacle != null)
        {
            toggle.isOn = !toggle.isOn;
            return;
        }
        
        var model = UIService.Get.GetCachedModel<UICharacterWindowModel>(UIWindowType.CharacterWindow);

        model.Hero = hero;
        
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);
    }
    
    public void OnSelect()
    {
        if(isInit) return;

        if (obstacle == null)
        {
            OnClick();
            return;
        }
        
        hero.InAdventure = toggle.isOn ? obstacle.GetUid() : -1;
        InitForQuest();
        if(toggle.isOn) UIService.Get.CloseWindow(UIWindowType.TavernWindow, true);
    }
    
    private void InitForTavern()
    {
        var inAdventure = hero.InAdventure == -1;
        
        var message = inAdventure ? "Ready" : "In Mission";
        var color = inAdventure ? "00FF00" : "FF0000";
        
        buttonLabel.Text = "Level UP";
        messageLabel.Text = string.Format("<color=#{0}>{1}</color>", color, message);
        skillLabel.Text = string.Format("<color=#{0}>{1}</color>", color, hero.CurrentAbilityValue);
        
        progressGo.SetActive(true);
        buttonGo.SetActive(hero.CurrentProgress >= hero.TotalProgress);

        toggle.interactable = true;
        toggle.isOn = false;
        
        skillIcon.sprite = IconService.Current.GetSpriteById(hero.CurrentAbility.ToString());
    }

    private void InitForQuest()
    {
        var isWaiting = hero.GetAbilityValue(ability.Ability) >= ability.Value;
        
        var message = hero.InAdventure == -1 ? (isWaiting ? "Ready" : "Skill to Low") : "In Mission";
        var color = hero.InAdventure == -1 && isWaiting ? "00FF00" : "FF0000";
        var skillMessage = string.Format("{0}/{1}", hero.GetAbilityValue(ability.Ability), ability.Value);
        
        buttonLabel.Text = "Send";
        messageLabel.Text = string.Format("<color=#{0}>{1}</color>", color, message);
        skillLabel.Text = string.Format("<color=#{0}>{1}</color>", color, skillMessage);
        
        progressGo.SetActive(true);
        
        skillIcon.sprite = IconService.Current.GetSpriteById(ability.Ability.ToString());
        
        if (ability.Ability == hero.CurrentAbility)
        {
            toggle.interactable = true;
            toggle.isOn = hero.InAdventure == obstacle.GetUid();
            buttonGo.SetActive(!toggle.isOn);
        }
        else
        {
            toggle.interactable = false;
            toggle.isOn = false;
            buttonGo.SetActive(false);
        }
    }

    private void InitFake()
    {
        nameLabel.Text = "";
        levelLabel.Text = "";
        messageLabel.Text = "Not Available";
        progressLabel.Text = "";
        skillLabel.Text = "";
        
        var iconHero = IconService.Current.GetSpriteById("face_Unknown");
        
        heroIcon.sprite = iconHero;
        heroSmallIcon.sprite = iconHero;
        
        progressGo.SetActive(false);
        buttonGo.SetActive(false);
        
        toggle.interactable = false;
    }
}