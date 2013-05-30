SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[changeGroupID]
	@oldGroupID int,
	@newGroupID int
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE _baseline SET group_id = @newGroupID WHERE group_id = @oldGroupID;
	UPDATE _trial SET group_id = @newGroupID WHERE group_id = @oldGroupID;
	UPDATE _szenario_mean_time SET group_id = @newGroupID WHERE group_id = @oldGroupID;
	DELETE FROM _group WHERE id = @oldGroupID;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[changeGroupName]
	@groupID int,
	@newGroupName varchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE _group SET group_name = @newGroupName WHERE id = @groupID;
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[changeSubjectID]
	@oldSubjectID int,
	@newSubjectID int
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE _baseline SET subject_id = @newSubjectID WHERE subject_id = @oldSubjectID;
	UPDATE _trial SET subject_id = @newSubjectID WHERE subject_id = @oldSubjectID;
	UPDATE _szenario_mean_time SET subject_id = @newSubjectID WHERE subject_id = @oldSubjectID;
	DELETE FROM _subject WHERE id = @oldSubjectID;
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[changeSubjectName]
	@subjectID int,
	@newSubjectName varchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE _subject SET subject_name = @newSubjectName WHERE id = @subjectID;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[changeSubjectSubjectID]
	@subjectID int,
	@newSubjectSubjectID varchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE _subject SET subject_id = @newSubjectSubjectID WHERE id = @subjectID;
END




GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[checkMeasureFileHash]
	@measureFileHash varchar(max),
	@hashExists bit OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS 
	(
		SELECT * FROM dbo._measure_file WHERE 
		file_hash = @measureFileHash
	)
		SELECT @hashExists = 1;
	ELSE
		SELECT @hashExists = 0;
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[cleanOrphanedEntries]
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM _szenario_mean_time_data WHERE szenario_mean_time_id NOT IN (SELECT id FROM _szenario_mean_time);
	DELETE FROM _baseline_data WHERE baseline_id NOT IN (SELECT id FROM _baseline);
	DELETE FROM _measure_data_raw WHERE trial_id NOT IN (SELECT id FROM _trial);
	DELETE FROM _measure_data_filtered WHERE trial_id NOT IN (SELECT id FROM _trial);
	DELETE FROM _measure_data_normalized WHERE trial_id NOT IN (SELECT id FROM _trial);
	DELETE FROM _velocity_data_filtered WHERE trial_id NOT IN (SELECT id FROM _trial);
	DELETE FROM _velocity_data_normalized WHERE trial_id NOT IN (SELECT id FROM _trial);
	DELETE FROM _statistic_data WHERE trial_id NOT IN (SELECT id FROM _trial);
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[deleteMeasureFile]
	@measureFileID int
AS
BEGIN
	SET NOCOUNT ON;
	RAISERROR('--------------BEGIN--------------', 10, 1) WITH NOWAIT;
	RAISERROR('DELETE _baseline_data...', 10, 1) WITH NOWAIT;
	DELETE _baseline_data FROM (_baseline INNER JOIN _baseline_data on _baseline_data.baseline_id = _baseline.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _statistic_data...', 10, 1) WITH NOWAIT;
	DELETE _statistic_data FROM (_trial INNER JOIN _statistic_data on _statistic_data.trial_id = _trial.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _szenario_mean_time_data...', 10, 1) WITH NOWAIT;
	DELETE _szenario_mean_time_data FROM (_szenario_mean_time INNER JOIN _szenario_mean_time_data on _szenario_mean_time_data.szenario_mean_time_id = _szenario_mean_time.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _measure_data_filtered...', 10, 1) WITH NOWAIT;
	DELETE _measure_data_filtered FROM (_trial INNER JOIN _measure_data_filtered on _measure_data_filtered.trial_id = _trial.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _measure_data_normalized...', 10, 1) WITH NOWAIT;
	DELETE _measure_data_normalized FROM (_trial INNER JOIN _measure_data_normalized on _measure_data_normalized.trial_id = _trial.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _measure_data_raw...', 10, 1) WITH NOWAIT;
	DELETE _measure_data_raw FROM (_trial INNER JOIN _measure_data_raw on _measure_data_raw.trial_id = _trial.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _velocity_data_filtered...', 10, 1) WITH NOWAIT;
	DELETE _velocity_data_filtered FROM (_trial INNER JOIN _velocity_data_filtered on _velocity_data_filtered.trial_id = _trial.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _velocity_data_normalized...', 10, 1) WITH NOWAIT;
	DELETE _velocity_data_normalized FROM (_trial INNER JOIN _velocity_data_normalized on _velocity_data_normalized.trial_id = _trial.id) WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _baseline...', 10, 1) WITH NOWAIT;
	DELETE FROM _baseline WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _szenario_mean_time...', 10, 1) WITH NOWAIT;
	DELETE FROM _szenario_mean_time WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _trial...', 10, 1) WITH NOWAIT;
	DELETE FROM _trial WHERE measure_file_id = @measureFileID;
	
	RAISERROR('DELETE _measure_file...', 10, 1) WITH NOWAIT;
	DELETE FROM _measure_file WHERE id = @measureFileID;
	RAISERROR('--------------END--------------', 10, 1) WITH NOWAIT;
END




GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[deleteSubjectStatistics]
	@subjectID int

AS
BEGIN
	SET NOCOUNT ON;

	DELETE statistic FROM _statistic_data statistic JOIN _trial trial ON statistic.trial_id = trial.id
	WHERE trial.subject_id = @subjectID;
END



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[deleteBaselineData]
	@baselineID int

AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM _baseline_data WHERE baseline_id = @baselineID;
END



GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[getFaultyTrialFixInformation]
	@measureFileID int,
	@szenarioTrialNumber int,
	@upperTrialID int OUTPUT,
	@lowerTrialID int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @upperSzenarioTrialNumberID int, @lowerSzenarioTrialNumberID int;
	
	SELECT @upperSzenarioTrialNumberID = id FROM _szenario_trial_number WHERE szenario_trial_number = @szenarioTrialNumber-1;
	SELECT @lowerSzenarioTrialNumberID = id FROM _szenario_trial_number WHERE szenario_trial_number = @szenarioTrialNumber+1;

	IF(@upperSzenarioTrialNumberID IS NULL)
	BEGIN
		SELECT @upperSzenarioTrialNumberID = @lowerSzenarioTrialNumberID;
	END

	IF(@lowerSzenarioTrialNumberID IS NULL)
	BEGIN
		SELECT @lowerSzenarioTrialNumberID = @upperSzenarioTrialNumberID;
	END

	SELECT @upperTrialID = id FROM _trial WHERE measure_file_id = @measureFileID AND szenario_trial_number_id = @upperSzenarioTrialNumberID;
	SELECT @lowerTrialID = id FROM _trial WHERE measure_file_id = @measureFileID AND szenario_trial_number_id = @lowerSzenarioTrialNumberID;

END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[getTrialID]
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int,
	@turnDateTime datetime2,
	@target int,
	@trial int,
	@id int OUTPUT

AS
BEGIN
	SET NOCOUNT ON;

	SELECT @id = _trial.id FROM ( _trial
				INNER JOIN _measure_file on _measure_file.id = _trial.measure_file_id
				INNER JOIN _study on _study.id = _trial.study_id
				INNER JOIN _group on _group.id = _trial.group_id
				INNER JOIN _szenario on _szenario.id = _trial.szenario_id
				INNER JOIN _target on _target.id = _trial.target_id
				INNER JOIN _target_trial_number on _target_trial_number.id = _trial.target_trial_number_id)
				WHERE study_name = @studyName AND
				group_name = @groupName AND
				subject_id = @subjectID AND
				szenario_name = @szenarioName AND
				creation_time = @turnDateTime AND
				target_number = @target AND
				target_trial_number = @trial;
END




GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[getBaselineID]
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int,
	@target int,
	@id int OUTPUT

AS
BEGIN
	SET NOCOUNT ON;

	SELECT @id = _baseline.id FROM ( _baseline
				INNER JOIN _study on _study.id = _baseline.study_id
				INNER JOIN _group on _group.id = _baseline.group_id
				INNER JOIN _szenario on _szenario.id = _baseline.szenario_id
				INNER JOIN _target on _target.id = _baseline.target_id)
				WHERE study_name = @studyName AND
				group_name = @groupName AND
				subject_id = @subjectID AND
				szenario_name = @szenarioName AND
				target_number = @target;
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[getTurnDateTime]
(
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int,
	@turn int,
	@turnDateTime datetime2 OUTPUT
)
AS
BEGIN
	DECLARE @creationTimeTable TABLE( id int identity(1,1), creation_time datetime2, rownumber int );

	INSERT INTO @creationTimeTable
	SELECT temp.creation_time, ROW_NUMBER() OVER (ORDER BY temp.creation_time ASC) AS rownumber FROM 
	(
		SELECT DISTINCT creation_time FROM ( _trial
		INNER JOIN _measure_file on _measure_file.id = _trial.measure_file_id
		INNER JOIN _study on _study.id = _trial.study_id
		INNER JOIN _group on _group.id = _trial.group_id
		INNER JOIN _szenario on _szenario.id = _trial.szenario_id)			
		WHERE study_name = @studyName AND	
		group_name = @groupName AND
		szenario_name = @szenarioName AND
		subject_id = @subjectID
	) AS temp

	SELECT @turnDateTime = creation_time FROM @creationTimeTable WHERE rownumber = @turn;
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertBaseline]
	@subjectID int, 
	@studyID int,
	@groupID int,
	@targetID int,
	@szenarioID int,
	@measureFileID int,
	@id int OUTPUT
	
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._baseline WHERE 
		subject_id = @subjectID AND
		study_id = @studyID AND
		group_id = @groupID AND
		target_id = @targetID AND
		szenario_id = @szenarioID AND
		measure_file_id = @measureFileID
	)
		BEGIN
		
			INSERT INTO dbo._baseline	(	
											subject_id,
											study_id,
											group_id,
											target_id,
											szenario_id,
											measure_file_id
										) 
										VALUES
										(
											@subjectID,
											@studyID,
											@groupID,
											@targetID,
											@szenarioID,
											@measureFileID
										);
	
			SELECT @id = Scope_Identity();
			
		END
	ELSE
		BEGIN
		
			SELECT @id = _baseline.id FROM dbo._baseline WHERE 
			subject_id = @subjectID AND
			study_id = @studyID AND
			group_id = @groupID AND
			target_id = @targetID AND
			szenario_id = @szenarioID AND
			measure_file_id = @measureFileID
		
		END
		
	
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertBaselineData]
	@baselineID int,
	@pseudoTimeStamp datetime2, 
	@baselinePositionCartesianX float,
	@baselinePositionCartesianY float,
	@baselinePositionCartesianZ float,
	@baselineVelocityX float,
	@baselineVelocityY float,
	@baselineVelocityZ float,
	@id int OUTPUT

AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo._baseline_data	(	
										baseline_id,
										pseudo_time_stamp,
										baseline_position_cartesian_x,
										baseline_position_cartesian_y,
										baseline_position_cartesian_z,
										baseline_velocity_x,
										baseline_velocity_y,
										baseline_velocity_z
									) 
									VALUES
									(
										@baselineID, 
										@pseudoTimeStamp,
										@baselinePositionCartesianX,
										@baselinePositionCartesianY,
										@baselinePositionCartesianZ,
										@baselineVelocityX,
										@baselineVelocityY,
										@baselineVelocityZ
									);

	SELECT @id = Scope_Identity();
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertGroup]
	@groupName varchar(max),
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._group WHERE 
		group_name = @groupName
	)
		BEGIN
			INSERT INTO dbo._group	(	
										group_name
									) 
									VALUES
									(
										@groupName
									);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._group WHERE 
			group_name = @groupName
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertIsCatchTrial]
	@isCatchTrial bit,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._is_catch_trial WHERE 
		is_catch_trial = @isCatchTrial
	)
		BEGIN

			INSERT INTO dbo._is_catch_trial	(	
												is_catch_trial
											) 
											VALUES
											(
												@isCatchTrial
											);

			SELECT @id = Scope_Identity();

		END
	ELSE
		BEGIN

			SELECT @id = id FROM dbo._is_catch_trial WHERE 
			is_catch_trial = @isCatchTrial

		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertMeasureFile]
	@creationTime datetime2, 
	@fileHash varchar(max),
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._measure_file WHERE 
		creation_time = @creationTime AND
		file_hash = @fileHash
	)
	BEGIN
		INSERT INTO dbo._measure_file	(	
											creation_time,
											file_hash
										) 
										VALUES
										(
											@creationTime,
											@fileHash
										);

		SELECT @id = Scope_Identity();
	END
	
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._measure_file WHERE 
			creation_time = @creationTime AND
			file_hash = @fileHash
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertStatisticData]
	@trialID int,
	@velocityVectorCorrelation float, 
	@trajectoryLengthAbs float,
	@trajectoryLengthRatioBaseline float,
	@perpendicularDisplacement300msAbs float,
	@maximalPerpendicularDisplacementAbs float,
	@meanPerpendicularDisplacementAbs float,
	@perpendicularDisplacement300msSign float,
	@maximalPerpendicularDisplacementSign float,
	@enclosedArea float,
	@rmse float,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo._statistic_data	(	
												trial_id,
												velocity_vector_correlation,
												trajectory_length_abs,
												trajectory_length_ratio_baseline,
												perpendicular_displacement_300ms_abs,
												maximal_perpendicular_displacement_abs,
												mean_perpendicular_displacement_abs,
												perpendicular_displacement_300ms_sign,
												maximal_perpendicular_displacement_sign,
												enclosed_area,
												rmse
											) 
											VALUES
											(
												@trialID,
												@velocityVectorCorrelation,
												@trajectoryLengthAbs,
												@trajectoryLengthRatioBaseline,
												@perpendicularDisplacement300msAbs,
												@maximalPerpendicularDisplacementAbs,
												@meanPerpendicularDisplacementAbs,
												@perpendicularDisplacement300msSign,
												@maximalPerpendicularDisplacementSign,
												@enclosedArea,
												@rmse
											);

	IF Scope_Identity() IS NULL
		SELECT @id = -1; 
	ELSE
		SELECT @id = Scope_Identity();
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertStudy]
	@studyName varchar(max),
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._study WHERE 
		study_name = @studyName
	)
		BEGIN
			INSERT INTO dbo._study	(	
										study_name
									) 
									VALUES
									(
										@studyName
									);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._study WHERE 
			study_name = @studyName
		END

END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertSubject]
	@subjectName varchar(max),
	@subjectID varchar(max),
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._subject WHERE 
		subject_name = @subjectName AND
		subject_id = @subjectID
	)
		BEGIN
			INSERT INTO dbo._subject(	
										subject_name,
										subject_id
									) 
									VALUES
									(
										@subjectName,
										@subjectID
		
									);
		
			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._subject WHERE 
			subject_name = @subjectName AND
			subject_id = @subjectID
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertSzenario]
	@szenarioName varchar(max),
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._szenario WHERE 
		szenario_name = @szenarioName
	)
		BEGIN
			INSERT INTO dbo._szenario	(	
											szenario_name
										) 
										VALUES
										(
											@szenarioName
										);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._szenario WHERE 
			szenario_name = @szenarioName
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertSzenarioMeanTime]
	@subjectID int,
	@studyID int,
	@groupID int,
	@targetID int,
	@szenarioID int,
	@measureFileID int,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._szenario_mean_time WHERE 
		subject_id = @subjectID AND
		study_id = @studyID AND
		group_id = @groupID AND
		target_id = @targetID AND
		szenario_id = @szenarioID AND
		measure_file_id = @measureFileID
	)
		BEGIN
			INSERT INTO dbo._szenario_mean_time	(	
													subject_id,
													study_id,
													group_id,
													target_id,
													szenario_id,
													measure_file_id
												) 
												VALUES
												(
													@subjectID,
													@studyID,
													@groupID,
													@targetID,
													@szenarioID,
													@measureFileID
												);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._szenario_mean_time WHERE 
			subject_id = @subjectID AND
			study_id = @studyID AND
			group_id = @groupID AND
			target_id = @targetID AND
			szenario_id = @szenarioID AND
			measure_file_id = @measureFileID
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertSzenarioMeanTimeData]
	@szenarioMeanTimeID int,
	@szenarioMeanTime time,
	@szenarioMeanTimeStd time,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
			INSERT INTO dbo._szenario_mean_time_data(	
														szenario_mean_time_id,
														szenario_mean_time,
														szenario_mean_time_std
													) 
													VALUES
													(
														@szenarioMeanTimeID,
														@szenarioMeanTime,
														@szenarioMeanTimeStd
													);
			IF Scope_Identity() IS NULL
				SELECT @id = -1; 
			ELSE
				SELECT @id = Scope_Identity();
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertSzenarioTrialNumber]
	@szenarioTrialNumber int,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._szenario_trial_number WHERE 
		szenario_trial_number = @szenarioTrialNumber
	)
		BEGIN
			INSERT INTO dbo._szenario_trial_number	(	
														szenario_trial_number
													) 
													VALUES
													(
														@szenarioTrialNumber
													);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._szenario_trial_number WHERE 
			szenario_trial_number = @szenarioTrialNumber
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertTarget]
	@targetNumber int,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._target WHERE 
		target_number = @targetNumber
	)
		BEGIN
			INSERT INTO dbo._target	(	
										target_number
									) 
									VALUES
									(
										@targetNumber
									);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._target WHERE 
			target_number = @targetNumber
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertTargetTrialNumber]
	@targetTrialNumber int,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._target_trial_number WHERE 
		target_trial_number = @targetTrialNumber
	)
		BEGIN
			INSERT INTO dbo._target_trial_number(	
													target_trial_number
												) 
												VALUES
												(
													@targetTrialNumber
												);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._target_trial_number WHERE 
			target_trial_number = @targetTrialNumber
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertTrial]
	@subjectID int,
	@studyID int,
	@groupID int,
	@isCatchTrialID int,
	@szenarioID int,
	@targetID int,
	@targetTrialNumberID int,
	@szenarioTrialNumberID int,
	@measureFileID int,
	@trialInformationID int,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._trial WHERE 
		subject_id = @subjectID AND
		study_id = @studyID AND
		group_id = @groupID AND
		is_catch_trial_id = @isCatchTrialID AND
		szenario_id = @szenarioID AND
		target_id = @targetID AND
		target_trial_number_id = @targetTrialNumberID AND
		szenario_trial_number_id = @szenarioTrialNumberID AND
		measure_file_id = @measureFileID AND
		trial_information_id = @trialInformationID
	)
		BEGIN
			INSERT INTO dbo._trial	(	
										subject_id,
										study_id,
										group_id,
										is_catch_trial_id,
										szenario_id,
										target_id,
										target_trial_number_id,
										szenario_trial_number_id,
										measure_file_id,
										trial_information_id
									) 
									VALUES
									(
										@subjectID,
										@studyID,
										@groupID,
										@isCatchTrialID,
										@szenarioID,
										@targetID,
										@targetTrialNumberID,
										@szenarioTrialNumberID,
										@measureFileID,
										@trialInformationID
									);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._trial WHERE 
			subject_id = @subjectID AND
			study_id = @studyID AND
			group_id = @groupID AND
			is_catch_trial_id = @isCatchTrialID AND
			szenario_id = @szenarioID AND
			target_id = @targetID AND
			target_trial_number_id = @targetTrialNumberID AND
			szenario_trial_number_id = @szenarioTrialNumberID AND
			measure_file_id = @measureFileID AND
			trial_information_id = @trialInformationID
			
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[insertTrialInformation]
	@faultyTrial bit,
	@butterworthFilterOrder int,
	@butterworthCutOffFreq int,
	@velocityTrimThreshold int,
	@id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS 
	(
		SELECT * FROM dbo._trial_information WHERE 
		faulty_trial = @faultyTrial AND
		butterworth_filterOrder = @butterworthFilterOrder AND
		butterworth_cutOffFreq = @butterworthCutOffFreq AND
		velocity_trim_threshold = @velocityTrimThreshold
	)
		BEGIN
			INSERT INTO dbo._trial_information	(	
													faulty_trial,
													butterworth_filterOrder,
													butterworth_cutOffFreq,
													velocity_trim_threshold
												) 
												VALUES
												(
													@faultyTrial,
													@butterworthFilterOrder,
													@butterworthCutOffFreq,
													@velocityTrimThreshold
												);

			SELECT @id = Scope_Identity();
		END
	ELSE
		BEGIN
			SELECT @id = id FROM dbo._trial_information WHERE 
			faulty_trial = @faultyTrial AND
			butterworth_filterOrder = @butterworthFilterOrder AND
			butterworth_cutOffFreq = @butterworthCutOffFreq AND
			velocity_trim_threshold = @velocityTrimThreshold;
		END
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getSzenarioTrials]
(
	@studyName varchar(max),
	@szenarioName varchar(max),
	@showCatchTrials bit,
	@showCatchTrialsExclusivly bit
)
RETURNS @trials TABLE (trial_name varchar(max))
AS
BEGIN
	IF @showCatchTrials = 1
	
		IF @showCatchTrialsExclusivly = 1

			INSERT INTO @trials 
			SELECT DISTINCT CONCAT('Trial ',RIGHT('00' + CONVERT(VARCHAR, szenario_trial_number ), 3), CASE WHEN is_catch_trial = 1 THEN ' - CatchTrial' END) FROM ( _trial
			INNER JOIN _study on _study.id = _trial.study_id
			INNER JOIN _szenario_trial_number on _szenario_trial_number.id = _trial.szenario_trial_number_id
			INNER JOIN _is_catch_trial on _is_catch_trial.id = _trial.is_catch_trial_id
			INNER JOIN _szenario on _szenario.id = _trial.szenario_id )
			WHERE study_name = @studyName AND
			szenario_name = @szenarioName AND
			is_catch_trial = 1;

		ELSE

			INSERT INTO @trials 
			SELECT DISTINCT CONCAT('Trial ',RIGHT('00' + CONVERT(VARCHAR, szenario_trial_number ), 3), CASE WHEN is_catch_trial = 1 THEN ' - CatchTrial' END) FROM ( _trial
			INNER JOIN _study on _study.id = _trial.study_id
			INNER JOIN _szenario_trial_number on _szenario_trial_number.id = _trial.szenario_trial_number_id
			INNER JOIN _is_catch_trial on _is_catch_trial.id = _trial.is_catch_trial_id
			INNER JOIN _szenario on _szenario.id = _trial.szenario_id )
			WHERE study_name = @studyName AND
			szenario_name = @szenarioName

	ELSE

		INSERT INTO @trials 
		SELECT DISTINCT CONCAT('Trial ',RIGHT('00' + CONVERT(VARCHAR, szenario_trial_number ), 3), CASE WHEN is_catch_trial = 1 THEN ' - CatchTrial' END) FROM ( _trial
		INNER JOIN _study on _study.id = _trial.study_id
		INNER JOIN _szenario_trial_number on _szenario_trial_number.id = _trial.szenario_trial_number_id
		INNER JOIN _is_catch_trial on _is_catch_trial.id = _trial.is_catch_trial_id
		INNER JOIN _szenario on _szenario.id = _trial.szenario_id )
		WHERE study_name = @studyName AND
		szenario_name = @szenarioName AND		
		is_catch_trial = 0;

	RETURN 
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getTurns]
(
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int
)
RETURNS @turns TABLE (turn_name varchar(max))
AS
BEGIN
	
	DECLARE @turnCount int;

	SELECT @turnCount =  COUNT( DISTINCT creation_time ) FROM ( _trial
    INNER JOIN _measure_file on _measure_file.id = _trial.measure_file_id
    INNER JOIN _study on _study.id = _trial.study_id
    INNER JOIN _group on _group.id = _trial.group_id
    INNER JOIN _szenario on _szenario.id = _trial.szenario_id)
    WHERE study_name = @studyName AND
	group_name = @groupName AND
	szenario_name = @szenarioName AND
    subject_id = @subjectID;


	WHILE @turnCount > 0
	BEGIN
		INSERT INTO @turns
		SELECT CONCAT('Turn ', CONVERT(VARCHAR, @turnCount ) );
		SET @turnCount = @turnCount - 1;
	END

	RETURN
END






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getBaseLineData]
(
	@subjectID int,
	@studyID int,
	@groupID int,
	@targetID int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM _baseline_data WHERE baseline_id = ( SELECT TOP 1 id FROM _baseline 
															WHERE subject_id = @subjectID AND 
															study_id = @studyID AND
															group_id = @groupID AND
															target_id = @targetID
														   )
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[getBaselineData2]
(
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int
)
RETURNS TABLE
AS
RETURN
(	
	SELECT _baseline_data.*, target_number FROM ( _baseline
    INNER JOIN _study on _study.id = _baseline.study_id
    INNER JOIN _group on _group.id = _baseline.group_id
    INNER JOIN _szenario on _szenario.id = _baseline.szenario_id
	INNER JOIN _target on _target.id = _baseline.target_id
    INNER JOIN _baseline_data on _baseline_data.baseline_id = _baseline.id)
    WHERE study_name = @studyName AND
    group_name = @groupName AND
	szenario_name = @szenarioName AND
    subject_id = @subjectID    
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getFaultyTrialInformation]
(
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT	_trial.id AS trial_id,
			_trial.measure_file_id,
			_study.study_name,
			_group.group_name,
			_trial.subject_id,
			_szenario.szenario_name,
			_measure_file.creation_time,
			_szenario_trial_number.szenario_trial_number
			
	FROM	(	_trial
				INNER JOIN _trial_information on _trial_information.id = _trial.trial_information_id
				INNER JOIN _study on _study.id = _trial.study_id
				INNER JOIN _group on _group.id = _trial.group_id
				INNER JOIN _szenario on _szenario.id = _trial.szenario_id
				INNER JOIN _measure_file on _measure_file.id = _trial.measure_file_id
				INNER JOIN _szenario_trial_number on _szenario_trial_number.id = _trial.szenario_trial_number_id
			)

	WHERE	_trial.id NOT IN (SELECT trial_id FROM _statistic_data) AND
			faulty_trial = 1
)








GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getGroupNames]
(
	@studyName varchar(max)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT group_name FROM ( _trial
										INNER JOIN _study on _study.id = _trial.study_id
										INNER JOIN _group on _group.id = _trial.group_id
									)
                                    WHERE study_name = @studyName
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getMeasureDataNormalizedData]
(
	@trialID int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT _measure_data_normalized.*, _is_catch_trial.is_catch_trial
	FROM (_measure_data_normalized	INNER JOIN _trial ON _trial.id = @trialID 
			INNER JOIN _is_catch_trial ON _trial.is_catch_trial_id = _is_catch_trial.id) 
	WHERE _measure_data_normalized.trial_id = @trialID
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getStatisticCalculationInformation]
(
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT	_trial.id,
			_trial.subject_id,
			_trial.study_id,
			_trial.group_id,
			_trial.target_id,
			_target.target_number
			
	FROM	(	_trial
				INNER JOIN _trial_information on _trial_information.id = _trial.trial_information_id
				INNER JOIN _target on _target.id = _trial.target_id
			)

	WHERE	_trial.id NOT IN (SELECT trial_id FROM _statistic_data) AND
			faulty_trial = 0
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getStatisticData]
(
	@trialID int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * FROM _statistic_data WHERE trial_id = @trialID
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getStatisticData2]
(
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int,
	@turnDateTime datetime2	
)
RETURNS TABLE
AS
RETURN
(	
	SELECT _statistic_data.*, szenario_trial_number FROM ( _trial
    INNER JOIN _measure_file on _measure_file.id = _trial.measure_file_id
    INNER JOIN _study on _study.id = _trial.study_id
    INNER JOIN _group on _group.id = _trial.group_id
    INNER JOIN _szenario on _szenario.id = _trial.szenario_id
	INNER JOIN _szenario_trial_number on _szenario_trial_number.id = _trial.szenario_trial_number_id
    INNER JOIN _statistic_data on _statistic_data.trial_id = _trial.id)
    WHERE study_name = @studyName AND
    group_name = @groupName AND
    subject_id = @subjectID AND
    szenario_name = @szenarioName AND
    creation_time = @turnDateTime
)





GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getStudyNames]
()
RETURNS TABLE 
AS
RETURN 
(
	SELECT study_name FROM _study
)






GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getSubjectInformations]
(
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT _subject.id, _subject.subject_name, _subject.subject_id FROM ( _trial
											INNER JOIN _study on _study.id = _trial.study_id
											INNER JOIN _group on _group.id = _trial.group_id
											INNER JOIN _szenario on _szenario.id = _trial.szenario_id
											INNER JOIN _subject on _subject.id = _trial.subject_id
										)
										WHERE	study_name = @studyName AND
												group_name = @groupName AND
												szenario_name = @szenarioName
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[getSzenarioMeanTimeData]
(
	@studyName varchar(max),
	@groupName varchar(max),
	@szenarioName varchar(max),
	@subjectID int,
	@turnDateTime datetime2	
)
RETURNS TABLE
AS
RETURN
(	
	SELECT _szenario_mean_time_data.*, target_number FROM ( _szenario_mean_time
    INNER JOIN _measure_file on _measure_file.id = _szenario_mean_time.measure_file_id
    INNER JOIN _study on _study.id = _szenario_mean_time.study_id
    INNER JOIN _group on _group.id = _szenario_mean_time.group_id
    INNER JOIN _subject on _subject.id = _szenario_mean_time.subject_id
    INNER JOIN _szenario on _szenario.id = _szenario_mean_time.szenario_id
	INNER JOIN _target on _target.id = _szenario_mean_time.target_id
    INNER JOIN _szenario_mean_time_data on _szenario_mean_time_data.szenario_mean_time_id = _szenario_mean_time.id)
    WHERE study_name = @studyName AND
    group_name = @groupName AND
    _subject.id = @subjectID AND
    szenario_name = @szenarioName AND
    creation_time = @turnDateTime
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getSzenarioNames]
(
	@studyName varchar(max),
	@groupName varchar(max)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT szenario_name FROM ( _trial
										INNER JOIN _study on _study.id = _trial.study_id
										INNER JOIN _group on _group.id = _trial.group_id
										INNER JOIN _szenario on _szenario.id = _trial.szenario_id
									)
                                    WHERE	study_name = @studyName AND
											group_name = @groupName
)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getTargets]
(
	@studyName varchar(max),
	@szenarioName varchar(max)
)
RETURNS TABLE
AS
RETURN
	(
		SELECT DISTINCT CONCAT('Target ', RIGHT('0' + CONVERT(VARCHAR, target_number ), 2) ) AS target_name FROM ( _trial
		INNER JOIN _study on _study.id = _trial.study_id
		INNER JOIN _szenario on _szenario.id = _trial.szenario_id
		INNER JOIN _target on _target.id = _trial.target_id )
		WHERE study_name = @studyName AND
		szenario_name = @szenarioName
	)






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getTrials]
(
	@studyName varchar(max),
	@szenarioName varchar(max)
)
RETURNS TABLE
AS
RETURN
(
	SELECT DISTINCT CONCAT('Trial ', RIGHT('00' + CONVERT(VARCHAR, target_trial_number ), 3 ) ) AS trial_name FROM ( _trial
    INNER JOIN _study on _study.id = _trial.study_id
    INNER JOIN _szenario on _szenario.id = _trial.szenario_id
	INNER JOIN _target on _target.id = _trial.target_id
    INNER JOIN _target_trial_number on _target_trial_number.id = _trial.target_trial_number_id )
    WHERE study_name = @studyName AND
	szenario_name = @szenarioName
)





GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getVelocityDataNormalizedData]
(
	@trialID int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT _velocity_data_normalized.*, _is_catch_trial.is_catch_trial
	FROM (_velocity_data_normalized	INNER JOIN _trial ON _trial.id = @trialID 
			INNER JOIN _is_catch_trial ON _trial.is_catch_trial_id = _is_catch_trial.id) 
	WHERE _velocity_data_normalized.trial_id = @trialID
)