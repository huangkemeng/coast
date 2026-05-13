namespace RequirementTrackingSystem.Primary.Bases;

public interface ICurrent
{
    Task<Guid> GetCurrentUserIdAsync();
}