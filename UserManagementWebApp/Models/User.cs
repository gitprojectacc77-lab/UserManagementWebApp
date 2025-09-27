using System.ComponentModel.DataAnnotations;

namespace UserManagementWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 120)]
        public int Age { get; set; }

        [Range(0, 100)]
        public int Grade { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
