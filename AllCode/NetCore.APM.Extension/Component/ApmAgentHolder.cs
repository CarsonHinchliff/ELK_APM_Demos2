using Elastic.Apm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension.Component
{
    public class ApmAgentHolder
    {
        public static IApmAgent ApmAgent { get; set; }
    }
}
