using System.Text.Json;

namespace EasySave.ConsoleApp;

/// <summary>
/// Singleton pattern: manages internationalization for English and French.
/// Loads translations from embedded JSON resources.
/// The spec requires: "usable by English-speaking and French-speaking users at a minimum."
/// </summary>
public class LanguageManager
{
    private static LanguageManager? _instance;
    private Dictionary<string, string> _translations = new();
    private string _currentCulture = "en";

    private LanguageManager()
    {
        LoadTranslations(_currentCulture);
    }

    public static LanguageManager Instance
    {
        get
        {
            _instance ??= new LanguageManager();
            return _instance;
        }
    }

    public string CurrentCulture => _currentCulture;

    /// <summary>
    /// Switches the application language and reloads translations.
    /// </summary>
    public void SetLanguage(string culture)
    {
        _currentCulture = culture.ToLower();
        LoadTranslations(_currentCulture);
    }

    /// <summary>
    /// Returns the translated string for the given key.
    /// Falls back to the key itself if no translation is found.
    /// </summary>
    public string GetString(string key)
    {
        return _translations.TryGetValue(key, out string? value) ? value : key;
    }

    /// <summary>
    /// Loads translations from the embedded dictionary.
    /// In a larger project these would be in separate .json resource files.
    /// </summary>
    private void LoadTranslations(string culture)
    {
        _translations = culture switch
        {
            "fr" => GetFrenchTranslations(),
            _ => GetEnglishTranslations()
        };
    }

    private Dictionary<string, string> GetEnglishTranslations() => new()
    {
        // Menu
        ["app_title"] = "=== EasySave v1.0 ===",
        ["select_language"] = "Select language / Choisir la langue:",
        ["lang_english"] = "1. English",
        ["lang_french"] = "2. Français",
        ["main_menu"] = "--- Main Menu ---",
        ["menu_create"] = "1. Create a backup job",
        ["menu_delete"] = "2. Delete a backup job",
        ["menu_list"] = "3. List all backup jobs",
        ["menu_execute_one"] = "4. Execute a backup job",
        ["menu_execute_all"] = "5. Execute all backup jobs",
        ["menu_exit"] = "6. Exit",
        ["menu_choice"] = "Your choice: ",

        // Job creation
        ["enter_name"] = "Enter backup name: ",
        ["enter_source"] = "Enter source directory: ",
        ["enter_target"] = "Enter target directory: ",
        ["select_type"] = "Select backup type (1=Full, 2=Differential): ",
        ["job_created"] = "Backup job '{0}' created successfully (ID: {1}).",
        ["job_limit"] = "Cannot create more than 5 backup jobs.",
        ["invalid_source"] = "Source directory does not exist.",

        // Job deletion
        ["enter_id_delete"] = "Enter the ID of the job to delete: ",
        ["job_deleted"] = "Backup job deleted successfully.",
        ["job_not_found"] = "Job not found.",

        // Job listing
        ["no_jobs"] = "No backup jobs configured.",
        ["job_list_header"] = "  ID | Name                 | Type         | Source",

        // Execution
        ["enter_id_execute"] = "Enter the job ID to execute: ",
        ["executing_job"] = "Executing backup: {0} ({1})...",
        ["job_started"] = "[STARTED] {0}",
        ["file_copied"] = "  Copied: {0} ({1} bytes, {2}ms)",
        ["file_error"] = "  ERROR: {0} (failed after {1}ms)",
        ["job_completed"] = "[COMPLETED] {0} - {1:F1}%",
        ["job_error"] = "[ERROR] {0} - Invalid configuration",
        ["all_completed"] = "All backup jobs completed.",
        ["progress"] = "  Progress: {0:F1}% ({1}/{2} files)",

        // General
        ["invalid_input"] = "Invalid input. Please try again.",
        ["press_key"] = "Press any key to continue..."
    };

    private Dictionary<string, string> GetFrenchTranslations() => new()
    {
        // Menu
        ["app_title"] = "=== EasySave v1.0 ===",
        ["select_language"] = "Select language / Choisir la langue:",
        ["lang_english"] = "1. English",
        ["lang_french"] = "2. Français",
        ["main_menu"] = "--- Menu Principal ---",
        ["menu_create"] = "1. Créer une tâche de sauvegarde",
        ["menu_delete"] = "2. Supprimer une tâche de sauvegarde",
        ["menu_list"] = "3. Lister les tâches de sauvegarde",
        ["menu_execute_one"] = "4. Exécuter une tâche de sauvegarde",
        ["menu_execute_all"] = "5. Exécuter toutes les tâches",
        ["menu_exit"] = "6. Quitter",
        ["menu_choice"] = "Votre choix : ",

        // Job creation
        ["enter_name"] = "Nom de la sauvegarde : ",
        ["enter_source"] = "Répertoire source : ",
        ["enter_target"] = "Répertoire cible : ",
        ["select_type"] = "Type de sauvegarde (1=Complète, 2=Différentielle) : ",
        ["job_created"] = "Tâche de sauvegarde '{0}' créée avec succès (ID : {1}).",
        ["job_limit"] = "Impossible de créer plus de 5 tâches de sauvegarde.",
        ["invalid_source"] = "Le répertoire source n'existe pas.",

        // Job deletion
        ["enter_id_delete"] = "Entrez l'ID de la tâche à supprimer : ",
        ["job_deleted"] = "Tâche de sauvegarde supprimée avec succès.",
        ["job_not_found"] = "Tâche non trouvée.",

        // Job listing
        ["no_jobs"] = "Aucune tâche de sauvegarde configurée.",
        ["job_list_header"] = "  ID | Nom                  | Type         | Source",

        // Execution
        ["enter_id_execute"] = "Entrez l'ID de la tâche à exécuter : ",
        ["executing_job"] = "Exécution de la sauvegarde : {0} ({1})...",
        ["job_started"] = "[DÉMARRÉ] {0}",
        ["file_copied"] = "  Copié : {0} ({1} octets, {2}ms)",
        ["file_error"] = "  ERREUR : {0} (échec après {1}ms)",
        ["job_completed"] = "[TERMINÉ] {0} - {1:F1}%",
        ["job_error"] = "[ERREUR] {0} - Configuration invalide",
        ["all_completed"] = "Toutes les tâches de sauvegarde sont terminées.",
        ["progress"] = "  Progression : {0:F1}% ({1}/{2} fichiers)",

        // General
        ["invalid_input"] = "Saisie invalide. Veuillez réessayer.",
        ["press_key"] = "Appuyez sur une touche pour continuer..."
    };
}
