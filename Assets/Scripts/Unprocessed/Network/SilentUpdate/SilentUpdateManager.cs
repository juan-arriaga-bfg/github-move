 using System;
 using System.Collections.Generic;
 using System.IO;
 using System.Text;
 using BestHTTP;
 using ICSharpCode.SharpZipLib.Core;
 using ICSharpCode.SharpZipLib.Zip;

 public class SilentUpdateManager : ECSEntity
{    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;


    private PrefsFileSystem prefsFileSystem;

    private const string ROOT_DIR = "SilentUpdates";
    private const string PENDING_DIR = "Packages/Pending";
    private const string INSTALLED_DIR = "Packages/Installed";
    private const string CONFIGS_DIR = "configs";

    public readonly string PathToInstalledUpdates = $"{ROOT_DIR}/{INSTALLED_DIR}";
    
    private AsyncTaskSeries<SilentUpdatePackage> series;

    public void Init()
    {
        prefsFileSystem = new PrefsFileSystem(ROOT_DIR);
        
        ApplyPendingUpdates();
        
        ServerSideConfigService.Current.OnDataReceived += OnDataReceived;
        Check();
    }

    public void Cleanup()
    {
        ServerSideConfigService.Current.OnDataReceived -= OnDataReceived;
    }

    private void OnDataReceived(int guid, object data)
    {
        if (guid == SilentUpdateServerSideConfigLoader.ComponentGuid)
        {
            Check();
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
        if (series != null)
        {
            IW.Logger.Log($"[SilentUpdateManager] => Check: Skip by series != null");
            return;
        }
        
        List<SilentUpdatePackage> serverData = ServerSideConfigService.Current?.GetData<List<SilentUpdatePackage>>();
        if (serverData == null)
        {
            IW.Logger.Log($"[SilentUpdateManager] => Check: Skip by no data");
            return;
        }

        series = new AsyncTaskSeries<SilentUpdatePackage>();
        
        foreach (var data in serverData)
        {
            if (!CheckPackageVersion(data))
            {
                IW.Logger.Log($"[SilentUpdateManager] => Check: Skip package: version mismatch: ID:{data.Id} (cur: {IWProjectVersionSettings.Instance.ProductionVersion}, from: {data.VersionFrom} to: {data.VersionTo})");
                continue;
            }

            series.AddTask(onTaskComplete =>
            {
                var clone = data.Clone();
                DownloadZip(clone, (isOk, package) =>
                {
                    onTaskComplete(isOk, package);
                });
            });
        }

        if (series.TasksCount == 0)
        {
            series = null;
        }
        else
        {
            series.Execute(OnZipsDownloaded);
        }
    }

    private void OnZipsDownloaded(bool isOk, List<SilentUpdatePackage> successful, List<SilentUpdatePackage> failed)
    {
        if (!isOk)
        {
            IW.Logger.LogError($"[SilentUpdateManager] => Zip download or unpack failed");
            return;
        }

        foreach (var package in successful)
        {
            string root = $"{PENDING_DIR}/{package.Id}";
                
            prefsFileSystem.RemoveDirectory(root);

            foreach (var item in package.Items)
            {
                prefsFileSystem.WriteFile($"{root}/{item.FileName}", item.Content);
                
                IW.Logger.Log($"[SilentUpdateManager] => Add to pending: package ID: {package.Id}, file: {item.FileName}");
            }
        }
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
        var request = new HTTPRequest(new Uri(package.Url), (req, response) =>
        {
            if (response != null && response.StatusCode == 200)
            {
                try
                {
                    var data = response.Data;
                    var hash = GetMD5Hash(data);
                    if (hash != package.Crc.ToUpper())
                    {
                        throw new Exception("Wrong crc");
                    }

                    if (!Unzip(data, out var items))
                    {
                        throw new Exception($"Can't unzip package with ID {package.Id}");
                    }
                    package.Items = items;
                    
                    callback(true, package);
                }
                catch (Exception e)
                {
                    IW.Logger.LogError(e.Message);
                    callback(false, null);
                }
            }
            else
            {
                IW.Logger.Log(response != null ? "Response code = " + response.StatusCode : "Response is null");
                callback(false, null);
            }
        });

        request.DisableCache = true;
        request.Timeout = TimeSpan.FromSeconds(30);
        request.Send();
    }

    private void ApplyPendingUpdates()
    {
        IW.Logger.Log($"[SilentUpdateManager] => ApplyPendingUpdates...");

        List<string> pendingPackages = prefsFileSystem.GetDirs(PENDING_DIR);
        
        if (pendingPackages.Count == 0)
        {
            IW.Logger.Log($"[SilentUpdateManager] => No pending packages found...");
            return;
        }

        IW.Logger.Log($"[SilentUpdateManager] => {pendingPackages.Count} pending packages found");

        try
        {
            List<int> pendingPackageIds = new List<int>(pendingPackages.Count);
            foreach (var pendingPackage in pendingPackages)
            {
                string id = pendingPackage.Remove(0, PENDING_DIR.Length + 1);
                pendingPackageIds.Add(int.Parse(id));
            }
            
            pendingPackageIds.Sort();

            foreach (var id in pendingPackageIds)
            {
                var packageDir = $"{PENDING_DIR}/{id}";
                var files = prefsFileSystem.GetFiles(packageDir);
                foreach (var file in files)
                {
                    var targetFileName = Path.GetFileNameWithoutExtension(file);
                    var targetPath = $"{INSTALLED_DIR}/{CONFIGS_DIR}/{targetFileName}";

                    IW.Logger.Log(prefsFileSystem.Exists(targetPath) 
                        ? $"[SilentUpdateManager] => Installed: {targetPath} (existing file was replaced)" 
                        : $"[SilentUpdateManager] => Installed: {targetPath}");
                    
                    prefsFileSystem.Copy(file, targetPath);
                }
                
                IW.Logger.Log($"[SilentUpdateManager] => Installed package with ID: {id}");

                prefsFileSystem.RemoveDirectory(packageDir);
            }
        }
        catch (Exception e)
        {
            IW.Logger.Log($"[SilentUpdateManager] => ApplyPendingUpdates failed: {e.Message}");
        }
    }
}