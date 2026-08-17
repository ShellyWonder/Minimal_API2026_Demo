using Microsoft.AspNetCore.Components.Authorization;
using ThePlatoProject.Client.Authentication;
using ThePlatoProject.Client.Authorization;
using ThePlatoProject.Client.Services.Interfaces;

namespace ThePlatoProject.Client.ClientProgramExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomClientServices(this IServiceCollection services)
        {
            services.AddScoped<ApiAuthenticationStateProvider>();

            services.AddScoped<AuthenticationStateProvider>(provider =>
                provider.GetRequiredService<ApiAuthenticationStateProvider>());

            services.AddScoped<IAuthenticationService>(provider =>
                provider.GetRequiredService<ApiAuthenticationStateProvider>());

            services.AddScoped<IPlatoUiAuthorizationService, PlatoUiAuthorizationService>();
            return services;
        }
    }
}
