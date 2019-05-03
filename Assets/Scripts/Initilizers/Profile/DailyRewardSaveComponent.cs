
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public interface ICodexDailyRewardSaveComponent
{
    DailyRewardSaveComponent DailyRewardSave { get; }
}

public partial class UserProfile : ICodexDailyRewardSaveComponent
{
    protected DailyRewardSaveComponent dailyRewardSaveComponent;

    [JsonIgnore]
    public DailyRewardSaveComponent DailyRewardSave
    {
        get
        {
            if (dailyRewardSaveComponent == null)
            {
                dailyRewardSaveComponent = GetComponent<DailyRewardSaveComponent>(DailyRewardSaveComponent.ComponentGuid);
            }

            return dailyRewardSaveComponent;
        }
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class DailyRewardSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty]
    public long TimerStart;

    [JsonProperty]
    public int Day;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        var manager = GameDataService.Current.DailyRewardManager;
        TimerStart = manager.Timer?.StartTimeLong ?? 0;
        Day = manager.Day;
    }
}