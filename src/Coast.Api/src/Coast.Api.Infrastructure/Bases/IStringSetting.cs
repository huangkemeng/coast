namespace Coast.Api.Infrastructure.Bases;

public interface IStringSetting : ISetting
{
    string Value { get; }
}