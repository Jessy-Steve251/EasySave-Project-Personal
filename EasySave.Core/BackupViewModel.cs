using EasySave.Core.Interfaces;
using EasySave.Core.Models;
using EasySave.Core.Services;
using EasySave.Logger;

namespace EasySave.Core;

/// <summary>
/// Central orchestrator for backup operations.
/// Manages up to 5 backup jobs, uses Factory to create strategies,
/// and raises Observer events for logging, state tracking, and UI updates.
/// 
/// In v2.0 (WPF/MVVM), this class becomes the ViewModel with data bindings.
/// The Observer events map directly to INotifyPropertyChanged notifications.
/// </summary>
public class BackupViewModel
{
    private readonly List<BackupJob> _jobs;
    private readonly BackupStrategyFactory _factory;
    private readonly IStorageProvider _storage;
    private const int MaxJobs = 5;

    // Observer pattern: C# events for decoupled notification
    public event EventHandler<LogEntry>? OnFileCopied;
    public event EventHandler<JobState>? OnStateChanged;
    public event EventHandler<JobState>? OnJobStarted;
    public event EventHandler<JobState>? OnJobCompleted;

    /// <summary>
    /// Creates a new BackupViewModel with the specified dependencies.
    /// </summary>
    /// <param name="jobs">Initial list of backup jobs (from ConfigManager).</param>
    /// <param name="factory">Factory for creating backup strategies.</param>
    /// <param name="storage">Storage provider for file operations.</param>
    public BackupViewModel(
        List<BackupJob> jobs,
        BackupStrategyFactory factory,
        IStorageProvider storage)
    {
        _jobs = jobs;
        _factory = factory;
        _storage = storage;
    }

    /// <summary>
    /// Returns a read-only view of all configured backup jobs.
    /// </summary>
    public IReadOnlyList<BackupJob> Jobs => _jobs.AsReadOnly();

    /// <summary>
    /// Creates a new backup job if the maximum (5) has not been reached.
    /// </summary>
    public bool CreateJob(string name, string sourceDir, string targetDir, BackupType type)
    {
        if (_jobs.Count >= MaxJobs)
            return false;

        int nextId = _jobs.Count > 0 ? _jobs.Max(j => j.Id) + 1 : 1;

        var job = new BackupJob
        {
            Id = nextId,
            Name = name,
            SourceDir = sourceDir,
            TargetDir = targetDir,
            Type = type
        };

        _jobs.Add(job);
        SaveJobs();
        return true;
    }

    /// <summary>
    /// Deletes a backup job by its ID.
    /// </summary>
    public bool DeleteJob(int id)
    {
        var job = _jobs.FirstOrDefault(j => j.Id == id);
        if (job == null) return false;

        _jobs.Remove(job);
        SaveJobs();
        return true;
    }

    /// <summary>
    /// Executes specific backup jobs by their IDs (1-indexed for user display).
    /// Handles CLI inputs like "1-3" (range) or "1;3" (specific).
    /// </summary>
    public void ExecuteJobs(List<int> jobIds)
    {
        foreach (int id in jobIds)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == id);
            if (job == null) continue;

            ExecuteSingleJob(job);
        }
    }

    /// <summary>
    /// Executes all configured backup jobs sequentially.
    /// </summary>
    public void ExecuteAllJobs()
    {
        foreach (var job in _jobs)
        {
            ExecuteSingleJob(job);
        }
    }

    /// <summary>
    /// Executes a single backup job using the Factory to get the right Strategy,
    /// then runs the Strategy with the Bridge storage provider.
    /// Observer events are raised throughout the process.
    /// </summary>
    private void ExecuteSingleJob(BackupJob job)
    {
        if (!job.Validate())
        {
            OnJobCompleted?.Invoke(this, new JobState
            {
                JobName = job.Name,
                Status = JobStatus.Error,
                LastActionTimestamp = DateTime.Now
            });
            return;
        }

        // Notify observers that job has started
        var startState = new JobState
        {
            JobName = job.Name,
            Status = JobStatus.Active,
            LastActionTimestamp = DateTime.Now
        };
        OnJobStarted?.Invoke(this, startState);
        OnStateChanged?.Invoke(this, startState);

        // Factory pattern: create the right strategy based on job type
        IBackupStrategy strategy = _factory.CreateStrategy(job.Type);

        // Track the last progress state so completion preserves file counts
        JobState lastProgressState = startState;

        // Strategy pattern: execute with callbacks for Observer notifications
        strategy.Execute(
            job,
            _storage,
            // Observer callback: file copied → notify Logger
            logEntry => OnFileCopied?.Invoke(this, logEntry),
            // Observer callback: progress update → notify StateManager and UI
            jobState =>
            {
                lastProgressState = jobState;
                OnStateChanged?.Invoke(this, jobState);
            }
        );

        // Notify observers that job has completed — preserve the final file counts
        var completeState = new JobState
        {
            JobName = job.Name,
            Status = JobStatus.Completed,
            LastActionTimestamp = DateTime.Now,
            Progression = 100,
            TotalEligibleFiles = lastProgressState.TotalEligibleFiles,
            TotalFileSize = lastProgressState.TotalFileSize,
            FilesRemaining = 0,
            SizeRemaining = 0,
            CurrentSourceFile = "",
            CurrentTargetFile = ""
        };
        OnJobCompleted?.Invoke(this, completeState);
        OnStateChanged?.Invoke(this, completeState);
    }

    /// <summary>
    /// Saves the current job list to persistent storage.
    /// </summary>
    public void SaveJobs()
    {
        ConfigManager.Instance.SaveJobs(_jobs);
    }
}
