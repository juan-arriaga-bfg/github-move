using System;

public class UIMessageWindowModel : IWWindowModel
{
    public enum ButtonColor
    {
        Red,
        Green,
        Blue,
        Sepia
    }
    
    public enum HintType
    {
        FireflyHint,
        SuperMatchHint,
        RemoverHint
    }
    
    public string Title { get; set; }
    public string Message { get; set; }
    
    public bool IsHardAccept { get; set; }
    public bool IsBuy { get; set; }
    public bool IsTopMessage { get; set; }
    public bool IsShine { get; set; }

    public int ShineSize = 700;
    public int ButtonSize = 260;
    
    public bool IsAcceptLeft { get; set; }
    
    public string AcceptLabel { get; set; }
    public string CancelLabel { get; set; }

    public ButtonColor AcceptColor = ButtonColor.Green;
    public ButtonColor CancelColor = ButtonColor.Red;
    
    public string Image { get; set; }
    public string Prefab { get; set; }
    
    public TimerComponent Timer { get; set; }

    public Action OnAccept { get; set; }
    public Action OnCancel { get; set; }
    public Action OnClose { get; set; }
    
    /// <summary>
    /// Disable (X), background click, hardware back button.
    /// Also disables autoclose on accept and other actions 
    /// </summary>
    public bool ProhibitClose;
    
    public void Reset()
    {
        Title = null;
        Message = null;

        AcceptLabel = null;
        CancelLabel = null;
        
        Image = null;
        Prefab = null;
        
        Timer = null;
        
        IsHardAccept = false;
        IsBuy = false;
        IsAcceptLeft = false;
        IsTopMessage = false;
        IsShine = false;
        
        ShineSize = 700;
        ButtonSize = 260;
        
        AcceptColor = ButtonColor.Green;
        CancelColor = ButtonColor.Red;

        ProhibitClose = false;
    }
}
