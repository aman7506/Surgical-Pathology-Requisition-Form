using System.ComponentModel.DataAnnotations;

namespace PathologyFormApp.Models
{
    // This model is specifically for the list of available specimen types
    public class SpecimenType // Using a different name to avoid conflict
    {
        [Key] // Designate Id as the primary key
        public int Id { get; set; }

        [Required] // Specimen name should be required
        public string Name { get; set; } = string.Empty;
    }
} 