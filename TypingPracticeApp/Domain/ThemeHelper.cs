#region References

using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

#endregion

namespace TypingPracticeApp.Domain
{
    public static class ThemeHelper
    {
        public static void ApplyBase(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        }

        public static void ApplyPrimary(Swatch swatch)
        {
            new PaletteHelper().ReplacePrimaryColor(swatch);
        }

        public static void ApplyAccent(Swatch swatch)
        {
            new PaletteHelper().ReplaceAccentColor(swatch);
        }
    }
}
