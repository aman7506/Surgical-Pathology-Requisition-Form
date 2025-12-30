using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PathologyFormApp.ViewModels
{
    /// <summary>
    /// Represents the data a doctor submits during their review.
    /// Contains only the fields a doctor is responsible for editing.
    /// </summary>
    public class DoctorFormViewModel
    {
        [Required]
        public string UHID { get; set; } = null!;

        [Display(Name = "Gross Description")]
        public string? GrossDescription { get; set; }

        [Display(Name = "Microscopic Examination")]
        public string? MicroscopicExamination { get; set; }

        [Required(ErrorMessage = "Impression is required.")]
        public string Impression { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pathologist Name is required.")]
        public string Pathologist { get; set; } = string.Empty;

        [Display(Name = "Special Stains")]
        public string? SpecialStains { get; set; }

        [Display(Name = "Advice")]
        public string? Advice { get; set; }

        // For the Base64 signature image
        public string? Signature { get; set; }
        
        public List<IFormFile> UploadedFiles { get; set; } = [];
    }
} 