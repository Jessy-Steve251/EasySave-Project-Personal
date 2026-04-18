# EasySave v1.0 — User Manual

**ProSoft Suite** — Backup software (console edition)

## Launching the software

Open a terminal in the installation folder and run:

- `EasySave.exe` — starts the interactive menu.
- `EasySave.exe 1-3` — runs backup jobs 1, 2 and 3 sequentially, without prompts.
- `EasySave.exe 1;3` — runs backup jobs 1 and 3 only.

On first launch, EasySave asks you to choose a language (English / Français).

## Managing backup jobs (max 5)

From the main menu:

1. **List** — shows every defined job with its index, name, type, source and target.
2. **Create** — prompts for a job name, a source directory, a target directory and a type (`1` = Full, `2` = Differential). A full backup copies every file. A differential backup copies only files modified since the last successful full backup for that same job.
3. **Delete** — removes a job by name.
4. **Run** — executes one or several jobs. Use the same selector syntax as the command line (`2`, `1-3`, `1;3`).
5. **Change language** — switches the UI between English and French at any time.
6. **Quit** — exits.

Source and target directories can be on local disks, external drives, or network shares (UNC paths supported).

## Runtime files

EasySave stores its runtime files under `%ProgramData%\ProSoft\EasySave\`:

- `jobs.json` — your defined backup jobs.
- `Logs\YYYY-MM-DD.json` — one log file per day, appended in real time.
- `state.json` — live status of every job (progress, current file, etc.).
- `differential-index.json` — timestamps used by differential backups.

All files are UTF-8 JSON, indented for Notepad readability.

## Exit codes (scripted mode)

`0` success — `1` one or more jobs reported errors — `2` invalid selector.

---
© ProSoft. Support: see the technical documentation for minimum configuration and file locations.
