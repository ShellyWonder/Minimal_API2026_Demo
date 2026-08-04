using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Services;
using ThePlatoProject.Client.Services;

namespace ThePlatoProject.Client.Infrastructure
{
    public static class UiAndUtilitiesExtensions
    {
        public static IServiceCollection AddUiAndUtilities(this IServiceCollection services, string baseAddress)
        {
            // Register UI components and utilities here
            services.AddBlazoredLocalStorage();
            services.AddScoped<ThemeManagerService>();
            //services.AddScoped<IAppAuthorizationService, AppAuthorizationService>();

            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            });

            services.AddScoped(_ => new HttpClient{ BaseAddress = new Uri(baseAddress)});


            return services;
        }

    }
}
