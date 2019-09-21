using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreTokenBasedAuth.Model
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
    }
}
