using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace StackBook.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

            if (string.IsNullOrEmpty(roleClaim) || !_roles.Any(r => string.Equals(r, roleClaim, StringComparison.OrdinalIgnoreCase)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
