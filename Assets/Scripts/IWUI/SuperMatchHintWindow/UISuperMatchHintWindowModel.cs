using UnityEngine;
using System.Collections;

public class UISuperMatchHintWindowModel : IWWindowModel
{
    public string Title => LocalizationService.Get("highlight.task.supermatch.window.title", "highlight.task.supermatch.window.title");
}
