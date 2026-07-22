
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomSwagger();
//data base connection
var connectionString = DataUtility.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
// Add Identity services
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//Admin policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
builder.Services.AddTransient<IEmailSender, ConsoleEmailService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IArtifactService , ArtifactService>();
builder.Services.AddScoped<IArtifactMediaFileService, ArtifactMediaFileService>();
//enable validation
builder.Services.AddValidation();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    await DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlockIdentityEndpoints>();

var authRouteGroup = app.MapGroup("api/auth").WithTags("Admin");
authRouteGroup.MapIdentityApi<ApplicationUser>();
app.MapCustomIdentityEndpoints(); // Map custom identity endpoints
app.MapHomeEndpoints(); // Map Home endpoints
app.MapSiteEndpoints(); // Map Site endpoints
app.MapArtifactEndpoints(); // Map Artifact endpoints
app.MapArtifactMediaEndpoints(); //Map Artifact Image endpoints
app.Run();

