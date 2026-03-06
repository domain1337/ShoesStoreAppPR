using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesStoreApp.Models
{
    [Table("orders")]
    public class Order : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("customer_email")]
        public string CustomerEmail { get; set; }

        [Column("order_content")]
        public string OrderContent { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
