using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace IntegrationTests;

internal class TestDatabaseUpgradeStartupFilter : IStartupFilter
{
    private readonly ILogger logger;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Creates a new instance of the <see cref="TestDatabaseUpgradeStartupFilter" /> class.
    /// </summary>
    /// <param name="logger"> logger engine. </param>
    /// <param name="configuration"> Configuration used to get connection string. </param>
    public TestDatabaseUpgradeStartupFilter(ILogger<TestDatabaseUpgradeStartupFilter> logger, IConfiguration configuration)
    {
        this.logger = logger;
        this.configuration = configuration;
    }

    /// <summary>
    /// Configure
    /// </summary>
    /// <param name="next"> next step for statup. </param>
    /// <returns> next step for startup. </returns>
    /// <exception cref="Exception"> thrown after logging exception detail. </exception>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        try
        {
            string connectionString = configuration.GetConnectionString("DataSource=:memory:");
            using SqliteConnection? mySqlConnection = new SqliteConnection(connectionString);
            Assembly? assembly = Assembly.GetAssembly(typeof(Program));

            Evolve.Evolve? evolve = new Evolve.Evolve(mySqlConnection, msg => logger.LogInformation("{Message}", msg))
            {
                IsEraseDisabled = true,
                EmbeddedResourceAssemblies = new[] { assembly },
                TransactionMode = Evolve.Configuration.TransactionKind.CommitEach,
                EnableClusterMode = true
            };

            evolve.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogCritical("Database migration failed.", ex);
            throw;
        }

        return next;
    }
}