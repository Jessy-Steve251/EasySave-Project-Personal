# EasySave v1.0 — Technical Support Documentation

## Minimum configuration

| Requirement   | Value                          |
|---------------|--------------------------------|
| OS            | Windows 10 / 11 (x64) or any OS with .NET 8 runtime |
| Runtime       | .NET 8.0 Runtime               |
| RAM           | 256 MB minimum                 |
| Disk          | 10 MB for the application + space for backups and logs |
| Network       | Required only if source/target directories are on network shares |

## Default installation location

The executable (`EasySave.exe`) is distributed as a self-contained folder. There is no installer; it runs directly from the extracted directory.

Recommended placement: `C:\Program Files\ProSoft\EasySave\`

## Runtime file locations

All runtime data is stored under:

**Windows:** `C:\ProgramData\ProSoft\EasySave\`

| File / Folder               | Purpose                                       |
|-----------------------------|-----------------------------------------------|
| `jobs.json`                 | Persisted backup job definitions (max 5 jobs)  |
| `state.json`                | Real-time status of every job (overwritten on each file operation) |
| `Logs\YYYY-MM-DD.json`     | Daily log file — one JSON array of log entries per day |
| `differential-index.json`  | Timestamp of the last successful full backup per job |

All files are UTF-8 encoded JSON with `WriteIndented = true` for Notepad readability.

## Configuration file details

### jobs.json

JSON array of up to 5 backup job objects:

```json
[
  {
    "name": "DailyBackup",
    "sourceDirectory": "C:\\Users\\Data",
    "targetDirectory": "D:\\Backups\\Data",
    "type": 0
  }
]
```

`type`: `0` = Full, `1` = Differential.

### state.json

JSON array with one entry per defined job. Updated in real time during execution:

```json
[
  {
    "jobName": "DailyBackup",
    "lastActionTimestamp": "2025-01-15T10:30:00",
    "status": 2,
    "totalFilesToCopy": 150,
    "totalFilesSize": 52428800,
    "filesRemaining": 0,
    "sizeRemaining": 0,
    "progressPercent": 100.0,
    "currentSourceFile": "",
    "currentDestinationFile": ""
  }
]
```

`status`: `0` = Inactive, `1` = Active, `2` = Completed, `3` = Error.

### Log file (Logs/YYYY-MM-DD.json)

JSON array of individual file-transfer entries:

```json
[
  {
    "timestamp": "2025-01-15T10:30:01",
    "jobName": "DailyBackup",
    "sourcePath": "C:\\Users\\Data\\report.docx",
    "destinationPath": "D:\\Backups\\Data\\report.docx",
    "fileSize": 25600,
    "transferTimeMs": 12.5
  }
]
```

A negative `transferTimeMs` indicates an error occurred during that file transfer.

## Exit codes (scripted / scheduled execution)

| Code | Meaning                        |
|------|--------------------------------|
| 0    | All requested jobs succeeded   |
| 1    | One or more jobs had errors    |
| 2    | Invalid command-line selector  |

## Library dependency

EasySave depends on `EasyLog.dll` (ProSoft.EasyLog), a reusable logging library maintained as a separate project within the solution. This DLL is built alongside the main application.

## Troubleshooting

| Symptom | Probable cause | Action |
|---------|---------------|--------|
| "Source directory does not exist" | Path typo or network share not mounted | Verify path with `dir` or `ls`; check network connectivity |
| No log file created | Insufficient permissions on `%ProgramData%` | Run as administrator or change ProgramData ACLs |
| Differential copies everything | No prior full backup recorded for that job | Run a Full backup first |
| `state.json` shows Error | A file failed to copy (check log for negative `transferTimeMs`) | Inspect log entries with negative transfer time |
