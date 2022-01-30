using System.Data.Common;
using System.Reflection;

namespace GrpcService;

public class DatabaseUpgradeStartupFilter : IStartupFilter
{
    private readonly ILogger logger;
    private readonly IConfiguration configuration;
    private readonly IConnection connection;

    /// <summary>
    /// Creates a new instance of the <see cref="DatabaseUpgradeStartupFilter" /> class.
    /// </summary>
    /// <param name="logger"> logger engine. </param>
    /// <param name="configuration"> Configuration used to get connection string. </param>
    public DatabaseUpgradeStartupFilter(ILogger<DatabaseUpgradeStartupFilter> logger, IConfiguration configuration, IConnection connection)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.connection = connection;
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
            using DbConnection? dbConnection = connection.GetConnection();
            Assembly? assembly = Assembly.GetExecutingAssembly();

            Evolve.Evolve? evolve = new Evolve.Evolve(dbConnection, msg => logger.LogInformation("{Message}", msg))
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

public interface IConnection : IDisposable
{
    DbConnection GetConnection();
}