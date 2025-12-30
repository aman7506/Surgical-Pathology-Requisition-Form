USE [master]
GO

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'pathology_db')
BEGIN
    CREATE DATABASE [pathology_db]
END
GO

USE [pathology_db]
GO

-- Create PathologyForm table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PathologyForm')
BEGIN
    CREATE TABLE [dbo].[PathologyForm](
        [UHID] [nvarchar](50) NOT NULL,
        [LabRefNo] [nvarchar](50) NOT NULL,
        [Date] [datetime2](7) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Age] [int] NOT NULL,
        [Gender] [nvarchar](10) NOT NULL,
        [CRNo] [nvarchar](50) NULL,
        [OPD_IPD] [nvarchar](10) NULL,
        [IPDNo] [nvarchar](50) NULL,
        [Consultant] [nvarchar](100) NOT NULL,
        [ClinicalDiagnosis] [nvarchar](max) NULL,
        [DateTimeOfCollection] [datetime2](7) NOT NULL,
        [MensesOnset] [int] NULL,
        [LastingDays] [int] NULL,
        [Character] [nvarchar](50) NULL,
        [LMP] [nvarchar](50) NULL,
        [Gravida] [nvarchar](50) NULL,
        [Para] [nvarchar](50) NULL,
        [MenopauseAge] [int] NULL,
        [HormoneTherapy] [nvarchar](50) NULL,
        [XRayUSGCTMRIFindings] [nvarchar](max) NULL,
        [LaboratoryFindings] [nvarchar](max) NULL,
        [OperativeFindings] [nvarchar](max) NULL,
        [PostOperativeDiagnosis] [nvarchar](max) NULL,
        [PreviousHPCytReport] [nvarchar](max) NULL,
        [DateTimeOfReceivingSpecimen] [datetime2](7) NOT NULL,
        [NoOfSpecimenReceived] [int] NOT NULL,
        [SpecimenNo] [nvarchar](50) NOT NULL,
        [MicroSectionNo] [nvarchar](50) NOT NULL,
        [SpecialStains] [nvarchar](100) NULL,
        [GrossDescription] [nvarchar](max) NULL,
        [MicroscopicExamination] [nvarchar](max) NULL,
        [Impression] [nvarchar](max) NULL,
        [Pathologist] [nvarchar](100) NOT NULL,
        [PathologistDate] [datetime2](7) NOT NULL,
        [UploadedFilePath] [nvarchar](255) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [DateTimeOfProcessing] [datetime2](7) NOT NULL,
        [MenopauseYears] [int] NULL,
        [MensesCycle] [int] NULL,
        [SpecimenName] [nvarchar](100) NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        [Advice] [nvarchar](max) NULL,
        [Status] [nvarchar](50) NOT NULL DEFAULT 'Draft',
        [CreatedById] [nvarchar](450) NOT NULL,
        [ReviewedById] [nvarchar](450) NULL,
        [ReviewedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_PathologyForm] PRIMARY KEY CLUSTERED ([UHID] ASC)
    )
END
GO

-- Create FormHistory table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FormHistory')
BEGIN
    CREATE TABLE [dbo].[FormHistory](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [FormUHID] [nvarchar](50) NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [Action] [nvarchar](50) NOT NULL,
        [Comments] [nvarchar](max) NULL,
        [Timestamp] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_FormHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create stored procedure for managing forms
CREATE OR ALTER PROCEDURE [dbo].[sp_ManagePathologyForm]
    @Action NVARCHAR(20),
    @UHID NVARCHAR(50),
    @LabRefNo NVARCHAR(50) = NULL,
    @Date DATETIME2 = NULL,
    @Name NVARCHAR(100) = NULL,
    @Age INT = NULL,
    @Gender NVARCHAR(10) = NULL,
    @CRNo NVARCHAR(50) = NULL,
    @OPD_IPD NVARCHAR(10) = NULL,
    @IPDNo NVARCHAR(50) = NULL,
    @Consultant NVARCHAR(100) = NULL,
    @ClinicalDiagnosis NVARCHAR(MAX) = NULL,
    @MensesOnset INT = NULL,
    @LastingDays INT = NULL,
    @Character NVARCHAR(50) = NULL,
    @LMP NVARCHAR(50) = NULL,
    @Gravida NVARCHAR(50) = NULL,
    @Para NVARCHAR(50) = NULL,
    @MenopauseAge INT = NULL,
    @HormoneTherapy NVARCHAR(50) = NULL,
    @XRayUSGCTMRIFindings NVARCHAR(MAX) = NULL,
    @LaboratoryFindings NVARCHAR(MAX) = NULL,
    @OperativeFindings NVARCHAR(MAX) = NULL,
    @PostOperativeDiagnosis NVARCHAR(MAX) = NULL,
    @PreviousHPCytReport NVARCHAR(MAX) = NULL,
    @NoOfSpecimenReceived INT = NULL,
    @SpecimenNo NVARCHAR(50) = NULL,
    @MicroSectionNo NVARCHAR(50) = NULL,
    @SpecialStains NVARCHAR(100) = NULL,
    @GrossDescription NVARCHAR(MAX) = NULL,
    @MicroscopicExamination NVARCHAR(MAX) = NULL,
    @Impression NVARCHAR(MAX) = NULL,
    @Pathologist NVARCHAR(100) = NULL,
    @UploadedFilePath NVARCHAR(255) = NULL,
    @MenopauseYears INT = NULL,
    @MensesCycle INT = NULL,
    @SpecimenName NVARCHAR(100) = NULL,
    @Advice NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(50) = NULL,
    @UserId NVARCHAR(450) = NULL,
    @Comments NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @FormExists BIT;
    DECLARE @CurrentStatus NVARCHAR(50);

    BEGIN TRY
        -- Validate required parameters
        IF @Action IS NULL OR @UHID IS NULL
        BEGIN
            RAISERROR('Action and UHID are required parameters.', 16, 1);
            RETURN;
        END

        -- Check if form exists
        SELECT @FormExists = 1, @CurrentStatus = Status
        FROM [dbo].[PathologyForm]
        WHERE UHID = @UHID;

        -- Validate action based on form existence
        IF @Action = 'INSERT' AND @FormExists = 1
        BEGIN
            RAISERROR('Form with this UHID already exists.', 16, 1);
            RETURN;
        END

        IF @Action IN ('UPDATE', 'REVIEW', 'DELETE') AND @FormExists = 0
        BEGIN
            RAISERROR('Form not found.', 16, 1);
            RETURN;
        END

        -- Begin transaction
        BEGIN TRANSACTION;

        -- Perform the requested action
        IF @Action = 'INSERT'
        BEGIN
            -- Validate required fields for insert
            IF @LabRefNo IS NULL OR @Name IS NULL OR @Age IS NULL OR @Gender IS NULL
            BEGIN
                RAISERROR('Required fields (LabRefNo, Name, Age, Gender) cannot be null for new forms.', 16, 1);
                RETURN;
            END

            INSERT INTO [dbo].[PathologyForm] (
                UHID, LabRefNo, Date, Name, Age, Gender, CRNo, OPD_IPD, IPDNo,
                Consultant, ClinicalDiagnosis, DateTimeOfCollection, MensesOnset, LastingDays,
                Character, LMP, Gravida, Para, MenopauseAge, HormoneTherapy,
                XRayUSGCTMRIFindings, LaboratoryFindings, OperativeFindings, PostOperativeDiagnosis,
                PreviousHPCytReport, DateTimeOfReceivingSpecimen, NoOfSpecimenReceived, SpecimenNo,
                MicroSectionNo, SpecialStains, GrossDescription, MicroscopicExamination,
                Impression, Pathologist, PathologistDate, UploadedFilePath, CreatedAt,
                DateTimeOfProcessing, MenopauseYears, MensesCycle, SpecimenName, UpdatedAt,
                Advice, Status, CreatedById
            )
            VALUES (
                @UHID, @LabRefNo, @Date, @Name, @Age, @Gender, @CRNo, @OPD_IPD, @IPDNo,
                @Consultant, @ClinicalDiagnosis, GETDATE(), @MensesOnset, @LastingDays,
                @Character, @LMP, @Gravida, @Para, @MenopauseAge, @HormoneTherapy,
                @XRayUSGCTMRIFindings, @LaboratoryFindings, @OperativeFindings, @PostOperativeDiagnosis,
                @PreviousHPCytReport, GETDATE(), @NoOfSpecimenReceived, @SpecimenNo,
                @MicroSectionNo, @SpecialStains, @GrossDescription, @MicroscopicExamination,
                @Impression, @Pathologist, GETDATE(), @UploadedFilePath, GETDATE(),
                GETDATE(), @MenopauseYears, @MensesCycle, @SpecimenName, GETDATE(),
                @Advice, ISNULL(@Status, 'Draft'), @UserId
            );
        END
        ELSE IF @Action = 'UPDATE'
        BEGIN
            UPDATE [dbo].[PathologyForm]
            SET 
                LabRefNo = ISNULL(@LabRefNo, LabRefNo),
                Date = ISNULL(@Date, Date),
                Name = ISNULL(@Name, Name),
                Age = ISNULL(@Age, Age),
                Gender = ISNULL(@Gender, Gender),
                CRNo = ISNULL(@CRNo, CRNo),
                OPD_IPD = ISNULL(@OPD_IPD, OPD_IPD),
                IPDNo = ISNULL(@IPDNo, IPDNo),
                Consultant = ISNULL(@Consultant, Consultant),
                ClinicalDiagnosis = ISNULL(@ClinicalDiagnosis, ClinicalDiagnosis),
                MensesOnset = ISNULL(@MensesOnset, MensesOnset),
                LastingDays = ISNULL(@LastingDays, LastingDays),
                Character = ISNULL(@Character, Character),
                LMP = ISNULL(@LMP, LMP),
                Gravida = ISNULL(@Gravida, Gravida),
                Para = ISNULL(@Para, Para),
                MenopauseAge = ISNULL(@MenopauseAge, MenopauseAge),
                HormoneTherapy = ISNULL(@HormoneTherapy, HormoneTherapy),
                XRayUSGCTMRIFindings = ISNULL(@XRayUSGCTMRIFindings, XRayUSGCTMRIFindings),
                LaboratoryFindings = ISNULL(@LaboratoryFindings, LaboratoryFindings),
                OperativeFindings = ISNULL(@OperativeFindings, OperativeFindings),
                PostOperativeDiagnosis = ISNULL(@PostOperativeDiagnosis, PostOperativeDiagnosis),
                PreviousHPCytReport = ISNULL(@PreviousHPCytReport, PreviousHPCytReport),
                NoOfSpecimenReceived = ISNULL(@NoOfSpecimenReceived, NoOfSpecimenReceived),
                SpecimenNo = ISNULL(@SpecimenNo, SpecimenNo),
                MicroSectionNo = ISNULL(@MicroSectionNo, MicroSectionNo),
                SpecialStains = ISNULL(@SpecialStains, SpecialStains),
                GrossDescription = ISNULL(@GrossDescription, GrossDescription),
                MicroscopicExamination = ISNULL(@MicroscopicExamination, MicroscopicExamination),
                Impression = ISNULL(@Impression, Impression),
                Pathologist = ISNULL(@Pathologist, Pathologist),
                UploadedFilePath = ISNULL(@UploadedFilePath, UploadedFilePath),
                MenopauseYears = ISNULL(@MenopauseYears, MenopauseYears),
                MensesCycle = ISNULL(@MensesCycle, MensesCycle),
                SpecimenName = ISNULL(@SpecimenName, SpecimenName),
                Advice = ISNULL(@Advice, Advice),
                Status = ISNULL(@Status, Status),
                UpdatedAt = GETDATE()
            WHERE UHID = @UHID;
        END
        ELSE IF @Action = 'REVIEW'
        BEGIN
            UPDATE [dbo].[PathologyForm]
            SET 
                Status = 'DoctorReviewed',
                ReviewedById = @UserId,
                ReviewedAt = GETDATE(),
                UpdatedAt = GETDATE()
            WHERE UHID = @UHID;
        END
        ELSE IF @Action = 'DELETE'
        BEGIN
            DELETE FROM [dbo].[FormHistory] WHERE FormUHID = @UHID;
            DELETE FROM [dbo].[PathologyForm] WHERE UHID = @UHID;
        END

        -- Add to form history
        IF @Action IN ('INSERT', 'UPDATE', 'REVIEW')
        BEGIN
            INSERT INTO [dbo].[FormHistory] (
                FormUHID,
                UserId,
                Action,
                Comments,
                Timestamp
            )
            VALUES (
                @UHID,
                @UserId,
                @Action,
                @Comments,
                GETUTCDATE()
            );
        END

        COMMIT TRANSACTION;

        -- Return success message
        SELECT 
            'Success' AS Status,
            'Operation completed successfully' AS Message,
            @UHID AS UHID,
            @Status AS NewStatus;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_SEVERITY() AS ErrorSeverity,
            ERROR_STATE() AS ErrorState;

        THROW;
    END CATCH
END
GO

-- Create stored procedure for getting form history
CREATE OR ALTER PROCEDURE [dbo].[sp_GetFormHistory]
    @FormUHID NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        h.Id,
        h.FormUHID,
        h.UserId,
        u.UserName,
        h.Action,
        h.Comments,
        h.Timestamp
    FROM [dbo].[FormHistory] h
    INNER JOIN [dbo].[AspNetUsers] u ON h.UserId = u.Id
    WHERE h.FormUHID = @FormUHID
    ORDER BY h.Timestamp DESC;
END
GO

-- Create stored procedure for getting forms by status
CREATE OR ALTER PROCEDURE [dbo].[sp_GetFormsByStatus]
    @Status NVARCHAR(50) = NULL,
    @UserId NVARCHAR(450) = NULL,
    @IsDoctor BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pf.*,
        u1.UserName AS CreatedByUserName,
        u2.UserName AS ReviewedByUserName
    FROM [dbo].[PathologyForm] pf
    LEFT JOIN [dbo].[AspNetUsers] u1 ON pf.CreatedById = u1.Id
    LEFT JOIN [dbo].[AspNetUsers] u2 ON pf.ReviewedById = u2.Id
    WHERE 
        (@Status IS NULL OR pf.Status = @Status)
        AND (
            @UserId IS NULL 
            OR pf.CreatedById = @UserId 
            OR (@IsDoctor = 1 AND pf.Status IN ('NurseSubmitted', 'Draft'))
        )
    ORDER BY pf.CreatedAt DESC;
END
GO 