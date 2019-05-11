using System;

public class UIIslandMessageWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("window.island.title", "window.island.title");
    public string Message => LocalizationService.Get("window.island.message", "window.island.message");
    public string Button;

    public Action OnAccept;
    public Action OnClose;
}
