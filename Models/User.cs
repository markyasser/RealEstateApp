using Microsoft.AspNetCore.Identity;

namespace RealState.Models
{
    public class User : IdentityUser
    {
        public string ArabicUsername { get; set; }
        public string IDNumber { get; set; }
        public string JobTitle { get; set; }
        public string Address { get; set; }
    }
}
