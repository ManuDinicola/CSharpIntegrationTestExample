using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit.Abstractions;

namespace IntegrationTests;

internal class XUnitLogger : ILogger
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly string categoryName;
    private readonly LoggerExternalScopeProvider scopeProvider;

    public static ILogger CreateLogger(ITestOutputHelper testOutputHelper)
    {
        return new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), "");
    }

    public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper)
    {
        return new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());
    }

    public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string categoryName)
    {
        this.testOutputHelper = testOutputHelper;
        this.scopeProvider = scopeProvider;
        this.categoryName = categoryName;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return scopeProvider.Push(state);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        StringBuilder? sb = new StringBuilder();
        _ = sb.Append(GetLogLevelString(logLevel))
          .Append(" [").Append(categoryName).Append("] ")
          .Append(formatter(state, exception));

        if (exception != null)
        {
            _ = sb.Append('\n').Append(exception);
        }

        // Append scopes
        scopeProvider.ForEachScope((scope, state) =>
        {
            _ = state.Append("\n => ");
            _ = state.Append(scope);
        }, sb);

        testOutputHelper.WriteLine(sb.ToString());
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}

[ExcludeFromCodeCoverage]
internal sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
{
    public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider)
        : base(testOutputHelper, scopeProvider, typeof(T).FullName)
    {
    }
}