using System.Text.Json.Serialization;

namespace EasySave.Logger;

/// <summary>
/// Represents a single action logged during a backup operation.
/// Fields match the EasySave v1.0 specification requirements.
/// </summary>
public class LogEntry
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("backupName")]
    public string BackupName { get; set; } = string.Empty;

    [JsonPropertyName("sourceFilePath")]
    public string SourceFilePath { get; set; } = string.Empty;

    [JsonPropertyName("targetFilePath")]
    public string TargetFilePath { get; set; } = string.Empty;

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    [JsonPropertyName("transferTimeMs")]
    public long TransferTimeMs { get; set; }
}
