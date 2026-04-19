# EasySave v1.0 — UML Diagrams

## How to View

1. Go to **[mermaid.live](https://mermaid.live)**
2. Open any `.mmd` file in a text editor
3. Copy the entire content (skip the `%%` comment lines at the top)
4. Paste into the left panel — diagram renders instantly
5. Export as PNG/SVG via Actions menu

## Diagrams

| File | Type | What it shows |
|------|------|---------------|
| `class_diagram.mmd` | Class Diagram | All classes, interfaces, enums with exact fields and methods from the codebase |
| `sequence_diagram.mmd` | Sequence Diagram | Runtime flow of executing `EasySave.exe 1-3` through all layers |
| `activity_diagram.mmd` | Activity Diagram | Complete execution flow with decision points |
| `usecase_diagram.mmd` | Use Case Diagram | All user interactions (menu + CLI) |
| `component_diagram.mmd` | Component Diagram | Three-project solution architecture |

## Design Patterns

- **Strategy**: `IBackupStrategy` → `FullBackupStrategy`, `DifferentialBackupStrategy`
- **Factory**: `BackupStrategyFactory.CreateStrategy(type)`
- **Observer**: `BackupViewModel` C# events → `Logger`, `StateManager`, `ConsoleView`
- **Singleton**: `ConfigManager`, `LanguageManager`
- **Bridge**: `IStorageProvider` → `LocalStorageProvider`
