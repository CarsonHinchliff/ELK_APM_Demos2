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

namespace NetCore.APM.Extension
{
    public class ApmDbCommandWrapper : IObservable<DbCommand>
    {
        private readonly DbCommand _command;

        public ApmDbCommandWrapper(DbCommand command)
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
}
