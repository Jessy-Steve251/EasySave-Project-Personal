using EasySave.Core.Interfaces;
using EasySave.Core.Models;
using EasySave.Core.Strategies;

namespace EasySave.Core.Services;

/// <summary>
/// Factory pattern: creates the appropriate IBackupStrategy based on BackupType.
/// Adding a new backup type requires only adding a new case here and a new strategy class.
/// The rest of the application remains untouched (Open/Closed Principle).
/// </summary>
public class BackupStrategyFactory
{
    /// <summary>
    /// Creates and returns the backup strategy matching the specified type.
    /// </summary>
    /// <param name="type">The backup type (Full or Differential).</param>
    /// <returns>An IBackupStrategy implementation.</returns>
    public IBackupStrategy CreateStrategy(BackupType type)
    {
        return type switch
        {
            BackupType.Full => new FullBackupStrategy(),
            BackupType.Differential => new DifferentialBackupStrategy(),
            _ => throw new ArgumentException($"Unknown backup type: {type}", nameof(type))
        };
    }
}
