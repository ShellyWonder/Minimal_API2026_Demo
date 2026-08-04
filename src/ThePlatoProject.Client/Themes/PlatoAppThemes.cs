using MudBlazor;

namespace ThePlatoProject.Client.Themes
{
    /// <summary>
    /// Application-wide MudBlazor themes derived from the Plato public site's
    /// dark, technical, cyan-accented visual language.
    /// </summary>
    public static class PlatoAppThemes
    {
        // Core brand tokens from sites-map.css.
        private const string PlatoCyan = "#00E0FF";
        private const string PlatoCyanDim = "#00B8CC";
        private const string PlatoBackground = "#0D0D0D";
        private const string PlatoMapBackground = "#0A0A0A";
        private const string PlatoSurface = "#121212";
        private const string PlatoText = "#E0E0E0";
        private const string PlatoTextSecondary = "#BDBDBD";
        private const string PlatoBorder = "#222222";

        /// <summary>
        /// Light operational theme. The cyan is darkened where necessary so that
        /// links, labels, and controls remain readable on light surfaces.
        /// </summary>
        public static readonly MudTheme PaletteLightTheme = new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#006F7E",
                Secondary = "#008FA3",
                Tertiary = "#245E68",

                Success = "#267A53",
                Info = "#126A8A",
                Warning = "#A85E00",
                Error = "#B3263E",
                Dark = PlatoBackground,

                Background = "#F3F7F8",
                Surface = "#FFFFFF",
                AppbarBackground = PlatoBackground,
                DrawerBackground = "#E9F1F2",

                TextPrimary = "#172024",
                TextSecondary = "#526166",
                LinesDefault = "#CDD9DC"
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "4px"
            },
            Typography = GetPlatoTypography()
        };

        /// <summary>
        /// Primary Plato theme. This is the closest MudBlazor translation of the
        /// existing public map, authentication, modal, and artifact-card styling.
        /// </summary>
        public static readonly MudTheme PaletteDarkTheme = new()
        {
            PaletteDark = new PaletteDark
            {
                Primary = PlatoCyan,
                Secondary = PlatoCyanDim,
                Tertiary = "#6AF0FF",

                Success = "#5ED39B",
                Info = "#66BFFF",
                Warning = "#FFB454",
                Error = "#FF6473",
                Dark = PlatoMapBackground,

                Background = PlatoBackground,
                Surface = PlatoSurface,
                AppbarBackground = PlatoBackground,
                DrawerBackground = PlatoSurface,

                TextPrimary = PlatoText,
                TextSecondary = PlatoTextSecondary,
                LinesDefault = PlatoBorder
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "4px"
            },
            Typography = GetPlatoTypography()
        };

        /// <summary>
        /// The public site establishes Source Code Pro as the application voice.
        /// The hierarchy remains compact and utilitarian rather than editorial.
        /// </summary>
        private static Typography GetPlatoTypography()
        {
            string[] fontFamily = ["source-code-pro", "Consolas", "monospace"];

            return new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.95rem",
                    LineHeight = "1.45"
                },
                H1 = new H1Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "2.25rem",
                    FontWeight = "700"
                },
                H2 = new H2Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "1.85rem",
                    FontWeight = "700"
                },
                H3 = new H3Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "1.55rem",
                    FontWeight = "600"
                },
                H4 = new H4Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "1.30rem",
                    FontWeight = "600"
                },
                H5 = new H5Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "1.10rem",
                    FontWeight = "600"
                },
                H6 = new H6Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.95rem",
                    FontWeight = "600"
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.95rem",
                    FontWeight = "600"
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.85rem",
                    FontWeight = "600"
                },
                Body1 = new Body1Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.95rem",
                    FontWeight = "400"
                },
                Body2 = new Body2Typography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.875rem",
                    FontWeight = "400"
                },
                Button = new ButtonTypography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.875rem",
                    FontWeight = "600"
                },
                Caption = new CaptionTypography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.78rem",
                    FontWeight = "400"
                },
                Overline = new OverlineTypography
                {
                    FontFamily = fontFamily,
                    FontSize = "0.75rem",
                    FontWeight = "600",
                    TextTransform = "uppercase"
                }
            };
        }
    }
}
