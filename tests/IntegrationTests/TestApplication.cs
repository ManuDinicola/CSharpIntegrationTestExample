using GrpcService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace IntegrationTests;

internal partial class TestApplication : WebApplicationFactory<Program>
{
    private readonly ITestOutputHelper testOutputHelper;

    public TestApplication(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        LoggerFactory? x = new LoggerFactory();
        x.AddProvider(new XUnitLoggerProvider(testOutputHelper));

        builder.ConfigureLogging(options => options.AddProvider(new XUnitLoggerProvider(testOutputHelper)));

        _ = builder.ConfigureServices(services =>
        {
            //services.RemoveAll<IStartupFilter>();
            //services.AddSingleton<IStartupFilter, TestDatabaseUpgradeStartupFilter>();
            services.AddSingleton<IConnection, TestCustomConnection>();
            services.Replace(ServiceDescriptor.Scoped<GrpcService.ITest, TestImplementTest>());
        });

        return base.CreateHost(builder);
    }
}