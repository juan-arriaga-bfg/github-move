public class UICreditsWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.credits.title", "window.credits.title");
    
    public string Team => Replace(LocalizationService.Get("window.credits.team", "window.credits.team"));
    public string BigFish => Replace(LocalizationService.Get("window.credits.teamBigFish", "window.credits.teamBigFish"));
    public string Trademark => LocalizationService.Get("window.credits.trademark", "window.credits.trademark");

    private string Replace(string value)
    {
        return value
            .Replace("{style}", "<font=\"POETSENONE-REGULAR SDF\" material=\"POETSENONE-REGULAR Default\"><color=#983A07>")
            .Replace("{/style}", "</color></font>")
            .Replace("{", "<")
            .Replace("}", ">");
    }
}
