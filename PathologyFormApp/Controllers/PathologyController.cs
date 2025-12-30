using Microsoft.AspNetCore.Mvc;
using PathologyFormApp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PathologyFormApp.Controllers
{
    [Authorize]
    public class PathologyController(PathologyContext context, UserManager<User> userManager) : Controller
    {
        private readonly PathologyContext _context = context;
        private readonly UserManager<User> _userManager = userManager;

        // GET: Pathology
        public async Task<IActionResult> Index(int? pageNumber, string searchUHID, string searchName, string searchLabRefNo, string searchFromDate, string searchToDate, int pageSize = 10)
        {
            ViewData["CurrentFilterUHID"] = searchUHID;
            ViewData["CurrentFilterName"] = searchName;
            ViewData["CurrentFilterLabRefNo"] = searchLabRefNo;
            ViewData["CurrentFilterFromDate"] = searchFromDate;
            ViewData["CurrentFilterToDate"] = searchToDate;

            var forms = _context.PathologyRequisitionForms.AsNoTracking();

            if (!string.IsNullOrEmpty(searchUHID))
            {
                forms = forms.Where(f => f.UHID.Contains(searchUHID));
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                forms = forms.Where(f => f.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchLabRefNo))
            {
                forms = forms.Where(f => f.LabRefNo.Contains(searchLabRefNo));
            }

            if (!string.IsNullOrEmpty(searchFromDate) && DateTime.TryParse(searchFromDate, out DateTime fromDate))
            {
                forms = forms.Where(f => f.Date >= fromDate);
            }

            if (!string.IsNullOrEmpty(searchToDate) && DateTime.TryParse(searchToDate, out DateTime toDate))
            {
                forms = forms.Where(f => f.Date <= toDate);
            }

            return View(await PaginatedList<PathologyRequisitionForm>.CreateAsync(forms, pageNumber ?? 1, pageSize));
        }

        // GET: Pathology/Details/5
        public async Task<IActionResult> Details(string? uhid)
        {
            if (string.IsNullOrEmpty(uhid))
            {
                return NotFound();
            }

            var pathologyForm = await _context.PathologyRequisitionForms
                .FirstOrDefaultAsync(m => m.UHID == uhid);
            if (pathologyForm == null)
            {
                return NotFound();
            }

            return View(pathologyForm);
        }

        // GET: Pathology/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pathology/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PathologyRequisitionForm form)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
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
                ModelState.AddModelError(string.Empty, "User not found.");
            }
            return View(form);
        }

        // GET: Pathology/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var form = await _context.PathologyRequisitionForms
                .Include(f => f.CreatedBy)
                .Include(f => f.ReviewedBy)
                .FirstOrDefaultAsync(f => f.UHID == id);

            if (form == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(form.UploadedFilePath))
            {
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadedFiles = form.UploadedFilePath.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(filePath => {
                        var fullPath = Path.Combine(wwwrootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        DateTime? timestamp = System.IO.File.Exists(fullPath) ? System.IO.File.GetLastWriteTime(fullPath) : (DateTime?)null;
                        return new UploadedFileInfo { FilePath = filePath, Timestamp = timestamp };
                    })
                    .ToList();
                ViewBag.UploadedFilesEdit = uploadedFiles;
            }
            else
            {
                ViewBag.UploadedFilesEdit = new List<UploadedFileInfo>();
            }

            return View(form);
        }

        // POST: Pathology/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PathologyRequisitionForm form, List<IFormFile> fileUpload)
        {
            if (id != form.UHID)
            {
                return NotFound();
            }

            // These navigation properties are not expected to be bound from the form.
            // We remove them from model state to prevent validation errors.
            ModelState.Remove("CreatedBy");
            ModelState.Remove("ReviewedBy");

            if (ModelState.IsValid)
            {
                var existingForm = await _context.PathologyRequisitionForms.FindAsync(id);
                if (existingForm == null)
                {
                    return NotFound();
                }

                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found.");
                        return View(form);
                    }

                    // Manually update properties to avoid overwriting critical data
                    existingForm.LabRefNo = form.LabRefNo;
                    existingForm.Date = form.Date;
                    existingForm.Name = form.Name;
                    existingForm.Age = form.Age;
                    existingForm.Gender = form.Gender;
                    existingForm.CRNo = form.CRNo;
                    existingForm.OPD_IPD = form.OPD_IPD;
                    existingForm.IPDNo = form.IPDNo;
                    existingForm.Consultant = form.Consultant;
                    existingForm.MensesOnset = form.MensesOnset;
                    existingForm.MensesCycle = form.MensesCycle;
                    existingForm.LastingDays = form.LastingDays;
                    existingForm.Character = form.Character;
                    existingForm.LMP = form.LMP;
                    existingForm.Gravida = form.Gravida;
                    existingForm.Para = form.Para;
                    existingForm.MenopauseAge = form.MenopauseAge;
                    existingForm.MenopauseYears = form.MenopauseYears;
                    existingForm.HormoneTherapy = form.HormoneTherapy;
                    existingForm.ClinicalDiagnosis = form.ClinicalDiagnosis;
                    existingForm.XRayUSGCTMRIFindings = form.XRayUSGCTMRIFindings;
                    existingForm.LaboratoryFindings = form.LaboratoryFindings;
                    existingForm.OperativeFindings = form.OperativeFindings;
                    existingForm.PostOperativeDiagnosis = form.PostOperativeDiagnosis;
                    existingForm.PreviousHPCytReport = form.PreviousHPCytReport;
                    existingForm.SpecimenName = form.SpecimenName;
                    existingForm.DateTimeOfCollection = form.DateTimeOfCollection;
                    existingForm.DateTimeOfReceivingSpecimen = form.DateTimeOfReceivingSpecimen;
                    existingForm.NoOfSpecimenReceived = form.NoOfSpecimenReceived;
                    existingForm.SpecimenNo = form.SpecimenNo;
                    existingForm.MicroSectionNo = form.MicroSectionNo;
                    existingForm.GrossDescription = form.GrossDescription;
                    existingForm.MicroscopicExamination = form.MicroscopicExamination;
                    existingForm.Impression = form.Impression;
                    existingForm.SpecialStains = form.SpecialStains;
                    existingForm.Advice = form.Advice;
                    existingForm.Pathologist = form.Pathologist;
                    existingForm.PathologistDate = form.PathologistDate;
                    existingForm.UpdatedAt = DateTime.UtcNow;


                    // Handle file uploads
                    if (fileUpload != null && fileUpload.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var existingFiles = (existingForm.UploadedFilePath ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

                        foreach (var file in fileUpload)
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

            // If model state is invalid, re-populate the navigation properties for the view
            var formWithNav = await _context.PathologyRequisitionForms.AsNoTracking()
                .Include(f => f.CreatedBy).Include(f => f.ReviewedBy)
                .FirstOrDefaultAsync(f => f.UHID == id);
            
            if (formWithNav != null)
            {
                form.CreatedBy = formWithNav.CreatedBy;
                form.ReviewedBy = formWithNav.ReviewedBy;
            }

            return View(form);
        }

        // GET: Pathology/Delete/5
        public async Task<IActionResult> Delete(string? uhid)
        {
            if (string.IsNullOrEmpty(uhid))
            {
                return NotFound();
            }

            var pathologyForm = await _context.PathologyRequisitionForms
                .FirstOrDefaultAsync(m => m.UHID == uhid);
            if (pathologyForm == null)
            {
                return NotFound();
            }

            return View(pathologyForm);
        }

        // POST: Pathology/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string uhid)
        {
            var pathologyForm = await _context.PathologyRequisitionForms
                .FirstOrDefaultAsync(m => m.UHID == uhid);
            if (pathologyForm != null)
            {
                _context.PathologyRequisitionForms.Remove(pathologyForm);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Form deleted successfully.";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool FormExists(string id)
        {
            return _context.PathologyRequisitionForms.Any(e => e.UHID == id);
        }

        public class FileDeleteModel
        {
            public string? Uhid { get; set; }
            public string? FilePath { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUploadedFile([FromBody] FileDeleteModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Uhid) || string.IsNullOrEmpty(model.FilePath))
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            var form = await _context.PathologyRequisitionForms.FirstOrDefaultAsync(f => f.UHID == model.Uhid);
            if (form == null)
            {
                return Json(new { success = false, message = "Form not found." });
            }

            try
            {
                var files = (form.UploadedFilePath ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (files.RemoveAll(f => f.Equals(model.FilePath, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    form.UploadedFilePath = string.Join(";", files);

                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    // Basic path traversal check
                    if (model.FilePath.StartsWith("/uploads/"))
                    {
                        var fullFilePath = Path.Combine(wwwrootPath, model.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(fullFilePath))
                        {
                            System.IO.File.Delete(fullFilePath);
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "File deleted successfully." });
                }
                
                return Json(new { success = false, message = "File not found on this record." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while deleting the file." });
            }
        }
    }
}
