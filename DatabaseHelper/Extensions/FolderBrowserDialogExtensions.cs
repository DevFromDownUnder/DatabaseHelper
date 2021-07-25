using FolderBrowserEx;

namespace DatabaseHelper.Extensions
{
    public static class FolderBrowserDialogExtensions
    {
        public static bool ShowDialogB(this FolderBrowserDialog dialog)
        {
            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
        }
    }
}