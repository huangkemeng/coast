namespace Coast.Api.Primary.Bases;

public interface ICurrent
{
    Task<Guid> GetCurrentUserIdAsync();
}