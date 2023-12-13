using GHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);


AttributeMap[] attributeMap = new AttributeMap[] {
    DIHelper.diHelperMapper,
    DIHelper.useHelperMapper,
};

builder.Services
    .AddHostedService<App>()
    .AddAttributeDefinedServices(typeof(App), attributeMap);
using IHost host = builder.Build();
await host.RunAsync();