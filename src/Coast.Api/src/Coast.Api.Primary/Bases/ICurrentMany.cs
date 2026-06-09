namespace Coast.Api.Primary.Bases;

public interface ICurrentMany<T> : ICurrent
{
    Task<List<T>> QueryManyAsync(CancellationToken cancellationToken = default);
}