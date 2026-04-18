# EasySave v1.0 — Release Note

**Release date:** _[to be set by the team]_  
**Product:** EasySave — ProSoft Suite  
**Version:** 1.0.0  
**Target framework:** .NET 8.0

---

## Features

- **Backup job management:** Create, list, and delete up to 5 backup jobs, each with a name, source directory, target directory, and type (Full or Differential).
- **Full backup:** Recursively copies every file and subdirectory from the source to the target.
- **Differential backup:** Copies only files modified since the last successful full backup for that job. Falls back to full copy if no prior full backup exists.
- **Command-line execution:** Run jobs non-interactively via selectors — range (`1-3`), list (`1;3`), or single (`2`).
- **Interactive menu:** Full-featured console menu for job management and execution.
- **Bilingual interface:** English and French, selectable at startup and switchable at any time.
- **Real-time daily log:** Every file operation logged to `Logs/YYYY-MM-DD.json` with timestamp, job name, source/destination paths (UNC), file size, and transfer time in ms. Negative transfer time indicates an error. Implemented in a reusable library (`EasyLog.dll`).
- **Real-time state file:** `state.json` updated after every file operation with progress, files remaining, and current source/destination paths.
- **Portable file locations:** All runtime files stored under `%ProgramData%\ProSoft\EasySave\`.
- **UNC path support:** Source and target directories can be on local disks, external drives, or network shares.

## Architecture highlights

- **Three-project structure:** `EasySave.Core` (business logic), `EasySave.CLI` (console UI), `EasyLog` (reusable DLL). Core has zero dependency on the console layer.
- **Strategy pattern:** Backup types implemented as interchangeable strategies behind `IBackupStrategy`.
- **No code duplication:** File scanning shared via `FileScanner`; both strategies reuse it.

## Known limitations

- Maximum of 5 backup jobs (by specification).
- Sequential execution only; no parallel job processing.
- No file encryption or compression.
- No visual progress bar in console (percentage is written to `state.json` for external monitoring).

## Dependencies

| Component   | Version | Notes                              |
|-------------|---------|-------------------------------------|
| .NET Runtime | 8.0     | Required on the target machine      |
| EasyLog.dll | 1.0.0   | Built from the EasyLog project in this solution |

## Upgrade path

Version 2.0 will introduce a graphical user interface (WPF / MVVM). The `EasySave.Core` library is designed to be reused without modification.
