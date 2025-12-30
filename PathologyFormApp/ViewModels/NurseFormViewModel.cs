using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PathologyFormApp.ViewModels
{
    /// <summary>
    /// Represents the data submitted from the Nurse's form.
    /// This model only includes fields that the nurse is responsible for,
    /// preventing validation errors for fields that will be filled in later (e.g., by a doctor).
    /// </summary>
    public class NurseFormViewModel
    {
        // Patient Information
        [Required(ErrorMessage = "UHID is required")]
        public string UHID { get; set; } = null!;

        [Required(ErrorMessage = "Lab Reference Number is required")]
        public string LabRefNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Patient Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        public string? CRNo { get; set; }
        public string? OPD_IPD { get; set; }
        public string? IPDNo { get; set; }

        [Required(ErrorMessage = "Consultant is required")]
        public string Consultant { get; set; } = string.Empty;

        // Gynae Cases
        public int? MensesOnset { get; set; }
        public int? MensesCycle { get; set; }
        public int? LastingDays { get; set; }
        public string? Character { get; set; }
        public string? LMP { get; set; }
        public string? Gravida { get; set; }
        public string? Para { get; set; }
        public int? MenopauseAge { get; set; }
        public int? MenopauseYears { get; set; }
        public string? HormoneTherapy { get; set; }

        // Clinical Data
        public string? ClinicalDiagnosis { get; set; }
        public string? XRayUSGCTMRIFindings { get; set; }
        public string? LaboratoryFindings { get; set; }
        public string? OperativeFindings { get; set; }
        public string? PostOperativeDiagnosis { get; set; }
        public string? PreviousHPCytReport { get; set; }

        // Specimen
        public string? SpecimenName { get; set; }
        public DateTime? DateTimeOfCollection { get; set; }

        // Signature & Upload
        public string? Signature { get; set; }
        public List<IFormFile> UploadedFiles { get; set; } = [];
    }
} 