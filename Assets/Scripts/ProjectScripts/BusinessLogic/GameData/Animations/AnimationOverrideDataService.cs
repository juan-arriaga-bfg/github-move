public class AnimationOverrideDataService: IWService<AnimationOverrideDataService, AnimationOverrideDataManager>
{
    public static AnimationOverrideDataManager Current
    {
        get { return Instance.Manager; }
    }
}
