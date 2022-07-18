using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class UserRating
    {
        public int UserRatingId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public decimal? Rating { get; set; }

    }
}
