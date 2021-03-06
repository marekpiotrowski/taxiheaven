USE [master]
GO
/****** Object:  Database [TaxiHeaven]    Script Date: 03/05/2016 13:59:31 ******/
CREATE DATABASE [TaxiHeaven]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TaxiHeaven', FILENAME = N'g:\SQLEXPRESS_OWN\MSSQL11.SQLEXPRESS\MSSQL\DATA\TaxiHeaven.mdf' , SIZE = 4160KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TaxiHeaven_log', FILENAME = N'g:\SQLEXPRESS_OWN\MSSQL11.SQLEXPRESS\MSSQL\DATA\TaxiHeaven_log.ldf' , SIZE = 1040KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TaxiHeaven] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TaxiHeaven].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TaxiHeaven] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TaxiHeaven] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TaxiHeaven] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TaxiHeaven] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TaxiHeaven] SET ARITHABORT OFF 
GO
ALTER DATABASE [TaxiHeaven] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TaxiHeaven] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TaxiHeaven] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TaxiHeaven] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TaxiHeaven] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TaxiHeaven] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TaxiHeaven] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TaxiHeaven] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TaxiHeaven] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TaxiHeaven] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TaxiHeaven] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TaxiHeaven] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TaxiHeaven] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TaxiHeaven] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TaxiHeaven] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TaxiHeaven] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TaxiHeaven] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TaxiHeaven] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TaxiHeaven] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TaxiHeaven] SET  MULTI_USER 
GO
ALTER DATABASE [TaxiHeaven] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TaxiHeaven] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TaxiHeaven] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TaxiHeaven] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [TaxiHeaven]
GO
/****** Object:  StoredProcedure [dbo].[AssignCar]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AssignCar] (
	@DriverId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CarId INT

	SELECT @CarId = t.Id FROM dbo.Car t
				LEFT OUTER JOIN dbo.Driver d ON t.Id = d.CarId WHERE d.Id IS NULL

	UPDATE dbo.Driver
	SET CarId = @CarId WHERE Id = @DriverId
END

GO
/****** Object:  StoredProcedure [dbo].[AssignDevice]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AssignDevice] (
	@DriverId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DeviceId INT

	SELECT @DeviceId = t.Id FROM dbo.Device t
				LEFT OUTER JOIN dbo.Driver d ON t.Id = d.DeviceId WHERE d.Id IS NULL

	UPDATE dbo.Driver
	SET DeviceId = @DeviceId WHERE Id = @DriverId
END

GO
/****** Object:  StoredProcedure [dbo].[AssignOrder]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AssignOrder]
	@OrderId INT
AS
BEGIN
	SET NOCOUNT ON;
	-- const
	DECLARE @AcceptableRange REAL
	SET @AcceptableRange = 1000

	DECLARE @MaxRadius REAL
	SET @MaxRadius = 8000

	DECLARE @StartLatitude REAL
	DECLARE @StartLongitude REAL
	DECLARE @RequiresSpecialCar BIT
	DECLARE @OrderStatusId INT
	DECLARE @WorkSessionId INT

	SELECT @StartLatitude = StartLatitude, @StartLongitude = StartLongitude,
	@RequiresSpecialCar = RequiresSpecialCar, @OrderStatusId = StatusId, @WorkSessionId = WorkSessionId
	FROM [dbo].[Order] WHERE Id = @OrderId

	IF @OrderStatusId = 2 AND @WorkSessionId IS NULL
	BEGIN
		DECLARE @NearestBasementId INT
		SELECT TOP 1 @NearestBasementId = BasementId FROM dbo.BasementDistances(@StartLatitude, @StartLongitude) ORDER BY BasementDistance


		CREATE TABLE #DriverOrdersCount (
			DriverId INT,
			BasementId INT, 
			Distance REAL,
			DriverStatusId INT,
			BasementDistance REAL,
			WorkSessionId INT,
			DeviceActive BIT,
			HasSpecialCar BIT,
			OrdersCount INT 
		)

		INSERT INTO #DriverOrdersCount
		(DriverId, BasementId, Distance, DriverStatusId, BasementDistance, WorkSessionId, DeviceActive, HasSpecialCar, OrdersCount)
		SELECT DriverId, BasementId, Distance, DriverStatusId, BasementDistance, WorkSessionId, DeviceActive, HasSpecialCar, OrdersCount
		FROM dbo.DriverOrdersCount(@StartLatitude, @StartLongitude)
		WHERE DeviceActive = 1 AND HasSpecialCar >= @RequiresSpecialCar

		SELECT * FROM #DriverOrdersCount ORDER BY BasementDistance
		-- mamy informację o odległościach do poszczególnych baz,
		-- a także kto ma najmniej przejazdów
		DECLARE @BestMatch INT;

		SELECT TOP 1 @BestMatch = DriverId FROM #DriverOrdersCount
		WHERE BasementId = @NearestBasementId AND DriverStatusId = 2 -- na bazie
		ORDER BY OrdersCount

		IF @@ROWCOUNT = 0
		BEGIN
			-- sprawdzamy, czy jacys kierowcy z tej bazy wracaja na baze
			SELECT TOP 1 @BestMatch = DriverId FROM #DriverOrdersCount
			WHERE BasementId = @NearestBasementId AND DriverStatusId = 5 -- wracaja do bazy
			ORDER BY OrdersCount
			IF @@ROWCOUNT = 0
			-- nie ma kierowcow wracajacych, ani na bazie, szukamy najblizszego ktory wraca do bazy/jest na innej bazie
			BEGIN
				DECLARE @MinDistance REAL
				SELECT @MinDistance = MIN(Distance) FROM #DriverOrdersCount

				SELECT TOP 1 @BestMatch = DriverId FROM #DriverOrdersCount
				WHERE (Distance <= @MinDistance + @AcceptableRange) AND (Distance <= @MaxRadius) AND (DriverStatusId = 2 OR DriverStatusId = 5)
				ORDER BY OrdersCount
				IF @@ROWCOUNT = 0 --jesli nikt nie jest wolny, kolejkujemy zlecenie do najodpowiedniejszego taksowkarza z oryginalnej bazy
				BEGIN
					SELECT TOP 1 @BestMatch = DriverId FROM #DriverOrdersCount
					WHERE BasementId = @NearestBasementId AND (DriverStatusId <> 1 AND DriverStatusId <> 6)
					ORDER BY OrdersCount
				END
			END
		END

		UPDATE dbo.[Order]
		SET WorkSessionId = t.WorkSessionId
		FROM #DriverOrdersCount t WHERE t.DriverId = @BestMatch AND Id = @OrderId
	END
END

GO
/****** Object:  StoredProcedure [dbo].[UpdateTotalMileage]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateTotalMileage] 
	@OrderId INT
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @PreviousMileage REAL
	SELECT @PreviousMileage = ws.TotalMileage FROM dbo.WorkSession ws INNER JOIN dbo.[Order] o ON ws.Id = o.WorkSessionId
														WHERE o.Id = @OrderId
	DECLARE @OrderStartLatitude REAL
	DECLARE @OrderDestinationLatitude REAL
	DECLARE @OrderStartLongitude REAL
	DECLARE @OrderDestinationLongitude REAL

	DECLARE @OrderMileage REAL

	SELECT @OrderStartLatitude = StartLatitude, @OrderStartLongitude = StartLongitude, 
	@OrderDestinationLatitude = DestinationLatitude, @OrderDestinationLongitude = DestinationLongitude FROM
	dbo.[Order] WHERE Id = @OrderId

	SELECT @OrderMileage = Distance FROM dbo.Distance(@OrderStartLatitude, @OrderStartLongitude,
												@OrderDestinationLatitude, @OrderDestinationLongitude)

	UPDATE ws
	SET ws.TotalMileage = @PreviousMileage + @OrderMileage
	FROM dbo.WorkSession ws
	INNER JOIN dbo.[Order] o ON ws.Id = o.WorkSessionId
	WHERE o.Id = @OrderId
END

GO
/****** Object:  UserDefinedFunction [dbo].[BasementDistances]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[BasementDistances] 
(
	@StartLatitude REAL,
	@StartLongitude REAL
)
RETURNS @BasementDistance TABLE 
(
	BasementId INT,
	BasementDistance REAL
)
AS
BEGIN
	WITH Basement AS
	(
		SELECT Id, Latitude, Longitude FROM dbo.Basement
	),
	Distance AS
	(
		SELECT Id, Distance
		FROM Basement
		CROSS APPLY [dbo].[Distance](Basement.Latitude, Basement.Longitude, @StartLatitude, @StartLongitude)
	)
	INSERT INTO @BasementDistance
	SELECT Id, Distance FROM Distance 

	RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[Distance]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[Distance]
(	
	@Latitude1 REAL,
	@Longitude1 REAL,
	@Latitude2 REAL,
	@Longitude2 REAL
)
RETURNS @Distance TABLE
(
	Distance REAL
)
AS
BEGIN
	DECLARE @R REAL
	SET @R = 6371000

	DECLARE @Fi1 REAL
	DECLARE @Fi2 REAL
	SET @Fi1 = RADIANS(@Latitude1)
	SET @Fi2 = RADIANS(@Latitude2)

	DECLARE @DeltaFi REAL
	SET @DeltaFi = RADIANS(@Latitude2 - @Latitude1)

	DECLARE @DeltaLambda REAL
	SET @DeltaLambda = RADIANS(@Longitude2 - @Longitude1)

	DECLARE @a REAL
	SET @a = SIN(@DeltaFi/2) * SIN(@DeltaFi/2) + COS(@Fi1) * COS(@Fi2)
			* SIN(@DeltaLambda/2) * SIN(@DeltaLambda/2)

	DECLARE @c REAL
	SET @c = 2 * ATN2(SQRT(@a), SQRT(1 - @a));


	DECLARE @d REAL
	SET @d = @R * @c;

	INSERT INTO @Distance (Distance) VALUES (@d)

	RETURN
RETURN
END


GO
/****** Object:  UserDefinedFunction [dbo].[DriverDistances]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DriverDistances]
(
	@StartLatitude REAL,
	@StartLongitude REAL
)
RETURNS @DriverDistance TABLE 
(
	DriverId INT,
	Distance REAL
)
AS
BEGIN
	WITH Driver AS
	(
		SELECT Id, Latitude, Longitude FROM dbo.Driver
	),
	Distance AS
	(
		SELECT Id, Distance
		FROM Driver
		CROSS APPLY [dbo].[Distance](Driver.Latitude, Driver.Longitude, @StartLatitude, @StartLongitude)
	)
	INSERT INTO @DriverDistance
	SELECT Id, Distance FROM Distance 

	RETURN
END

GO
/****** Object:  UserDefinedFunction [dbo].[DriverOrdersCount]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DriverOrdersCount]
(
	@StartLatitude REAL,
	@StartLongitude REAL
)
RETURNS 
@DriverOrdersCount TABLE 
(
	DriverId INT,
	BasementId INT, 
	Distance REAL,
	DriverStatusId INT,
	BasementDistance REAL,
	WorkSessionId INT,
	DeviceActive BIT,
	HasSpecialCar BIT,
	OrdersCount INT
)
AS
BEGIN
	INSERT INTO @DriverOrdersCount
	SELECT t.DriverId, t.BasementId, Distance, DriverStatusId, BasementDistance, t.WorkSessionId, DeviceActive, HasSpecialCar, 0 FROM (
		SELECT Driver.Id AS DriverId, BasementId, Driver.StatusId AS DriverStatusId,
		WorkSession.Id AS WorkSessionId,
		d.Active AS DeviceActive,
		c.Special AS HasSpecialCar
		FROM dbo.Driver
		INNER JOIN dbo.WorkSession
		ON Driver.Id = WorkSession.DriverId AND WorkSession.EndTime IS NULL
		INNER JOIN dbo.Car c
		ON Driver.CarId = c.Id
		INNER JOIN dbo.Device d
		ON Driver.DeviceId = d.Id
		) t INNER JOIN dbo.DriverDistances(@StartLatitude, @StartLongitude) p ON t.DriverId = p.DriverId
			INNER JOIN dbo.BasementDistances(@StartLatitude, @StartLongitude) u ON t.BasementId = u.BasementId
		GROUP BY t.DriverId, t.BasementId, Distance, DriverStatusId, BasementDistance, t.WorkSessionId, DeviceActive, HasSpecialCar

	UPDATE p
	SET p.OrdersCount = (SELECT COUNT (*) FROM dbo.[Order] t WHERE t.WorkSessionId = p.WorkSessionId)
	FROM @DriverOrdersCount p

	RETURN
END

GO
/****** Object:  Table [dbo].[Basement]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Basement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Longitude] [real] NULL,
	[Latitude] [real] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Car]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Car](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VehicleIdentificationNumber] [char](17) NOT NULL,
	[Make] [nvarchar](255) NOT NULL,
	[Year] [int] NOT NULL,
	[Mileage] [int] NOT NULL,
	[Special] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Device]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Device](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HardwareId] [char](17) NOT NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Driver]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Driver](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NULL,
	[DeviceId] [int] NULL,
	[BasementId] [int] NULL,
	[StatusId] [int] NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Longitude] [real] NULL,
	[Latitude] [real] NULL,
	[PhoneNumber] [char](12) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DriverStatus]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DriverStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Order]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NULL,
	[WorkSessionId] [int] NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[StartLatitude] [real] NOT NULL,
	[StartLongitude] [real] NOT NULL,
	[DestinationLatitude] [real] NULL,
	[DestinationLongitude] [real] NULL,
	[RequiresSpecialCar] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderStatus]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WorkSession]    Script Date: 03/05/2016 13:59:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkSession](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DriverId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[TotalMileage] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Basement] ON 

INSERT [dbo].[Basement] ([Id], [Name], [Longitude], [Latitude]) VALUES (1, N'Galeria Dominikańska', 17.0392952, 51.1083)
INSERT [dbo].[Basement] ([Id], [Name], [Longitude], [Latitude]) VALUES (2, N'Dworzec Główny PKP', 17.0344429, 51.0994225)
SET IDENTITY_INSERT [dbo].[Basement] OFF
SET IDENTITY_INSERT [dbo].[Car] ON 

INSERT [dbo].[Car] ([Id], [VehicleIdentificationNumber], [Make], [Year], [Mileage], [Special]) VALUES (1, N'1HGCM82633A004352', N'Ford Mondeo', 2013, 1567, 0)
INSERT [dbo].[Car] ([Id], [VehicleIdentificationNumber], [Make], [Year], [Mileage], [Special]) VALUES (2, N'2EGCM82633B013242', N'Fiat Ducato', 2008, 166897, 1)
SET IDENTITY_INSERT [dbo].[Car] OFF
SET IDENTITY_INSERT [dbo].[Device] ON 

INSERT [dbo].[Device] ([Id], [HardwareId], [Active]) VALUES (1, N'48-86-E8-11-0E-18', 0)
INSERT [dbo].[Device] ([Id], [HardwareId], [Active]) VALUES (2, N'48-86-E8-11-FF-18', 1)
INSERT [dbo].[Device] ([Id], [HardwareId], [Active]) VALUES (3, N'48-86-E8-11-FF-19', 1)
SET IDENTITY_INSERT [dbo].[Device] OFF
SET IDENTITY_INSERT [dbo].[Driver] ON 

INSERT [dbo].[Driver] ([Id], [CarId], [DeviceId], [BasementId], [StatusId], [FirstName], [LastName], [Longitude], [Latitude], [PhoneNumber]) VALUES (1, 1, 3, 1, 5, N'Jan', N'Kowalski', 17.0392952, 51.1083, N'+48725997451')
INSERT [dbo].[Driver] ([Id], [CarId], [DeviceId], [BasementId], [StatusId], [FirstName], [LastName], [Longitude], [Latitude], [PhoneNumber]) VALUES (2, 2, 2, 2, 2, N'Tomasz', N'Kot', 17.0390968, 51.0994034, N'+48669784567')
SET IDENTITY_INSERT [dbo].[Driver] OFF
SET IDENTITY_INSERT [dbo].[DriverStatus] ON 

INSERT [dbo].[DriverStatus] ([Id], [Title]) VALUES (1, N'Nieaktywny')
INSERT [dbo].[DriverStatus] ([Id], [Title]) VALUES (2, N'Oczekuje na zlecenia')
INSERT [dbo].[DriverStatus] ([Id], [Title]) VALUES (3, N'Jedzie do zlecenia')
INSERT [dbo].[DriverStatus] ([Id], [Title]) VALUES (4, N'Realizuje zlecenie')
INSERT [dbo].[DriverStatus] ([Id], [Title]) VALUES (5, N'Wraca do bazy')
INSERT [dbo].[DriverStatus] ([Id], [Title]) VALUES (6, N'Nie dotyczy')
SET IDENTITY_INSERT [dbo].[DriverStatus] OFF
SET IDENTITY_INSERT [dbo].[Order] ON 

INSERT [dbo].[Order] ([Id], [StatusId], [WorkSessionId], [StartTime], [EndTime], [StartLatitude], [StartLongitude], [DestinationLatitude], [DestinationLongitude], [RequiresSpecialCar]) VALUES (1, 4, 1, CAST(0x0000A5F100CF5940 AS DateTime), CAST(0x0000A5F101532F40 AS DateTime), 51.1088257, 17.0321884, 51.1088257, 17.0321884, 0)
INSERT [dbo].[Order] ([Id], [StatusId], [WorkSessionId], [StartTime], [EndTime], [StartLatitude], [StartLongitude], [DestinationLatitude], [DestinationLongitude], [RequiresSpecialCar]) VALUES (2, 4, 2, CAST(0x0000A5F100CF5940 AS DateTime), CAST(0x0000A5F101532F40 AS DateTime), 51.1088257, 17.0321884, 51.1088257, 17.0321884, 0)
INSERT [dbo].[Order] ([Id], [StatusId], [WorkSessionId], [StartTime], [EndTime], [StartLatitude], [StartLongitude], [DestinationLatitude], [DestinationLongitude], [RequiresSpecialCar]) VALUES (3, 4, 2, CAST(0x0000A5F100CF5940 AS DateTime), CAST(0x0000A5F101532F40 AS DateTime), 51.1088257, 17.0321884, 51.1088257, 17.0321884, 0)
INSERT [dbo].[Order] ([Id], [StatusId], [WorkSessionId], [StartTime], [EndTime], [StartLatitude], [StartLongitude], [DestinationLatitude], [DestinationLongitude], [RequiresSpecialCar]) VALUES (4, 4, 3, CAST(0x0000A5F100CF5940 AS DateTime), CAST(0x0000A5F101532F40 AS DateTime), 51.1088257, 17.0321884, 51.1088257, 17.0321884, 0)
INSERT [dbo].[Order] ([Id], [StatusId], [WorkSessionId], [StartTime], [EndTime], [StartLatitude], [StartLongitude], [DestinationLatitude], [DestinationLongitude], [RequiresSpecialCar]) VALUES (5, 3, NULL, CAST(0x0000A5F100CF5940 AS DateTime), CAST(0x0000A5F101532F40 AS DateTime), 51.1121178, 17.0395317, 51.11, 17.0278778, 1)
SET IDENTITY_INSERT [dbo].[Order] OFF
SET IDENTITY_INSERT [dbo].[OrderStatus] ON 

INSERT [dbo].[OrderStatus] ([Id], [Title]) VALUES (1, N'Do potwierdzenia przez klienta')
INSERT [dbo].[OrderStatus] ([Id], [Title]) VALUES (2, N'Do przydzielenia')
INSERT [dbo].[OrderStatus] ([Id], [Title]) VALUES (3, N'Do potwierdzenia przez kierowce')
INSERT [dbo].[OrderStatus] ([Id], [Title]) VALUES (4, N'W realizacji')
INSERT [dbo].[OrderStatus] ([Id], [Title]) VALUES (5, N'Zakończone')
SET IDENTITY_INSERT [dbo].[OrderStatus] OFF
SET IDENTITY_INSERT [dbo].[WorkSession] ON 

INSERT [dbo].[WorkSession] ([Id], [DriverId], [StartTime], [EndTime], [TotalMileage]) VALUES (1, 1, CAST(0x0000A5F100CF5940 AS DateTime), CAST(0x0000A5F101532F40 AS DateTime), 1111)
INSERT [dbo].[WorkSession] ([Id], [DriverId], [StartTime], [EndTime], [TotalMileage]) VALUES (2, 1, CAST(0x0000A5F200C90810 AS DateTime), NULL, 846)
INSERT [dbo].[WorkSession] ([Id], [DriverId], [StartTime], [EndTime], [TotalMileage]) VALUES (3, 2, CAST(0x0000A5F200871D10 AS DateTime), NULL, 0)
SET IDENTITY_INSERT [dbo].[WorkSession] OFF
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Car__ADC26AB97DECDCB7]    Script Date: 03/05/2016 13:59:31 ******/
ALTER TABLE [dbo].[Car] ADD UNIQUE NONCLUSTERED 
(
	[VehicleIdentificationNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Device__13A9B5A97FEBB4A5]    Script Date: 03/05/2016 13:59:31 ******/
ALTER TABLE [dbo].[Device] ADD UNIQUE NONCLUSTERED 
(
	[HardwareId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Driver_CarId_Index]    Script Date: 03/05/2016 13:59:31 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Driver_CarId_Index] ON [dbo].[Driver]
(
	[CarId] ASC
)
WHERE ([CarId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [Driver_DeviceId_Index]    Script Date: 03/05/2016 13:59:31 ******/
CREATE UNIQUE NONCLUSTERED INDEX [Driver_DeviceId_Index] ON [dbo].[Driver]
(
	[DeviceId] ASC
)
WHERE ([DeviceId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Car] ADD  DEFAULT ((0)) FOR [Special]
GO
ALTER TABLE [dbo].[Device] ADD  DEFAULT ((0)) FOR [Active]
GO
ALTER TABLE [dbo].[Order] ADD  DEFAULT ((0)) FOR [RequiresSpecialCar]
GO
ALTER TABLE [dbo].[WorkSession] ADD  DEFAULT ((0)) FOR [TotalMileage]
GO
ALTER TABLE [dbo].[Driver]  WITH CHECK ADD  CONSTRAINT [FK_Driver_Basement] FOREIGN KEY([BasementId])
REFERENCES [dbo].[Basement] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Driver] CHECK CONSTRAINT [FK_Driver_Basement]
GO
ALTER TABLE [dbo].[Driver]  WITH CHECK ADD  CONSTRAINT [FK_Driver_Car] FOREIGN KEY([CarId])
REFERENCES [dbo].[Car] ([Id])
GO
ALTER TABLE [dbo].[Driver] CHECK CONSTRAINT [FK_Driver_Car]
GO
ALTER TABLE [dbo].[Driver]  WITH CHECK ADD  CONSTRAINT [FK_Driver_Device] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Device] ([Id])
GO
ALTER TABLE [dbo].[Driver] CHECK CONSTRAINT [FK_Driver_Device]
GO
ALTER TABLE [dbo].[Driver]  WITH CHECK ADD  CONSTRAINT [FK_Driver_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[DriverStatus] ([Id])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Driver] CHECK CONSTRAINT [FK_Driver_Status]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[OrderStatus] ([Id])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Status]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_WorkSession] FOREIGN KEY([WorkSessionId])
REFERENCES [dbo].[WorkSession] ([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_WorkSession]
GO
ALTER TABLE [dbo].[WorkSession]  WITH CHECK ADD  CONSTRAINT [FK_WorkSession_Driver] FOREIGN KEY([DriverId])
REFERENCES [dbo].[Driver] ([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[WorkSession] CHECK CONSTRAINT [FK_WorkSession_Driver]
GO
USE [master]
GO
ALTER DATABASE [TaxiHeaven] SET  READ_WRITE 
GO
