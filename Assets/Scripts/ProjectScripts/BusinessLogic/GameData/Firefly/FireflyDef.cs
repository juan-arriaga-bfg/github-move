public enum FireflyType
{
    Production,
    Event,
    Other,
}

public class FireflyDef
{
    public FireflyType Uid;

    public int Level;
    public float Speed;
    
    public AmountRange Amount;
    public AmountRange DelayFirstSpawn;
    public AmountRange DelaySpawn;
    
    public int SleepDelay;
    public int TapDelay;
}