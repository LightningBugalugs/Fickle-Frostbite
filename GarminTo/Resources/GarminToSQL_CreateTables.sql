use [{DatabaseName}]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE xtype='U' AND name = 'DatabaseVersion')
BEGIN
	CREATE TABLE [dbo].[DatabaseVersion]([VersionNumber] [nvarchar](25) NOT NULL,
								         [LastUpdated] [datetime] NOT NULL,
										 [SchemaUpdatedBy] [nvarchar](250) NOT NULL) ON [PRIMARY]
	ALTER TABLE [dbo].[DatabaseVersion] ADD CONSTRAINT [DF_DatabaseVersion_LastUpdated]  DEFAULT (getdate()) FOR [LastUpdated]
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE xtype='U' AND name = 'Activity')
BEGIN
	CREATE TABLE [dbo].[DatabaseVersion_Log]([LogTimestamp] [datetime] NOT NULL,
											 [VersionNumber] [nvarchar](25) NOT NULL,
											 [LastUpdated] [datetime] NOT NULL,
											 [SchemaUpdatedBy] [nvarchar](250) NOT NULL) ON [PRIMARY]

	ALTER TABLE dbo.DatabaseVersion_Log ADD CONSTRAINT DF_DatabaseVersion_Log_LogTimestamp DEFAULT GETDATE() FOR LogTimestamp

	EXEC('CREATE TRIGGER [dbo].[DatabaseVersion_Trigger_InsertUpdate] ON  [dbo].[DatabaseVersion] AFTER INSERT,UPDATE AS 
			BEGIN
				INSERT INTO DatabaseVersion_Log (LogTimestamp, VersionNumber, LastUpdated, SchemaUpdatedBy)
				SELECT GETDATE(), VersionNumber, LastUpdated, SchemaUpdatedBy
				FROM inserted
			END')
END
GO

IF NOT EXISTS (SELECT * FROM DatabaseVersion)
BEGIN
	INSERT INTO DatabaseVersion (VersionNumber, LastUpdated, SchemaUpdatedBy) VALUES ('1.0', GETDATE(), 'GarminToSQL:CreationScript')
END
ELSE
BEGIN
	UPDATE DatabaseVersion SET VersionNumber = '1.0', LastUpdated = GETDATE(), SchemaUpdatedBy = 'GarminToSQL:CreationScript'
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE xtype='U' AND name = 'Activity')
BEGIN
	CREATE TABLE [dbo].[Activity]([ActivityId] [int] IDENTITY(1,1) NOT NULL,
								  [TcxActivityId] [nvarchar](250) NOT NULL,
								  [Sport] [nvarchar](350) NOT NULL,
								  CONSTRAINT [PK_Acrivity] PRIMARY KEY CLUSTERED ([ActivityId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE xtype='U' AND name = 'Lap')
BEGIN
	CREATE TABLE [dbo].[Lap]([LapId] [int] IDENTITY(1,1) NOT NULL,
							 [ActivityId] [int] NOT NULL,
							 [StartTime] [nvarchar](250) NOT NULL,
							 [TotalTimeSeconds] [decimal](18, 5) NOT NULL,
							 [DistanceMeters] [decimal](18, 5) NOT NULL,
							 [Calories] [int] NOT NULL,
							 [AverageHeartRateBpm] [int] NULL,
							 [MaximumHeartRateBpm] [int] NULL,
							 CONSTRAINT [PK_Lap] PRIMARY KEY CLUSTERED ([LapId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]

	 ALTER TABLE [dbo].[Lap] ADD CONSTRAINT FK_Lap_Activity FOREIGN KEY (ActivityId) REFERENCES dbo.Activity (ActivityId) ON UPDATE NO ACTION ON DELETE NO ACTION 
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE xtype='U' AND name = 'Trackpoint')
BEGIN
	CREATE TABLE [dbo].[Trackpoint]([TrackpointId] [int] IDENTITY(1,1) NOT NULL,
									[LapId] [int] NOT NULL,
									[Time] [datetime] NOT NULL,
									[LatitudeDegrees] [decimal](18, 5) NULL,
									[LongitudeDegrees] [decimal](18, 5) NULL,
									[AltitudeMeters] [decimal](18, 5) NOT NULL,
									[DistanceMeters] [decimal](18, 5) NOT NULL,
									[HeartRateBpm] [int] NULL,
									CONSTRAINT [PK_Trackpoint] PRIMARY KEY CLUSTERED ([TrackpointId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]

	ALTER TABLE [dbo].[Trackpoint] ADD CONSTRAINT FK_Trackpoint_Lap FOREIGN KEY	(LapId) REFERENCES dbo.Lap (LapId) ON UPDATE NO ACTION ON DELETE NO ACTION 
END
