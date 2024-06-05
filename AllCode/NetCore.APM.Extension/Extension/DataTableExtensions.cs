using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension
{
    internal static class DataTableExtensions
    {
        public static string DataTableToJson(this DataTable table)
        {
            var jsonStr = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                jsonStr.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    jsonStr.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            jsonStr.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            jsonStr.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        jsonStr.Append("}");
                    }
                    else
                    {
                        jsonStr.Append("},");
                    }
                }
                jsonStr.Append("]");
            }
            return jsonStr.ToString();
        }
    }
}
