using System.Text.Json;

namespace EasySave.Logger;

/// <summary>
/// Manages the real-time status file (state.json) for all backup jobs.
/// Records progress and current action in real time as required by the spec.
/// All job states are stored in a single file.
/// </summary>
public class StateManager
{
    private readonly string _stateFilePath;
    private readonly List<JobState> _states;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Creates a new StateManager writing to the specified file path.
    /// </summary>
    /// <param name="stateFilePath">Full path to the state.json file.</param>
    public StateManager(string stateFilePath)
    {
        _stateFilePath = stateFilePath;
        _states = new List<JobState>();

        // Ensure the directory exists
        string? directory = Path.GetDirectoryName(stateFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Initializes state entries for all backup jobs.
    /// Called at startup to populate the state file with all configured jobs.
    /// </summary>
    /// <param name="jobNames">Names of all configured backup jobs.</param>
    public void InitializeStates(IEnumerable<string> jobNames)
    {
        _states.Clear();
        foreach (string name in jobNames)
        {
            _states.Add(new JobState
            {
                JobName = name,
                Status = JobStatus.Inactive,
                LastActionTimestamp = DateTime.Now
            });
        }
        WriteStateToJson();
    }

    /// <summary>
    /// Updates the state of a specific job and writes to disk immediately.
    /// This is called in real time during backup operations.
    /// </summary>
    /// <param name="state">The updated job state.</param>
    public void UpdateState(JobState state)
    {
        int index = _states.FindIndex(s => s.JobName == state.JobName);
        if (index >= 0)
        {
            _states[index] = state;
        }
        else
        {
            _states.Add(state);
        }
        WriteStateToJson();
    }

    /// <summary>
    /// Writes the complete state list to the JSON file.
    /// Line breaks are inserted between elements for Notepad readability.
    /// </summary>
    public void WriteStateToJson()
    {
        string json = JsonSerializer.Serialize(_states, JsonOptions);
        File.WriteAllText(_stateFilePath, json);
    }
}
