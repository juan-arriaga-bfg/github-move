public class CollectionDef
{
    public string Uid { get; set; }
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public int Weight { get; set; }

    public ItemWeight GetItemWeight()
    {
        return new ItemWeight {Uid = this.Uid, Weight = this.Weight};
    }
}