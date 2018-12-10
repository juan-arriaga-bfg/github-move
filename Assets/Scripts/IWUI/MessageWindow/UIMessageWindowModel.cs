using System;

public class UIMessageWindowModel : IWWindowModel
{
    [Flags]
    public enum WindowComponents
    {
        Auto                       = 0,
        Image                      = 2,
        ImageAndMessageDelimiter   = 4,
        Message                    = 8,
        MessageAndButtonsDelimiter = 16,
        Timer                      = 32,
        ButtonAccept               = 64,
        ButtonCancel               = 128,
        ButtonBuy                  = 256
    }
    
    public enum ButtonColor
    {
        Red,
        Green,
        Blue
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
    public TimerComponent Timer { get; set; }

    public Action OnAccept { get; set; }
    public Action OnCancel { get; set; }
    public Action OnClose { get; set; }

    public WindowComponents VisibleComponents;

    public void ResetColor()
    {
        AcceptColor = ButtonColor.Green;
        CancelColor = ButtonColor.Red;
    }
}
