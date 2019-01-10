using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseButtonViewController : UIButtonViewController 
{
    
    public override void UpdateView()
    {
        base.UpdateView();
        
    }

    public override void ClickEvent(int pointerId)
    {
        NSAudioService.Current.Play(SoundId.button_click);
        
        base.ClickEvent(pointerId);
    }
}
