# EasySave v1.0 — Package / Component Diagram

```mermaid
graph TB
    subgraph Solution["EasySave Solution (single Git repo)"]
        subgraph CLI_PKG["EasySave.CLI (exe)"]
            Program["Program.cs\n(composition root)"]
            ConsoleApp["ConsoleApp.cs"]
            ArgParser["ArgumentParser.cs"]
        end

        subgraph Core_PKG["EasySave.Core (class library)"]
            Models["Models\nBackupJob, BackupType\nJobState, JobStatus"]
            Repos["Repositories\nJobRepository"]
            Services["Services\nBackupExecutor\nStateService\nDifferentialIndex\nAppPaths"]
            Strategies["Strategies\nIBackupStrategy\nFullBackupStrategy\nDifferentialBackupStrategy\nFileScanner, Factory"]
            Localization["Localization\nLanguageService"]
        end

        subgraph Log_PKG["EasyLog (class library / DLL)"]
            Logger["Logger.cs"]
            LogEntry["LogEntry.cs"]
        end

        subgraph Future["v2.0 (planned)"]
            WPF["EasySave.WPF\n(MVVM shell)"]
        end
    end

    subgraph Data["Runtime files (%ProgramData%/ProSoft/EasySave/)"]
        jobs["jobs.json"]
        state["state.json"]
        logs["Logs/YYYY-MM-DD.json"]
        diffidx["differential-index.json"]
    end

    CLI_PKG -->|ProjectReference| Core_PKG
    Core_PKG -->|ProjectReference| Log_PKG
    WPF -.->|ProjectReference (v2.0)| Core_PKG

    Repos --> jobs
    Services --> state
    Services --> diffidx
    Logger --> logs

    style Future fill:#eee,stroke:#aaa,stroke-dasharray: 5 5
    style WPF fill:#eee,stroke:#aaa,stroke-dasharray: 5 5
```

## Architecture rationale

- **EasySave.Core has zero dependency on the console layer.** This means for v2.0 we just add an `EasySave.WPF` project that references Core — no business logic rewrite.
- **EasyLog is a separate project** within the same repo (separate folder, own `.csproj`), satisfying the spec requirement for a reusable DLL while keeping everything in one Git repository for simpler branch management.
- **Strategy pattern** on backup types means adding a new type (e.g., incremental) costs exactly one new class — no changes to the executor.
