 using System;
 using System.Collections.Generic;
 using System.IO;
 using System.Linq;
 using System.Text;
 using BestHTTP;
 using CodeStage.AntiCheat.ObscuredTypes;
 using ICSharpCode.SharpZipLib.Core;
 using ICSharpCode.SharpZipLib.Zip;
 using Newtonsoft.Json;

 public class SilentUpdateManager : ECSEntity
{    
    private const int TIME_BEFORE_APPLY = 60 * 30;
    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private PrefsFileSystem prefsFileSystem;

    private const string ROOT_DIR = "SilentUpdates";
    private const string PENDING_DIR = "Packages/Pending";
    private const string INSTALLED_DIR = "Packages/Installed";
    private const string CONFIGS_DIR = "configs";

    public readonly string PathToInstalledUpdates = $"{ROOT_DIR}/{INSTALLED_DIR}";
    public readonly string UpdatesInfoPath = $"{ROOT_DIR}/info";
    
    private AsyncTaskSeries<SilentUpdatePackage> taskSeries;

    private List<SilentUpdatePackage> packagesInfo;

    public Action<bool> OnInitComplete;

    private void RaiseOnInitComplete(bool isOk)
    {
        OnInitComplete?.Invoke(isOk);
        OnInitComplete = null;
    }
    
    public void Init(Action<bool> onInitComplete)
    {
        IW.Logger.Log($"[SilentUpdateManager] => Init: Current game version: {IWProjectVersionSettings.Instance.CurrentVersion}");

        OnInitComplete = onInitComplete;
        
        prefsFileSystem = new PrefsFileSystem(ROOT_DIR);

        LoadInfo();

        ValidatePackages();
        
        ApplyPendingPackages();
        
        ServerSideConfigService.Current.OnDataReceived += OnDataReceived;
        ServerSideConfigService.Current.OnDataRequestFailed += OnDataRequestFailed;
        Check();
    }

    private void ValidatePackages()
    {
        if (packagesInfo.Count == 0)
        {
            IW.Logger.Log($"[SilentUpdateManager] => ValidatePackages: No packages found");
            return;
        }
        
        List<int> idsToRemove = new List<int>();
        
        foreach (var update in packagesInfo)
        {
            if (!CheckPackageVersion(update))
            {
                idsToRemove.Add(update.Id);
            }
        }

        if (idsToRemove.Count == 0)
        {
            IW.Logger.Log($"[SilentUpdateManager] => ValidatePackages: {packagesInfo.Count} packages found, all is OK");
            return;
        }
        
        IW.Logger.Log($"[SilentUpdateManager] => ValidatePackages: {idsToRemove.Count} packages are outdated!");

        foreach (var id in idsToRemove)
        {
            RemovePackage(id);
        }
    }

    private void LoadInfo()
    {
        var data = prefsFileSystem.ReadFile(UpdatesInfoPath);
        
        packagesInfo = string.IsNullOrEmpty(data) 
            ? new List<SilentUpdatePackage>()
            : JsonConvert.DeserializeObject<List<SilentUpdatePackage>>(data);
    }

    private void SaveInfo()
    {
        if (packagesInfo == null)
        {
            return;
        }

        string data = JsonConvert.SerializeObject(packagesInfo);
        prefsFileSystem.WriteFile(UpdatesInfoPath, data);
    }

    public void Cleanup()
    {
        ServerSideConfigService.Current.OnDataReceived -= OnDataReceived;
        ServerSideConfigService.Current.OnDataRequestFailed -= OnDataRequestFailed;
    }

    private void OnDataReceived(int guid, object data)
    {
        if (guid == SilentUpdateServerSideConfigLoader.ComponentGuid)
        {
            Check();
        }
    }
    
    private void OnDataRequestFailed(int guid, string error)
    {
        if (guid == SilentUpdateServerSideConfigLoader.ComponentGuid)
        {
            RaiseOnInitComplete(false);
        }
    }

    private bool CheckPackageVersion(SilentUpdatePackage package)
    {
        long currentVersion = IWProjectVersionSettings.VersionToLong(IWProjectVersionSettings.Instance.ProductionVersion);
        long fromVersion = IWProjectVersionSettings.VersionToLong(package.VersionFrom);
        long toVersion = IWProjectVersionSettings.VersionToLong(package.VersionTo);

        return currentVersion >= fromVersion && currentVersion <= toVersion;
    }
    
    public void Check()
    {
        if (taskSeries != null)
        {
            IW.Logger.Log($"[SilentUpdateManager] => Check: Skip by taskSeries != null");
            return;
        }
        
        List<SilentUpdatePackage> serverData = ServerSideConfigService.Current?.GetData<List<SilentUpdatePackage>>();
        if (serverData == null)
        {
            IW.Logger.Log($"[SilentUpdateManager] => Check: Skip by no data received from server");
            return;
        }

        taskSeries = new AsyncTaskSeries<SilentUpdatePackage>();
        
        foreach (var data in serverData)
        {
            if (!CheckPackageVersion(data))
            {
                IW.Logger.Log($"[SilentUpdateManager] => Check: Skip package: Version mismatch: ID:{data.Id} (cur: {IWProjectVersionSettings.Instance.ProductionVersion}, from: {data.VersionFrom} to: {data.VersionTo})");
                continue;
            }
            
            if (IsPackageInstalled(data.Id))
            {
                IW.Logger.Log($"[SilentUpdateManager] => Check: Skip package: Already installed: ID:{data.Id}");
                continue;
            }            

            taskSeries.AddTask(onTaskComplete =>
            {
                var clone = data.Clone();
                DownloadZip(clone, (isOk, package) =>
                {
                    onTaskComplete(isOk, package);
                });
            });
        }

        if (taskSeries.TasksCount == 0)
        {
            taskSeries = null;
            RaiseOnInitComplete(true);
        }
        else
        {
            taskSeries.Execute(OnZipsDownloaded);
        }
    }

    private bool IsPackageInstalled(int id)
    {
        return packagesInfo.Any(e => e.Id == id && e.Installed);
    }
    
    private bool IsPackageRegistered(int id)
    {
        return packagesInfo.Any(e => e.Id == id);
    }

    private bool RegisterPendingPackage(SilentUpdatePackage package)
    {
        if (IsPackageRegistered(package.Id))
        {
            IW.Logger.LogError($"[SilentUpdateManager] => RegisterPendingPackage: Already registered or installed: {package.Id}");
            return false;
        }
        
        packagesInfo.Add(package);
        return true;
    }

    private string GetPathToInstalledFile(string fileNameFromPackage)
    {
        var targetFileName = Path.GetFileNameWithoutExtension(fileNameFromPackage);
        var targetPath = $"{INSTALLED_DIR}/{CONFIGS_DIR}/{targetFileName}";
        
        return targetPath;
    }
    
    private bool InstallPackage(int id)
    {
        SilentUpdatePackage package = packagesInfo.FirstOrDefault(e => e.Id == id && e.Installed == false);
        if (package == null)
        {
            IW.Logger.LogError($"[SilentUpdateManager] => InstallPackage: Not found or already installed: {id}");
            return false;
        }

        var pendingPackageDir = $"{PENDING_DIR}/{id}";
        var files = prefsFileSystem.GetFiles(pendingPackageDir);
        foreach (var file in files)
        {
            var targetPath = GetPathToInstalledFile(file);

            IW.Logger.Log(prefsFileSystem.Exists(targetPath) 
                ? $"[SilentUpdateManager] => InstallPackage: Installed: {targetPath} (existing file was replaced)" 
                : $"[SilentUpdateManager] => InstallPackage: Installed: {targetPath}");
                    
            prefsFileSystem.Copy(file, targetPath);
        }
                
        IW.Logger.Log($"[SilentUpdateManager] => InstallPackage: Installed package with ID: {id}");

        prefsFileSystem.RemoveDirectory(pendingPackageDir);

        package.Installed = true;
        
        SaveInfo();
        
        return true;
    }

    private SilentUpdatePackage GetPackageInfo(int id)
    {
        return packagesInfo.FirstOrDefault(e => e.Id == id);
    }
    
    private void RemovePackage(int id)
    {
        if (!IsPackageRegistered(id))
        {
            IW.Logger.LogError($"[SilentUpdateManager] => RemovePackage: Not found: {id}");
        }

        if (!IsPackageInstalled(id))
        {
            var pendingPackageDir = $"{PENDING_DIR}/{id}";
            prefsFileSystem.RemoveDirectory(pendingPackageDir);
        }
        else
        {
            SilentUpdatePackage package = GetPackageInfo(id);

            List<string> filesToDelete = package.Items.Select(e => e.FileName).ToList();
            IW.Logger.Log($"[SilentUpdateManager] => RemovePackage: Package ID {package.Id}, Content: {string.Join(" | ", filesToDelete)}");

            List<SilentUpdatePackage> installedPackages = packagesInfo.Where(e => e.Installed).ToList();
            foreach (var packageToCheck in installedPackages)
            {
                if (packageToCheck.Id <= package.Id)
                {
                    continue;
                }

                List<string> filesInPackageToCheck = packageToCheck.Items.Select(e => e.FileName).ToList();
                foreach (var file in filesInPackageToCheck)
                {
                    if (filesToDelete.Contains(file))
                    {
                        IW.Logger.Log($"[SilentUpdateManager] => RemovePackage: Can't remove file {file} because it overriden in update {packageToCheck.Id}");
                        filesToDelete.Remove(file);
                    }
                }
            }
            
            foreach (var file in filesToDelete)
            {
                var targetPath = GetPathToInstalledFile(file);
                prefsFileSystem.DeleteFile(targetPath);

                IW.Logger.Log($"[SilentUpdateManager] => RemovePackage: File deleted: {targetPath}");
            }
        }
        
        packagesInfo.RemoveAll(e => e.Id == id);
        
        IW.Logger.Log($"[SilentUpdateManager] => RemovePackage: Package removed: ID {id}");
        
        SaveInfo();
    }
    
    private void OnZipsDownloaded(bool isOk, List<SilentUpdatePackage> successful, List<SilentUpdatePackage> failed)
    {
        if (!isOk)
        {
            IW.Logger.LogError($"[SilentUpdateManager] => Zip download or unpack failed");
            
        }
        else
        {
            foreach (var package in successful)
            {
                string root = $"{PENDING_DIR}/{package.Id}";

                prefsFileSystem.RemoveDirectory(root);

                if (RegisterPendingPackage(package))
                {
                    foreach (var item in package.Items)
                    {
                        prefsFileSystem.WriteFile($"{root}/{item.FileName}", item.Content);
                        IW.Logger.Log($"[SilentUpdateManager] => Add to pending: package ID: {package.Id}, file: {item.FileName}");
                    }
                }
            }

            SaveInfo();
        }
        
        RaiseOnInitComplete(true);
    }

    private string GetMD5Hash(byte[] data)
    {
        byte[] hash;
        using (var md5 = System.Security.Cryptography.MD5.Create()) {
            md5.TransformFinalBlock(data, 0, data.Length);
            hash = md5.Hash;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        var ret = sb.ToString().ToUpper();
        
        return ret;
    }

    private bool Unzip(byte[] src, out List<SilentUpdateItem> list)
    {
        list = new List<SilentUpdateItem>();
        
        try
        {
            using (Stream inputStream = new MemoryStream(src))
            {
                using (ZipInputStream zipInputStream = new ZipInputStream(inputStream))
                {
                    ZipEntry zipEntry = zipInputStream.GetNextEntry();
                    while (zipEntry != null)
                    {
                        String entryFileName = zipEntry.Name;
                        // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                        // Optionally match entrynames against a selection list here to skip as desired.
                        // The unpacked length is available in the zipEntry.Size property.

                        byte[] buffer = new byte[4096]; // 4K is optimum

                        // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                        // of the file, but does not waste memory.
                        // The "using" will close the stream even if an exception occurs.
                        using (MemoryStream ms = new MemoryStream())
                        {
                            StreamUtils.Copy(zipInputStream, ms, buffer);

                            string data = Encoding.UTF8.GetString(ms.ToArray(), 0, (int) ms.Length);

                            var silentUpdateItem = new SilentUpdateItem
                            {
                                Content = data,
                                FileName = entryFileName
                            };
                            list.Add(silentUpdateItem);
                        }

                        zipEntry = zipInputStream.GetNextEntry();
                    }
                }
            }
        }
        catch (Exception e)
        {
            IW.Logger.LogError(e.Message);
            return false;
        }
        return true;
    }
    
    private delegate void DownloadZipCallback(bool isOk, SilentUpdatePackage package);
    private void DownloadZip(SilentUpdatePackage package, DownloadZipCallback callback)
    {
        IW.Logger.Log($"[SilentUpdateManager] => DownloadZip: ID {package.Id}...");
        var request = new HTTPRequest(new Uri(package.Url), (req, response) =>
        {
            if (response != null && response.IsSuccess)
            {
                try
                {
                    IW.Logger.Log($"[SilentUpdateManager] => DownloadZip: ID {package.Id} RECEIVED");
                    
                    var data = response.Data;
                    var hash = GetMD5Hash(data);
                    if (hash != package.Crc.ToUpper())
                    {
                        throw new Exception("Wrong crc");
                    }

                    if (!Unzip(data, out var items))
                    {
                        throw new Exception($"[SilentUpdateManager] => DownloadZip: Can't unzip package with ID {package.Id}");
                    }
                    package.Items = items;
                    
                    IW.Logger.Log($"[SilentUpdateManager] => DownloadZip: ID {package.Id} OK");
                    
                    callback(true, package);
                }
                catch (Exception e)
                {
                    IW.Logger.Log($"[SilentUpdateManager] => DownloadZip: ID {package.Id} FAILED: {e.Message}");
                    callback(false, null);
                }
            }
            else
            {
                string mess = response != null ? "Response code = " + response.StatusCode : "Response is null";
                IW.Logger.Log($"[SilentUpdateManager] => DownloadZip: ID {package.Id} FAILED: {mess}");
                callback(false, null);
            }
        });

        request.DisableCache = true;
        request.Timeout = TimeSpan.FromSeconds(30);
        request.Send();
    }

    public bool ApplyPendingPackages()
    {
        IW.Logger.Log($"[SilentUpdateManager] => ApplyPendingPackages...");

        List<string> pendingPackages = prefsFileSystem.GetDirs(PENDING_DIR);
        
        if (pendingPackages.Count == 0)
        {
            IW.Logger.Log($"[SilentUpdateManager] => No pending packages found");
            return false;
        }

        IW.Logger.Log($"[SilentUpdateManager] => {pendingPackages.Count} pending packages found");

        try
        {
            List<int> pendingPackageIds = packagesInfo.Where(e => !e.Installed).Select(e => e.Id).ToList();
            pendingPackageIds.Sort();

            foreach (var id in pendingPackageIds)
            {
                InstallPackage(id);
            }

            SaveInfo();
        }
        catch (Exception e)
        {
            IW.Logger.LogError($"[SilentUpdateManager] => ApplyPendingPackages failed: {e.Message}");
        }

        return true;
    }

    public void ApplyPendingAndReloadScene(long timeInBackground)
    {
        if (TIME_BEFORE_APPLY > timeInBackground)
        {
            IW.Logger.Log($"[SilentUpdateManager] => ApplyPendingAndReloadScene: Skip by time: {TIME_BEFORE_APPLY - timeInBackground}s remaining");
            return;
        }
        
        IW.Logger.Log($"[SilentUpdateManager] => ApplyPendingAndReloadScene");

        bool isGameLoaded = AsyncInitService.Current?.IsAllComponentsInited() ?? false;
        if (!isGameLoaded)
        {
            IW.Logger.Log($"[SilentUpdateManager] => ApplyPendingAndReloadScene: Skip by !isGameLoaded");
            return;
        }
        
        if (ApplyPendingPackages())
        {
            DevTools.ReloadScene();
        }
    }
}