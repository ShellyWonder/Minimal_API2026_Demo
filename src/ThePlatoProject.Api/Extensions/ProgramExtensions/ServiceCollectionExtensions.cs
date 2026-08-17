namespace MinimalAPI2026Demo.Extensions.ProgramExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, ConsoleEmailService>();
            services.AddScoped<ISiteService, SiteService>();
            services.AddScoped<IArtifactService, ArtifactService>();
            services.AddScoped<IArtifactMediaFileService, ArtifactMediaFileService>();
            services.AddScoped<ICatalogRecordsService, CatalogRecordsService>();
            services.AddScoped<ICurrentEmployeeService, CurrentEmployeeService>();
            services.AddScoped<IPlatoAuthorizationService, PlatoAuthorizationService>();
            return services;
        }
    }
}
