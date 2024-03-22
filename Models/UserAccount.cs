using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public class UserAccount
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<string> Roles { get; set; } = new();
        [JsonIgnore]
        public bool IsAdmin => Roles.Contains(AdminRole);
    }
}
