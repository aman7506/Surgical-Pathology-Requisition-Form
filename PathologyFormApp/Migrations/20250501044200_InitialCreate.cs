using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathologyFormApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PathologyRequisitionForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabRefNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CRNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OPD_IPD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPDNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Consultant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalDiagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameOfSpecimens = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTimeOfCollection = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MensesOnset = table.Column<int>(type: "int", nullable: true),
                    LastingDays = table.Column<int>(type: "int", nullable: true),
                    Character = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LMP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gravida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Para = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Menopause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenopauseAge = table.Column<int>(type: "int", nullable: true),
                    HormoneTherapy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XRayUSGCTMRIFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaboratoryFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperativeFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostOperativeDiagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousHPCytReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LabRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeOfReceivingSpecimen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoOfSpecimenReceived = table.Column<int>(type: "int", nullable: true),
                    SpecimenNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicroSectionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialStains = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrossDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicroscopicExamination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Impression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pathologist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathologistDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadedFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PathologyRequisitionForms", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PathologyRequisitionForms");
        }
    }
}
