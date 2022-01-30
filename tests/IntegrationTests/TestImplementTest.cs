using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace GrpcService
{
    public class TestImplementTest : ITest
    {
        private readonly ILogger<TestImplementTest> testLogger;

        public TestImplementTest(ILogger<TestImplementTest> testLogger)
        {
            this.testLogger = testLogger;
        }

        public string Console(string message)
        {
            testLogger.LogInformation($"coucou from test {message}");

            return "Test";
        }
    }

    public class TestCustomConnection : IConnection
    {
        public void Dispose()
        {
        }

        public DbConnection GetConnection()
        {
            return new SqliteConnection("DataSource=:memory:");
        }
    }
}