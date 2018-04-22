﻿using System;
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
        
        cardLabel.Text = string.Format("<color=#3D7AA4>{0}</color>", "Rare");

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
        levelLabel.Text = string.Format("<size=45>{0}</size> <size=35>Lvl</size>", 1);
        
        progressLabel.Text = string.Format("{0}/{1}", 50, 100);
        progress.sizeDelta = new Vector2(Mathf.Clamp(190 * 50 / (float) 100, 0, 190), progress.sizeDelta.y);
        
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
        if (obstacle != null && hero.GetAbilityValue(ability.Ability) >= ability.Value)
        {
            toggle.isOn = !toggle.isOn;
            return;
        }
    }
    
    public void OnSelect()
    {
        if(isInit) return;

        if (obstacle == null || hero.GetAbilityValue(ability.Ability) < ability.Value)
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
        skillLabel.Text = string.Format("<color=#{0}>{1}</color>", color, hero.GetAbilityValue(AbilityType.Power));
        
        progressGo.SetActive(true);
        buttonGo.SetActive(50 >= 100);

        toggle.interactable = true;
        toggle.isOn = false;
        
        skillIcon.sprite = IconService.Current.GetSpriteById(hero.GetAbilityValue(AbilityType.Power).ToString());
    }

    private void InitForQuest()
    {
        var isWaiting = hero.GetAbilityValue(ability.Ability) >= ability.Value;
        var inMission = hero.InAdventure != -1;
        
        var message = !inMission ? (isWaiting ? "Ready" : "Skill to Low") : "In Mission";
        var color = !inMission && isWaiting ? "00FF00" : "FF0000";
        var skillMessage = string.Format("{0}/{1}", hero.GetAbilityValue(ability.Ability), ability.Value);
        
        buttonLabel.Text = "Send";
        messageLabel.Text = string.Format("<color=#{0}>{1}</color>", color, message);
        skillLabel.Text = string.Format("<color=#{0}>{1}</color>", color, skillMessage);
        
        progressGo.SetActive(true);
        
        skillIcon.sprite = IconService.Current.GetSpriteById(ability.Ability.ToString());
        
        
        if (ability.Ability == AbilityType.Power)
        {
            toggle.interactable = !inMission || hero.InAdventure == obstacle.GetUid();
            
            toggle.isOn = hero.InAdventure == obstacle.GetUid();
            buttonGo.SetActive(!inMission && !toggle.isOn);
            
            if (!isWaiting && 50 >= 100)
            {
                buttonLabel.Text = "Level UP";
                buttonGo.SetActive(true);
            }
        }
        else
        {
            toggle.interactable = !inMission;
            
            buttonLabel.Text = "Level UP";
            toggle.isOn = false;
            buttonGo.SetActive(!inMission && 50 >= 100);
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