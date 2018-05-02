using UnityEngine;
using UnityEngine.UI;

public class UICharactersItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    [SerializeField] private NSText nameLabel;
    [SerializeField] private NSText powerLabel;
    [SerializeField] private NSText buttonLabel;
    [SerializeField] private NSText timerLabel;
    
    [SerializeField] private GameObject back;
    
    [SerializeField] private GameObject buttonWakeUp;
    [SerializeField] private GameObject buttonSend;

    public Hero Hero;
    private CurrencyPair price = new CurrencyPair {Currency = "Crystals", Amount = 1};
    
    public void Decoration(Hero character)
    {
        Hero = character;
        
        if (Hero == null)
        {
            back.SetActive(false);
            buttonWakeUp.SetActive(false);
            buttonSend.SetActive(false);
            nameLabel.gameObject.SetActive(false);
            icon.color = new Color(1, 1, 1, 0.5f);
            return;
        }
        
        var model = UIService.Get.GetCachedModel<UIRobberyWindowModel>(UIWindowType.RobberyWindow);
        var isSleep = Hero.IsSleep;
        var isActiveEnemy = model.Enemy != null;
        
        back.SetActive(!isSleep);
        
        nameLabel.gameObject.SetActive(!isActiveEnemy || !isSleep && model.Enemy.ActiveReward != PieceType.None.Id);
        buttonSend.SetActive(isActiveEnemy && !isSleep && model.Enemy.ActiveReward == PieceType.None.Id);
        buttonWakeUp.SetActive(isActiveEnemy && isSleep);

        icon.sprite = IconService.Current.GetSpriteById(string.Format("{0}_head", Hero.Def.Uid));
        icon.color = new Color(1, 1, 1, isSleep ? 0.5f : 1);

        nameLabel.Text = Hero.Def.Uid;
        buttonLabel.Text = string.Format("Wake Up {0}<sprite name={1}>", price.Amount, price.Currency);
        powerLabel.Text = Hero.GetAbilityValue(AbilityType.Power).ToString();
    }
    
    public void OnClickWakeUp()
    {
        if(Hero == null) return;

        CurrencyHellper.Purchase("WakeUp", 1, price, succees =>
        {
            if (succees == false) return;
            Hero.WakeUp();
            
            var robbery = UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow);
            if (robbery != null) (robbery.CurrentView as UIRobberyWindowView).Decoration();
            
            var characters = UIService.Get.GetShowedWindowByName(UIWindowType.CharactersWindow);
            if (characters != null) (characters.CurrentView as UICharactersWindowView).UpdateDecoration();
        });
    }

    private void Update()
    {
        if(Hero == null || timerLabel.gameObject.activeInHierarchy == false) return;
        
        timerLabel.Text = string.Format("<mspace=2.5em>Zzz\n{0}</mspace>", Hero.GetSlepTime());
        
        if(Hero.IsSleep) return;
        
        var robbery = UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow);
        if (robbery != null) (robbery.CurrentView as UIRobberyWindowView).Decoration();
            
        var characters = UIService.Get.GetShowedWindowByName(UIWindowType.CharactersWindow);
        if (characters != null) (characters.CurrentView as UICharactersWindowView).UpdateDecoration();
    }

    public void OnClickSend()
    {
        if(Hero == null) return;
        
        var model = UIService.Get.GetCachedModel<UIRobberyWindowModel>(UIWindowType.RobberyWindow);

        model.From = GetComponent<RectTransform>().position;
        
        var robbery = UIService.Get.GetShowedWindowByName(UIWindowType.RobberyWindow);
        if (robbery != null) (robbery.CurrentView as UIRobberyWindowView).Attack(Hero);
        
        var characters = UIService.Get.GetShowedWindowByName(UIWindowType.CharactersWindow);
        if (characters != null) (characters.CurrentView as UICharactersWindowView).Fly(Hero);
    }
}