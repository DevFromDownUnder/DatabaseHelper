using DatabaseHelper.Contracts;
using PropertyChanged;

namespace DatabaseHelper.Helpers
{
    [AddINotifyPropertyChangedInterface]
    public class MenuHelper
    {
        public static MenuItem[] MenuItems { get; set; } = {
            new MenuItem {PageKey="pagBackupDatabase", Content=new Pages.pagBackupDatabase(), Icon="DatabaseExport", Title="Backup Database"},
            new MenuItem {PageKey="pagRestoreDatabase", Content=new Pages.pagRestoreDatabase(), Icon="DatabaseImport", Title="Restore Database"},
            new MenuItem {PageKey="pagDeleteDatabase", Content=new Pages.pagDeleteDatabase(), Icon="DatabaseRemove", Title="Delete Database"},
            new MenuItem {PageKey="pagCreateSnapshot", Content=new Pages.pagCreateSnapshot(), Icon="DatabasePlus", Title="Create Snapshot"},
            new MenuItem {PageKey="pagRestoreSnapshot", Content=new Pages.pagRestoreSnapshot(), Icon="DatabaseRefresh", Title="Restore Snapshot"},
            new MenuItem {PageKey="pagDeleteSnapshot", Content=new Pages.pagDeleteSnapshot(), Icon="DatabaseMinus", Title="Delete Snapshot"},
            new MenuItem {PageKey="pagExecuteFile", Content=new Pages.pagExecuteFile(), Icon="FileAlert", Title="Execute File"},
            new MenuItem {PageKey="pagExecuteSavedFiles", Content=new Pages.pagExecuteSavedFiles(), Icon="FileReplace", Title="Execute Saved Files"},
            new MenuItem {PageKey="pagExecuteQuery", Content=new Pages.pagExecuteQuery(), Icon="Script", Title="Execute Query"},
            new MenuItem {PageKey="pagExecuteSavedQueries", Content=new Pages.pagExecuteSavedQueries(), Icon="ScriptText", Title="Execute Saved Queries"},
            new MenuItem {PageKey="pagSettings", Content=new Pages.pagSettings(), Icon="Settings", Title="Settings"}
        };
    }
}