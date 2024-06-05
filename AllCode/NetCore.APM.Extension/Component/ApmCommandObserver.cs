using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Apm.Api;
using Elastic.Apm;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

namespace NetCore.APM.Extension
{
    public class ApmCommandObserver : IObserver<DbCommand>
    {     
        public void OnNext(DbCommand command)
        {
            command.AppendToSpan();
        }

        

        public void OnCompleted() { }

        public void OnError(Exception error) { }
    }
}
