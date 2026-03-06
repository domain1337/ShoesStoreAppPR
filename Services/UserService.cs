using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesStoreApp.Services
{
    public static class UserService
    {
        public static string CurrentRole { get; set; } = "guest";
        public static string UserEmail { get; set; }

        public static bool IsAdmin => CurrentRole.ToLower() == "admin";

        public static bool IsManagerOrAdmin =>
            CurrentRole.ToLower() == "admin" || CurrentRole.ToLower() == "manager";
        public static bool IsSimpleUser =>
            CurrentRole.ToLower() == "client" || CurrentRole.ToLower() == "guest";
        public static bool IsAuthenticated => CurrentRole.ToLower() != "guest";
    }
}
