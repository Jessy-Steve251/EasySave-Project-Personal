# EasySave v1.0 — Use Case Diagram

```mermaid
flowchart LR
    subgraph Actors
        User["👤 End User (EN/FR)"]
        Scheduler["⏱️ Scheduler / Shell Script"]
    end

    subgraph EasySave["EasySave v1.0"]
        UC1["Create backup job"]
        UC2["List backup jobs"]
        UC3["Delete backup job"]
        UC4["Run selected job(s)"]
        UC5["Change language"]
        UC6["Write daily log\n(included)"]
        UC7["Update real-time state\n(included)"]
    end

    User --> UC1
    User --> UC2
    User --> UC3
    User --> UC4
    User --> UC5
    Scheduler -->|"EasySave.exe 1-3"| UC4
    Scheduler -->|"EasySave.exe 1;3"| UC4
    UC4 -.->|includes| UC6
    UC4 -.->|includes| UC7
```

## Notes

- **Max 5 jobs** can be defined at any time.
- The **scheduler/shell** uses command-line selectors to run jobs non-interactively.
- Every file operation during a run triggers both a **log write** (EasyLog.dll) and a **state update** (state.json).
- Language selection affects only the console UI; log and state files are always in English.
