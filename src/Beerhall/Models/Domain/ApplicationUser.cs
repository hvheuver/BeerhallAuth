using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Beerhall.Models.Domain
{
    public class ApplicationUser : IdentityUser {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Street { get; set; }
        public Location Location { get; set; }

    }
}
