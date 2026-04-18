# EasySave v1.0.0 — Release Notes

**Release Date:** April 2026  
**Product:** EasySave — ProSoft Suite  
**Target Framework:** .NET 8.0  

---

## New Features

### Backup Job Management
- Create up to **5 backup jobs**, each with a unique name, source directory, target directory, and backup type
- Delete existing backup jobs by ID
- List all configured jobs with formatted display
- Persistent configuration stored in `%AppData%/EasySave/config.json`

### Backup Execution
- **Full Backup**: copies all files and subdirectories from source to target
- **Differential Backup**: copies only new or modified files by comparing timestamps
- Execute a single job, all jobs sequentially, or a selection via command line
- CLI support: `EasySave.exe 1-3` (range) and `EasySave.exe 1;3` (specific jobs)

### Logging (EasyLog.dll)
- Daily JSON log file (`{date}.json`) recording every file transfer in real time
- Fields: timestamp, backup name, source path, target path, file size, transfer time (ms)
- Negative transfer time indicates a copy error
- Developed as a separate reusable DLL for other ProSoft Suite applications

### Real-Time Status
- Single `state.json` file tracking progress of all backup jobs
- Fields: job name, status, total/remaining files and sizes, progression percentage, current file paths
- Updated in real time during backup operations

### Internationalization
- Full support for **English** and **French** interfaces
- Language selection at application startup

### Directory Support
- Local disks
- External drives
- Network drives (UNC paths)

## Architecture

- **3-project solution**: `EasySave.ConsoleApp` (.exe), `EasySave.Core` (.dll), `EasyLog.dll`
- **Design Patterns**: Strategy, Factory, Observer, Singleton, Bridge
- **Prepared for v2.0**: business logic separated from UI for future GUI migration (MVVM)

## Known Limitations
- Maximum of 5 backup jobs
- Console interface only (GUI planned for v2.0)
- Sequential job execution (no parallel processing)
- No file encryption support (planned for future versions)

## Technical Support Information
- **Configuration file**: `%AppData%/EasySave/config.json`
- **Log files**: `%AppData%/EasySave/{date}.json`
- **State file**: `%AppData%/EasySave/state.json`
- **Minimum configuration**: Windows 10+, .NET 8.0 Runtime, 50 MB disk space
- **Default installation**: standard .NET publish directory

---
*ProSoft — EasySave v1.0.0 | © 2026*
