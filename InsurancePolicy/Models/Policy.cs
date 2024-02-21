using System;
using System.Collections.Generic;

namespace InsurancePolicy.Models
{
    public partial class Policy
    {
        public Policy()
        {
            Claims = new HashSet<Claim>();
        }

        public int PolicyId { get; set; }
        public string? PolicyType { get; set; }
        public string? VehicleRegistrationNum { get; set; }
        public int? Pincode { get; set; }
        public int? UserId { get; set; }
        public string? PolicyName { get; set; }
        public double? PolicyAmount { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Claim> Claims { get; set; }
    }
}
