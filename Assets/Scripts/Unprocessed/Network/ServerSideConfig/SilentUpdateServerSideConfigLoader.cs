using System.Collections.Generic;
using System.Linq;
using IW.SimpleJSON;

/*
[
  {
    "id": 1,
    "version_from": "1.0.0",
    "version_to": "1.0.1",
    "url": "http://127.0.0.1:8080/assets/packages/file.zip"
  },
  {
    "id": 2,
    "version_from": "1.0.0",
    "version_to": "1.1.0",
    "url": "http://127.0.0.1:8080/assets/packages/update_2019.05.17_17-39 - Copy.zip"
  }
]
 */
public class SilentUpdateItem
{
    public string Content;
    public string FileName;
}

public class SilentUpdatePackage
{
    public int Id; 
    public string VersionFrom; 
    public string VersionTo; 
    public string Url;
    public string Crc;

    public List<SilentUpdateItem> Items;

    public override string ToString()
    {
        return $"[ID {Id}] v{VersionFrom} - v{VersionTo} at {Url} (MD5: {Crc})";
    }

    public SilentUpdatePackage Clone()
    {
        SilentUpdatePackage package = new SilentUpdatePackage
        {
            Id = this.Id,
            VersionFrom = this.VersionFrom,
            VersionTo = this.VersionTo,
            Url = this.Url,
            Crc = this.Crc,
            Items = Items != null ? Items.ToList() : null
        };

        return package;
    }
}

public class SilentUpdateServerSideConfigLoader : ServerSideConfigLoaderBase
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    protected override object ParseResponse(JSONNode data)
    {
        List<SilentUpdatePackage> ret = new List<SilentUpdatePackage>();
        
        JSONNode.ValueEnumerator arr = data.AsArray.Values;
        foreach (JSONNode itemJson in arr)
        {
            SilentUpdatePackage package = new SilentUpdatePackage
            {
                Id = itemJson["id"].AsInt, 
                VersionFrom = itemJson["version_from"],
                VersionTo = itemJson["version_to"],
                Url = itemJson["url"],
                Crc = itemJson["crc"],
            };

            ret.Add(package);
        }

#if DEBUG
        foreach (var item in ret)
        {
            IW.Logger.Log($"SilentUpdateServerSideConfigLoader: Received Update data: {item}"); 
        }
#endif 

        return ret;
    }
}