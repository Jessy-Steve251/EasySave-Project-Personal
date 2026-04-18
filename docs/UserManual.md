# EasySave v1.0 — User Manual

## Getting Started

EasySave is a console backup application that lets you create and manage up to **5 backup jobs**. Each job copies files from a source directory to a target directory, supporting both local and network drives.

## Running EasySave

### Interactive Mode
Double-click `EasySave.exe` or run it from the terminal without arguments. You will be prompted to select a language (English or French), then presented with the main menu.

### Command-Line Mode
Run backups directly from the terminal:

| Command | Description |
|---------|-------------|
| `EasySave.exe 1-3` | Execute backup jobs 1 through 3 |
| `EasySave.exe 1;3` | Execute backup jobs 1 and 3 |
| `EasySave.exe 2` | Execute backup job 2 only |

## Menu Options

| Option | Description |
|--------|-------------|
| **1. Create a backup job** | Define a new job with name, source directory, target directory, and type (Full or Differential) |
| **2. Delete a backup job** | Remove an existing job by its ID |
| **3. List all backup jobs** | Display all configured jobs with their details |
| **4. Execute a backup job** | Run a single backup job by ID |
| **5. Execute all backup jobs** | Run all configured jobs sequentially |
| **6. Exit** | Close the application |

## Backup Types

- **Full Backup**: Copies ALL files and subdirectories from the source to the target directory, regardless of whether they have changed.
- **Differential Backup**: Copies only files that are new or have been modified since the last backup. Compares file modification timestamps.

## Output Files

| File | Location | Description |
|------|----------|-------------|
| `config.json` | `%AppData%/EasySave/` | Stores your backup job configurations |
| `{date}.json` | `%AppData%/EasySave/` | Daily log file recording every file transfer |
| `state.json` | `%AppData%/EasySave/` | Real-time progress of all backup jobs |

## Requirements

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 Runtime
- Read access to source directories
- Write access to target directories

---
*ProSoft — EasySave v1.0 | Unit price: €200 HT | Support contract: 12% annually (5/7, 8am–5pm)*
