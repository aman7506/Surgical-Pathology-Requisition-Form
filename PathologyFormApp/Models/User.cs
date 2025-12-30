using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PathologyFormApp.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public required string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public required string Role { get; set; } // "Doctor" or "Nurse"

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for forms created by this user
        public ICollection<PathologyRequisitionForm> CreatedForms { get; set; } = new List<PathologyRequisitionForm>();
    }
} 