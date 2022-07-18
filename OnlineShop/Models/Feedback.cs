using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class Feedback
    { 
        [Required]
        public string Email { get; set; }
        [Required]
        public string Description { get; set; }

    }
}
