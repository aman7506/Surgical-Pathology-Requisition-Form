using System.Collections.Generic;

namespace PathologyFormApp.Models
{
    public class PathologyForm
    {
        public string? PatientName { get; set; }
        public string? PatientID { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhysicianName { get; set; }
        public string? ClinicalHistory { get; set; }
        public List<Specimen> Specimens { get; set; } = [];
    }

    public class Specimen
    {
        public string? Site { get; set; }
        public string? Laterality { get; set; }
        public string? SpecimenType { get; set; }
        public string? ClinicalDiagnosis { get; set; }
    }
}