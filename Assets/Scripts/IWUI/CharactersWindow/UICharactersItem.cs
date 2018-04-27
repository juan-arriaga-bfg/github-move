using UnityEngine;
using UnityEngine.UI;

public class UICharactersItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    [SerializeField] private NSText nameLabel;
    [SerializeField] private NSText powerLabel;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private GameObject back;
    
    [SerializeField] private GameObject buttonWakeUp;
    [SerializeField] private GameObject buttonSend;

    private Hero character;
    private CurrencyPair price = new CurrencyPair {Currency = "Crystals", Amount = 1};
    
    public void Decoration(Hero hero)
    {
        character = hero;
        
        if (character == null)
        {
            back.SetActive(false);
            buttonWakeUp.SetActive(false);
            buttonSend.SetActive(false);
            nameLabel.gameObject.SetActive(false);
            icon.color = new Color(1, 1, 1, 0.5f);
            return;
        }
        
        var model = UIService.Get.GetCachedModel<UIRobberyWindowModel>(UIWindowType.RobberyWindow);
        
        var isSleep = character.IsSleep;
        
        back.SetActive(!isSleep);
        
        nameLabel.gameObject.SetActive(!isSleep && model.Enemy.ActiveReward != PieceType.None.Id);
        buttonSend.SetActive(!isSleep && model.Enemy.ActiveReward == PieceType.None.Id);
        buttonWakeUp.SetActive(isSleep);

        icon.sprite = IconService.Current.GetSpriteById(string.Format("{0}_head", hero.Def.Uid));
        icon.color = new Color(1, 1, 1, isSleep ? 0.5f : 1);

        nameLabel.Text = character.Def.Uid;
        buttonLabel.Text = string.Format("Wake Up {0}<sprite name={1}>", price.Amount, price.Currency);
        powerLabel.Text = hero.GetAbilityValue(AbilityType.Power).ToString();
    }
    
    public void OnClickWakeUp()
    {
        if(character == null) return;

        CurrencyHellper.Purchase("WakeUp", 1, price, succees =>
        {
            if (succees == false) return;
            character.WakeUp();
            
            var robbery = UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow);
            if (robbery != null) (robbery.CurrentView as UIRobberyWindowView).Decoration();
            
            var characters = UIService.Get.GetShowedWindowByName(UIWindowType.CharactersWindow);
            if (characters != null) (characters.CurrentView as UICharactersWindowView).UpdateDecoration();
        });
    }
    
    public void OnClickSend()
    {
        if(character == null) return;
        
        var robbery = UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow);
        if (robbery != null) (robbery.CurrentView as UIRobberyWindowView).Attack(character);
    }
}