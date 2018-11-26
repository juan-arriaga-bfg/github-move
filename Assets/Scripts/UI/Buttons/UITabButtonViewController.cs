using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITabButtonViewController : UIBaseButtonViewController 
{
    [IWUIBinding("#TabBack")]  private Image tabBack;

    [IWUIBinding("#TabLabel")]  private NSText tabLabel;
    
    private Color activeBackColor = new Color(1f,1f,1f,1f);
    private Color unActiveBackColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    public override void UpdateView()
    {
        base.UpdateView();

        DOTween.Kill(tabBack);
        var sequence = DOTween.Sequence().SetId(tabBack);
        sequence.Insert(0f, tabBack.DOColor(state == GenericButtonState.Active ? activeBackColor : unActiveBackColor, 0.35f));
        sequence.Insert(0f, tabLabel.TextLabel.DOColor(state == GenericButtonState.Active ? Color.white : Color.gray, 0.35f));
    }

}
