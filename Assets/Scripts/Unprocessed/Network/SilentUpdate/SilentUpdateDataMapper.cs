public class SilentUpdateDataMapper<T> : StoragePlayerPrefsDataMapper<T>
{
    public SilentUpdateDataMapper(string dataPath) : base(dataPath)
    {
        this.dataPath = $"{SilentUpdateService.Current.PathToInstalledUpdates}/{dataPath}";
    }
}