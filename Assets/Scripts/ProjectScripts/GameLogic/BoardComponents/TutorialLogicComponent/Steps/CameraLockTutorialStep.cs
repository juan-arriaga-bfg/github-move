using UnityEngine;

public class CameraLockTutorialStep : BaseTutorialStep
{
    private CameraScroll cameraScroll;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        cameraScroll = GameObject.FindObjectOfType<CameraScroll>();
    }
    
    public override void Perform()
    {
        if (IsPerform) return;

        base.Perform();
        
        Context.Context.Manipulator.CameraManipulator.CameraZoom.Lock(this);
        Context.Context.Manipulator.CameraManipulator.CameraMove.Lock(this);
        if(cameraScroll != null) cameraScroll.Lock(this);
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        Context.Context.Manipulator.CameraManipulator.CameraZoom.UnLock(this);
        Context.Context.Manipulator.CameraManipulator.CameraMove.UnLock(this);
        if(cameraScroll != null) cameraScroll.UnLock(this);
    }
}