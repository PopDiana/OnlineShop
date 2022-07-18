using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCore.Models
{
    public class CouponCode
    {
        public int CouponCodeId { get; set; }

        public string CouponName { get; set; }

        public int CouponPercentage { get; set; }

        public bool isAvailable { get; set; }

    }
}
