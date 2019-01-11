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

    public virtual void PlaySound()
    {
        NSAudioService.Current.Play(SoundId.ButtonClick);
    }

    public override void ClickEvent(int pointerId)
    {  
        base.ClickEvent(pointerId);
        PlaySound();
    }
}
