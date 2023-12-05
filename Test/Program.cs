using GHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddHostedService<App>()
    .UseDIHelper(typeof(App));


using IHost host = builder.Build();
await host.RunAsync();