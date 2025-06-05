using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Obscura_Live.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<IGameSettingsService, GameSettingsService>();

await builder.Build().RunAsync();
