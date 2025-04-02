using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace StackBook.Middleware
{
    public static class Authorization
    {
        public static bool CheckUserRole(IHttpContextAccessor httpContextAccessor, string role)
        {
            var user = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            if(user == null)
            {
                return false;
            }
            return user.Equals(role, StringComparison.OrdinalIgnoreCase);
        }
    }
}