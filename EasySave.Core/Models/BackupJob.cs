using System.Text.Json.Serialization;

namespace EasySave.Core.Models;

/// <summary>
/// Type of backup operation.
/// </summary>
public enum BackupType
{
    Full,
    Differential
}

/// <summary>
/// Represents a backup job configuration.
/// Pure data model — no behavior beyond validation.
/// </summary>
public class BackupJob
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sourceDir")]
    public string SourceDir { get; set; } = string.Empty;

    [JsonPropertyName("targetDir")]
    public string TargetDir { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BackupType Type { get; set; }

    /// <summary>
    /// Validates that the backup job has all required fields configured.
    /// </summary>
    /// <returns>True if the job configuration is valid.</returns>
    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(Name)
            && !string.IsNullOrWhiteSpace(SourceDir)
            && !string.IsNullOrWhiteSpace(TargetDir)
            && Directory.Exists(SourceDir);
    }
}
