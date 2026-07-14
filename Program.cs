
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomSwagger();
//data base connection
var connectionString = DataUtility.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapHomeEndpoints(); // Map the Home endpoints
app.Run();

