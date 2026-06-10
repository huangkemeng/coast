using Coast.Api.Engines.AuditEngines;
using Coast.Api.Engines.Bases;
using Coast.Api.FilterAndMiddlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AutoResolveFilter>();
var app = builder.BuildWithEngines();

// 使用审计中间件
app.UseAuditMiddleware();

app.Run();