using System;
using System.Collections.Generic;
using System.Globalization;
using IW.SimpleJSON;

// [
//     {
//         "id": 1,
//         "type": "heloween_12",
//         "intro_duration": 12,
//         "start": "2012-12-20T02:00:00.000Z",
//         "end": "2023-04-30T00:00:00.000Z"
//     },
//     {
//         "id": 7,
//         "type": "yo123",
//         "intro_duration": 2,
//         "start": "2019-04-30T00:00:00.000Z",
//         "end": "2020-02-10T00:00:00.000Z"
//     }
// ]

public class GameEventServerConfig
{
    public int Id;
    /// <summary>
    /// Hours
    /// </summary>
    public int IntroDuration;
    public string Type;
    public DateTime Start;
    public DateTime End;
}

public class GameEventServerSideConfigLoader : ServerSideConfigLoaderBase
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    protected override object ParseResponse(JSONNode data)
    {
        List<GameEventServerConfig> ret = null;

        JSONNode.ValueEnumerator arr = data.AsArray.Values;
        foreach (JSONNode itemJson in arr)
        {
            GameEventServerConfig item = new GameEventServerConfig
            {
                Id    = itemJson["id"].AsInt,
                IntroDuration = itemJson["intro_duration"].AsInt,
                Type  = itemJson["type"].Value,
                Start = DateTime.Parse(itemJson["start"], CultureInfo.InvariantCulture).ToUniversalTime(),
                End   = DateTime.Parse(itemJson["end"],   CultureInfo.InvariantCulture).ToUniversalTime(),
            };

            if (ret == null)
            {
                ret = new List<GameEventServerConfig>();
            }
            
            ret.Add(item);
        }

        return ret;
    }
}