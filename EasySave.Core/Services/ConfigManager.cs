using System.Text.Json;
using EasySave.Core.Models;

namespace EasySave.Core.Services;

/// <summary>
/// Singleton pattern: manages backup job configuration persistence.
/// Stores job configurations in a JSON file at a location suitable
/// for customer servers (AppData, not hardcoded paths like c:\temp\).
/// </summary>
public class ConfigManager
{
    private static ConfigManager? _instance;
    private static readonly object Lock = new();
    private readonly string _configPath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Private constructor enforces the Singleton pattern.
    /// </summary>
    private ConfigManager()
    {
        string configDir = GetConfigDirectory();
        Directory.CreateDirectory(configDir);
        _configPath = Path.Combine(configDir, "config.json");
    }

    /// <summary>
    /// Thread-safe Singleton accessor using double-check locking.
    /// </summary>
    public static ConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance ??= new ConfigManager();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Returns the application data directory for EasySave.
    /// Uses Environment.SpecialFolder.ApplicationData for server compatibility.
    /// </summary>
    public static string GetConfigDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EasySave");
    }

    /// <summary>
    /// Loads all backup jobs from the configuration file.
    /// Returns an empty list if no configuration exists yet.
    /// </summary>
    public List<BackupJob> LoadJobs()
    {
        if (!File.Exists(_configPath))
            return new List<BackupJob>();

        try
        {
            string json = File.ReadAllText(_configPath);
            var jobs = JsonSerializer.Deserialize<List<BackupJob>>(json);
            return jobs ?? new List<BackupJob>();
        }
        catch (JsonException)
        {
            return new List<BackupJob>();
        }
    }

    /// <summary>
    /// Saves all backup jobs to the configuration file.
    /// </summary>
    public void SaveJobs(List<BackupJob> jobs)
    {
        string json = JsonSerializer.Serialize(jobs, JsonOptions);
        File.WriteAllText(_configPath, json);
    }
}
