using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private List<Image> backs;
    
    [SerializeField] private NSText heroLabel;
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText messageLabel;
    
    [SerializeField] private Button button;
    
    private List<UIHeroCollectionItem> collection;
    private List<UIHeroChargerItem> chargers;
    
    private Hero hero;
    
    public void Decoration(Hero character)
    {
        hero = character;
        
        var isUnlock = hero.IsUnlock;
        var isCollect = hero.IsCollect;
        
        if (collection == null) collection = GetComponentsInChildren<UIHeroCollectionItem>().ToList();
        if (chargers == null) chargers = GetComponentsInChildren<UIHeroChargerItem>().ToList();

        for (var i = 0; i < hero.Collection.Count; i++)
        {
            if(i >= collection.Count) break;
            
            collection[i].Decoration(hero.Collection[i].Currency, isUnlock);
        }
        
        for (var i = 0; i < hero.Price.Count; i++)
        {
            if(i >= chargers.Count) break;
            
            chargers[i].Decoration(hero.Price[i], isUnlock, isCollect);
        }
        
        if(isUnlock) icon.sprite = IconService.Current.GetSpriteById(string.Format("{0}_head" , hero.Def.Uid));
        icon.color = new Color(1, 1, 1, isCollect ? 1 : 0.3f);
        
        heroLabel.Text = hero.Def.Uid;
        messageLabel.Text = string.Format("Unlock at {0} level", hero.Def.Level);
        buttonLabel.Text = string.Format("Hire{0}", hero.IsCollectChargers ? "" : string.Format(" {0}<sprite name={1}>", hero.ChargersPrice, Currency.Crystals.Name));
        
        button.interactable = isUnlock && !isCollect;
        
        button.gameObject.SetActive(!isCollect && isUnlock && hero.IsCollectCollections);
        messageLabel.gameObject.SetActive(!isUnlock);
        heroLabel.gameObject.SetActive(isUnlock);
        
        foreach (var back in backs)
        {
            back.sprite = IconService.Current.GetSpriteById(string.Format("back_{0}active", isUnlock ? "" : "not_"));
        }
    }
    
    public void OnClick()
    {
        if (hero.IsCanPurchase)
        {
            hero.Purchase();
            Decoration(hero);
        }

        if (hero.IsCollectChargers) return;

        if (CurrencyHellper.IsCanPurchase(Currency.Crystals.Name, hero.ChargersPrice))
        {
            hero.PurchaseChargers();
            Decoration(hero);
            return;
        }
        
        UIMessageWindowController.CreateNeedCurrencyMessage(Currency.Crystals.Name);
    }
}