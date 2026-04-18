using EasySave.Core.Models;
using EasySave.Logger;

namespace EasySave.Core.Interfaces;

/// <summary>
/// Strategy pattern interface: defines the contract for backup algorithms.
/// Each implementation (Full, Differential) encapsulates its own file selection
/// and copying logic. Adding a new backup type means creating one new class.
/// </summary>
public interface IBackupStrategy
{
    /// <summary>
    /// Executes the backup operation for the given job.
    /// </summary>
    /// <param name="job">The backup job configuration.</param>
    /// <param name="storage">The storage provider for file operations (Bridge pattern).</param>
    /// <param name="onFileCopied">Callback invoked after each file is copied (for logging).</param>
    /// <param name="onProgress">Callback invoked to report progress (for state tracking).</param>
    void Execute(
        BackupJob job,
        IStorageProvider storage,
        Action<LogEntry> onFileCopied,
        Action<JobState> onProgress);
}
