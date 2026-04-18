using System.Diagnostics;
using EasySave.Core.Interfaces;
using EasySave.Core.Models;
using EasySave.Logger;

namespace EasySave.Core.Strategies;

/// <summary>
/// Strategy pattern implementation: Full backup.
/// Copies ALL files and subdirectories from the source directory to the target directory.
/// Every file is copied regardless of whether it has changed.
/// </summary>
public class FullBackupStrategy : IBackupStrategy
{
    public void Execute(
        BackupJob job,
        IStorageProvider storage,
        Action<LogEntry> onFileCopied,
        Action<JobState> onProgress)
    {
        // Get all files recursively from the source directory
        List<string> allFiles = storage.GetFiles(job.SourceDir);

        // Calculate total size for progress tracking
        long totalSize = 0;
        foreach (string file in allFiles)
        {
            totalSize += storage.GetFileSize(file);
        }

        int totalFiles = allFiles.Count;
        int filesCopied = 0;
        long sizeCopied = 0;

        foreach (string sourceFile in allFiles)
        {
            // Calculate the relative path to preserve directory structure
            string relativePath = Path.GetRelativePath(job.SourceDir, sourceFile);
            string targetFile = Path.Combine(job.TargetDir, relativePath);

            // Get file size before copying
            long fileSize = storage.GetFileSize(sourceFile);

            // Report progress before copying
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

            // Copy the file and measure transfer time
            var stopwatch = Stopwatch.StartNew();
            try
            {
                storage.CopyFile(sourceFile, targetFile);
                stopwatch.Stop();

                // Log the successful file transfer
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

                // Log the failed file transfer with negative time
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
}
