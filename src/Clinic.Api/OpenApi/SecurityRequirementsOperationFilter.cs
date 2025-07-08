namespace Clinic.Api.OpenApi;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (allowAnonymous)
        {
            return;
        }

        var authorizeAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? [])
            .OfType<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var requiredRoles = authorizeAttributes
                .Select(attr => attr.Roles)
                .Where(roles => !string.IsNullOrEmpty(roles))
                .SelectMany(roles => roles?.Split(',') ?? [])
                .Where(roles => !string.IsNullOrEmpty(roles))
                .Select(role => role.Trim())
                .Distinct()
                .ToList();

            operation.Description += requiredRoles.Count != 0
                ? $"\n\n> **Required roles:** `{string.Join("`, `", requiredRoles)}`"
                : $"\n\n> **Authentication needed**";
        }
    }
}