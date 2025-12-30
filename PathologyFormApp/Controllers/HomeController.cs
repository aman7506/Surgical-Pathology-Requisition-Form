using Microsoft.AspNetCore.Mvc;
using PathologyFormApp.Models;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace PathologyFormApp.Controllers
{
    [Authorize]
    public class HomeController(IConfiguration configuration, ILogger<HomeController> logger) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<HomeController> _logger = logger;

        public async Task<IActionResult> Index(
            string searchUHID,
            string searchName,
            string searchLabRefNo,
            string searchDate,
            int? pageNumber,
            int? pageSize)
        {
            ViewData["CurrentFilterUHID"] = searchUHID;
            ViewData["CurrentFilterName"] = searchName;
            ViewData["CurrentFilterLabRefNo"] = searchLabRefNo;
            ViewData["CurrentFilterDate"] = searchDate;

            int currentPageSize = pageSize ?? 10; // Default page size
            int currentPageNumber = pageNumber ?? 1; // Default to first page

            await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var sql = "SELECT * FROM PathologyForm WHERE 1=1";
            var countSql = "SELECT COUNT(*) FROM PathologyForm WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(searchUHID))
            {
                sql += " AND UHID LIKE @SearchUHID";
                countSql += " AND UHID LIKE @SearchUHID";
                parameters.Add("SearchUHID", "%" + searchUHID + "%");
            }
            if (!string.IsNullOrEmpty(searchName))
            {
                sql += " AND Name LIKE @SearchName";
                countSql += " AND Name LIKE @SearchName";
                parameters.Add("SearchName", "%" + searchName + "%");
            }
            if (!string.IsNullOrEmpty(searchLabRefNo))
            {
                sql += " AND LabRefNo LIKE @SearchLabRefNo";
                countSql += " AND LabRefNo LIKE @SearchLabRefNo";
                parameters.Add("SearchLabRefNo", "%" + searchLabRefNo + "%");
            }
            if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out DateTime date))
            {
                sql += " AND CAST(Date AS DATE) = @SearchDate";
                countSql += " AND CAST(Date AS DATE) = @SearchDate";
                parameters.Add("SearchDate", date.Date);
            }

            // Add pagination to the SQL query
            sql += " ORDER BY Date DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (currentPageNumber - 1) * currentPageSize);
            parameters.Add("PageSize", currentPageSize);

            var forms = await connection.QueryAsync<PathologyRequisitionForm>(sql, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            var paginatedList = new PaginatedList<PathologyRequisitionForm>(forms.ToList(), totalCount, currentPageNumber, currentPageSize);

            ViewData["PageSize"] = currentPageSize;

            return View(paginatedList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PathologyRequisitionForm model)
        {
            if (model == null) return BadRequest();
            if (!ModelState.IsValid) return View("Index", model);

            if (Request.Form.Files.Count > 0)
            {
                var fileUpload = Request.Form.Files.ToList();
                if (fileUpload != null)
                {
                    foreach (var file in fileUpload)
                    {
                        if (file.Length > 10 * 1024 * 1024) // 10MB limit
                        {
                            ModelState.AddModelError("", "File size exceeds 10MB limit");
                            return View("Index", model);
                        }
                        // Add file extension validation if needed
                    }
                }
            }

            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("Date", model.Date);
                parameters.Add("LabRefNo", model.LabRefNo);
                parameters.Add("Name", model.Name);
                parameters.Add("Age", model.Age);
                parameters.Add("Gender", model.Gender);
                parameters.Add("CRNo", model.CRNo);
                parameters.Add("OPD_IPD", model.OPD_IPD);
                parameters.Add("IPDNo", model.IPDNo);
                parameters.Add("Consultant", model.Consultant);
                parameters.Add("ClinicalDiagnosis", model.ClinicalDiagnosis);
                parameters.Add("DateTimeOfCollection", model.DateTimeOfCollection);
                parameters.Add("MensesOnset", model.MensesOnset);
                parameters.Add("LastingDays", model.LastingDays);
                parameters.Add("Character", model.Character);
                parameters.Add("LMP", model.LMP);
                parameters.Add("Gravida", model.Gravida);
                parameters.Add("Para", model.Para);
                parameters.Add("HormoneTherapy", model.HormoneTherapy);
                parameters.Add("XRayUSGCTMRIFindings", model.XRayUSGCTMRIFindings);
                parameters.Add("LaboratoryFindings", model.LaboratoryFindings);
                parameters.Add("OperativeFindings", model.OperativeFindings);
                parameters.Add("DateTimeOfReceivingSpecimen", model.DateTimeOfReceivingSpecimen);
                parameters.Add("NoOfSpecimenReceived", model.NoOfSpecimenReceived);
                parameters.Add("SpecimenNo", model.SpecimenNo);
                parameters.Add("MicroSectionNo", model.MicroSectionNo);
                parameters.Add("SpecialStains", model.SpecialStains);
                parameters.Add("GrossDescription", model.GrossDescription);
                parameters.Add("MicroscopicExamination", model.MicroscopicExamination);
                parameters.Add("Impression", model.Impression);
                parameters.Add("PreviousHPCytReport", model.PreviousHPCytReport);
                parameters.Add("Pathologist", model.Pathologist);
                parameters.Add("PathologistDate", model.PathologistDate);
                parameters.Add("UploadedFilePath", model.UploadedFilePath);
                parameters.Add("MenopauseYears", model.MenopauseYears ?? 0);
                parameters.Add("MensesCycle", model.MensesCycle);
                parameters.Add("SpecimenName", model.SpecimenName);
                parameters.Add("Advice", model.Advice);
                parameters.Add("Status", model.Status);

                await connection.ExecuteAsync("sp_InsertPathologyForm", parameters);
                
                TempData["Success"] = "Form submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create failed");
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View("Index", model);
            }
        }

        [Route("test-db")]
        public async Task<IActionResult> TestDbConnection()
        {
            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                
                // Verify pathology_db exists and is accessible
                var dbName = await connection.ExecuteScalarAsync<string>(
                    "IF DB_ID('pathology_db') IS NOT NULL SELECT 'pathology_db' ELSE SELECT 'Database not found'");
                    
                return Content($"Database connection successful. Current DB: {dbName}");
            }
            catch (Exception ex)
            {
                return Content($"Connection failed: {ex.Message}");
            }
        }

        public async Task<IActionResult> Details(string uhid)
        {
            if (string.IsNullOrEmpty(uhid)) return BadRequest();

            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var form = await connection.QueryFirstOrDefaultAsync<PathologyRequisitionForm>(
                    "SELECT * FROM PathologyForm WHERE UHID = @UHID", 
                    new { UHID = uhid });
                
                return form == null ? NotFound() : View(form);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database error");
                return StatusCode(500, "Database error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                return StatusCode(500, "Internal error");
            }
        }

        public async Task<IActionResult> TestDapper()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var result = await connection.QueryFirstOrDefaultAsync<int>("SELECT 1");
            return Content($"Dapper works! Result: {result}");
        }
    }
}