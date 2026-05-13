namespace RequirementTrackingSystem.Infrastructure.Bases;

public interface IJsonFileSetting : ISetting
{
    string JsonFilePath { get; }
}