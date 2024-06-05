using Elastic.Apm;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.Instrumentations.SqlClient;
using Elastic.Apm.NetCoreAll;
using NetCore.APM.Extension;
using NetCore.APM.Extension.DependencyInjection;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

//working
//app.UseElasticApm(builder.Configuration, new ApmDiagnosticsSubscriber());
//app.UseAllElasticApm(builder.Configuration);
app.AddApmComponents(builder.Configuration);

app.Run();
