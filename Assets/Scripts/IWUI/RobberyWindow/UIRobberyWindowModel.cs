using UnityEngine;

public class UIRobberyWindowModel : IWWindowModel 
{
    public Enemy Enemy { get; set; }
    public EnemyView View { get; set; }
    
    public string Title
    {
        get { return "Stolen Treasures"; }
    }
    
    public string Message
    {
        get { return "Drag Your Heroes to rob Enemies"; }
    }
    
    public string SendText
    {
        get { return "Send Hero"; }
    }
    
    public string ClaimText
    {
        get { return "Claim Reward"; }
    }
    
    public Vector2 From { get; set; }
    public Vector2 To { get; set; }

    public bool IsAllSleep
    {
        get
        {
            return GameDataService.Current.HeroesManager.Heroes.Find(hero => hero.IsSleep == false) == null;
        }
    }

    public void Attack()
    {
        var model = UIService.Get.GetCachedModel<UICharactersWindowModel>(UIWindowType.CharactersWindow);
        var heroes = model.Items.FindAll(item => item.Hero != null && item.Hero.IsSleep == false);

        if (heroes.Count == 0) return;
        
        heroes.Shuffle();
        heroes[0].OnClickSend();
    }
}