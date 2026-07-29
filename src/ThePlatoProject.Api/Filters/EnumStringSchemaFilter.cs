using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalAPI2026Demo.Filters
{
    /// <summary>
    /// Adds allowed ArtifactType values to the 'Type' property description
    /// when the model uses a string instead of an enum.
    /// </summary>
    public class EnumStringSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            //Only target CreateArtifactRequest.Type
            if (context.Type == typeof(string) && context.MemberInfo?.Name == "Type")
            {
                schema.Description = "Allowed values: " +
                    string.Join(",", Enum.GetNames(typeof(ArtifactType)));

            }
        }
    }
}
