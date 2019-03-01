public class UIProfileCheatSheetElementViewController : UIContainerElementViewController
{
    [IWUIBinding("#LblRev")] private NSText lblRev;
    [IWUIBinding("#LblTimestamp")] private NSText lblTimestamp;
    [IWUIBinding("#LblData")] private NSText lblData;
    [IWUIBinding("#LblGameVersion")] private NSText lblGameVersion;
    
    [IWUIBinding("#BtnSave")] private UIButtonViewController btnDlgSave;
    [IWUIBinding("#BtnLoad")] private UIButtonViewController btnDlgLoad;
    [IWUIBinding("#BtnDel")]  private UIButtonViewController btnDel;

    private UIProfileCheatSheetElementEntity targetEntity;
    private UIProfileCheatSheetSlotData slotData;
    private ProfileManager<UserProfile> profile;
    private UserProfile userProfile;

    public override void Init()
    {
        base.Init();
        
        targetEntity = entity as UIProfileCheatSheetElementEntity;
        slotData = targetEntity.SlotData;
        profile = slotData.Profile;
        userProfile = profile.CurrentProfile;
        
        InitButtons();
        
        UpdateUi();
    }

    private void InitButtons()
    {
        btnDlgSave.OnClick(() =>
        {

        });
        
        btnDlgLoad.OnClick(() =>
        {

        });
        
        btnDel.OnClick(() =>
        {

        });

        if (slotData.SlotId == 0)
        {
            btnDlgLoad.gameObject.SetActive(false);
            btnDel.gameObject.SetActive(false);
        }
    }


    public override void OnViewClose(IWUIWindowView context)
    {
        base.OnViewClose(context);
    }

    public void UpdateUi()
    {
        GameDataManager dm = new GameDataManager();
        dm.SetupComponents(userProfile);
        dm.Reload();
        
        lblRev.Text = $"Rev {userProfile.Version.ToString()}";
        lblTimestamp.Text = $"at {userProfile.Timestamp}";
        lblTimestamp.Text = $"at {userProfile.Timestamp}";

        var level = dm.LevelsManager.Level;
        
        lblData.Text = $"{level}";
    }

    private string Colorize(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
   
}