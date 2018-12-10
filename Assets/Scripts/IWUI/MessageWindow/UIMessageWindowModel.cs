using System;

public class UIMessageWindowModel : IWWindowModel
{
    public enum ButtonColor
    {
        Red,
        Green,
        Blue
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
        
        AcceptColor = ButtonColor.Green;
        CancelColor = ButtonColor.Red;
    }
}
