using Coast.Api.Engines.Bases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Coast.Api.Engines.SwaggerEngines;

public class RegisterSwagger : IBuilderEngine
{
    private readonly IServiceCollection services;

    public RegisterSwagger(IServiceCollection services)
    {
        this.services = services;
    }

    public void Run()
    {
        services.AddSwaggerGen(options =>
        {
            // 简化为单个 v1 文档
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "需求跟踪管理系统 API",
                Version = "v1",
                Description = "需求跟踪管理系统 RESTful API"
            });

            options.DescribeAllParametersInCamelCase();

            // 添加 JWT Bearer 认证
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // 加载 XML 注释
            var basePath = AppContext.BaseDirectory;
            options.IncludeXmlComments(Path.Combine(basePath, "Coast.Api.xml"), true);
            options.IncludeXmlComments(Path.Combine(basePath, "Coast.Api.Primary.xml"), true);
            options.IncludeXmlComments(Path.Combine(basePath, "Coast.Api.Infrastructure.xml"), true);

            // 添加过滤器
            options.SchemaFilter<DisplayEnumDescFilter>();
            options.SchemaFilter<SwaggerSchemaPropertyFilter>();
            options.OperationFilter<SwaggerQueryPropertyFilter>();
            options.OperationFilter<SetDefaultOperationIdFilter>();
        });
    }
}