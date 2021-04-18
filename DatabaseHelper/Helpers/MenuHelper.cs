using DatabaseHelper.Contracts;
using PropertyChanged;

namespace DatabaseHelper.Helpers
{
    [AddINotifyPropertyChangedInterface]
    public class MenuHelper
    {
        public static MenuItem[] MenuItems { get; set; } = {
            new MenuItem {PagePath="\\Pages\\pagBackupDatabase.xaml", Icon="DatabaseExport", Title="Backup Database"},
            new MenuItem {PagePath="\\Pages\\pagRestoreDatabase.xaml", Icon="DatabaseImport", Title="Restore Database"},
            new MenuItem {PagePath="\\Pages\\pagDeleteDatabase.xaml", Icon="DatabaseRemove", Title="Delete Database"},
            new MenuItem {PagePath="\\Pages\\pagCreateSnapshot.xaml", Icon="DatabasePlus", Title="Create Snapshot"},
            new MenuItem {PagePath="\\Pages\\pagRestoreSnapshot.xaml", Icon="DatabaseRefresh", Title="Restore Snapshot"},
            new MenuItem {PagePath="\\Pages\\pagDeleteSnapshot.xaml", Icon="DatabaseMinus", Title="Delete Snapshot"},
            new MenuItem {PagePath="\\Pages\\pagExecuteFile.xaml", Icon="FileAlert", Title="Execute File"},
            new MenuItem {PagePath="\\Pages\\pagExecuteSavedFiles.xaml", Icon="FileReplace", Title="Execute Saved Files"},
            new MenuItem {PagePath="\\Pages\\pagExecuteQuery.xaml", Icon="Script", Title="Execute Query"},
            new MenuItem {PagePath="\\Pages\\pagExecuteSavedQueries.xaml", Icon="ScriptText", Title="Execute Saved Queries"},
            new MenuItem {PagePath="\\Pages\\pagSettings.xaml", Icon="Settings", Title="Settings"}
        };
    }
}