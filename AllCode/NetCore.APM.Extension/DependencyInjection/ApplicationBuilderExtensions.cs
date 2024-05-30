using Elastic.Apm.AspNetCore;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NetCore.APM.Extension.Component;

namespace NetCore.APM.Extension.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static void AddApmComponents(this IApplicationBuilder builder, IConfiguration configuration)
        {
#pragma warning disable 
            builder.UseElasticApm(configuration, new ApmDiagnosticsSubscriber());
            builder.UseAllElasticApm(configuration);
#pragma warning restore
        }
    }
}
