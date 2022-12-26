#region References

using System;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

#endregion

namespace TypingPracticeApp.Domain
{
    public static class ThemeHelper
    {
        private static readonly PaletteHelper PaletteHelper = new PaletteHelper();

        public static void ApplyBase(bool isDark)
        {
            var theme = ThemeHelper.PaletteHelper.GetTheme();
            var baseThem = isDark ? (IBaseTheme)new MaterialDesignDarkTheme() : new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseThem);
            ThemeHelper.PaletteHelper.SetTheme(theme);
        }

        [Obsolete("未サポート", true)]
        public static void ApplyPrimary(Swatch swatch) => throw new NotSupportedException();

        [Obsolete("未サポート", true)]
        public static void ApplyAccent(Swatch swatch) => throw new NotSupportedException();
    }
}
