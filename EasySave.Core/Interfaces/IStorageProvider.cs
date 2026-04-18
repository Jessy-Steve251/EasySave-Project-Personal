namespace EasySave.Core.Interfaces;

/// <summary>
/// Bridge pattern interface: abstracts file system operations
/// so that backup strategies work identically across local disks,
/// external drives, and network drives.
/// </summary>
public interface IStorageProvider
{
    /// <summary>Copies a file from source to target path.</summary>
    void CopyFile(string sourcePath, string targetPath);

    /// <summary>Returns all file paths in a directory.</summary>
    List<string> GetFiles(string directoryPath);

    /// <summary>Returns all subdirectory paths in a directory.</summary>
    List<string> GetDirectories(string directoryPath);

    /// <summary>Gets the last modification time of a file.</summary>
    DateTime GetLastModifiedTime(string filePath);

    /// <summary>Gets the size of a file in bytes.</summary>
    long GetFileSize(string filePath);

    /// <summary>Checks whether a file exists at the given path.</summary>
    bool FileExists(string filePath);

    /// <summary>Creates a directory if it does not already exist.</summary>
    void CreateDirectory(string path);
}
