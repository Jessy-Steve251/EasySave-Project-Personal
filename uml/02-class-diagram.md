# EasySave v1.0 — Class Diagram

classDiagram
    direction TB

    %% ===== EasyLog.dll =====
    namespace EasyLog {
        class LogEntry {
            +DateTime Timestamp
            +string JobName
            +string SourcePath
            +string DestinationPath
            +long FileSize
            +double TransferTimeMs
        }
        class Logger {
            -string _logDirectory
            -object _fileLock
            +Logger(logDirectory: string)
            +Write(entry: LogEntry) void
            +GetCurrentLogFilePath() string
        }
    }
    Logger ..> LogEntry : writes

    %% ===== Models =====
    namespace Models {
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
        class BackupJob {
            +string Name
            +string SourceDirectory
            +string TargetDirectory
            +BackupType Type
            +BackupJob(name, source, target, type)
        }
        class JobState {
            +string JobName
            +DateTime LastActionTimestamp
            +JobStatus Status
            +int TotalFilesToCopy
            +long TotalFilesSize
            +int FilesRemaining
            +long SizeRemaining
            +double ProgressPercent
            +string CurrentSourceFile
            +string CurrentDestinationFile
        }
    }
    BackupJob --> BackupType
    JobState --> JobStatus

    %% ===== Repositories =====
    namespace Repositories {
        class JobRepository {
            +int MaxJobs = 5$
            -string _filePath
            -List~BackupJob~ _jobs
            +Add(job: BackupJob) void
            +Remove(name: string) bool
            +GetByIndex(i: int) BackupJob
            +IReadOnlyList~BackupJob~ Jobs
            +int Count
        }
    }
    JobRepository "1" o-- "0..5" BackupJob

    %% ===== Strategies =====
    namespace Strategies {
        class IBackupStrategy {
            <<interface>>
            +BackupType Type
            +PlanFiles(job, lastFullUtc) IReadOnlyList~FileCopyPlan~
        }
        class FullBackupStrategy {
            +BackupType Type
            +PlanFiles(job, lastFullUtc)
        }
        class DifferentialBackupStrategy {
            +BackupType Type
            +PlanFiles(job, lastFullUtc)
        }
        class FileCopyPlan {
            <<record>>
            +string Source
            +string Destination
            +long Size
        }
        class FileScanner {
            <<static>>
            +EnumerateAll(job) IEnumerable~FileCopyPlan~
        }
        class BackupStrategyFactory {
            <<static>>
            +Create(type: BackupType) IBackupStrategy
        }
    }
    IBackupStrategy <|.. FullBackupStrategy
    IBackupStrategy <|.. DifferentialBackupStrategy
    FullBackupStrategy ..> FileScanner
    DifferentialBackupStrategy ..> FileScanner
    BackupStrategyFactory ..> IBackupStrategy : creates

    %% ===== Services =====
    namespace Services {
        class AppPaths {
            <<static>>
            +string Root$
            +string LogsDirectory$
            +string StateFile$
            +string JobsFile$
            +string DifferentialIndexFile$
        }
        class StateService {
            -string _filePath
            -Dictionary _states
            +InitializeFor(jobs) void
            +Update(state: JobState) void
        }
        class DifferentialIndex {
            -string _filePath
            -Dictionary _lastFullByJob
            +GetLastFull(jobName) DateTime?
            +SetLastFull(jobName, ts) void
        }
        class BackupExecutor {
            -Logger _logger
            -StateService _stateService
            -DifferentialIndex _differentialIndex
            +Execute(job: BackupJob) bool
        }
    }
    BackupExecutor --> Logger
    BackupExecutor --> StateService
    BackupExecutor --> DifferentialIndex
    BackupExecutor ..> BackupStrategyFactory
    StateService ..> JobState

    %% ===== Localization =====
    namespace Localization {
        class Language {
            <<enumeration>>
            English
            French
        }
        class LanguageService {
            +Language Current
            +SetLanguage(l: Language) void
            +Get(key: string) string
        }
    }
    LanguageService --> Language

    %% ===== CLI =====
    namespace CLI {
        class Program {
            <<entry point>>
        }
        class ConsoleApp {
            -JobRepository _repository
            -BackupExecutor _executor
            -LanguageService _language
            +Run() void
            +RunFromSelector(s: string) int
        }
        class ArgumentParser {
            <<static>>
            +TryParseSelector(input, indices, error) bool
        }
    }
    Program --> ConsoleApp
    ConsoleApp --> JobRepository
    ConsoleApp --> BackupExecutor
    ConsoleApp --> LanguageService
    ConsoleApp ..> ArgumentParser
