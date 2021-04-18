using MaterialDesignThemes.Wpf;

namespace DatabaseHelper.Helpers
{
    internal class ThemeHelper
    {
        public static void BindTheme()
        {
            SettingsHelper.Settings.Theme_IsDarkTheme_Changed += ChangeTheme;
            SettingsHelper.Settings.Theme_PrimaryColor_Changed += ChangePrimaryColor;
            SettingsHelper.Settings.Theme_SecondaryColor_Changed += ChangeSecondaryColor;
        }

        public static void ChangePrimaryColor(object sender, System.Windows.Media.Color color)
        {
            var palette = new PaletteHelper();
            var theme = palette.GetTheme();
            theme.SetPrimaryColor(color);
            palette.SetTheme(theme);
        }

        public static void ChangeSecondaryColor(object sender, System.Windows.Media.Color color)
        {
            var palette = new PaletteHelper();
            var theme = palette.GetTheme();
            theme.SetSecondaryColor(color);
            palette.SetTheme(theme);
        }

        public static void ChangeTheme(object sender, bool isDarkTheme)
        {
            var palette = new PaletteHelper();
            var theme = palette.GetTheme();

            if (isDarkTheme)
            {
                theme.SetBaseTheme((IBaseTheme)new MaterialDesignDarkTheme());
            }
            else
            {
                theme.SetBaseTheme((IBaseTheme)new MaterialDesignLightTheme());
            }

            palette.SetTheme(theme);
        }
    }
}