using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerDemo01
{
    public class ObservableDbCommandWrapper : IObservable<DbCommand>
    {
        private readonly DbCommand _command;

        public ObservableDbCommandWrapper(DbCommand command)
        {
            _command = command;
        }

        public IDisposable Subscribe(IObserver<DbCommand> observer)
        {
            observer.OnNext(_command);
            observer.OnCompleted();
            return new EmptyDisposable();
        }

        private class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }

    public class CommandObserver : IObserver<DbCommand>
    {
        public void OnNext(DbCommand command)
        {
            // 处理 DbCommand，例如记录参数
            Console.WriteLine("Command Text: " + command.CommandText);
            foreach (DbParameter parameter in command.Parameters)
            {
                if (parameter is SqlParameter parameter1 && parameter1.SqlDbType == SqlDbType.Structured)
                {
                    var paramVal = parameter1.Value as DataTable;
                    Console.Write(paramVal?.Copy().DataTableToJson());
                } else
                {
                    Console.WriteLine($"Parameter: {parameter.ParameterName}, Value: {parameter.Value}");
                }
            }
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }
    }

    public class Test
    {
        public static void Start()
        {
            Start_Basic();
            Start_Table();
        }

        public static void Start_Basic()
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

                // 包装 DbCommand 为一个 Observable
                var observableCommand = new ObservableDbCommandWrapper(command);

                // 订阅 ObservableDbCommandWrapper
                using (observableCommand.Subscribe(new CommandObserver()))
                {
                    // 执行命令
                    var reader = command.ExecuteReader();

                    var ret = command.Parameters["@age"].Value?.ToString();
                }
            }
        }

        public static void Start_Table()
        {
            // 创建一个 SqlConnection
            var connectionString = "Data Source=.;Initial Catalog=sqlsugar_db;User ID=sa;Password=sa1234";
            using (var connection = new SqlConnection(connectionString))
            {

                // 创建一个 DbCommand
                var command = connection.CreateCommand();
                command.CommandText = "dbo.[sp_school_table]";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var dt = new DataTable();
                dt.Columns.Add("name", typeof(string));
                dt.Columns.Add("age", typeof(int));
                var row = dt.NewRow();
                row[0] = "Changfu";
                row[1] = 100;
                dt.Rows.Add(row);
                dt.AcceptChanges();
                command.Parameters.Add(new SqlParameter
                {
                    SqlDbType = System.Data.SqlDbType.Structured,
                    ParameterName = "@table1",
                    SqlValue = dt
                });
                connection.Open();

                // 包装 DbCommand 为一个 Observable
                var observableCommand = new ObservableDbCommandWrapper(command);

                // 订阅 ObservableDbCommandWrapper
                using (observableCommand.Subscribe(new CommandObserver()))
                {
                    // 执行命令
                    var reader = command.ExecuteReader();
                }
            }
        }
    }
}
