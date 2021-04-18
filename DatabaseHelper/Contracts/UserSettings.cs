using PropertyChanged;
using Swordfish.NET.Collections;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DatabaseHelper.Contracts
{
    [AddINotifyPropertyChangedInterface]
    public class UserSettings
    {
        #region Theme Event Handler Workarounds

        public event EventHandler<bool> Theme_IsDarkTheme_Changed;

        public event EventHandler<Color> Theme_PrimaryColor_Changed;

        public event EventHandler<Color> Theme_SecondaryColor_Changed;

        public void OnTheme_IsDarkThemeChanged() => Theme_IsDarkTheme_Changed?.Invoke(this, Theme_IsDarkTheme);

        public void OnTheme_PrimaryColorChanged() => Theme_PrimaryColor_Changed?.Invoke(this, Theme_PrimaryColor);

        public void OnTheme_SecondaryColorChanged() => Theme_SecondaryColor_Changed?.Invoke(this, Theme_SecondaryColor);

        #endregion Theme Event Handler Workarounds

        #region Theme Settings

        public bool Theme_IsDarkTheme { get; set; }
        public Color Theme_PrimaryColor { get; set; }
        public Color Theme_SecondaryColor { get; set; }

        #endregion Theme Settings

        #region Server Settings

        public string Server_CurrentServer { get; set; }
        public ObservableCollection<string> Server_Servers { get; set; }
        public string Server_PreferredServer { get; set; }
        public string Server_Domain { get; set; }
        public string Server_Username { get; set; }
        public string Server_Password { get; set; }
        public bool Server_UseSQLPassword { get; set; }
        public bool Server_UseActiveDirectory { get; set; }
        public bool Server_UseIntegratedSecurity { get; set; }

        #endregion Server Settings

        #region Backup Database Settings

        public string BackupDatabase_BackupFolder { get; set; }
        public string BackupDatabase_BackupNameFormat { get; set; }
        public string BackupDatabase_BackupDataFilenameFormat { get; set; }
        public bool BackupDatabase_CopyOnly { get; set; }
        public bool BackupDatabase_Compress { get; set; }

        #endregion Backup Database Settings

        #region Restore Database Settings

        public string RestoreDatabase_DefaultBackupFolder { get; set; }

        #endregion Restore Database Settings

        #region Create Snapshot Settings

        public string CreateSnapshot_SnapshotFolder { get; set; }
        public string CreateSnapshot_SnapshotNameFormat { get; set; }
        public string CreateSnapshot_SnapshotDataFilenameFormat { get; set; }

        #endregion Create Snapshot Settings

        #region Execute File Settings

        public string ExecuteFile_LastFilename { get; set; }

        #endregion Execute File Settings

        #region Execute Saved Files Settings

        public ConcurrentObservableDictionary<string, string> ExecuteSavedFiles_Files { get; set; }

        #endregion Execute Saved Files Settings

        #region Execute Query Settings

        public string ExecuteQuery_LastQuery { get; set; }

        #endregion Execute Query Settings

        #region Execute Saved Queries Settings

        public ConcurrentObservableDictionary<string, string> ExecuteSavedQueries_Queries { get; set; }

        #endregion Execute Saved Queries Settings
    }
}