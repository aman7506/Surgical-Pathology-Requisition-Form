using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PathologyFormApp.Models; // Make sure this using directive is present
using System; // Add this using directive for StringComparison
using Microsoft.Data.SqlClient; // For SqlConnection
using Dapper; // For Dapper extensions
using Microsoft.Extensions.Configuration; // For IConfiguration
using System.Collections.Generic; // Add for IEnumerable type hint
using Microsoft.AspNetCore.Authorization;

namespace PathologyFormApp.Controllers
{
    [Route("[controller]")]
    [Authorize] // Add authorization
    public class SpecimenController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        // Helper to get connection string
        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")!;
        }

        // GET /Specimen - Main specimen management page
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();
                
                var specimenTypes = await connection.QueryAsync<SpecimenType>(
                    "[dbo].[sp_GetAllSpecimens]", 
                    commandType: System.Data.CommandType.StoredProcedure
                );
                
                return View(specimenTypes.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading specimen types: {ex.Message}";
                return View(new List<SpecimenType>());
            }
        }

        // GET /Specimen/GetAll - Uses Stored Procedure
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            string sql = "[dbo].[sp_GetAllSpecimens]"; // Stored procedure name

            using var connection = new SqlConnection(GetConnectionString());
            await connection.OpenAsync();
            // Dapper query to execute stored procedure
            IEnumerable<SpecimenType> specimenTypes = await connection.QueryAsync<SpecimenType>(sql, commandType: System.Data.CommandType.StoredProcedure);
             // Select Id and Name to match JavaScript expectation
             var result = specimenTypes.Select(s => new { s.Id, s.Name }).ToList();
            return Json(result);
        }

        // POST /Specimen/Add - Uses Stored Procedure
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] SpecimenType specimenType)
        {
            if (specimenType == null || string.IsNullOrEmpty(specimenType.Name))
            {
                return BadRequest(new { message = "Specimen name is required." });
            }

            string sql = "[dbo].[sp_AddSpecimen]"; // Stored procedure name

            using var connection = new SqlConnection(GetConnectionString());
            await connection.OpenAsync();
            // Dapper execute to run stored procedure
            // The SP returns NewId, so use QueryFirstOrDefaultAsync to capture it
            var newId = await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { specimenType.Name },
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (newId.HasValue && newId.Value > 0)
            {
                 // Assuming SP returns the ID of the newly added specimen
                 return Ok(new { message = "Specimen type added successfully!", id = newId.Value, name = specimenType.Name });
            }
            else if (!newId.HasValue) // SP might not return an ID if item exists
            {
                 // Refetching by name to get the ID if SP didn't return it on conflict
                 // Use string.Equals for case-insensitive comparison and nameof for property name
                 var existingSpecimen = await connection.QueryFirstOrDefaultAsync<SpecimenType>(
                    "SELECT Id, Name FROM SpecimenTypes WHERE Name = @Name",
                    new { specimenType.Name } // Parameter name matches SP parameter
                 );
                 if(existingSpecimen != null)
                 {
                     return Conflict(new { message = $"Specimen type '{specimenType.Name}' already exists.", id = existingSpecimen.Id, name = existingSpecimen.Name });
                 } else {
                      // Handle cases where SP didn't add and we couldn't find the existing one
                       return StatusCode(500, new { message = "Failed to add specimen type and could not find existing." });
                 }
            }
            else {
                 // Handle cases where SP returns 0 or a non-positive value indicating an issue
                 return StatusCode(500, new { message = "Failed to add specimen type." });
            }
        }

        // POST /Specimen/Delete - Uses Stored Procedure
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] SpecimenType specimenType)
        {
             if (specimenType == null || specimenType.Id <= 0)
             {
                 return BadRequest(new { message = "Specimen type ID is required." });
             }

            string sql = "[dbo].[sp_RemoveSpecimen]"; // Stored procedure name

            using var connection = new SqlConnection(GetConnectionString());
            await connection.OpenAsync();
            // Dapper execute to run stored procedure
            int rowsAffected = await connection.ExecuteAsync(
                sql,
                new { specimenType.Id },
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (rowsAffected > 0)
            {
                 return Ok(new { message = "Specimen type deleted successfully!" });
            }
            else
            {
                 return NotFound(new { message = "Specimen type not found or not deleted." });
            }
        }

        // POST /Specimen/AddFromForm - For form-based addition
        [HttpPost("AddFromForm")]
        public async Task<IActionResult> AddFromForm([FromForm] SpecimenType specimenType)
        {
            if (specimenType == null || string.IsNullOrEmpty(specimenType.Name))
            {
                TempData["ErrorMessage"] = "Specimen name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();
                
                var newId = await connection.QueryFirstOrDefaultAsync<int?>(
                    "[dbo].[sp_AddSpecimen]",
                    new { specimenType.Name },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (newId.HasValue && newId.Value > 0)
                {
                    TempData["SuccessMessage"] = $"Specimen type '{specimenType.Name}' added successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Specimen type '{specimenType.Name}' already exists.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding specimen type: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST /Specimen/DeleteFromForm - For form-based deletion
        [HttpPost("DeleteFromForm")]
        public async Task<IActionResult> DeleteFromForm(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid specimen type ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();
                
                int rowsAffected = await connection.ExecuteAsync(
                    "[dbo].[sp_RemoveSpecimen]",
                    new { Id = id },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "Specimen type deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Specimen type not found or could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting specimen type: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 