using System.Text.Json.Serialization;

namespace EasySave.Logger;

/// <summary>
/// Represents the real-time state of a backup job.
/// All fields match the EasySave v1.0 specification for state.json.
/// </summary>
public class JobState
{
    [JsonPropertyName("jobName")]
    public string JobName { get; set; } = string.Empty;

    [JsonPropertyName("lastActionTimestamp")]
    public DateTime LastActionTimestamp { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JobStatus Status { get; set; } = JobStatus.Inactive;

    [JsonPropertyName("totalEligibleFiles")]
    public int TotalEligibleFiles { get; set; }

    [JsonPropertyName("totalFileSize")]
    public long TotalFileSize { get; set; }

    [JsonPropertyName("filesRemaining")]
    public int FilesRemaining { get; set; }

    [JsonPropertyName("sizeRemaining")]
    public long SizeRemaining { get; set; }

    [JsonPropertyName("progression")]
    public double Progression { get; set; }

    [JsonPropertyName("currentSourceFile")]
    public string CurrentSourceFile { get; set; } = string.Empty;

    [JsonPropertyName("currentTargetFile")]
    public string CurrentTargetFile { get; set; } = string.Empty;
}

/// <summary>
/// Status of a backup job.
/// </summary>
public enum JobStatus
{
    Inactive,
    Active,
    Completed,
    Error
}
