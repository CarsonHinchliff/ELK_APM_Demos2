using Elastic.Apm;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.Instrumentations.SqlClient;
using Elastic.Apm.NetCoreAll;
using NetCore.APM.Extension.Component;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

//working
app.UseElasticApm(builder.Configuration, new DiagnosticsSubscriber01());
//Agent.Subscribe(new DiagnosticsSubscriber01());

app.UseAllElasticApm(builder.Configuration);

app.Run();
