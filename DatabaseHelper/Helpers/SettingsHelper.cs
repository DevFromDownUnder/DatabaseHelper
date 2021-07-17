using DatabaseHelper.Contracts;
using Swordfish.NET.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DatabaseHelper.Helpers
{
    public class SettingsHelper
    {
        public const string DEFAULT_SETTINGS_PATH = "settings.json";
        private static UserSettings settings = GetDefaultSettings();

        public static UserSettings Settings { get => settings; }

        public static async Task ExportUserSettings(UserSettings value, string path)
        {
            await File.WriteAllTextAsync(path, FormatUserSettings(value)).ConfigureAwait(false);
        }

        public static string FormatUserSettings(UserSettings value)
        {
            return JsonSerializer.Serialize(value, new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
        }

        public static string FormatUserSettingsJson(string json)
        {
            var value = ParseUserSettingsJson(json);

            return FormatUserSettings(value);
        }

        public static UserSettings GetDefaultSettings()
        {
            return new()
            {
                Theme_IsDarkTheme = true,
                Theme_PrimaryColor = Color.FromArgb(255, 33, 150, 243),
                Theme_SecondaryColor = Color.FromArgb(255, 1, 66, 96),
                Server_Servers = new ObservableCollection<SQLServer>(),
                Server_PreferredServer = new(),
                Server_CurrentServer = new(),
                Server_Domain = "",
                Server_Username = "",
                Server_Password = "",
                Server_UseActiveDirectory = false,
                Server_UseIntegratedSecurity = false,
                Server_UseSQLPassword = true,
                Server_ConnectTimeout = 10,
                Server_CommandTimeout = 300,
                BackupDatabase_BackupFolder = "",
                BackupDatabase_BackupNameFormat = "",
                BackupDatabase_BackupDataFilenameFormat = "",
                BackupDatabase_AddTimestamp = true,
                BackupDatabase_CopyOnly = true,
                BackupDatabase_Compress = true,
                RestoreDatabase_DefaultBackupFolder = "",
                CreateSnapshot_SnapshotFolder = "",
                CreateSnapshot_SnapshotNameFormat = "",
                CreateSnapshot_SnapshotDataFilenameFormat = "",
                CreateSnapshot_AddTimestamp = true,
                ExecuteFile_LastFilename = "",
                ExecuteSavedFiles_Files = new ConcurrentObservableDictionary<string, string>(),
                ExecuteQuery_LastQuery = "",
                ExecuteSavedQueries_Queries = new ConcurrentObservableDictionary<string, string>()
            };
        }

        public static string GetSettingsJson()
        {
            return FormatUserSettings(Settings);
        }

        public static SQLConnectionDetails GetSQLConnectionDetails(string database = SQLHelper.MASTER_DB)
        {
            return new SQLConnectionDetails
            {
                Database = database,
                Domain = Settings.Server_Domain,
                Password = Settings.Server_Password,
                Port = Settings.Server_CurrentServer.Port,
                Server = Settings.Server_CurrentServer.Server,
                Username = Settings.Server_Username,
                UseActiveDirectory = Settings.Server_UseActiveDirectory,
                UseIntegratedSecurity = Settings.Server_UseIntegratedSecurity,
                UseSQLPassword = Settings.Server_UseSQLPassword,
                ConnectTimeout = Settings.Server_ConnectTimeout,
                CommandTimeout = Settings.Server_CommandTimeout
            };
        }

        public static async Task<bool> LoadSettings(bool track = true, string path = DEFAULT_SETTINGS_PATH)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            var settings = await File.ReadAllTextAsync(path).ConfigureAwait(false);

            UpdateSettings(ParseUserSettingsJson(settings), track);

            return true;
        }

        public static UserSettings ParseUserSettingsJson(string json)
        {
            return JsonSerializer.Deserialize<UserSettings>(json);
        }

        public static async Task SaveSettings(bool track = true, string path = DEFAULT_SETTINGS_PATH)
        {
            await File.WriteAllTextAsync(path, GetSettingsJson());

            if (track)
            {
                Settings.IsChanged = false;
            }
        }

        public static void ShallowCopy(UserSettings source, UserSettings destination, bool track)
        {
            if (source == null || destination == null)
            {
                return;
            }

            //Hand written copy, to avoid WPF unbinding issues setting normal assingment
            destination.Theme_IsDarkTheme = source.Theme_IsDarkTheme;
            destination.Theme_PrimaryColor = source.Theme_PrimaryColor;
            destination.Theme_SecondaryColor = source.Theme_SecondaryColor;
            destination.Server_Servers = source.Server_Servers;
            destination.Server_PreferredServer = source.Server_PreferredServer;
            destination.Server_CurrentServer = source.Server_CurrentServer;
            destination.Server_Domain = source.Server_Domain;
            destination.Server_Username = source.Server_Username;
            destination.Server_Password = source.Server_Password;
            destination.Server_UseActiveDirectory = source.Server_UseActiveDirectory;
            destination.Server_UseIntegratedSecurity = source.Server_UseIntegratedSecurity;
            destination.Server_UseSQLPassword = source.Server_UseSQLPassword;
            destination.Server_ConnectTimeout = source.Server_ConnectTimeout;
            destination.Server_CommandTimeout = source.Server_CommandTimeout;
            destination.BackupDatabase_BackupFolder = source.BackupDatabase_BackupFolder;
            destination.BackupDatabase_BackupNameFormat = source.BackupDatabase_BackupNameFormat;
            destination.BackupDatabase_BackupDataFilenameFormat = source.BackupDatabase_BackupDataFilenameFormat;
            destination.BackupDatabase_AddTimestamp = source.BackupDatabase_AddTimestamp;
            destination.BackupDatabase_CopyOnly = source.BackupDatabase_CopyOnly;
            destination.BackupDatabase_Compress = source.BackupDatabase_Compress;
            destination.RestoreDatabase_DefaultBackupFolder = source.RestoreDatabase_DefaultBackupFolder;
            destination.CreateSnapshot_SnapshotFolder = source.CreateSnapshot_SnapshotFolder;
            destination.CreateSnapshot_SnapshotNameFormat = source.CreateSnapshot_SnapshotNameFormat;
            destination.CreateSnapshot_SnapshotDataFilenameFormat = source.CreateSnapshot_SnapshotDataFilenameFormat;
            destination.CreateSnapshot_AddTimestamp = source.CreateSnapshot_AddTimestamp;
            destination.ExecuteFile_LastFilename = source.ExecuteFile_LastFilename;
            destination.ExecuteSavedFiles_Files = source.ExecuteSavedFiles_Files;
            destination.ExecuteQuery_LastQuery = source.ExecuteQuery_LastQuery;
            destination.ExecuteSavedQueries_Queries = source.ExecuteSavedQueries_Queries;

            if (!track)
            {
                destination.IsChanged = false;
            }
        }

        public static void UpdateConnectionDetails(UserSettings settings)
        {
            if (!settings.Server_UseActiveDirectory) settings.Server_Domain = string.Empty;
            if (settings.Server_UseIntegratedSecurity) settings.Server_Username = string.Empty;
            if (settings.Server_UseIntegratedSecurity) settings.Server_Password = string.Empty;
        }

        public static void UpdateSettings(UserSettings value)
        {
            UpdateSettings(value, true);
        }

        public static void UpdateSettings(UserSettings value, bool track)
        {
            if (value == null)
            {
                return;
            }

            ShallowCopy(value, Settings, track);

            ThemeHelper.RefreshTheme();
        }
    }
}