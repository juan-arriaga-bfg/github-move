using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class HybridConfigDataMapper<T> : IJsonDataMapper<T>
{
    private readonly string dataPath;

    private readonly bool isUseEncrypting;

    private IJsonDataMapper<T> mapper;
    
    public HybridConfigDataMapper(string dataPath, bool isUseEncrypting)
    {
        this.dataPath = dataPath;
        this.isUseEncrypting = isUseEncrypting;
    }
    
    public bool IsDataExists()
    {
        return mapper != null && mapper.IsDataExists();
    }
    
    public void LoadData(Action<T, string> onComplete)
    {
        if (mapper == null)
        {
            mapper = new SilentUpdateDataMapper<T>(dataPath);
            if (!mapper.IsDataExists())
            {
                mapper = new ResourceConfigDataMapper<T>(dataPath, isUseEncrypting);
                if (!mapper.IsDataExists())
                {
                    mapper = null;
                }
            }

            if (mapper == null)
            {
                IW.Logger.LogWarning($"[HybridConfigDataMapper] => Can't load data with any mapper for {dataPath}!");
                return;
            }

            IW.Logger.Log($"[HybridConfigDataMapper] => Selected {mapper.GetType()} for {dataPath}");

            mapper.LoadData(onComplete);
        }
    }

    public void UploadData(T data, DataMapperUploadCallback onComplete)
    {
        if (mapper == null)
        {
            onComplete($"No data mapper selected for {dataPath}", null);
            return;
        }
        
        mapper.UploadData(data, onComplete);
    }

    public string GetJsonDataAsString()
    {
        return mapper?.GetJsonDataAsString();
    }

    public void SetCustomConverters(params JsonConverter[] converters)
    {
        mapper?.SetCustomConverters(converters);
    }

    public void SetCustomContractResolver(IContractResolver resolver)
    {
        mapper?.SetCustomContractResolver(resolver);
    }
}