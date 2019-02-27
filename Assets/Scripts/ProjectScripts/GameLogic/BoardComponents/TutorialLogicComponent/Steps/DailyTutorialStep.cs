﻿using DG.Tweening;

public class DailyTutorialStep : UIArrowTutorialStep
{
    private enum Step
    {
        Start,
        FirstOpen,
        Quest,
        SecondOpen,
        Complete
    }

    private Step currentStep;
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        var view = UIService.Get.GetShowedView<UIMainWindowView>(UIWindowType.MainWindow);
        
        anchor = view.HintAnchorDailyButton;
        
        if (GameDataService.Current.QuestsManager.DailyQuest == null) GameDataService.Current.QuestsManager.StartNewDailyQuest();
        
        QuestChanged(GameDataService.Current.QuestsManager.DailyQuest, null);
        
        UIService.Get.OnShowWindowEvent += OnShowWindow;
        GameDataService.Current.QuestsManager.DailyQuest.OnChanged += QuestChanged;
        
        base.Perform();
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        UnSubscribe();
    }

    private void UnSubscribe()
    {
        UIService.Get.OnShowWindowEvent -= OnShowWindow;
        GameDataService.Current.QuestsManager.DailyQuest.OnChanged -= QuestChanged;
    }

    protected override void Complete()
    {
        currentStep = Step.Complete;

        UnSubscribe();
        
        var model = UIService.Get.GetCachedModel<UIDailyQuestWindowModel>(UIWindowType.DailyQuestWindow);
        
        model.IsTutorial = false;
        
        base.Complete();
    }

    protected override void CreateArrow()
    {
        if (currentStep != Step.Start && currentStep != Step.Quest) return;
        
        base.CreateArrow();
    }

    private void OnShowWindow(IWUIWindow window)
    {
        if(window.WindowName != UIWindowType.DailyQuestWindow) return;
        
        var model = UIService.Get.GetCachedModel<UIDailyQuestWindowModel>(UIWindowType.DailyQuestWindow);
        var view = UIService.Get.GetShowedView<UIDailyQuestWindowView>(UIWindowType.DailyQuestWindow);
        
        switch (currentStep)
        {
            case Step.Start:
            case Step.FirstOpen:
                currentStep = Step.FirstOpen;

                var sequence = DOTween.Sequence();

                sequence.AppendInterval(0.5f);
                sequence.AppendCallback(() => view.ScrollToFirstNotCompletedOrNotClaimedTask());
                break;
            case Step.Quest:
            case Step.SecondOpen:
                currentStep = Step.SecondOpen;
                model.IsTutorial = true;
                break;
        }
    }

    private void QuestChanged(QuestEntity quest, TaskEntity task)
    {
        if (quest.GetCompletedTasksCount() > 0 && currentStep < Step.Quest) currentStep = Step.Quest;
        if (isPauseOn == false && currentStep == Step.Quest) CreateArrow();
        if (quest.GetClaimedTasksCount() <= 0) return;
        
        currentStep = Step.Complete;
        isAutoComplete = true;
    }
}