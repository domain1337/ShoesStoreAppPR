using Supabase.Postgrest.Models;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesStoreApp.Models
{
    [Table("pickup_points")]
    public class PickupPoint : BaseModel
    {
        [Column("index_code")] public string Index { get; set; } = string.Empty;
        [Column("city")] public string City { get; set; } = string.Empty;
        [Column("street")] public string Street { get; set; } = string.Empty;
        [Column("house")] public string House { get; set; } = string.Empty;
        [JsonIgnore]
        public string FullAddress => $"{Index}, {City}, {Street}, д. {House}";
    }
}
