using UnityEngine;

public class LifeComponent : ECSEntity
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected Piece thisContext;
    
    public int HP { get; protected set; }
    public bool IsDead => current == HP;
    
    protected int current;

    public int Value => HP == -1 ? 0 : HP - Current;
    public int Current => current;
    public float GetProgress => 1 - current/(float)HP;
    
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