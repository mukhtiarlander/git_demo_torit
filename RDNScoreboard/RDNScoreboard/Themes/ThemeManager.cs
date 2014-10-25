﻿namespace RDNScoreboard.Themes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    public static class ThemeManager
    {
        public static ResourceDictionary GetThemeResourceDictionary(string theme)
        {
            if (theme != null)
            {
                string packUri = String.Format(@"/RDNScoreboard;component/Themes/{0}/Theme.xaml", theme);
                return Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
            }
            return null;
        }

        public static string[] GetThemes()
        {
            string[] themes = new string[] 
            { 
                "ExpressionDark", "ExpressionLight", 
                //"RainierOrange", "RainierPurple", "RainierRadialBlue", 
                "ShinyBlue", "ShinyRed", 
                //"ShinyDarkTeal", "ShinyDarkGreen", "ShinyDarkPurple",
                "DavesGlossyControls", 
                "WhistlerBlue", 
                "BureauBlack", "BureauBlue", 
                "BubbleCreme", 
                "TwilightBlue",
                "UXMusingsRed", "UXMusingsGreen", 
                //"UXMusingsRoughRed", "UXMusingsRoughGreen", 
                "UXMusingsBubblyBlue"
            };
            return themes;
        }

        public static List<Theme> GetSkaterSlideshowThemes()
        {
            List<Theme> themes = new List<Theme>();
            themes.Add(new Theme("CRGBlackScoreboard", ThemeEnum.SkatersLineUp));
            themes.Add(new Theme("CRGPurpScoreboard", ThemeEnum.SkatersLineUp));
            return themes;
        }

        public static List<Theme> GetCRGThemesScoreboard()
        {
            List<Theme> themes = new List<Theme>();
            themes.Add(new Theme("CRGScoreboard", ThemeEnum.CRGScoreboard));
            themes.Add(new Theme("CRGBlackScoreboard", ThemeEnum.CRGScoreboard));
            themes.Add(new Theme("CRGPurpScoreboard", ThemeEnum.CRGScoreboard));
            return themes;
        }

        public static List<Theme> GetTournamentThemesScoreboard()
        {
            List<Theme> themes = new List<Theme>();
            themes.Add(new Theme("DerbyInkScoreboard", ThemeEnum.TournamentScoreboard));
            themes.Add(new Theme("MensWorldCupScoreboard", ThemeEnum.TournamentScoreboard));
            return themes;
        }
        public static List<Theme> GetDefaultThemesScoreboard()
        {
            List<Theme> themes = new List<Theme>();
            themes.Add(new Theme("RDNDefaultScoreboard", ThemeEnum.DefaultScoreboard));

            return themes;
        }
        public static List<Theme> GetAllScoreboardThemes()
        {
            List<Theme> themes = new List<Theme>();
            themes.AddRange(GetDefaultThemesScoreboard());
            themes.AddRange(GetCRGThemesScoreboard());
            themes.AddRange(GetTournamentThemesScoreboard());
            return themes;
        }

        public static void ApplyTheme(this Application app, string theme)
        {
            ResourceDictionary dictionary = ThemeManager.GetThemeResourceDictionary(theme);

            if (dictionary != null)
            {
                app.Resources.MergedDictionaries.Clear();
                app.Resources.MergedDictionaries.Add(dictionary);
            }
        }

        public static void ClearThemes(this Window window)
        {
            window.Resources.MergedDictionaries.Clear();
        }

        public static void ApplyTheme(this Window window, string theme)
        {
            ResourceDictionary dictionary = ThemeManager.GetThemeResourceDictionary(theme);

            if (dictionary != null)
            {
                window.Resources.MergedDictionaries.Clear();
                window.Resources.MergedDictionaries.Add(dictionary);
            }
        }

        public static void ApplyTheme(this ContentControl control, string theme)
        {
            ResourceDictionary dictionary = ThemeManager.GetThemeResourceDictionary(theme);

            if (dictionary != null)
            {
                control.Resources.MergedDictionaries.Clear();
                control.Resources.MergedDictionaries.Add(dictionary);
            }
        }

        #region Theme

        /// <summary>
        /// Theme Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.RegisterAttached("Theme", typeof(string), typeof(ThemeManager),
                new FrameworkPropertyMetadata((string)string.Empty,
                    new PropertyChangedCallback(OnThemeChanged)));

        /// <summary>
        /// Gets the Theme property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static string GetTheme(DependencyObject d)
        {
            return (string)d.GetValue(ThemeProperty);
        }

        /// <summary>
        /// Sets the Theme property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetTheme(DependencyObject d, string value)
        {
            d.SetValue(ThemeProperty, value);
        }

        /// <summary>
        /// Handles changes to the Theme property.
        /// </summary>
        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string theme = e.NewValue as string;
            if (theme == string.Empty)
                return;

            ContentControl control = d as ContentControl;
            if (control != null)
            {
                control.ApplyTheme(theme);
            }
        }

        #endregion



    }
}