namespace Coast.Api.Infrastructure.DataPersistence.EfCore.Entities.Enums;

/// <summary>
/// 变更字段类型枚举
/// </summary>
public enum ChangeFieldType
{
    /// <summary>文本</summary>
    Text = 0,

    /// <summary>数值</summary>
    Number = 1,

    /// <summary>日期</summary>
    DateTime = 2,

    /// <summary>枚举</summary>
    Enum = 3,

    /// <summary>外键引用</summary>
    ForeignKey = 4,

    /// <summary>布尔值</summary>
    Boolean = 5
}
