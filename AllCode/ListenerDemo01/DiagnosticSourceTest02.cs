using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapr;
using System.Data.SqlClient;

namespace ListenerDemo01.DiagnosticSourceTest02
{
    public sealed class ExampleDiagnosticObserver : IObserver<DiagnosticListener>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {
            Console.WriteLine(value.Name);
        }
    }

    public class ExampleDiagnosticObserver1 : IObserver<DiagnosticListener>,
     IObserver<KeyValuePair<string, object>>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
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
              || name == "System.Data.SqlClient.WriteCommandAfter";
        }

        private readonly AsyncLocal<Stopwatch> _stopwatch = new AsyncLocal<Stopwatch>();

        private void Write(string name, object value)
        {
            switch (name)
            {
                case "System.Data.SqlClient.WriteCommandBefore":
                    {
                        _stopwatch.Value = Stopwatch.StartNew();
                        break;
                    }
                case "System.Data.SqlClient.WriteCommandAfter":
                    {
                        var stopwatch = _stopwatch.Value;
                        stopwatch.Stop();
                        var command = GetProperty<SqlCommand>(value, "Command");
                        Console.WriteLine($"CommandText: {command.CommandText}");
                        Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
                        Console.WriteLine();
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



    internal class DiagnosticSourceTest02
    {
        public const string ConnectionString = @"Data Source=.;Initial Catalog=sqlsugar_db;User ID=sa;Password=sa1234";
        public static void Start()
        {
            IDisposable subscription = DiagnosticListener.AllListeners.Subscribe(new ExampleDiagnosticObserver());
            IDisposable subscription1 = DiagnosticListener.AllListeners.Subscribe(new ExampleDiagnosticObserver1());
            Get();
        }

        public static void Get()
        {
            // 创建一个 SqlConnection
            var connectionString = "Data Source=.;Initial Catalog=sqlsugar_db;User ID=sa;Password=sa1234";
            using (var connection = new SqlConnection(connectionString))
            {

                // 创建一个 DbCommand
                var command = connection.CreateCommand();
                command.CommandText = "dbo.[sp_school]";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@name", "c");
                //var retParam = new SqlParameter { Direction = System.Data.ParameterDirection.Output, 
                //    ParameterName = "@age", SqlDbType = System.Data.SqlDbType.Int };
                //command.Parameters.Add(retParam);
                command.Parameters.Add("@age", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;

                connection.Open();
                // 执行命令
                var reader = command.ExecuteReader();

                var ret = command.Parameters["@age"].Value?.ToString();
            }
        }
    }
}
