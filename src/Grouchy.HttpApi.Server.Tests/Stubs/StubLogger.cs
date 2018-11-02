using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Grouchy.HttpApi.Server.Tests
{
    public class StubLogger<T> : ILogger<T>
    {
        public IList<string> Logs { get; } = new List<string>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logs.Add(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }
    }
}