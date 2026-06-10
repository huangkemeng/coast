namespace Coast.Api.Infrastructure.Audit;

/// <summary>
/// 审计上下文访问器
/// </summary>
public interface IAuditContextAccessor
{
    /// <summary>获取当前审计上下文</summary>
    AuditContext? Current { get; set; }
}

/// <summary>
/// 审计上下文访问器实现（基于AsyncLocal支持异步）</summary>
public class AuditContextAccessor : IAuditContextAccessor
{
    private static readonly AsyncLocal<AuditContextHolder> _currentContext = new();

    public AuditContext? Current
    {
        get => _currentContext.Value?.Context;
        set
        {
            var holder = _currentContext.Value;
            if (holder != null)
            {
                holder.Context = null;
            }

            if (value != null)
            {
                _currentContext.Value = new AuditContextHolder { Context = value };
            }
        }
    }

    private class AuditContextHolder
    {
        public AuditContext? Context;
    }
}