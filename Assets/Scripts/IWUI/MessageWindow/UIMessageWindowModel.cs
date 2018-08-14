using System;

public class UIMessageWindowModel : IWWindowModel
{
    public string Title { get; set; }
    public string Message { get; set; }
    
    public bool isHardAccept { get; set; }
    
    public string AcceptLabel { get; set; }
    public string CancelLabel { get; set; }
    
    public string Image { get; set; }
    public TimerComponent Timer { get; set; }

    public Action OnAccept { get; set; }
    public Action OnCancel { get; set; }
}
