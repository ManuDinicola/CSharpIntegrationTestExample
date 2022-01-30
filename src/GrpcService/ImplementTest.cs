using MySqlConnector;
using System.Data.Common;

namespace GrpcService;

public class ImplementTest : ITest
{
    private readonly ILogger<ImplementTest> testLogger;

    public ImplementTest(ILogger<ImplementTest> testLogger)
    {
        this.testLogger = testLogger;
    }

    public string Console(string message)
    {
        testLogger.LogInformation(message);

        return message;
    }
}

public class CustomConnection : IConnection
{
    public void Dispose()
    {
    }

    public DbConnection GetConnection()
    {
        return new MySqlConnection("");
    }
}