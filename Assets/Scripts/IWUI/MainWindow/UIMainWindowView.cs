using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;
    
    private List<UiQuestButton> quests = new List<UiQuestButton>();
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        GameDataService.Current.QuestsManager.OnUpdateActiveQuests += UpdateQuest;
        
        UpdateQuest();
    }

    public override void OnViewClose()
    {
        GameDataService.Current.QuestsManager.OnUpdateActiveQuests -= UpdateQuest;
        base.OnViewClose();
    }

    public void UpdateQuest()
    {
        var active = GameDataService.Current.QuestsManager.ActiveQuests;
        
        CheckQuestButtons(active);
        
        for (var i = 0; i < active.Count; i++)
        {
            quests[i].Init(active[i]);
        }
    }

    private void CheckQuestButtons(List<Quest> active)
    {
        pattern.SetActive(true);

        if (quests.Count < active.Count)
        {
            for (var i = quests.Count; i < active.Count; i++)
            {
                var item = Instantiate(pattern, pattern.transform.parent);
                quests.Add(item.GetComponent<UiQuestButton>());
            }
        }
        
        pattern.SetActive(false);

        if (quests.Count > active.Count)
        {
            while (quests.Count != active.Count)
            {
                var item = quests[0];
                
                quests.RemoveAt(0);
                Destroy(item.gameObject);
            }
        }
    }

    public void OnClickReset()
    {
        var profileBuilder = new DefaultProfileBuilder();
        ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
        
        // gamedata configs
        GameDataManager dataManager = new GameDataManager();
        GameDataService.Instance.SetManager(dataManager);
        
        dataManager.ChestsManager.LoadData(new ResourceConfigDataMapper<List<ChestDef>>("configs/chests.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.EnemiesManager.LoadData(new ResourceConfigDataMapper<List<EnemyDef>>("configs/enemies.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.HeroesManager.LoadData(new ResourceConfigDataMapper<List<HeroDef>>("configs/heroes.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.PiecesManager.LoadData(new ResourceConfigDataMapper<List<PieceDef>>("configs/pieces.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.ObstaclesManager.LoadData(new ResourceConfigDataMapper<List<ObstacleDef>>("configs/obstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.SimpleObstaclesManager.LoadData(new ResourceConfigDataMapper<List<SimpleObstaclesDef>>("configs/simpleObstacles.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.QuestsManager.LoadData(new ResourceConfigDataMapper<List<QuestDef>>("configs/quests.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.FogsManager.LoadData(new ResourceConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.CollectionManager.LoadData(new ResourceConfigDataMapper<CollectionDataManager>("configs/collection.data", NSConfigsSettings.Instance.IsUseEncryption));
        dataManager.LevelsManager.LoadData(new ResourceConfigDataMapper<List<LevelsDef>>("configs/levels.data", NSConfigsSettings.Instance.IsUseEncryption));
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
