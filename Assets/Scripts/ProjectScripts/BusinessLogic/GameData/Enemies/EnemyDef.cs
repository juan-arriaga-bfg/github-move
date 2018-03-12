public class EnemyDef
{
    public string Skin { get; set; }
    
    public int HP { get; set; }
    
    public CurrencyPair Price { get; set; }
    
    public string Chest { get; set; }

    public override string ToString()
    {
        return string.Format("Skin: {0}, HP: {1}, Chest: {2}, Price: {3} - {4}", Skin, HP, Chest, Price.Currency, Price.Amount);
    }
}