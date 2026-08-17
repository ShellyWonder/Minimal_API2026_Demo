using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Services;
using ThePlatoProject.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace ThePlatoProject.Client.ClientProgramExtensions;

public static class UiAndUtilitiesExtensions
{
    public static IServiceCollection AddUiAndUtilities(this IServiceCollection services, string baseAddress)
    {
        services.AddBlazoredLocalStorage();
        services.AddScoped<ThemeManagerService>();

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass =
                Defaults.Classes.Position.BottomLeft;
        });

        services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        });

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();


        return services;
    }
}
