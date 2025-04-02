using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.Configurations
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string Environment { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}