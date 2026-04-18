using System.Text.Json;

namespace EasySave.Logger;

/// <summary>
/// Writes backup actions to a daily JSON log file in real time.
/// Designed as a reusable library (EasyLog.dll) for all ProSoft Suite applications.
/// Implements the Observer pattern by subscribing to ViewModel events.
/// </summary>
public class Logger
{
    private readonly string _logDirectory;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Creates a new Logger instance writing to the specified directory.
    /// </summary>
    /// <param name="logDirectory">Directory where daily log files will be stored.</param>
    public Logger(string logDirectory)
    {
        _logDirectory = logDirectory;
        Directory.CreateDirectory(_logDirectory);
    }

    /// <summary>
    /// Logs a single backup action to the daily log file.
    /// Each day produces a separate file named {yyyy-MM-dd}.json.
    /// Line breaks are inserted between JSON elements for Notepad readability.
    /// </summary>
    /// <param name="entry">The log entry containing file transfer details.</param>
    public void LogAction(LogEntry entry)
    {
        string dailyLogPath = GetDailyLogPath();
        WriteJsonEntry(entry, dailyLogPath);
    }

    /// <summary>
    /// Returns the file path for today's log file.
    /// Format: {logDirectory}/{yyyy-MM-dd}.json
    /// </summary>
    private string GetDailyLogPath()
    {
        string fileName = $"{DateTime.Now:yyyy-MM-dd}.json";
        return Path.Combine(_logDirectory, fileName);
    }

    /// <summary>
    /// Writes a log entry to the specified file path.
    /// If the file already exists, appends to the existing JSON array.
    /// If not, creates a new JSON array with the entry.
    /// </summary>
    private void WriteJsonEntry(LogEntry entry, string path)
    {
        List<LogEntry> entries = new();

        if (File.Exists(path))
        {
            try
            {
                string existingContent = File.ReadAllText(path);
                var existingEntries = JsonSerializer.Deserialize<List<LogEntry>>(existingContent);
                if (existingEntries != null)
                {
                    entries = existingEntries;
                }
            }
            catch (JsonException)
            {
                // If file is corrupted, start fresh
            }
        }

        entries.Add(entry);

        string json = JsonSerializer.Serialize(entries, JsonOptions);
        File.WriteAllText(path, json);
    }
}
