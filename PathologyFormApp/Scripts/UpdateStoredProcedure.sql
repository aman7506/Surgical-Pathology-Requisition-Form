USE [pathology_db]
GO

-- First, add missing columns to PathologyForm table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PathologyForm]') AND name = 'CreatedById')
BEGIN
    ALTER TABLE [dbo].[PathologyForm]
    ADD [CreatedById] NVARCHAR(450) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PathologyForm]') AND name = 'ReviewedById')
BEGIN
    ALTER TABLE [dbo].[PathologyForm]
    ADD [ReviewedById] NVARCHAR(450) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PathologyForm]') AND name = 'ReviewedAt')
BEGIN
    ALTER TABLE [dbo].[PathologyForm]
    ADD [ReviewedAt] DATETIME2 NULL
END
GO

-- Create FormHistory table if it doesn't exist
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

    -- Add foreign key constraints
    ALTER TABLE [dbo].[FormHistory] WITH CHECK ADD CONSTRAINT [FK_FormHistory_PathologyForm] 
    FOREIGN KEY([FormUHID]) REFERENCES [dbo].[PathologyForm] ([UHID])

    ALTER TABLE [dbo].[FormHistory] WITH CHECK ADD CONSTRAINT [FK_FormHistory_AspNetUsers] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
END
GO

-- Create stored procedure for inserting form history
CREATE OR ALTER PROCEDURE [dbo].[sp_InsertFormHistory]
    @FormUHID NVARCHAR(50),
    @UserId NVARCHAR(450),
    @Action NVARCHAR(50),
    @Comments NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[FormHistory] (FormUHID, UserId, Action, Comments, Timestamp)
    VALUES (@FormUHID, @UserId, @Action, @Comments, GETUTCDATE())
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
    ORDER BY h.Timestamp DESC
END
GO

-- Create or update the main stored procedure for managing pathology forms
CREATE OR ALTER PROCEDURE [dbo].[sp_ManagePathologyForm]
    @Action NVARCHAR(20), -- 'INSERT', 'UPDATE', 'REVIEW', 'DELETE'
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
    DECLARE @IsValidAction BIT = 0;

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

        -- Validate action based on form existence and current status
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
            -- First delete related records
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

-- Ensure all datetime columns have proper defaults
IF NOT EXISTS (SELECT * FROM sys.default_constraints 
    WHERE parent_object_id = OBJECT_ID(N'[dbo].[PathologyForm]') 
    AND name = 'DF_PathologyForm_Date')
BEGIN
    ALTER TABLE [dbo].[PathologyForm] 
    ADD CONSTRAINT [DF_PathologyForm_Date] DEFAULT (GETDATE()) FOR [Date]
END
GO

-- Create the InsertPathologyForm stored procedure
CREATE OR ALTER PROCEDURE [dbo].[sp_InsertPathologyForm]
    @UHID varchar(50),
    @LabRefNo varchar(50),
    @Date datetime2(7),
    @Name varchar(100),
    @Age int,
    @Gender varchar(10),
    @CRNo varchar(50) = NULL,
    @OPD_IPD varchar(10) = NULL,
    @IPDNo varchar(50) = NULL,
    @Consultant varchar(100),
    @ClinicalDiagnosis varchar(MAX) = NULL,
    @DateTimeOfCollection datetime2(7) = NULL,
    @MensesOnset int = NULL,
    @LastingDays int = NULL,
    @Character varchar(50) = NULL,
    @LMP varchar(50) = NULL,
    @Gravida varchar(50) = NULL,
    @Para varchar(50) = NULL,
    @MenopauseAge int = NULL,
    @HormoneTherapy varchar(50) = NULL,
    @XRayUSGCTMRIFindings varchar(MAX) = NULL,
    @LaboratoryFindings varchar(MAX) = NULL,
    @OperativeFindings varchar(MAX) = NULL,
    @PostOperativeDiagnosis varchar(MAX) = NULL,
    @PreviousHPCytReport varchar(MAX) = NULL,
    @DateTimeOfReceivingSpecimen datetime2(7) = NULL,
    @NoOfSpecimenReceived int,
    @SpecimenNo varchar(50) = NULL,
    @MicroSectionNo varchar(50) = NULL,
    @SpecialStains varchar(100) = NULL,
    @GrossDescription varchar(MAX) = NULL,
    @MicroscopicExamination varchar(MAX) = NULL,
    @Impression varchar(MAX) = NULL,
    @Pathologist varchar(100),
    @PathologistDate datetime2(7) = NULL,
    @UploadedFilePath varchar(255) = NULL,
    @MenopauseYears int = NULL,
    @MensesCycle int = NULL,
    @SpecimenName varchar(100) = NULL,
    @Advice varchar(MAX) = NULL,
    @Status varchar(50) = 'Draft',
    @CreatedById varchar(450) = NULL,
    @Comments varchar(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    -- Validate required fields
    IF (
        @UHID IS NULL OR
        @LabRefNo IS NULL OR
        @Date IS NULL OR
        @Name IS NULL OR
        @Age IS NULL OR
        @Gender IS NULL OR
        @Consultant IS NULL OR
        @Pathologist IS NULL OR
        @NoOfSpecimenReceived IS NULL OR
        @CreatedById IS NULL
    )
    BEGIN
        RAISERROR('Required fields (UHID, LabRefNo, Date, Name, Age, Gender, Consultant, Pathologist, NoOfSpecimenReceived, CreatedById) must be provided.', 16, 1);
        RETURN;
    END

    -- Check for duplicate UHID
    IF EXISTS (SELECT 1 FROM [dbo].[PathologyForm] WHERE [UHID] = @UHID)
    BEGIN
        RAISERROR('UHID already exists. Please provide a unique UHID.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert the record
        INSERT INTO [dbo].[PathologyForm] (
            [UHID],
            [LabRefNo],
            [Date],
            [Name],
            [Age],
            [Gender],
            [CRNo],
            [OPD_IPD],
            [IPDNo],
            [Consultant],
            [ClinicalDiagnosis],
            [DateTimeOfCollection],
            [MensesOnset],
            [LastingDays],
            [Character],
            [LMP],
            [Gravida],
            [Para],
            [MenopauseAge],
            [HormoneTherapy],
            [XRayUSGCTMRIFindings],
            [LaboratoryFindings],
            [OperativeFindings],
            [PostOperativeDiagnosis],
            [PreviousHPCytReport],
            [DateTimeOfReceivingSpecimen],
            [NoOfSpecimenReceived],
            [SpecimenNo],
            [MicroSectionNo],
            [SpecialStains],
            [GrossDescription],
            [MicroscopicExamination],
            [Impression],
            [Pathologist],
            [PathologistDate],
            [UploadedFilePath],
            [MenopauseYears],
            [MensesCycle],
            [SpecimenName],
            [Advice],
            [Status],
            [CreatedById],
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES (
            @UHID,
            @LabRefNo,
            @Date,
            @Name,
            @Age,
            @Gender,
            @CRNo,
            @OPD_IPD,
            @IPDNo,
            @Consultant,
            @ClinicalDiagnosis,
            @DateTimeOfCollection,
            @MensesOnset,
            @LastingDays,
            @Character,
            @LMP,
            @Gravida,
            @Para,
            @MenopauseAge,
            @HormoneTherapy,
            @XRayUSGCTMRIFindings,
            @LaboratoryFindings,
            @OperativeFindings,
            @PostOperativeDiagnosis,
            @PreviousHPCytReport,
            @DateTimeOfReceivingSpecimen,
            @NoOfSpecimenReceived,
            @SpecimenNo,
            @MicroSectionNo,
            @SpecialStains,
            @GrossDescription,
            @MicroscopicExamination,
            @Impression,
            @Pathologist,
            @PathologistDate,
            @UploadedFilePath,
            @MenopauseYears,
            @MensesCycle,
            @SpecimenName,
            @Advice,
            @Status,
            @CreatedById,
            GETUTCDATE(),
            GETUTCDATE()
        );

        -- Add to form history
        INSERT INTO [dbo].[FormHistory] (
            [FormUHID],
            [UserId],
            [Action],
            [Comments],
            [Timestamp]
        )
        VALUES (
            @UHID,
            @CreatedById,
            'Created',
            @Comments,
            GETUTCDATE()
        );

        COMMIT TRANSACTION;
        PRINT 'Record saved successfully.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

