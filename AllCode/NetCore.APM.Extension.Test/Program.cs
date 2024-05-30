﻿using System.Data.SqlClient;
using System.Data;
using NetCore.APM.Extension;

namespace NetCore.APM.Extension.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            Start_Table();
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
                var observableCommand = new ApmDbCommandWrapper(command);

                // 订阅 ObservableDbCommandWrapper
                using (observableCommand.Subscribe(new ApmCommandObserver()))
                {
                    // 执行命令
                    var reader = command.ExecuteReader();
                }
            }
        }
    }
}
