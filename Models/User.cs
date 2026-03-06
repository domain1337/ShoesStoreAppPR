using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesStoreApp.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Role { get; set; }

        public bool CanEdit => Role == "admin";
        public bool CanViewOrders => Role == "admin" || Role == "manager";
        public bool IsGuest => Role == "guest";
    }
}
