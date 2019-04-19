public enum FireflyLogicType
{
    Normal,
    Event,
}

public class FireflyDef
{
    public FireflyLogicType Uid;
    
    public AmountRange AmountProduction;
    public AmountRange AmountEvent;
    public AmountRange DelayFirstSpawn;
    public AmountRange DelaySpawn;
    
    public int SleepDelay;
    public int TapDelay;
}