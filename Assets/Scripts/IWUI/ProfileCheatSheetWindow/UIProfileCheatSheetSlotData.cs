public class UIProfileCheatSheetSlotData
{
    public int SlotIndex;
    public string SlotPath;
    public ProfileManager<UserProfile> Profile;
    /// <summary>
    /// Means that we can't use profile for some reason and default profile created
    /// </summary>
    public bool IsDefault;

    public string Error;
}