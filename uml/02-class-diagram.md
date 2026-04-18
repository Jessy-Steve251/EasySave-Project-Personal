%% ============================================================
%% EasySave v1.0 — Complete Class Diagram (Mermaid)
%% Patterns: Strategy, Factory, Observer, Singleton, Bridge
%% Software Engineering Project
%% ============================================================

classDiagram
    direction TB

    %% ==================== ENUMERATIONS ====================
    class BackupType {
        <<enumeration>>
        Full
        Differential
    }

    class JobStatus {
        <<enumeration>>
        Inactive
        Active
        Completed
        Error
    }

    %% ==================== DATA MODELS ====================
    class LogEntry {
        +DateTime Timestamp
        +string BackupName
        +string SourceFilePath
        +string TargetFilePath
        +long FileSize
        +long TransferTimeMs
    }

    class JobState {
        +string JobName
        +DateTime LastActionTimestamp
        +JobStatus Status
        +int TotalEligibleFiles
        +long TotalFileSize
        +int FilesRemaining
        +long SizeRemaining
        +double Progression
        +string CurrentSourceFile
        +string CurrentTargetFile
    }

    %% ==================== ENTRY POINT (.exe) ====================
    class Program {
        +Main(args string[])$ void
    }

    class ConsoleView {
        -BackupViewModel _viewModel
        -LanguageManager _lang
        +DisplayMenu() void
        +ParseArguments(args string[]) int[]
        +UpdateProgressDisplay(state JobState) void
        +Run() void
    }

    class LanguageManager {
        <<Singleton>>
        -LanguageManager _instance$
        -string _currentCulture
        -Dictionary translations
        -LanguageManager()
        +Instance LanguageManager$
        +CurrentCulture string
        +GetString(key string) string
        +SetLanguage(culture string) void
    }

    %% ==================== CORE (.dll) ====================
    class BackupViewModel {
        <<ViewModel>>
        -List~BackupJob~ _jobs
        -BackupStrategyFactory _factory
        +ExecuteJobs(jobIds List~int~) void
        +ExecuteAllJobs() void
        +CreateJob(name string, source string, target string, type BackupType) void
        +DeleteJob(id int) void
        +LoadJobs() void
        +SaveJobs() void
        +event OnFileCopied~LogEntry~
        +event OnStateChanged~JobState~
        +event OnJobStarted~JobState~
        +event OnJobCompleted~JobState~
    }

    class BackupJob {
        +int Id
        +string Name
        +string SourceDir
        +string TargetDir
        +BackupType Type
        +bool IsActive
        +Validate() bool
    }

    class BackupStrategyFactory {
        <<Factory>>
        +CreateStrategy(type BackupType) IBackupStrategy
    }

    class IBackupStrategy {
        <<interface>>
        +Execute(job BackupJob, storage IStorageProvider, onFileCopied Action~LogEntry~, onProgress Action~JobState~) void
    }

    class FullBackupStrategy {
        +Execute(job BackupJob, storage IStorageProvider, onFileCopied Action~LogEntry~, onProgress Action~JobState~) void
        -GetAllFiles(sourceDir string, storage IStorageProvider) List~string~
        -CopyAllFilesAndDirs(source string, target string, storage IStorageProvider) void
    }

    class DifferentialBackupStrategy {
        +Execute(job BackupJob, storage IStorageProvider, onFileCopied Action~LogEntry~, onProgress Action~JobState~) void
        -GetModifiedFiles(source string, target string, storage IStorageProvider) List~string~
        -IsFileModified(sourceFile string, targetFile string, storage IStorageProvider) bool
    }

    class IStorageProvider {
        <<interface>>
        +CopyFile(source string, target string) void
        +GetFiles(directory string) List~string~
        +GetDirectories(directory string) List~string~
        +GetLastModifiedTime(filePath string) DateTime
        +GetFileSize(filePath string) long
        +FileExists(filePath string) bool
        +CreateDirectory(path string) void
        +Initialize(connectionString string) void
    }

    class LocalStorageProvider {
        +CopyFile(source string, target string) void
        +GetFiles(directory string) List~string~
        +GetDirectories(directory string) List~string~
        +GetLastModifiedTime(filePath string) DateTime
        +GetFileSize(filePath string) long
        +FileExists(filePath string) bool
        +CreateDirectory(path string) void
        +Initialize(connectionString string) void
    }

    class NetworkStorageProvider {
        -string _uncBasePath
        +CopyFile(source string, target string) void
        +GetFiles(directory string) List~string~
        +GetDirectories(directory string) List~string~
        +GetLastModifiedTime(filePath string) DateTime
        +GetFileSize(filePath string) long
        +FileExists(filePath string) bool
        +CreateDirectory(path string) void
        +Initialize(connectionString string) void
    }

    class ConfigManager {
        <<Singleton>>
        -ConfigManager _instance$
        -object _lock$
        -string _configPath
        -ConfigManager()
        +Instance ConfigManager$
        +LoadJobs() List~BackupJob~
        +SaveJobs(jobs List~BackupJob~) void
        -GetConfigDirectory() string
    }

    %% ==================== LOGGER (EasyLog.dll) ====================
    class Logger {
        -string _logDirectory
        +Logger(logDirectory string)
        +LogAction(entry LogEntry) void
        +Subscribe(viewModel BackupViewModel) void
        -GetDailyLogPath() string
        -WriteJsonEntry(entry LogEntry, path string) void
    }

    class StateManager {
        -string _stateFilePath
        -List~JobState~ _states
        +StateManager(stateFilePath string)
        +UpdateState(state JobState) void
        +Subscribe(viewModel BackupViewModel) void
        +WriteStateToJson() void
        +InitializeStates(jobs List~BackupJob~) void
    }

    %% ==================== RELATIONSHIPS ====================

    %% Entry point
    Program --> ConsoleView : creates

    %% ConsoleApp layer
    ConsoleView --> BackupViewModel : uses
    ConsoleView --> LanguageManager : requests translations
    ConsoleView ..> BackupViewModel : subscribes to OnStateChanged

    %% ViewModel composition and dependencies
    BackupViewModel "1" *-- "0..5" BackupJob : contains
    BackupViewModel --> BackupStrategyFactory : uses
    BackupViewModel --> ConfigManager : loads/saves via
    BackupViewModel --> IStorageProvider : passes to strategies

    %% BackupJob uses enum
    BackupJob --> BackupType : has type

    %% Factory creates strategies
    BackupStrategyFactory ..> IBackupStrategy : creates
    BackupStrategyFactory ..> BackupType : reads type

    %% Strategy implementations (dashed = implements)
    FullBackupStrategy ..|> IBackupStrategy : implements
    DifferentialBackupStrategy ..|> IBackupStrategy : implements

    %% Bridge: Strategy uses StorageProvider
    IBackupStrategy --> IStorageProvider : uses via Bridge

    %% Bridge implementations
    LocalStorageProvider ..|> IStorageProvider : implements
    NetworkStorageProvider ..|> IStorageProvider : implements

    %% Observer subscriptions
    Logger ..> BackupViewModel : subscribes OnFileCopied
    StateManager ..> BackupViewModel : subscribes OnStateChanged

    %% Data model usage
    Logger --> LogEntry : writes
    StateManager --> JobState : manages
    JobState --> JobStatus : has status
