using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public JwtSettings(IConfiguration configuration)
        {
            Secret = configuration["JwtSettings:Secret"] ?? string.Empty;
        }
    }
}