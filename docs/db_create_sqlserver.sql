/****** Object:  Database [WareMaster]    Script Date: 2023/11/3 23:15:35 ******/
CREATE DATABASE [WareMaster]  (EDITION = 'GeneralPurpose', SERVICE_OBJECTIVE = 'GP_S_Gen5_1', MAXSIZE = 32 GB) WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [WareMaster] SET COMPATIBILITY_LEVEL = 150
GO
ALTER DATABASE [WareMaster] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WareMaster] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WareMaster] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WareMaster] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WareMaster] SET ARITHABORT OFF 
GO
ALTER DATABASE [WareMaster] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [WareMaster] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WareMaster] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WareMaster] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WareMaster] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WareMaster] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WareMaster] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WareMaster] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WareMaster] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [WareMaster] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WareMaster] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [WareMaster] SET  MULTI_USER 
GO
ALTER DATABASE [WareMaster] SET ENCRYPTION ON
GO
ALTER DATABASE [WareMaster] SET QUERY_STORE = ON
GO
ALTER DATABASE [WareMaster] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 2023/11/3 23:15:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Category] [varchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [Category_UNIQUE] UNIQUE NONCLUSTERED 
(
	[Category] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Items]    Script Date: 2023/11/3 23:15:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Itemname] [varchar](200) NOT NULL,
	[Category_Id] [int] NOT NULL,
	[Unit] [varchar](45) NULL,
	[Location] [varchar](200) NULL,
	[Description] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [Itemname_UNIQUE] UNIQUE NONCLUSTERED 
(
	[Itemname] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settlement]    Script Date: 2023/11/3 23:15:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settlement](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Total] [money] NOT NULL,
	[Settle_Date] [date] NOT NULL,
 CONSTRAINT [PK__Settleme__3213E83F8C93AF4C] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 2023/11/3 23:15:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Total] [money] NOT NULL,
	[Transaction_Date] [date] NOT NULL,
	[User_Id] [int] NOT NULL,
 CONSTRAINT [PK__Transact__3213E83F9D0A647E] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2023/11/3 23:15:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](45) NOT NULL,
	[Role] [int] NOT NULL,
	[Password] [varchar](64) NOT NULL,
	[Email] [varchar](200) NOT NULL,
 CONSTRAINT [PK__Users__3213E83F9038FA42] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [Username_UNIQUE] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [Items_Category_idx]    Script Date: 2023/11/3 23:15:35 ******/
CREATE NONCLUSTERED INDEX [Items_Category_idx] ON [dbo].[Items]
(
	[Category_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [Settlement_Item_Id_idx]    Script Date: 2023/11/3 23:15:35 ******/
CREATE NONCLUSTERED INDEX [Settlement_Item_Id_idx] ON [dbo].[Settlement]
(
	[Item_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [item_id_idx]    Script Date: 2023/11/3 23:15:35 ******/
CREATE NONCLUSTERED INDEX [item_id_idx] ON [dbo].[Transactions]
(
	[Item_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [user_id_idx]    Script Date: 2023/11/3 23:15:35 ******/
CREATE NONCLUSTERED INDEX [user_id_idx] ON [dbo].[Transactions]
(
	[User_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [Items_Category] FOREIGN KEY([Category_Id])
REFERENCES [dbo].[Categories] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [Items_Category]
GO
ALTER TABLE [dbo].[Settlement]  WITH CHECK ADD  CONSTRAINT [Settlement_Item_Id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Items] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Settlement] CHECK CONSTRAINT [Settlement_Item_Id]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [item_id] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[Items] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [item_id]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [user_id] FOREIGN KEY([User_Id])
REFERENCES [dbo].[Users] ([id])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [user_id]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [CK_Users] CHECK  (([Role]=(0) OR [Role]=(1)))
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [CK_Users]
GO
ALTER DATABASE [WareMaster] SET  READ_WRITE 
GO
