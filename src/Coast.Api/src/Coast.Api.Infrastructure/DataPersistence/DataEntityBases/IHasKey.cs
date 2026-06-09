namespace Coast.Api.Infrastructure.DataPersistence.DataEntityBases;

public interface IHasKey<T> : IEntity
{
    T Id { get; set; }
}