using UnityEngine;
using System.Collections;

public class UICharacterUnlockedWindowView : UIGenericWindowView
{
    [IWUIBinding("#UnlockedCharacterContainer")] protected RectTransform unlockedCharacterContainer;
    
    [IWUIBinding("#RewardsLabel")] protected NSText rewardsLabel;
    
    [IWUIBinding("#CharacterNameLabel")] protected NSText characterNameLabel;
    
    [IWUIBinding("#TapToContinueAnchor")] protected Transform tapToContinueAnchor;

    private TapToContinueTextViewController tapToContinue;

    private bool isClick;

    public override float DefaultDelayOnClose => 0.65f;
    
    protected UICharacterViewController cachedCharacter = null;
    
    private int tapAnimationId = Animator.StringToHash("Tap");
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UICharacterUnlockedWindowModel windowModel = Model as UICharacterUnlockedWindowModel;
        
        // create character
        CreateCharacter(windowModel.CharacterId);

        // set rewards
        rewardsLabel.Text = windowModel.RewardsString;

        // set character name
        UICharacterDef charDef = UiCharacterData.GetDef(windowModel.CharacterId);
        characterNameLabel.Text = LocalizationService.Get(charDef.Name, charDef.Name);
        
        isClick = true;
    }

    public override void OnViewClose()
    {
        base.OnViewClose();
        
        UICharacterUnlockedWindowModel windowModel = Model as UICharacterUnlockedWindowModel;
        
    }
    
    public override void OnViewShowCompleted()
    {
        base.OnViewShowCompleted();
        
        isClick = false;
        
        if(btnBackLayer != null) btnBackLayer.ToState(GenericButtonState.Active).OnClick(OnClick);
    }

    public override void OnViewCloseCompleted()
    {
        base.OnViewCloseCompleted();

        if (cachedCharacter != null)
        {
            UIService.Get.PoolContainer.Return(cachedCharacter.gameObject);
            cachedCharacter = null;
        }
    }
    
    public override void AnimateShow()
    {
        base.AnimateShow();
        InitTapToContinue(1f);
    }
    
    private void InitTapToContinue(float delay)
    {
        if (tapToContinue == null)
        {
            var pool = UIService.Get.PoolContainer;
            tapToContinue = pool.Create<TapToContinueTextViewController>(R.TapToContinueTextView);
            tapToContinue.transform.SetParent(tapToContinueAnchor, false);
            tapToContinue.transform.localPosition = Vector3.zero;
        }
        
        tapToContinue.Hide(false);
        tapToContinue.Show(true, delay);
    }

    protected virtual UICharacterViewController CreateCharacter(string characterId)
    {
        if (cachedCharacter != null)
        {
            UIService.Get.PoolContainer.Return(cachedCharacter.gameObject);
            cachedCharacter = null;
        }
        
        string viewName = $"UICharacter{characterId}View";
        
        UICharacterViewController character = UIService.Get.PoolContainer.Create<UICharacterViewController>(viewName);
        character.CharacterId = characterId;

        Transform anchor = unlockedCharacterContainer;

        character.transform.SetParentAndReset(anchor);
        
        character.Side = CharacterSide.Right;
        
        character.ToggleActive(true, CharacterEmotion.Normal, false);
        
        character.ResetPivotAndSizeToCenter();

        cachedCharacter = character;

        return character;
    }
    
    public void OnClick()
    {
        if(isClick) return;

        isClick = true;
        
        for (var i = 0; i < viewAnimators.Count; i++)
        {
            viewAnimators[i].SetTrigger(tapAnimationId);
        }
                
        Controller.CloseCurrentWindow();
    }
}
