using System.Diagnostics;
using EasySave.Core.Interfaces;
using EasySave.Core.Models;
using EasySave.Logger;

namespace EasySave.Core.Strategies;

/// <summary>
/// Strategy pattern implementation: Differential backup.
/// Copies only files that are new or have been modified since the last backup.
/// Compares source and target file modification timestamps to determine changes.
/// </summary>
public class DifferentialBackupStrategy : IBackupStrategy
{
    public void Execute(
        BackupJob job,
        IStorageProvider storage,
        Action<LogEntry> onFileCopied,
        Action<JobState> onProgress)
    {
        // Get all source files
        List<string> allSourceFiles = storage.GetFiles(job.SourceDir);

        // Filter to only files that need copying (new or modified)
        List<string> filesToCopy = GetModifiedFiles(allSourceFiles, job, storage);

        // Calculate total size of files to transfer
        long totalSize = 0;
        foreach (string file in filesToCopy)
        {
            totalSize += storage.GetFileSize(file);
        }

        int totalFiles = filesToCopy.Count;
        int filesCopied = 0;
        long sizeCopied = 0;

        foreach (string sourceFile in filesToCopy)
        {
            string relativePath = Path.GetRelativePath(job.SourceDir, sourceFile);
            string targetFile = Path.Combine(job.TargetDir, relativePath);
            long fileSize = storage.GetFileSize(sourceFile);

            // Report progress
            onProgress(new JobState
            {
                JobName = job.Name,
                LastActionTimestamp = DateTime.Now,
                Status = JobStatus.Active,
                TotalEligibleFiles = totalFiles,
                TotalFileSize = totalSize,
                FilesRemaining = totalFiles - filesCopied,
                SizeRemaining = totalSize - sizeCopied,
                Progression = totalFiles > 0 ? (double)filesCopied / totalFiles * 100 : 0,
                CurrentSourceFile = sourceFile,
                CurrentTargetFile = targetFile
            });

            // Copy and measure transfer time
            var stopwatch = Stopwatch.StartNew();
            try
            {
                storage.CopyFile(sourceFile, targetFile);
                stopwatch.Stop();

                onFileCopied(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    BackupName = job.Name,
                    SourceFilePath = sourceFile,
                    TargetFilePath = targetFile,
                    FileSize = fileSize,
                    TransferTimeMs = stopwatch.ElapsedMilliseconds
                });
            }
            catch (Exception)
            {
                stopwatch.Stop();

                onFileCopied(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    BackupName = job.Name,
                    SourceFilePath = sourceFile,
                    TargetFilePath = targetFile,
                    FileSize = fileSize,
                    TransferTimeMs = -stopwatch.ElapsedMilliseconds
                });
            }

            filesCopied++;
            sizeCopied += fileSize;
        }
    }

    /// <summary>
    /// Returns only files that are new or modified compared to the target directory.
    /// A file needs copying if it does not exist in the target or if its source
    /// modification time is newer than the target copy.
    /// </summary>
    private List<string> GetModifiedFiles(
        List<string> sourceFiles,
        BackupJob job,
        IStorageProvider storage)
    {
        List<string> modified = new();

        foreach (string sourceFile in sourceFiles)
        {
            string relativePath = Path.GetRelativePath(job.SourceDir, sourceFile);
            string targetFile = Path.Combine(job.TargetDir, relativePath);

            // File needs copying if it doesn't exist in target
            if (!storage.FileExists(targetFile))
            {
                modified.Add(sourceFile);
                continue;
            }

            // File needs copying if source is newer than target
            DateTime sourceModified = storage.GetLastModifiedTime(sourceFile);
            DateTime targetModified = storage.GetLastModifiedTime(targetFile);

            if (sourceModified > targetModified)
            {
                modified.Add(sourceFile);
            }
        }

        return modified;
    }
}
