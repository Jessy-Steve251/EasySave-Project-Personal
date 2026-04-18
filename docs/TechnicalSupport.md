# EasySave v1.0 — Technical Support Documentation

## For Customer Support Teams

### Default Installation Location

EasySave is a portable .NET 8.0 application. The executable (`EasySave.exe`) can be placed in any directory. No installer is required.

**Typical deployment path:**
```
C:\Program Files\ProSoft\EasySave\EasySave.exe
```

### Minimum System Configuration

| Requirement | Specification |
|-------------|---------------|
| **Operating System** | Windows 10 (1809+), Windows 11, Windows Server 2016+ |
| **Runtime** | .NET 8.0 Runtime (download from https://dotnet.microsoft.com/download/dotnet/8.0) |
| **Disk Space** | 50 MB for application + space for backup targets |
| **RAM** | 256 MB minimum |
| **Permissions** | Read access to source directories, Write access to target directories |

### Configuration Files Location

All configuration and log files are stored in the user's Application Data folder:

```
%AppData%\EasySave\
```

This typically resolves to:
```
C:\Users\{username}\AppData\Roaming\EasySave\
```

### File Descriptions

| File | Format | Purpose | Updated |
|------|--------|---------|---------|
| `config.json` | JSON | Stores all backup job definitions (up to 5 jobs) | When jobs are created, modified, or deleted |
| `state.json` | JSON | Real-time status of all backup jobs | Continuously during backup execution |
| `{yyyy-MM-dd}.json` | JSON | Daily log of all file transfer actions | After each file copy operation |

### Configuration File Structure (config.json)

```json
[
  {
    "id": 1,
    "name": "DailyBackup",
    "sourceDir": "C:\\Users\\data\\documents",
    "targetDir": "D:\\Backups\\documents",
    "type": "Full"
  }
]
```

### Daily Log File Structure ({date}.json)

```json
[
  {
    "timestamp": "2026-04-19T14:30:15.123",
    "backupName": "DailyBackup",
    "sourceFilePath": "C:\\Users\\data\\documents\\report.pdf",
    "targetFilePath": "D:\\Backups\\documents\\report.pdf",
    "fileSize": 245760,
    "transferTimeMs": 45
  }
]
```

**Note:** A negative `transferTimeMs` value indicates a file copy error.

### State File Structure (state.json)

```json
[
  {
    "jobName": "DailyBackup",
    "lastActionTimestamp": "2026-04-19T14:30:15.123",
    "status": "Active",
    "totalEligibleFiles": 150,
    "totalFileSize": 52428800,
    "filesRemaining": 75,
    "sizeRemaining": 26214400,
    "progression": 50.0,
    "currentSourceFile": "C:\\Users\\data\\documents\\report.pdf",
    "currentTargetFile": "D:\\Backups\\documents\\report.pdf"
  }
]
```

**Status values:** `Inactive`, `Active`, `Completed`, `Error`

### Command-Line Usage

| Command | Effect |
|---------|--------|
| `EasySave.exe` | Launches interactive menu mode |
| `EasySave.exe 1` | Executes backup job with ID 1 |
| `EasySave.exe 1-3` | Executes backup jobs 1, 2, and 3 sequentially |
| `EasySave.exe 1;3` | Executes backup jobs 1 and 3 |

### Common Troubleshooting

| Issue | Cause | Resolution |
|-------|-------|------------|
| "Source directory does not exist" | Invalid source path in job config | Verify the path exists and is accessible |
| Files not appearing in target | Insufficient write permissions | Ensure the user has write access to the target directory |
| Empty daily log file | No backups have been executed today | Execute a backup job to generate log entries |
| Network drive backup fails | UNC path not accessible | Verify network connectivity and credentials |
| Application crashes on startup | Missing .NET 8.0 Runtime | Install .NET 8.0 Runtime from Microsoft |
| config.json corrupted | Manual editing introduced syntax errors | Delete `%AppData%\EasySave\config.json` and recreate jobs |

### Application Architecture

```
EasySave.exe          → Console interface (replaceable by GUI in v2.0)
  ├── EasySave.Core.dll → Business logic (backup engine, strategies, config)
  └── EasyLog.dll       → Logging library (daily logs, real-time state)
```

**DLL Dependencies:**
- `EasySave.exe` depends on `EasySave.Core.dll` and `EasyLog.dll`
- `EasySave.Core.dll` depends on `EasyLog.dll`
- `EasyLog.dll` has no external dependencies (standalone, reusable)

### Support Contact

ProSoft Technical Support
- Contract: Annual maintenance 5/7 8am-5pm
- Cost: 12% of purchase price (€24/year based on €200 unit price)
- Contract type: Annual with tacit renewal, SYNTEC index revaluation

---
*ProSoft — EasySave v1.0.0 | Internal Document — Customer Support*
