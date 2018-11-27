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
    
    public string Title { get; set; }
    public string Message { get; set; }
    
    public bool isHardAccept { get; set; }
    public bool isBuy { get; set; }
    
    public string AcceptLabel { get; set; }
    public string CancelLabel { get; set; }
    
    public string Image { get; set; }
    public TimerComponent Timer { get; set; }

    public Action OnAccept { get; set; }
    public Action OnCancel { get; set; }

    public WindowComponents VisibleComponents;
}
