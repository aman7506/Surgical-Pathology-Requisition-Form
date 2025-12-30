using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PathologyFormApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PathologyFormApp.ViewModels;
using System;

namespace PathologyFormApp.Controllers
{
    [Authorize]
    public class PathologyFormController(PathologyContext context, UserManager<User> userManager) : Controller
    {
        private readonly PathologyContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet]
        [Authorize(Policy = "RequireNurseRole")]
        public IActionResult NurseForm()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RequireNurseRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NurseForm(NurseFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found. Please log in again.");
                    return View(viewModel);
                }

                // Map data from ViewModel to the main entity
                var form = new PathologyRequisitionForm
                {
                    UHID = viewModel.UHID,
                    LabRefNo = viewModel.LabRefNo,
                    Date = viewModel.Date,
                    Name = viewModel.Name,
                    Age = viewModel.Age,
                    Gender = viewModel.Gender,
                    CRNo = viewModel.CRNo ?? string.Empty,
                    OPD_IPD = viewModel.OPD_IPD ?? string.Empty,
                    IPDNo = viewModel.IPDNo ?? string.Empty,
                    Consultant = viewModel.Consultant,
                    MensesOnset = viewModel.MensesOnset,
                    MensesCycle = viewModel.MensesCycle,
                    LastingDays = viewModel.LastingDays,
                    Character = viewModel.Character ?? string.Empty,
                    LMP = viewModel.LMP ?? string.Empty,
                    Gravida = viewModel.Gravida ?? string.Empty,
                    Para = viewModel.Para ?? string.Empty,
                    MenopauseAge = viewModel.MenopauseAge,
                    MenopauseYears = viewModel.MenopauseYears,
                    HormoneTherapy = viewModel.HormoneTherapy ?? string.Empty,
                    ClinicalDiagnosis = viewModel.ClinicalDiagnosis ?? string.Empty,
                    XRayUSGCTMRIFindings = viewModel.XRayUSGCTMRIFindings ?? string.Empty,
                    LaboratoryFindings = viewModel.LaboratoryFindings ?? string.Empty,
                    OperativeFindings = viewModel.OperativeFindings ?? string.Empty,
                    PostOperativeDiagnosis = viewModel.PostOperativeDiagnosis ?? string.Empty,
                    PreviousHPCytReport = viewModel.PreviousHPCytReport ?? string.Empty,
                    SpecimenName = viewModel.SpecimenName,
                    DateTimeOfCollection = viewModel.DateTimeOfCollection ?? DateTime.Now,
                    SignatureImage = viewModel.Signature,

                    // Set system-managed properties
                    CreatedById = user.Id,
                    Status = FormStatus.NurseSubmitted,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                // Handle multiple file uploads
                if (viewModel.UploadedFiles != null && viewModel.UploadedFiles.Count > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var filePaths = new List<string>();
                    foreach (var file in viewModel.UploadedFiles)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            filePaths.Add("/uploads/" + uniqueFileName);
                        }
                    }
                    form.UploadedFilePath = string.Join(";", filePaths);
                }

                _context.PathologyRequisitionForms.Add(form);

                // Add to form history
                var history = new FormHistory
                {
                    FormUHID = form.UHID,
                    UserId = user.Id,
                    Action = "Created",
                    Comments = "Form created by nurse"
                };
                _context.FormHistory.Add(history);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Form submitted successfully!";
                return RedirectToAction(nameof(NurseForm));
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = "RequireDoctorRole")]
        public async Task<IActionResult> DoctorForm(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("A UHID is required to review a form.");
            }

            var form = await _context.PathologyRequisitionForms
                .Include(f => f.CreatedBy) // Eager load the creator's details
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.UHID == id);

            if (form == null)
            {
                return NotFound($"No form found with UHID: {id}");
            }

            // Create the composite view model for the doctor's review page
            var viewModel = new DoctorReviewViewModel
            {
                FormDetails = form,
                DoctorInput = new DoctorFormViewModel
                {
                    UHID = form.UHID,
                    GrossDescription = form.GrossDescription ?? string.Empty,
                    MicroscopicExamination = form.MicroscopicExamination ?? string.Empty,
                    Impression = form.Impression ?? string.Empty,
                    Pathologist = form.Pathologist ?? string.Empty,
                    SpecialStains = form.SpecialStains ?? string.Empty,
                    Advice = form.Advice ?? string.Empty,
                    Signature = form.SignatureImage
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "RequireDoctorRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorForm([Bind(Prefix = "DoctorInput")] DoctorFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // This is an unlikely edge case, but good to handle
                    ModelState.AddModelError(string.Empty, "Your user session could not be found. Please log in again.");
                    // We need to rebuild the full view model to return to the view
                    var originalForm = await _context.PathologyRequisitionForms.AsNoTracking().FirstOrDefaultAsync(f => f.UHID == viewModel.UHID);
                    if (originalForm == null) return NotFound();
                    var reviewViewModel = new DoctorReviewViewModel { FormDetails = originalForm, DoctorInput = viewModel };
                    return View(reviewViewModel);
                }

                var existingForm = await _context.PathologyRequisitionForms
                    .FirstOrDefaultAsync(f => f.UHID == viewModel.UHID);

                if (existingForm == null)
                {
                    return NotFound($"The form you are trying to update (UHID: {viewModel.UHID}) no longer exists.");
                }

                // Map the updated fields from the view model to the existing entity
                existingForm.GrossDescription = viewModel.GrossDescription ?? string.Empty;
                existingForm.MicroscopicExamination = viewModel.MicroscopicExamination ?? string.Empty;
                existingForm.Impression = viewModel.Impression;
                existingForm.Pathologist = viewModel.Pathologist;
                existingForm.SpecialStains = viewModel.SpecialStains ?? string.Empty;
                existingForm.Advice = viewModel.Advice ?? string.Empty;
                existingForm.SignatureImage = viewModel.Signature; // Save the new signature

                // Handle file uploads
                if (viewModel.UploadedFiles != null && viewModel.UploadedFiles.Count > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    var existingFiles = existingForm.UploadedFilePath?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? [];

                    foreach (var file in viewModel.UploadedFiles)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            existingFiles.Add("/uploads/" + uniqueFileName);
                        }
                    }
                    existingForm.UploadedFilePath = string.Join(";", existingFiles);
                }

                // Update status and reviewer information
                existingForm.Status = FormStatus.DoctorReviewed;
                existingForm.ReviewedById = user.Id;
                existingForm.ReviewedAt = DateTime.UtcNow;
                existingForm.UpdatedAt = DateTime.UtcNow;

                // Add to form history
                var history = new FormHistory
                {
                    FormUHID = existingForm.UHID,
                    UserId = user.Id,
                    Action = "Reviewed",
                    Comments = "Form reviewed and updated by doctor"
                };
                _context.FormHistory.Add(history);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Form has been successfully reviewed and saved.";
                return RedirectToAction("Index", "Pathology"); // Redirect to the main dashboard
            }

            // If model state is invalid, we need to repopulate the view
            var formForDisplay = await _context.PathologyRequisitionForms.AsNoTracking().FirstOrDefaultAsync(f => f.UHID == viewModel.UHID);
             if (formForDisplay == null) return NotFound();

            // When returning the view with errors, we must ensure all fields are correctly repopulated.
            var fullViewModel = new DoctorReviewViewModel
            {
                FormDetails = formForDisplay, // The original, unmodified data for display
                DoctorInput = viewModel // The user's incorrect data to show back to them
            };

            return View(fullViewModel);
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle case where user is not found, e.g., redirect to login or show an error
                return RedirectToAction("Login", "Account");
            }
            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            var forms = await _context.PathologyRequisitionForms
                .Include(f => f.CreatedBy)
                .Include(f => f.ReviewedBy)
                .ToListAsync();

            return View(forms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PathologyRequisitionForm form)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found. Please log in again.");
                    return View(form);
                }

                form.CreatedById = user.Id;
                form.Status = FormStatus.Draft;

                _context.PathologyRequisitionForms.Add(form);
                await _context.SaveChangesAsync();

                // Add to form history
                var history = new FormHistory
                {
                    FormUHID = form.UHID,
                    UserId = user.Id,
                    Action = "Created",
                    Comments = "Form created"
                };
                _context.FormHistory.Add(history);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            var form = await _context.PathologyRequisitionForms
                .Include(f => f.CreatedBy)
                .Include(f => f.ReviewedBy)
                .FirstOrDefaultAsync(f => f.UHID == id);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PathologyRequisitionForm form)
        {
            if (id != form.UHID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found. Please log in again.");
                        return View(form);
                    }
                    
                    var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
                    var existingForm = await _context.PathologyRequisitionForms.FindAsync(id);

                    if (existingForm == null)
                    {
                        return NotFound();
                    }

                    // Update form properties
                    _context.Entry(existingForm).CurrentValues.SetValues(form);

                    // Add to form history
                    var history = new FormHistory
                    {
                        FormUHID = form.UHID,
                        UserId = user.Id,
                        Action = "Updated",
                        Comments = "Form updated"
                    };
                    _context.FormHistory.Add(history);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormExists(form.UHID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile(string uhid, string filePath)
        {
            if (string.IsNullOrEmpty(uhid) || string.IsNullOrEmpty(filePath))
                return RedirectToAction("Details", new { uhid });

            var form = await _context.PathologyRequisitionForms.FirstOrDefaultAsync(f => f.UHID == uhid);
            if (form == null)
                return RedirectToAction("Details", new { uhid });

            // Remove the file from disk
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullFilePath = Path.Combine(wwwrootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fullFilePath))
            {
                System.IO.File.Delete(fullFilePath);
            }

            // Remove the file path from UploadedFilePath
            var files = (form.UploadedFilePath ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            files.RemoveAll(f => f.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            form.UploadedFilePath = string.Join(";", files);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Pathology", new { uhid });
        }

        private bool FormExists(string id)
        {
            return _context.PathologyRequisitionForms.Any(e => e.UHID == id);
        }
    }
} 