using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainWindowView : IWUIWindowView
{
    [SerializeField] private GameObject pattern;
    [SerializeField] private CodexButton codexButton;
    
    private List<UiQuestButton> quests = new List<UiQuestButton>();

    public override void OnViewShow()
    {
        base.OnViewShow();
        
        var windowModel = Model as UIMainWindowModel;

        GameDataService.Current.QuestsManager.OnUpdateActiveQuests += UpdateQuest;
        GameDataService.Current.CodexManager.OnNewItemUnlocked += OnNewPieceBuilded;
        
        UpdateQuest();
        UpdateCodexButton();
    }

    public override void OnViewClose()
    {
        GameDataService.Current.QuestsManager.OnUpdateActiveQuests -= UpdateQuest;
        GameDataService.Current.CodexManager.OnNewItemUnlocked -= OnNewPieceBuilded;
        
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
        
        InitWindowViewControllers();
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

        if (quests.Count <= active.Count) return;
        
        while (quests.Count != active.Count)
        {
            var item = quests[0];
                
            quests.RemoveAt(0);
            Destroy(item.gameObject);
        }
    }

    public void OnClickReset()
    {
        var model= UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = "Reset the progress";
        model.Message = "Do you want to reset the progress and start playing from the beginning?";
        model.AcceptLabel = "<size=30>Reset progress!</size>";
        model.CancelLabel = "No!";
        
        model.OnAccept = () =>
        {
            var profileBuilder = new DefaultProfileBuilder();
            ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
            
            GameDataService.Current.Reload();
        
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
            
            var ecsSystems = new List<IECSSystem>(ECSService.Current.SystemProcessor.RegisteredSystems);
            
            foreach (var system in ecsSystems)
            {
                ECSService.Current.SystemProcessor.UnRegisterSystem(system);
            }
        };
        
        model.OnCancel = () => {};
        
        UIService.Get.ShowWindow(UIWindowType.MessageWindow);
    }
    
    private void UpdateCodexButton()
    {
        codexButton.UpdateState();
    }
    
    public void OnClickCodex()
    {
        var codexManager = GameDataService.Current.CodexManager;

        if (codexManager.CodexState == CodexState.NewPiece)
        {
            codexManager.CodexState = CodexState.PendingReward;
        }
        
        codexButton.UpdateState();
        
        var model = UIService.Get.GetCachedModel<UICodexWindowModel>(UIWindowType.CodexWindow);
        model.ActiveTabIndex = 0;
        model.CodexContent = codexManager.GetCodexContent();
        model.OnClaim = UpdateCodexButton;
        
        UIService.Get.ShowWindow(UIWindowType.CodexWindow);
    }

    private void OnNewPieceBuilded()
    {
        codexButton.UpdateState();
    }
    
    public void Debug1()
    {
        // var board = BoardService.Current.GetBoardById(0);
        //
        // board.ActionExecutor.AddAction(new SpawnPieceAtAction
        // {
        //     At = new BoardPosition(20, 20, 1),
        //     PieceTypeId = PieceType.D4.Id
        // });
        
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.F3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.D3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.E3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.G3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.H3.Id);
        
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.A3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.B3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.C3.Id);
        
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.ChestC3.Id);
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.Basket3.Id);
        
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.Coin3.Id);
    }
    
    public void Debug2()
    {
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.A2.Id);
    }
    
    public void Debug3()
    {
        GameDataService.Current.CodexManager.OnPieceBuilded(PieceType.B2.Id);
    }
}
