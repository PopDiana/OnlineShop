using IdentityCore.Models;
using System.Collections.Generic;

namespace IdentityCore.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> Products { get; set; }
    }
}
