using DatabaseHelper.Contracts;
using PropertyChanged;
using Swordfish.NET.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DatabaseHelper.Helpers
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsHelper
    {
        public static UserSettings Settings { get; set; } = new UserSettings()
        {
            Theme_IsDarkTheme = true,
            Theme_PrimaryColor = Color.FromArgb(255, 33, 150, 243),
            Theme_SecondaryColor = Color.FromArgb(255, 1, 66, 96),
            Server_Servers = new ObservableCollection<string>(),
            Server_PreferredServer = "",
            Server_CurrentServer = "",
            Server_Domain = "",
            Server_Username = "",
            Server_Password = "",
            Server_UseActiveDirectory = false,
            Server_UseIntegratedSecurity = false,
            Server_UseSQLPassword = true,
            BackupDatabase_BackupFolder = "",
            BackupDatabase_BackupNameFormat = "",
            BackupDatabase_BackupDataFilenameFormat = "",
            BackupDatabase_CopyOnly = true,
            BackupDatabase_Compress = true,
            RestoreDatabase_DefaultBackupFolder = "",
            CreateSnapshot_SnapshotFolder = "",
            CreateSnapshot_SnapshotNameFormat = "",
            CreateSnapshot_SnapshotDataFilenameFormat = "",
            ExecuteFile_LastFilename = "",
            ExecuteSavedFiles_Files = new ConcurrentObservableDictionary<string, string>(),
            ExecuteQuery_LastQuery = "",
            ExecuteSavedQueries_Queries = new ConcurrentObservableDictionary<string, string>()
        };

        public static SQLConnectionDetails GetSQLConnectionDetails()
        {
            return GetSQLConnectionDetails(SQLHelper.MASTER_DB);
        }

        public static SQLConnectionDetails GetSQLConnectionDetails(string database)
        {
            return new SQLConnectionDetails
            {
                Database = database,
                Domain = Settings.Server_Domain,
                Password = Settings.Server_Password,
                Server = Settings.Server_CurrentServer,
                Username = Settings.Server_Username,
                UseActiveDirectory = Settings.Server_UseActiveDirectory,
                UseIntegratedSecurity = Settings.Server_UseIntegratedSecurity,
                UseSQLPassword = Settings.Server_UseSQLPassword
            };
        }

        public static void UpdateConnectionDetails(UserSettings settings)
        {
            if (!settings.Server_UseActiveDirectory) settings.Server_Domain = string.Empty;
            if (settings.Server_UseIntegratedSecurity) settings.Server_Username = string.Empty;
            if (settings.Server_UseIntegratedSecurity) settings.Server_Password = string.Empty;
        }
    }
}