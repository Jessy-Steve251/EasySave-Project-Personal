# EasySave v1.0 — Sequence Diagram: Execute Backup Job

```mermaid
sequenceDiagram
    actor User
    participant CLI as ConsoleApp
    participant Parser as ArgumentParser
    participant Repo as JobRepository
    participant Exec as BackupExecutor
    participant Factory as BackupStrategyFactory
    participant Strategy as IBackupStrategy
    participant Index as DifferentialIndex
    participant State as StateService
    participant Log as Logger (EasyLog.dll)

    User->>CLI: Choose "Run jobs" + selector "1-3"
    CLI->>Parser: TryParseSelector("1-3")
    Parser-->>CLI: [1, 2, 3]

    loop For each job index
        CLI->>Repo: GetByIndex(i)
        Repo-->>CLI: BackupJob

        CLI->>Exec: Execute(job)
        Exec->>Factory: Create(job.Type)
        Factory-->>Exec: IBackupStrategy

        Exec->>Index: GetLastFull(job.Name)
        Index-->>Exec: DateTime? lastFull

        Exec->>Strategy: PlanFiles(job, lastFull)
        Strategy-->>Exec: List of FileCopyPlan

        Exec->>State: Update(Active, totals)
        Note over State: Writes state.json

        loop For each file
            Exec->>State: Update(currentSource, currentDest)
            Note over State: Writes state.json

            Exec->>Exec: File.Copy (timed with Stopwatch)

            alt Copy succeeded
                Exec->>Log: Write(entry, transferTimeMs > 0)
            else Copy failed
                Exec->>Log: Write(entry, transferTimeMs < 0)
            end
            Note over Log: Appends to YYYY-MM-DD.json

            Exec->>State: Update(filesRemaining, progressPercent)
            Note over State: Writes state.json
        end

        Exec->>State: Update(Completed or Error)
        Note over State: Writes state.json

        opt Success AND type == Full
            Exec->>Index: SetLastFull(job.Name, now)
        end

        Exec-->>CLI: bool success
        CLI-->>User: Display outcome
    end
```

## Key observations

- **State.json is updated three times per file**: before copy (current paths), after copy (progress), and on completion (final status). This provides true real-time monitoring.
- **Log entries are written immediately** after each file copy, not batched at the end.
- **Negative `transferTimeMs`** signals an error to any reader of the log file.
- The **DifferentialIndex** is only updated when a Full backup completes successfully.
