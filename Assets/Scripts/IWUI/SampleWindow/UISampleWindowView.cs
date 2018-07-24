using UnityEngine;

public class UISampleWindowView : UIGenericWindowView
{
    [SerializeField] private NSText label;
    
    public override void OnViewShow()
    {
        base.OnViewShow();
        
        UISampleWindowModel windowModel = Model as UISampleWindowModel;

        label.Text = string.Format("RandomNumber:{0}", windowModel.RandomNumber.ToString());

    }

    public override void OnViewClose()
    {
        base.OnViewClose();

        
        UISampleWindowModel windowModel = Model as UISampleWindowModel;
        
    }
}