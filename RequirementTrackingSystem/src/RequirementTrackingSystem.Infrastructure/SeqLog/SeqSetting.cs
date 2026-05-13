using RequirementTrackingSystem.Infrastructure.Bases;

namespace RequirementTrackingSystem.Infrastructure.SeqLog;

public class SeqSetting : IJsonFileSetting
{
    public string ServerUrl { get; set; }
    public string ApiKey { get; set; }
    public string JsonFilePath => "./SeqLog/seq-setting.json";
}