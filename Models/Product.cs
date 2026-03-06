using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;
using System;

namespace ShoesStoreApp.Models
{
    [Table("products")]
    public class Product : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("article")]
        public string Article { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("unit")]
        public string Unit { get; set; } = "шт.";

        [Column("price")]
        public decimal Price { get; set; }

        [Column("supplier")]
        public string Supplier { get; set; }

        [Column("manufacturer")]
        public string Manufacturer { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("quantity_in_stock")]
        public int QuantityInStock { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }
        [JsonIgnore]
        public decimal FinalPrice => Price * (1 - Discount / 100);

        [JsonIgnore]
        public string FullImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                    return "/Assets/Images/logo.png";
                if (ImagePath.StartsWith("http"))
                    return ImagePath;
                string projectId = "eowxuwrkuqubyorlgqmp";
                return $"https://eowxuwrkuqubyorlgqmp.supabase.co/storage/v1/object/public/images/{ImagePath}";
            }
        }
    }
}