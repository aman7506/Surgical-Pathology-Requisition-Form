using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathologyFormApp.Models
{
    public class PathologyRequisitionForm
    {
        [Key]
        [Required(ErrorMessage = "UHID is required")]
        [Display(Name = "UHID")]
        public string UHID { get; set; } = null!;

        [Required(ErrorMessage = "Lab Reference Number is required")]
        [Display(Name = "Lab Ref No")]
        public string LabRefNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Patient Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "CR No")]
        public string CRNo { get; set; } = string.Empty;

        [Display(Name = "OPD/IPD")]
        public string OPD_IPD { get; set; } = string.Empty;

        [Display(Name = "IPD No")]
        public string IPDNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Consultant is required")]
        [Display(Name = "Consultant")]
        public string Consultant { get; set; } = string.Empty;

        [Display(Name = "Clinical Diagnosis")]
        public string ClinicalDiagnosis { get; set; } = string.Empty;

        [Display(Name = "Date/Time of Collection")]
        public DateTime DateTimeOfCollection { get; set; } = DateTime.Now;

        [Display(Name = "Menses Onset")]
        public int? MensesOnset { get; set; }

        [Display(Name = "Lasting Days")]
        public int? LastingDays { get; set; }

        [Display(Name = "Character")]
        public string Character { get; set; } = string.Empty;

        [Display(Name = "LMP")]
        public string LMP { get; set; } = string.Empty;

        [Display(Name = "Gravida")]
        public string Gravida { get; set; } = string.Empty;

        [Display(Name = "Para")]
        public string Para { get; set; } = string.Empty;

        [Display(Name = "Menopause Age")]
        public int? MenopauseAge { get; set; }

        [Display(Name = "Hormone Therapy")]
        public string HormoneTherapy { get; set; } = string.Empty;

        [Display(Name = "X-Ray/US/CT/MRI Findings")]
        public string XRayUSGCTMRIFindings { get; set; } = string.Empty;

        [Display(Name = "Laboratory Findings")]
        public string LaboratoryFindings { get; set; } = string.Empty;

        [Display(Name = "Operative Findings")]
        public string OperativeFindings { get; set; } = string.Empty;

        [Display(Name = "Post-Operative Diagnosis")]
        public string PostOperativeDiagnosis { get; set; } = string.Empty;

        [Display(Name = "Previous HP/Cytology Report")]
        public string PreviousHPCytReport { get; set; } = string.Empty;

        [Display(Name = "Date/Time of Receiving Specimen")]
        public DateTime DateTimeOfReceivingSpecimen { get; set; } = DateTime.Now;

        [Display(Name = "No. of Specimens Received")]
        public int NoOfSpecimenReceived { get; set; }

        [Display(Name = "Specimen No")]
        public string SpecimenNo { get; set; } = string.Empty;

        [Display(Name = "Micro Section No")]
        public string MicroSectionNo { get; set; } = string.Empty;

        [Display(Name = "Special Stains")]
        public string SpecialStains { get; set; } = string.Empty;

        [Display(Name = "Gross Description")]
        public string GrossDescription { get; set; } = string.Empty;

        [Display(Name = "Microscopic Examination")]
        public string MicroscopicExamination { get; set; } = string.Empty;

        [Display(Name = "Impression")]
        public string? Impression { get; set; } = string.Empty;

        [Display(Name = "Pathologist")]
        public string? Pathologist { get; set; }

        [Display(Name = "Pathologist Date")]
        public DateTime PathologistDate { get; set; } = DateTime.Now;

        [Display(Name = "Uploaded File Path")]
        public string UploadedFilePath { get; set; } = string.Empty;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Date/Time of Processing")]
        public DateTime DateTimeOfProcessing { get; set; } = DateTime.Now;

        [Display(Name = "Menopause Years")]
        public int? MenopauseYears { get; set; }

        [Display(Name = "Menses Cycle")]
        public int? MensesCycle { get; set; }

        [Display(Name = "Specimen Name")]
        public string? SpecimenName { get; set; }

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Advice")]
        public string Advice { get; set; } = string.Empty;

        [Display(Name = "Signature")]
        public string? SignatureImage { get; set; }

        [Display(Name = "Status")]
        public FormStatus Status { get; set; } = FormStatus.Draft;

        // Creator and Reviewer Information
        [ForeignKey("CreatedBy")]
        public string CreatedById { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;

        [ForeignKey("ReviewedBy")]
        public string? ReviewedById { get; set; }
        public User? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }

        // Navigation properties
        public ICollection<FormHistory> FormHistory { get; set; } = [];
    }

    public class FormHistory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Form")]
        public string FormUHID { get; set; } = null!;
        public PathologyRequisitionForm Form { get; set; } = null!;

        [ForeignKey("User")]
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public string Action { get; set; } = null!; // Created, Updated, Submitted, Reviewed
        public string? Comments { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
