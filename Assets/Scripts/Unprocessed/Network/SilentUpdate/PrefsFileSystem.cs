using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

public class PrefsFileSystem
{
    private const string FILESYSTEM = ".filesystem";
     
    private string root;

    private List<string> files;

    private string GetAbsolutePath(string relativePath)
    {
        if (relativePath.StartsWith("/"))
        {
            relativePath = relativePath.Remove(0, 1);
        }

        return $"{root}/{relativePath}";
    }
     
    public PrefsFileSystem(string root)
    {
        this.root = root;

        var filesystemPath = GetAbsolutePath(FILESYSTEM);
        if (ObscuredPrefs.HasKey(filesystemPath))
        {
            var data = ObscuredPrefs.GetString(filesystemPath, null);
            if (data == null)
            {
                ObscuredPrefs.DeleteKey(filesystemPath); 
            }
            else
            {
                files = data.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries ).ToList();
            } 
        }
         
        files = files ?? new List<string>();
    }

    private void UpdateFileSystem()
    {
        var filesystemPath = GetAbsolutePath(FILESYSTEM);
        string data = string.Join("\n", files);
        ObscuredPrefs.SetString(filesystemPath, data); 
    }

    public string ReadFile(string path)
    {
        return ObscuredPrefs.GetString(GetAbsolutePath(path), null);
    }
     
    public bool WriteFile(string path, string content, bool allowReplace = true)
    {
        string absolutePath = GetAbsolutePath(path);

        if (!allowReplace && ObscuredPrefs.HasKey(absolutePath))
        {
            IW.Logger.LogError($"[PrefsFileSystem] => WriteFile failed for '{path}' - file already exists and allowReplace == false");
            return false;
        }
         
        IW.Logger.Log($"[PrefsFileSystem] => WriteFile({path})");
        
        ObscuredPrefs.SetString(absolutePath, content);

        if (!files.Contains(absolutePath))
        {
            files.Add(absolutePath);
            UpdateFileSystem();
        }
         
        return true;
    }

    public bool RemoveFile(string path)
    {
        IW.Logger.Log($"[PrefsFileSystem] => RemoveFile({path})");
        
        var absolutePath = GetAbsolutePath(path);
         
        if (ObscuredPrefs.HasKey(absolutePath))
        {
            ObscuredPrefs.DeleteKey(GetAbsolutePath(path));
            files.Remove(absolutePath);
            UpdateFileSystem();
            return true;
        }

        return false;
    }

    public bool Exists(string path)
    {
        var absolutePath = GetAbsolutePath(path);

        return ObscuredPrefs.HasKey(absolutePath);
    }
     
    public List<string> GetFiles(string path)
    {
        var absolutePath = GetAbsolutePath(path);
        var rootPath = $"{root}/";
        List<string> ret = new List<string>();
        foreach (var file in files)
        {
            if (file.StartsWith(absolutePath))
            {
                ret.Add(file.Remove(0, rootPath.Length));
            }
        }

        return ret;
    }
    
    public List<string> GetDirs(string path)
    {
        var absolutePath = GetAbsolutePath(path);
        var rootPath = $"{root}/";
        HashSet<string> ret = new HashSet<string>();
        foreach (var file in files)
        {
            if (file.StartsWith(absolutePath))
            {
                string item = file;
                
                int lastIndex = item.LastIndexOf("/", StringComparison.InvariantCulture);
                if (lastIndex >= 0)
                {
                    item = item.Remove(lastIndex, item.Length - lastIndex);
                }

                item = item.Remove(0, rootPath.Length);
                ret.Add(item);
            }
        }

        return ret.ToList();
    }
     
    public bool RemoveDirectory(string path)
    {
        IW.Logger.Log($"[PrefsFileSystem] => RemoveDirectory({path})");
        
        var absolutePath = GetAbsolutePath(path);

        bool somethingAffected = false;

        for (var i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];
            if (file.StartsWith(absolutePath))
            {
                if (ObscuredPrefs.HasKey(file))
                {
                    ObscuredPrefs.DeleteKey(file);
                    files.RemoveAt(i);
                    somethingAffected = true;
                }
            }
        }

        if (somethingAffected)
        {
            UpdateFileSystem();
        }

        return somethingAffected;
    }

    public bool Copy(string src, string dst, bool allowReplace = true)
    {
        string val = ReadFile(src);
        if (val == null)
        {
            IW.Logger.LogError($"[PrefsFileSystem] => Copy failed: Source file not exists at '{src}'");
            return false;
        }

        return WriteFile(dst, val, allowReplace);
    }
}