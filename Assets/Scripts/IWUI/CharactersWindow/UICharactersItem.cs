using UnityEngine;
using UnityEngine.UI;

public class UICharactersItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    [SerializeField] private NSText name;
    [SerializeField] private NSText buttonLabel;
    
    [SerializeField] private GameObject back;
    [SerializeField] private GameObject button;

    private Hero characters;
    
    public void Decoration(Hero hero)
    {
        characters = hero;
        
        if (hero == null)
        {
            back.SetActive(false);
            button.SetActive(false);
            name.gameObject.SetActive(false);
            icon.color = new Color(1, 1, 1, 0.5f);
            return;
        }

        var isSleep = hero.IsSleep;
        
        back.SetActive(!isSleep);
        button.SetActive(isSleep);
        name.gameObject.SetActive(!isSleep);

        icon.sprite = IconService.Current.GetSpriteById(string.Format("{0}_head", hero.Def.Uid));
        icon.color = new Color(1, 1, 1, isSleep ? 0.5f : 1);
        
        name.Text = hero.Def.Uid;
        buttonLabel.Text = string.Format("Wake Up {0}<sprite name=Crystals>", 5);
    }

    public void OnClick()
    {
        if(characters == null) return;
        
        characters.WakeUp();
    }
}