using Microsoft.AspNetCore.Identity;

namespace WemaBankAssignment
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string name, string description, string status)
            : base(name)
        {
            this.Description = description;
            this.Status = status;
        }

        public string Description { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
    }
}
