using UnityEngine;
using System.Collections;

public class UIFireflyHintWindowModel : IWWindowModel 
{
    public string Title => LocalizationService.Get("highlight.task.firefly.window.title", "highlight.task.firefly.window.title");
}
