var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/Welcome", () => {
    var response = new
    {
        Message = "Welcome to ASP.NET Core 10.0 Minimal API Demonstration!",
        Version = "1.0.0" ,
        TimeOnly = DateTime.Now.ToString("T")
    };
    return Results.Ok(response);

    }).WithName("WelcomeMessage");
app.Run();

