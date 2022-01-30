using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace IntegrationTests;

[ExcludeFromCodeCoverage]
internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly LoggerExternalScopeProvider scopeProvider = new();

    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(testOutputHelper, scopeProvider, categoryName);
    }

    public void Dispose()
    {
        // Method intentionally left empty.
    }
}