namespace RequirementTrackingSystem.Primary.Bases;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter |
                AttributeTargets.Enum)]
public class SwaggerSchemaIgnoreAttribute : Attribute
{
}