using MinimalAPI2026Demo.Extensions.ProgramExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomServices(); //// Register domain services
builder.Services.AddCustomSwagger();
builder.Services.AddPersistence(builder.Configuration); // Register database context and connection
builder.Services.AddIdentityAndAuthentication(); // Register Identity and Authentication services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//seed the database with initial data from the DataUtility class
using (var scope = app.Services.CreateScope())
{
    await DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UsePlatoProjectPipeline(); //middleware pipeline configuration
app.MapPlatoProjectEndpoints(); //map endpoints 
app.Run();

