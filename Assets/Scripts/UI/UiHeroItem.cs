using System;
using UnityEngine;
using UnityEngine.UI;

public class UiHeroItem : MonoBehaviour
{
    [SerializeField] private NSText nameLabel;
    [SerializeField] private NSText levelLabel;
    [SerializeField] private NSText mesageLabel;
    [SerializeField] private NSText progressLabel;
    [SerializeField] private NSText skillLabel;
    [SerializeField] private NSText cardLabel;
    
    [SerializeField] private Image heroIcon;
    [SerializeField] private Image heroSmallIcon;
    [SerializeField] private Image skillIcon;
    
    [SerializeField] private RectTransform progress;
    
    [SerializeField] private GameObject progressGo;
    [SerializeField] private GameObject buttonGo;

    private Hero hero;
    
    public void Init(Hero hero)
    {
        this.hero = hero;

        var isWaiting = hero.CurrentProgress >= hero.TotalProgress;
        
        var message = hero.InAdventure == -1 ? (isWaiting ? "Ready" : "Skill too Low") : "In Mission";
        var color = hero.InAdventure == -1 && isWaiting ? "00FF00" : "FF0000";
        
        var iconHero = IconService.Current.GetSpriteById("face_" + hero.Def.Uid);

        nameLabel.Text = hero.Def.Uid;
        mesageLabel.Text = string.Format("<color=#{0}>{1}</color>", color, message);
        levelLabel.Text = string.Format("<size=45>{0}</size> <size=35>Lvl</size>", hero.Level + 1);
        skillLabel.Text = string.Format("<color=#{0}>{1}</color>", color, hero.CurrentAbilityValue);
        
        cardLabel.Text = string.Format("<color=#3D7AA4>{0}</color>", CharacterWindowCardTupe.Rare);
        
        progressLabel.Text = string.Format("{0}/{1}", hero.CurrentProgress, hero.TotalProgress);
        progress.sizeDelta = new Vector2(Mathf.Clamp(190 * hero.CurrentProgress / (float) hero.TotalProgress, 0, 190), progress.sizeDelta.y);


        heroIcon.sprite = iconHero;
        heroSmallIcon.sprite = iconHero;
        
        skillIcon.sprite = IconService.Current.GetSpriteById(hero.CurrentAbility.ToString());
        
        progressGo.SetActive(!isWaiting);
        buttonGo.SetActive(isWaiting);
    }

    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UICharacterWindowModel>(UIWindowType.CharacterWindow);

        model.Hero = hero;
        
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);
    }

    public void OnSelect()
    {
        
    }
}