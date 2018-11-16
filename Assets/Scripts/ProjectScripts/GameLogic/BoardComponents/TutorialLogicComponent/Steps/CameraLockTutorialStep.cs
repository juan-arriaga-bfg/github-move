using UnityEngine;

public class CameraLockTutorialStep : BaseTutorialStep
{
    private CameraScroll cameraMove;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        cameraMove = GameObject.FindObjectOfType<CameraScroll>();
    }
    
    public override void Perform()
    {
        if (IsPerform) return;

        base.Perform();
        
        Context.Context.Manipulator.CameraManipulator.CameraZoom.Lock(this);
        Context.Context.Manipulator.CameraManipulator.CameraMove.Lock(this);
        if(cameraMove != null) cameraMove.Lock(this);
    }
    
    protected override void Complete()
    {
        base.Complete();
        
        Context.Context.Manipulator.CameraManipulator.CameraZoom.UnLock(this);
        Context.Context.Manipulator.CameraManipulator.CameraMove.UnLock(this);
        if(cameraMove != null) cameraMove.UnLock(this);
    }
}