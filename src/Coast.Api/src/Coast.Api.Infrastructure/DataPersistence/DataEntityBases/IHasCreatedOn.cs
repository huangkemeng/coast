namespace Coast.Api.Infrastructure.DataPersistence.DataEntityBases;

public interface IHasCreatedOn : IEntity
{
    DateTime CreatedOn { get; set; }
}