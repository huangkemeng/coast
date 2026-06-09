using AutoMapper;

namespace Coast.Api.Primary.Bases;

public interface IMapFrom<in TSource> where TSource : class
{
    virtual void ConfigureMapper(IMapperConfigurationExpression cfg, TSource? source)
    {
    }
}