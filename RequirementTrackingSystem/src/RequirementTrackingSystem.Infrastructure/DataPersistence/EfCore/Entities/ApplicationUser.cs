using RequirementTrackingSystem.Infrastructure.DataPersistence.DataEntityBases;
using RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;

namespace RequirementTrackingSystem.Infrastructure.DataPersistence.EfCore.Entities;

public class ApplicationUser : IEfEntity<ApplicationUser>, IHasKey<Guid>
{
    public ApplicationUser()
    {
        this.InitPropertyValues();
    }

    public static void ConfigureEntityMapping(EntityTypeBuilder<ApplicationUser> builder,
        IRelationalTypeMappingSource mappingSource)
    {
        builder.AutoConfigure(mappingSource);
    }

    public Guid Id { get; set; }
}