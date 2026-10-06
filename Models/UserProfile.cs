using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ShoesStoreApp.Models;

[Table("profiles")]
public class UserProfile : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("role")]
    public string Role { get; set; } = "client";
}
