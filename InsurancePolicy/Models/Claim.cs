using System;
using System.Collections.Generic;

namespace InsurancePolicy.Models
{
    public partial class Claim
    {
        public int ClaimId { get; set; }
        public DateTime? Date { get; set; }
        public string Reason { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int? PolicyId { get; set; }
        public double? ClaimAmount { get; set; }

        public virtual Policy? Policy { get; set; }
    }
}
