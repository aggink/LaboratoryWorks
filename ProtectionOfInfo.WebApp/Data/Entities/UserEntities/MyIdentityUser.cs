using Microsoft.AspNetCore.Identity;
using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Data.Entities
{
    public class MyIdentityUser : IdentityUser
    {
        public bool BlockedUser { get; set; } = false;
        public bool PasswordValidation { get; set; } = false;
        public bool FirstAccess { get; set; } = true;
    }
}
