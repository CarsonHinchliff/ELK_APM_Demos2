using Elastic.Apm;
using Elastic.Apm.Api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension.Extension
{
    internal static class DbCommandExtensions
    {
        public static Tuple<string, IDictionary<string, object>> GetParamTuple(this IDbCommand command)
        {
            Dictionary<String, Object> paramAudits = new Dictionary<String, Object>();
            foreach (DbParameter parameter in command.Parameters)
            {
                object value = null;
                if (parameter is SqlParameter parameter1 && parameter1.SqlDbType == SqlDbType.Structured)
                {
                    var paramVal = parameter1.Value as DataTable;
                    value = paramVal?.Copy()?.DataTableToJson();
                    paramAudits.Add(parameter.ParameterName, value);
                }
                else
                {
                    value = parameter.Value;
                    paramAudits.Add(parameter.ParameterName, value);
                }
            }
            return new Tuple<string, IDictionary<string, object>>(command.CommandText, paramAudits);
        }

        private static ITransaction GetTransaction()
        {
            return Agent.Tracer.CurrentTransaction;
        }

        public static void AppendToSpan(this IDbCommand command)
        {
            // 处理 DbCommand，例如记录参数
            Console.WriteLine("Command SQL: " + command.CommandText);
            var paramTuple = command.GetParamTuple();
            var span = Agent.Tracer.CurrentSpan ?? GetTransaction()?.StartSpan("SQL Parameters Span", String.Empty);
            var paramDic = paramTuple.Item2;
            var index = 0;
            foreach (var param in paramTuple.Item2)
            {
                span?.SetLabel("lables.sql.param_" + index++, param.Value?.ToString());
            }
            var paramAuditLog = Newtonsoft.Json.JsonConvert.SerializeObject(paramDic);
            Console.WriteLine(paramAuditLog);
            span?.SetLabel("lables.sql.params", paramAuditLog);
            span?.SetLabel("lables.sql", command.CommandText);
            span?.End();
        }
    }
}
