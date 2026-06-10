using Coast.Api.Engines.Bases;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Coast.Api.Engines.SwaggerEngines;

public class UseSwagger : IAppEngine
{
    private readonly WebApplication app;

    public UseSwagger(WebApplication app)
    {
        this.app = app;
    }

    public void Run()
    {
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // 简化为单个 v1 端点
                options.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "需求跟踪管理系统 API - " + app.Environment.EnvironmentName);
            });
        }
    }
}