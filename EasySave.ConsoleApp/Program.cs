using EasySave.Core;
using EasySave.Core.Services;
using EasySave.Logger;

namespace EasySave.ConsoleApp;

/// <summary>
/// Entry point for EasySave v1.0 console application.
/// Wires up all dependencies: ConfigManager, Logger, StateManager, ViewModel, ConsoleView.
/// Observer subscriptions are established here before passing control to the View.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // Determine the application data directory for logs and state files
        // Uses AppData to ensure compatibility on customer servers (not c:\temp\)
        string appDataDir = ConfigManager.GetConfigDirectory();

        // Load saved backup jobs via Singleton ConfigManager
        var jobs = ConfigManager.Instance.LoadJobs();

        // Create the Factory and Bridge storage provider
        var factory = new BackupStrategyFactory();
        var storage = new LocalStorageProvider();

        // Create the ViewModel (orchestrator)
        var viewModel = new BackupViewModel(jobs, factory, storage);

        // Create Logger (EasyLog.dll) — writes daily JSON log files
        var logger = new EasySave.Logger.Logger(appDataDir);

        // Create StateManager — writes real-time state.json
        string stateFilePath = Path.Combine(appDataDir, "state.json");
        var stateManager = new StateManager(stateFilePath);

        // Initialize state entries for all existing jobs
        stateManager.InitializeStates(jobs.Select(j => j.Name));

        // Observer pattern: subscribe Logger to file-copied events
        viewModel.OnFileCopied += (sender, logEntry) =>
        {
            logger.LogAction(logEntry);
        };

        // Observer pattern: subscribe StateManager to state-changed events
        viewModel.OnStateChanged += (sender, jobState) =>
        {
            stateManager.UpdateState(jobState);
        };

        // Observer pattern: subscribe StateManager to job-completed events
        viewModel.OnJobCompleted += (sender, jobState) =>
        {
            stateManager.UpdateState(jobState);
        };

        // Create the ConsoleView (UI layer) and run
        // ConsoleView subscribes itself to ViewModel events in its constructor
        var consoleView = new ConsoleView(viewModel);
        consoleView.Run(args);
    }
}
