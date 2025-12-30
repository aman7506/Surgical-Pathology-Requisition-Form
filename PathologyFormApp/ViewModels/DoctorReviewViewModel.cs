using PathologyFormApp.Models;

namespace PathologyFormApp.ViewModels
{
    /// <summary>
    /// A composite view model for the Doctor's review page.
    /// It holds the full details of the form for display,
    /// and a separate model for the doctor's input to ensure correct validation.
    /// </summary>
    public class DoctorReviewViewModel
    {
        public PathologyRequisitionForm FormDetails { get; set; } = null!;
        public DoctorFormViewModel DoctorInput { get; set; } = null!;
    }
} 