using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerDemo01
{
    internal class CIDataAccess
    {
        public static void RunCmd()
        {
            using(var connection = new SqlConnection("abc"))
            {
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "exec @SP1";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter() { SqlValue = "Value1", ParameterName = "@Param1"});
                    var dbReader = command.ExecuteReader();
                }
            }
        }
    }
}
