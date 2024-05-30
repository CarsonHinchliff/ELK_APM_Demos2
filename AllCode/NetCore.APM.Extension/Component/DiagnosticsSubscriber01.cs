using Elastic.Apm;
using Elastic.Apm.Api;
using Elastic.Apm.DiagnosticSource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension.Component
{
    public class DiagnosticsSubscriber01 : IDiagnosticsSubscriber
    {     

        public IDisposable Subscribe(IApmAgent agentComponents)
        {
            ApmAgentHolder.ApmAgent = agentComponents;
            CompositeDisposable01 compositeDisposable = new CompositeDisposable01();
            if (!agentComponents.Configuration.Enabled)
                return (IDisposable)compositeDisposable;
            compositeDisposable.Add(DiagnosticListener.AllListeners.Subscribe((new DiagnosticObserver01())));
            return (IDisposable)compositeDisposable;
        }
    }
}
