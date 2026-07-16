using Microsoft.OpenApi;

namespace MinimalAPI2026Demo.Extensions
{
    public static class OpenAPISwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "ASP.NET Core 10.0 Minimal API Demo",
                        Description = """
    
    <img src="/images/AeonRegistryLogoBLK.png" height="120" />
    
    ## Aeon Research Division

    Internal API for managing recovered artifacts and research data.
    Provides secure access for field researchers and analysts. 

    ### Key Features:
    - Site and Artifact Catalog
    - Research record submissions
    - Secure media storage
    - User role managment

    [Launch Public Test Site](/site/sites-map.html)

    """,
        Contact = new OpenApiContact
        {
            Name = "SJ Wonder",
            Url = new Uri("https://example.com"),
            Email = "shelly.wonder@outlook.com"
        }
                    });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer' [space] and then your valid JWT token."
                    });

                    c.AddSecurityRequirement(document =>new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                    // Hide specific endpoints from Swagger documentation
                    string[] hiddenEndpoints = [
                        "api/auth/register",
                        "api/auth/refresh",
                        "api/auth/confirmemail",
                        "api/auth/resendconfirmationemail",
                        "api/auth/forgotpassword",
                        "api/auth/resetpassword",
                        "api/auth/reinitpassword",
                        "api/auth/manage",
                        "api/auth/manage/info",
                        "api/auth/manage/2fa",

                        ];

                    c. DocInclusionPredicate((docName, apiDesc) => 
                    { 
                        var path = apiDesc.RelativePath?.ToLowerInvariant();
                        if (path == null) return false;
                        if(hiddenEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase))
                        {

                            return false; //if the path is in the hiddenEndpoints array, exclude it from Swagger documentation
                        }
                        return true;
                    });
                });
            return services;
        }
    }
}