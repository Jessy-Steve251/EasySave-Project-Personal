using EasySave.Core;
using EasySave.Core.Models;
using EasySave.Logger;

namespace EasySave.ConsoleApp;

/// <summary>
/// Console user interface for EasySave v1.0.
/// Handles menu display, user input, CLI argument parsing, and progress display.
/// This entire class is replaced by a WPF window in v2.0 — no business logic lives here.
/// </summary>
public class ConsoleView
{
    private readonly BackupViewModel _viewModel;
    private readonly LanguageManager _lang;

    public ConsoleView(BackupViewModel viewModel)
    {
        _viewModel = viewModel;
        _lang = LanguageManager.Instance;

        // Observer pattern: subscribe to ViewModel events for progress display
        _viewModel.OnJobStarted += (_, state) =>
            Console.WriteLine(_lang.GetString("job_started"), state.JobName);

        _viewModel.OnFileCopied += (_, entry) =>
        {
            if (entry.TransferTimeMs >= 0)
                Console.WriteLine(_lang.GetString("file_copied"),
                    Path.GetFileName(entry.SourceFilePath), entry.FileSize, entry.TransferTimeMs);
            else
                Console.WriteLine(_lang.GetString("file_error"),
                    Path.GetFileName(entry.SourceFilePath), Math.Abs(entry.TransferTimeMs));
        };

        _viewModel.OnStateChanged += (_, state) =>
        {
            if (state.Status == JobStatus.Active && state.TotalEligibleFiles > 0)
            {
                Console.Write($"\r{string.Format(_lang.GetString("progress"),
                    state.Progression,
                    state.TotalEligibleFiles - state.FilesRemaining,
                    state.TotalEligibleFiles)}    ");
            }
        };

        _viewModel.OnJobCompleted += (_, state) =>
        {
            Console.WriteLine();
            if (state.Status == JobStatus.Error)
                Console.WriteLine(_lang.GetString("job_error"), state.JobName);
            else
                Console.WriteLine(_lang.GetString("job_completed"), state.JobName, state.Progression);
        };
    }

    /// <summary>
    /// Main entry point: if CLI arguments are provided, parse and execute them.
    /// Otherwise, show the interactive menu.
    /// </summary>
    public void Run(string[] args)
    {
        // Select language at startup
        SelectLanguage();

        if (args.Length > 0)
        {
            // CLI mode: parse arguments and execute
            List<int> jobIds = ParseArguments(args);
            if (jobIds.Count > 0)
            {
                _viewModel.ExecuteJobs(jobIds);
                Console.WriteLine(_lang.GetString("all_completed"));
            }
            else
            {
                Console.WriteLine(_lang.GetString("invalid_input"));
            }
        }
        else
        {
            // Interactive menu mode
            RunInteractiveMenu();
        }
    }

    /// <summary>
    /// Prompts the user to select English or French at startup.
    /// </summary>
    private void SelectLanguage()
    {
        Console.WriteLine(_lang.GetString("app_title"));
        Console.WriteLine(_lang.GetString("select_language"));
        Console.WriteLine(_lang.GetString("lang_english"));
        Console.WriteLine(_lang.GetString("lang_french"));
        Console.Write(_lang.GetString("menu_choice"));

        string? choice = Console.ReadLine()?.Trim();
        if (choice == "2")
        {
            _lang.SetLanguage("fr");
        }
        Console.Clear();
    }

    /// <summary>
    /// Runs the interactive menu loop until the user exits.
    /// </summary>
    private void RunInteractiveMenu()
    {
        bool running = true;
        while (running)
        {
            DisplayMenu();
            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    CreateJobInteractive();
                    break;
                case "2":
                    DeleteJobInteractive();
                    break;
                case "3":
                    ListJobs();
                    break;
                case "4":
                    ExecuteJobInteractive();
                    break;
                case "5":
                    ExecuteAllJobs();
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine(_lang.GetString("invalid_input"));
                    break;
            }

            if (running)
            {
                Console.WriteLine();
                Console.WriteLine(_lang.GetString("press_key"));
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    /// <summary>
    /// Displays the main menu options.
    /// </summary>
    private void DisplayMenu()
    {
        Console.WriteLine(_lang.GetString("app_title"));
        Console.WriteLine(_lang.GetString("main_menu"));
        Console.WriteLine(_lang.GetString("menu_create"));
        Console.WriteLine(_lang.GetString("menu_delete"));
        Console.WriteLine(_lang.GetString("menu_list"));
        Console.WriteLine(_lang.GetString("menu_execute_one"));
        Console.WriteLine(_lang.GetString("menu_execute_all"));
        Console.WriteLine(_lang.GetString("menu_exit"));
        Console.Write(_lang.GetString("menu_choice"));
    }

    /// <summary>
    /// Interactive job creation workflow.
    /// </summary>
    private void CreateJobInteractive()
    {
        if (_viewModel.Jobs.Count >= 5)
        {
            Console.WriteLine(_lang.GetString("job_limit"));
            return;
        }

        Console.Write(_lang.GetString("enter_name"));
        string name = Console.ReadLine()?.Trim() ?? "";

        Console.Write(_lang.GetString("enter_source"));
        string source = Console.ReadLine()?.Trim() ?? "";

        if (!Directory.Exists(source))
        {
            Console.WriteLine(_lang.GetString("invalid_source"));
            return;
        }

        Console.Write(_lang.GetString("enter_target"));
        string target = Console.ReadLine()?.Trim() ?? "";

        Console.Write(_lang.GetString("select_type"));
        string typeInput = Console.ReadLine()?.Trim() ?? "1";
        BackupType type = typeInput == "2" ? BackupType.Differential : BackupType.Full;

        bool created = _viewModel.CreateJob(name, source, target, type);
        if (created)
        {
            var lastJob = _viewModel.Jobs.Last();
            Console.WriteLine(string.Format(_lang.GetString("job_created"), name, lastJob.Id));
        }
    }

    /// <summary>
    /// Interactive job deletion workflow.
    /// </summary>
    private void DeleteJobInteractive()
    {
        ListJobs();
        Console.Write(_lang.GetString("enter_id_delete"));
        if (int.TryParse(Console.ReadLine()?.Trim(), out int id))
        {
            bool deleted = _viewModel.DeleteJob(id);
            Console.WriteLine(deleted
                ? _lang.GetString("job_deleted")
                : _lang.GetString("job_not_found"));
        }
        else
        {
            Console.WriteLine(_lang.GetString("invalid_input"));
        }
    }

    /// <summary>
    /// Displays a formatted list of all configured backup jobs.
    /// </summary>
    private void ListJobs()
    {
        if (_viewModel.Jobs.Count == 0)
        {
            Console.WriteLine(_lang.GetString("no_jobs"));
            return;
        }

        Console.WriteLine(_lang.GetString("job_list_header"));
        Console.WriteLine("  ---+----------------------+--------------+-------------------");
        foreach (var job in _viewModel.Jobs)
        {
            Console.WriteLine($"  {job.Id,2} | {job.Name,-20} | {job.Type,-12} | {job.SourceDir}");
        }
    }

    /// <summary>
    /// Interactive single job execution.
    /// </summary>
    private void ExecuteJobInteractive()
    {
        ListJobs();
        Console.Write(_lang.GetString("enter_id_execute"));
        if (int.TryParse(Console.ReadLine()?.Trim(), out int id))
        {
            _viewModel.ExecuteJobs(new List<int> { id });
        }
        else
        {
            Console.WriteLine(_lang.GetString("invalid_input"));
        }
    }

    /// <summary>
    /// Executes all configured backup jobs.
    /// </summary>
    private void ExecuteAllJobs()
    {
        _viewModel.ExecuteAllJobs();
        Console.WriteLine(_lang.GetString("all_completed"));
    }

    /// <summary>
    /// Parses CLI arguments into a list of job IDs.
    /// Supports two formats from the spec:
    ///   - Range: "1-3" → [1, 2, 3]
    ///   - Specific: "1;3" → [1, 3]
    /// Multiple arguments are also supported: "1" "3" → [1, 3]
    /// </summary>
    public static List<int> ParseArguments(string[] args)
    {
        List<int> jobIds = new();
        string joined = string.Join(" ", args);

        // Handle range format: "1-3"
        if (joined.Contains('-') && !joined.Contains(';'))
        {
            string[] parts = joined.Split('-');
            if (parts.Length == 2
                && int.TryParse(parts[0].Trim(), out int start)
                && int.TryParse(parts[1].Trim(), out int end))
            {
                for (int i = start; i <= end; i++)
                {
                    jobIds.Add(i);
                }
            }
        }
        // Handle specific format: "1;3" or "1 ;3"
        else if (joined.Contains(';'))
        {
            string[] parts = joined.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                if (int.TryParse(part.Trim(), out int id))
                {
                    jobIds.Add(id);
                }
            }
        }
        // Handle single ID or space-separated IDs
        else
        {
            foreach (string arg in args)
            {
                if (int.TryParse(arg.Trim(), out int id))
                {
                    jobIds.Add(id);
                }
            }
        }

        return jobIds;
    }
}
