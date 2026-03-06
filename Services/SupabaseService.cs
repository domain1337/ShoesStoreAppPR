using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesStoreApp.Services
{
    public static class SupabaseService
    {
        private const string Url = "https://eowxuwrkuqubyorlgqmp.supabase.co";
        private const string Key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVvd3h1d3JrdXF1YnlvcmxncW1wIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njk4Njk4OTgsImV4cCI6MjA4NTQ0NTg5OH0.vd1Lo1OGB14Gk-Vm1bVJl0jZ_EBM4kR9OqYmbFhwYlw";

        private static Client _client;
        public static Client Client => _client ??= new Client(Url, Key, new SupabaseOptions { AutoRefreshToken = true });
    }
}
