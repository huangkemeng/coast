namespace Coast.Api.Infrastructure.DataPersistence.DataEntityBases;

public interface IPageable
{
    public int Offset { get; set; }

    public int PageSize { get; set; }
}