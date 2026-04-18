using EasySave.Core.Interfaces;

namespace EasySave.Core.Services;

/// <summary>
/// Bridge pattern implementation: handles file operations on local disks
/// and external drives using standard System.IO operations.
/// </summary>
public class LocalStorageProvider : IStorageProvider
{
    public void CopyFile(string sourcePath, string targetPath)
    {
        // Ensure the target directory exists before copying
        string? targetDir = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
        File.Copy(sourcePath, targetPath, overwrite: true);
    }

    public List<string> GetFiles(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return new List<string>();

        return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).ToList();
    }

    public List<string> GetDirectories(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return new List<string>();

        return Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories).ToList();
    }

    public DateTime GetLastModifiedTime(string filePath)
    {
        return File.GetLastWriteTime(filePath);
    }

    public long GetFileSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }
}
