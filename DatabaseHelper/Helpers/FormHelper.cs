using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseHelper.Helpers
{
    public class FormHelper
    {
        public const string DIALOG_ERROR = "ErrorHost";
        public const string DIALOG_CAPTURE = "CaptureHost";
        public const string DIALOG_CLICKAWAY = "ClickAwayHost";

        public static async Task ExceptionDialogHandler(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                await DialogHost.Show(e, DIALOG_ERROR);
            }
        }

        public static async Task LoadingOriginalButton(Action action, Button button)
        {
            await LoadingButton(() => Task.Run(action), button, button.Style);
        }

        public static async Task LoadingOriginalButton(Func<Task> action, Button button)
        {
            await LoadingButton(action, button, button.Style);
        }

        public static async Task LoadingFlatDarkBgButton(Action action, Button button)
        {
            await LoadingFlatDarkBgButton(() => Task.Run(action), button);
        }

        public static async Task LoadingFlatDarkBgButton(Func<Task> action, Button button)
        {
            var loadingStyle = (System.Windows.Style)button.FindResource("MaterialDesignFlatDarkBgButton");

            if (loadingStyle == null)
            {
                loadingStyle = button.Style;
            }

            await LoadingButton(action, button, loadingStyle);
        }

        public static async Task LoadingButton(Func<Task> action, Button button, Style loadingStyle)
        {
            var originalStyle = button.Style;

            FunctionHelper.ConsumeException(() => button.Style = loadingStyle);
            FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndeterminate(button, true));
            FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndicatorVisible(button, true));

            try
            {
                await action();
            }
            finally
            {
                FunctionHelper.ConsumeException(() => button.Style = originalStyle);
                FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndeterminate(button, false));
                FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndicatorVisible(button, false));

                Window.GetWindow(button).IsEnabled = true;
            }
        }

        public static ComandProcessor GetNewCommandProcessor(string database)
        {
            return new ComandProcessor(SettingsHelper.GetSQLConnectionDetails(database));
        }
    }
}