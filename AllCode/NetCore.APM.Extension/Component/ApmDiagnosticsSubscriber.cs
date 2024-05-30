using Elastic.Apm;
using Elastic.Apm.Api;
using Elastic.Apm.DiagnosticSource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension
{
    public class ApmDiagnosticsSubscriber : IDiagnosticsSubscriber
    {     

        public IDisposable Subscribe(IApmAgent agentComponents)
        {
            ApmCompositeDisposable compositeDisposable = new ApmCompositeDisposable();
            compositeDisposable.Add(DiagnosticListener.AllListeners.Subscribe((new ApmDiagnosticObserver())));
            return (IDisposable)compositeDisposable;
        }
    }
}
