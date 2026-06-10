using Coast.Api.Infrastructure.DataPersistence.DataEntityBases;
using Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities;

/// <summary>
/// API访问日志实体（用于 API 性能监控和调用分析）
/// </summary>
public class ApiAccessLog : IEfEntity<ApiAccessLog>, IHasKey<int>, IHasCreatedOn
{
    public ApiAccessLog()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<ApiAccessLog> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);

        // 普通索引：会话ID、用户ID、API路径、时间、响应状态
        builder.HasIndex(a => a.SessionId);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.ApiPath);
        builder.HasIndex(a => a.CreatedOn);
        builder.HasIndex(a => a.ResponseStatusCode);

        // 配置字符串字段长度
        builder.Property(a => a.HttpMethod).HasMaxLength(10).IsRequired();
        builder.Property(a => a.ApiPath).HasMaxLength(200).IsRequired();
        builder.Property(a => a.QueryString).HasMaxLength(500);
        builder.Property(a => a.RequestBody).HasMaxLength(2000);
        builder.Property(a => a.IpAddress).HasMaxLength(50);
        builder.Property(a => a.UserAgent).HasMaxLength(500);
        builder.Property(a => a.ErrorMessage).HasMaxLength(1000);
    }

    /// <summary>主键，自增</summary>
    public int Id { get; set; }

    /// <summary>关联会话</summary>
    public Guid? SessionId { get; set; }

    /// <summary>用户ID（可为空，匿名访问）</summary>
    public Guid? UserId { get; set; }

    /// <summary>HTTP方法</summary>
    public string HttpMethod { get; set; } = string.Empty;

    /// <summary>API路径</summary>
    public string ApiPath { get; set; } = string.Empty;

    /// <summary>查询参数</summary>
    public string? QueryString { get; set; }

    /// <summary>请求体（脱敏）</summary>
    public string? RequestBody { get; set; }

    /// <summary>响应状态码</summary>
    public int ResponseStatusCode { get; set; }

    /// <summary>响应时间（毫秒）</summary>
    public int ResponseTimeMs { get; set; }

    /// <summary>IP 地址</summary>
    public string? IpAddress { get; set; }

    /// <summary>浏览器 User-Agent</summary>
    public string? UserAgent { get; set; }

    /// <summary>错误信息</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>访问时间</summary>
    public DateTime CreatedOn { get; set; }

    // 导航属性
    /// <summary>会话导航属性</summary>
    public AuditSession? Session { get; set; }

    /// <summary>用户导航属性</summary>
    public ApplicationUser? User { get; set; }
}
