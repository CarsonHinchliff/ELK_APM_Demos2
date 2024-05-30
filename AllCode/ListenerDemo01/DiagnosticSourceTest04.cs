using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapr;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Extensions.DiagnosticAdapter;

namespace ListenerDemo01.DiagnosticSourceTest04
{
    public class ExampleDiagnosticObserver4 : IObserver<DiagnosticListener>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly AsyncLocal<Stopwatch> _stopwatch = new AsyncLocal<Stopwatch>();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "SqlClientDiagnosticListener")
            {
                var subscription = value.SubscribeWithAdapter(this);
                _subscriptions.Add(subscription);
            }
        }

        [DiagnosticName("System.Data.SqlClient.WriteCommandBefore")]
        public void OnCommandBefore()
        {
            _stopwatch.Value = Stopwatch.StartNew();
        }

        [DiagnosticName("System.Data.SqlClient.WriteCommandAfter")]
        public void OnCommandAfter(DbCommand command)
        {
            var stopwatch = _stopwatch.Value;
            stopwatch.Stop();
            Console.WriteLine($"CommandText: {command.CommandText}");
            Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
            Console.WriteLine();
        }
    }

    internal class DiagnosticSourceTest04
    {
        public const string ConnectionString = @"Data Source=.;Initial Catalog=sqlsugar_db;User ID=sa;Password=sa1234";
        public static void Start()
        {
            IDisposable subscription = DiagnosticListener.AllListeners.Subscribe(new ExampleDiagnosticObserver4());            
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
