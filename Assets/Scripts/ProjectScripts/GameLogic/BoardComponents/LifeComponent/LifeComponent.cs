using UnityEngine;

public class LifeComponent : IECSComponent
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid
    {
        get { return ComponentGuid; }
    }

    protected Piece thisContext;
    
    public int HP { get; set; }
    
    protected int current;
    
    public int Current
    {
        get { return current; }
    }
    
    public float GetProgress
    {
        get { return 1 - current/(float)HP; }
    }
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void Damage(int damage)
    {
        if (current == HP) return;
        
        current = Mathf.Clamp(current + damage, 0, HP);
        AddResourceView.Show(thisContext.CachedPosition, new CurrencyPair{Currency = Currency.Life.Name, Amount = -damage});
    }
}