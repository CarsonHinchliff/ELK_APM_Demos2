using Elastic.Apm.Api;
using Elastic.Apm;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension
{
    internal class ApmDiagnosticObserver : IObserver<DiagnosticListener>,
     IObserver<KeyValuePair<string, object?>>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        private ITransaction GetTransaction()
        {
            return Agent.Tracer.CurrentTransaction;
        }

        private ISpan GetSpan()
        {
            return Agent.Tracer.CurrentSpan;
        }

        public void OnNext(KeyValuePair<string, object?> value)
        {
            Write(value.Key, value.Value);
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "SqlClientDiagnosticListener")
            {
                var subscription = value.Subscribe(this, IsEnabled);
                _subscriptions.Add(subscription);
            }
        }

        private bool IsEnabled(string name)
        {
            return name == "System.Data.SqlClient.WriteCommandBefore"
              || name == "System.Data.SqlClient.WriteCommandAfter"
              || name == "System.Data.SqlClient.WriteCommandError";
        }

        private readonly AsyncLocal<Stopwatch> _stopwatch = new AsyncLocal<Stopwatch>();

        private void Write(string name, object? value)
        {
            switch (name)
            {
                case "System.Data.SqlClient.WriteCommandBefore":
                    {
                        _stopwatch.Value = Stopwatch.StartNew();
                        break;
                    }
                case "System.Data.SqlClient.WriteCommandAfter":
                case "System.Data.SqlClient.WriteCommandError":
                    {
                        var stopwatch = _stopwatch.Value;
                        stopwatch.Stop();
                        var command = GetProperty<SqlCommand>(value, "Command");
                        Console.WriteLine($"CommandText: {command.CommandText}");
                        Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
                        Console.WriteLine();
                        command?.AppendToSpan();
                        break;
                    }
            }
        }

        private static T GetProperty<T>(object value, string name)
        {
            return (T)value.GetType()
                  .GetProperty(name)
                  .GetValue(value);
        }
    }
}
