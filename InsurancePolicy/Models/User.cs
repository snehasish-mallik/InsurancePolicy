using System;
using System.Collections.Generic;

namespace InsurancePolicy.Models
{
    public partial class User
    {
        public User()
        {
            Policies = new HashSet<Policy>();
        }

        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }

        public virtual ICollection<Policy> Policies { get; set; }
    }
}
