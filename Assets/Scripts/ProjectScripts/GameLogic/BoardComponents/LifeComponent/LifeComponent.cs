using UnityEngine;

public class LifeComponent : ECSEntity
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected Piece thisContext;
    
    // maximum amount of damage
    public int HP { get; protected set; } 
    
    // amount of damage received
    protected int current;
    public int Current => current;
    
    // how much damage can still get
    public int Value => HP == -1 ? 0 : HP - Current;
    
    public float GetProgress => 1 - current/(float)HP;
    
    public bool IsDead => current == HP;

    public Piece GetContext()
    {
        return thisContext;
    }
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        thisContext = entity as Piece;
    }
    
    public void Damage(int damage)
    {
        if (IsDead) return;

        if (HP != -1) current = Mathf.Clamp(current + damage, 0, HP);
        AddResourceView.Show(StartPosition(), new CurrencyPair{Currency = Currency.Life.Name, Amount = -damage});
    }

    protected BoardPosition StartPosition()
    {
        var multi = thisContext.GetComponent<MulticellularPieceBoardObserver>(MulticellularPieceBoardObserver.ComponentGuid);

        return multi?.GetTopPosition ?? thisContext.CachedPosition;
    }
}