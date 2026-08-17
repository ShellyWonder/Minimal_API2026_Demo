using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThePlatoProject.Client;
using ThePlatoProject.Client.ClientProgramExtensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//Register UI components and utilities including mudblazor & http client
builder.Services.AddUiAndUtilities(builder.HostEnvironment.BaseAddress); 

builder.Services.AddCustomClientServices();
await builder.Build().RunAsync();
