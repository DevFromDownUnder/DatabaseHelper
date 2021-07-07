using MaterialDesignThemes.Wpf;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DatabaseHelper.Helpers
{
    public class FormHelper
    {
        public const string DIALOG_ERROR = "ErrorHost";
        public const string DIALOG_CAPTURE_STRECHED = "CaptureStrechedHost";
        public const string DIALOG_CAPTURE_CENTERED = "CaptureCenteredHost";
        public const string DIALOG_CLICKAWAY_STRECHED = "ClickAwayStrechedHost";
        public const string DIALOG_CLICKAWAY_CENTERED = "ClickAwayCenteredHost";

        public static async Task ExceptionDialogHandler(Task action)
        {
            try
            {
                await action;
            }
            catch (Exception e)
            {
                await DialogHost.Show(e, DIALOG_ERROR);
            }
        }

        #region Custom Open Dialog

        public static RoutedCommand OpenCaptureStrechedCommand { get; } = new(DIALOG_CAPTURE_STRECHED, typeof(FormHelper));
        public static RoutedCommand OpenCaptureCenteredCommand { get; } = new(DIALOG_CAPTURE_CENTERED, typeof(FormHelper));
        public static RoutedCommand OpenClickAwayStrechedCommand { get; } = new(DIALOG_CLICKAWAY_STRECHED, typeof(FormHelper));
        public static RoutedCommand OpenClickAwayCenteredCommand { get; } = new(DIALOG_CLICKAWAY_CENTERED, typeof(FormHelper));

        #endregion Custom Open Dialog

        public static async Task LoadingOriginalButton(Action action, Button button)
        {
            await LoadingButton(Task.Run(action), button, button.Style);
        }

        public static async Task LoadingOriginalButton(Task action, Button button)
        {
            await LoadingButton(action, button, button.Style);
        }

        public static async Task LoadingFlatDarkBgButton(Action action, Button button)
        {
            await LoadingFlatDarkBgButton(Task.Run(action), button);
        }

        public static async Task LoadingFlatDarkBgButton(Task action, Button button)
        {
            var loadingStyle = (System.Windows.Style)button.FindResource("MaterialDesignFlatDarkBgButton");

            if (loadingStyle == null)
            {
                loadingStyle = button.Style;
            }

            await LoadingButton(action, button, loadingStyle);
        }

        public static async Task LoadingButton(Task action, Button button, Style loadingStyle)
        {
            var originalStyle = button.Style;

            FunctionHelper.ConsumeException(() => button.Style = loadingStyle);
            FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndeterminate(button, true));
            FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndicatorVisible(button, true));

            try
            {
                await action;
            }
            finally
            {
                FunctionHelper.ConsumeException(() => button.Style = originalStyle);
                FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndeterminate(button, false));
                FunctionHelper.ConsumeException(() => ButtonProgressAssist.SetIsIndicatorVisible(button, false));

                Window.GetWindow(button).IsEnabled = true;
            }
        }

        public static ComandProcessor GetNewCommandProcessor(string database = SQLHelper.MASTER_DB)
        {
            return new ComandProcessor(SettingsHelper.GetSQLConnectionDetails(database));
        }
    }
}