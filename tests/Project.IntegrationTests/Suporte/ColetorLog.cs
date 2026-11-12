using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Project.IntegrationTests.Suporte;

public sealed class ColetorLog : ILoggerProvider, ILogger
{
    private readonly ConcurrentQueue<string> _linhas = new();

    public IReadOnlyCollection<string> Linhas => _linhas.ToArray();

    public ILogger CreateLogger(string categoryName) => this;

    public void Dispose()
    {
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _linhas.Enqueue(formatter(state, exception));
    }
}
