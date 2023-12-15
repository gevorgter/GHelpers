using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddHostedService<App>()
    .AddSingleton<AWSFileSystem.AWSFileSystem>();

using IHost host = builder.Build();
await host.RunAsync();