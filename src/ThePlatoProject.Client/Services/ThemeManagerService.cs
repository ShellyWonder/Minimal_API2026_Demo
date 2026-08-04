using Blazored.LocalStorage;
using MudBlazor;
using ThePlatoProject.Client.Themes;

namespace ThePlatoProject.Client.Services
{
    public class ThemeManagerService(ILocalStorageService localStorage)
    {
        private readonly ILocalStorageService _localStorage = localStorage;
        private const string _themePreferenceKey = "user-theme";

        public MudTheme CurrentTheme { get; private set; } = PlatoAppThemes.PaletteDarkTheme;
        public bool IsDarkMode =>
            ReferenceEquals(CurrentTheme, PlatoAppThemes.PaletteDarkTheme);
        public event Action? OnThemeChanged;

        public async Task InitializeAsync()
        {
            try
            {
                var savedTheme = await _localStorage.GetItemAsync<string>(_themePreferenceKey);
                CurrentTheme = savedTheme == "light" ? PlatoAppThemes.PaletteLightTheme : PlatoAppThemes.PaletteDarkTheme;
            }
            catch
            {
                // fallback to default
                CurrentTheme = PlatoAppThemes.PaletteDarkTheme;
            }
            NotifyThemeChanged();
        }


        public async Task ToggleThemeAsync()
        {
            if (IsDarkMode)
            {
                CurrentTheme = PlatoAppThemes.PaletteLightTheme;
                await _localStorage.SetItemAsync(_themePreferenceKey, "light");
            }
            else
            {
                CurrentTheme = PlatoAppThemes.PaletteDarkTheme;
                await _localStorage.SetItemAsync(_themePreferenceKey, "dark");
            }

            NotifyThemeChanged();
        }

        private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
    }
}
