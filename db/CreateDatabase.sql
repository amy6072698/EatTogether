USE master;
GO

-- 檢查資料庫是否存在(注意資料庫名稱是否正確)，若存在則刪除 (開發階段方便重置，正式環境請小心)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'EatTogetherDB')
BEGIN
    ALTER DATABASE EatTogetherDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EatTogetherDB;
END
GO

CREATE DATABASE EatTogetherDB;
GO

ALTER DATABASE [EatTogetherDB] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EatTogetherDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [EatTogetherDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [EatTogetherDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [EatTogetherDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [EatTogetherDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [EatTogetherDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [EatTogetherDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [EatTogetherDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [EatTogetherDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [EatTogetherDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [EatTogetherDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [EatTogetherDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [EatTogetherDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [EatTogetherDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [EatTogetherDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [EatTogetherDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [EatTogetherDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [EatTogetherDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [EatTogetherDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [EatTogetherDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [EatTogetherDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [EatTogetherDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [EatTogetherDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [EatTogetherDB] SET RECOVERY FULL 
GO
ALTER DATABASE [EatTogetherDB] SET  MULTI_USER 
GO
ALTER DATABASE [EatTogetherDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [EatTogetherDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [EatTogetherDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [EatTogetherDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [EatTogetherDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [EatTogetherDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [EatTogetherDB] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [EatTogetherDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [EatTogetherDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [EatTogetherDB]
GO
/****** Object:  Table [dbo].[ArticleCategories]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArticleCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_ArticleCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Articles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[EventId] [int] NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CoverImageUrl] [nvarchar](255) NULL,
	[PublishDate] [datetime2](0) NOT NULL,
	[ExpiryDate] [datetime2](0) NULL,
	[IsPinned] [bit] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Articles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
	[ParentCategoryId] [int] NULL,
	[DisplayOrder] [int] NOT NULL,
	[ImageUrl] [nvarchar](300) NULL,
	[UpdatedAt] [datetime2](0) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Coupons]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Coupons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[DiscountType] [int] NOT NULL,
	[DiscountValue] [int] NOT NULL,
	[MinSpend] [int] NOT NULL,
	[StartDate] [datetime2](0) NOT NULL,
	[EndDate] [datetime2](0) NULL,
	[LimitCount] [int] NULL,
	[ReceivedCount] [int] NULL,
 CONSTRAINT [PK_Coupons] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dishes]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dishes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[DishName] [nvarchar](100) NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
	[Description] [nvarchar](300) NULL,
	[ImageUrl] [nvarchar](300) NULL,
	[IsTakeOut] [bit] NOT NULL,
	[IsLimited] [bit] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[UpdatedAt] [datetime2](0) NULL,
 CONSTRAINT [PK_Dishes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailQueues]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailQueues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RecipientEmail] [nvarchar](100) NOT NULL,
	[Subject] [nvarchar](200) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[ProcessedAt] [datetime2](0) NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_EmailQueues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Events]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Summary] [nvarchar](50) NULL,
	[MinSpend] [int] NOT NULL,
	[StartDate] [datetime2](0) NOT NULL,
	[EndDate] [datetime2](0) NOT NULL,
	[RewardItem] [nvarchar](100) NULL,
	[DiscountType] [nvarchar](20) NULL,
	[DiscountValue] [decimal](10, 2) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Functions]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Functions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Category] [nvarchar](10) NOT NULL,
	[FunctionName] [varchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[IsOwnerOnly] [bit] NOT NULL,
 CONSTRAINT [PK_Functions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MemberCoupons]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberCoupons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[CouponId] [int] NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[UsedDate] [datetime2](0) NULL,
 CONSTRAINT [PK_MemberCoupons] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MemberFavorites]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberFavorites](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_MemberFavorites] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Members]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Members](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Account] [varchar](100) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[HashedPassword] [varchar](70) NOT NULL,
	[Phone] [varchar](10) NULL,
	[BirthDate] [date] NULL,
	[IsBlacklisted] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedAt] [datetime2](0) NULL,
	[IsConfirmed] [bit] NOT NULL,
	[AvatarFileName] [varchar](100) NULL,
	[BlacklistReason] [nvarchar](200) NULL,
 CONSTRAINT [PK_Members] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductName] [nvarchar](50) NOT NULL,
	[UnitPrice] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[SubTotal] [int] NOT NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PreOrderId] [int] NOT NULL,
	[OrderNumber] [nvarchar](200) NOT NULL,
	[MemberId] [int] NULL,
	[InOrOut] [bit] NOT NULL,
	[TableId] [int] NULL,
	[UserId] [int] NULL,
	[OrderAt] [datetime2](0) NOT NULL,
	[CouponId] [int] NULL,
	[OriginalAmount] [int] NOT NULL,
	[DiscountAmount] [int] NOT NULL,
	[TotalAmount] [int] NOT NULL,
	[Note] [nvarchar](200) NULL,
	[PaymentId] [int] NOT NULL,
	[PayMethod] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasswordResetTokens]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordResetTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [varchar](32) NOT NULL,
	[ExpiresAt] [datetime2](0) NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[PreOrderId] [int] NOT NULL,
	[Method] [nvarchar](50) NOT NULL,
	[PaidAt] [datetime2](0) NOT NULL,
	[DoneOrCancel] [int] NOT NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreOrderDetails]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreOrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PreOrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductName] [nvarchar](50) NOT NULL,
	[UnitPrice] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[SubTotal] [int] NOT NULL,
	[DoneOrCancel] [int] NOT NULL,
 CONSTRAINT [PK_PreOrderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreOrders]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNumber] [nvarchar](200) NOT NULL,
	[MemberId] [int] NULL,
	[InOrOut] [bit] NOT NULL,
	[TableId] [int] NULL,
	[UserId] [int] NULL,
	[OrderAt] [datetime2](0) NOT NULL,
	[CouponId] [int] NULL,
	[OriginalAmount] [int] NOT NULL,
	[DiscountAmount] [int] NOT NULL,
	[TotalAmount] [int] NOT NULL,
	[Note] [nvarchar](200) NULL,
	[PaymentId] [int] NULL,
	[PayMethod] [nvarchar](50) NOT NULL,
	[DoneOrCancel] [int] NOT NULL,
 CONSTRAINT [PK_PreOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductType] [nvarchar](10) NOT NULL,
	[DishId] [int] NULL,
	[SetMealId] [int] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reservations]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookingNumber] [varchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[ReservationDate] [datetime2](0) NOT NULL,
	[AdultsCount] [int] NOT NULL,
	[ChildrenCount] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Remark] [nvarchar](200) NULL,
	[ReservedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_Reservations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleFunctions]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleFunctions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[FunctionId] [int] NOT NULL,
 CONSTRAINT [PK_RoleFunctions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SetMealItems]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SetMealItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SetMealId] [int] NOT NULL,
	[DishId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[IsOptional] [bit] NOT NULL,
	[OptionGroupNo] [int] NULL,
	[PickLimit] [int] NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_SetMealItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SetMeals]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SetMeals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SetMealName] [nvarchar](100) NOT NULL,
	[DiscountType] [nvarchar](20) NOT NULL,
	[DiscountValue] [decimal](10, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
	[SetPrice] [decimal](10, 2) NULL,
	[Description] [nvarchar](300) NULL,
	[ImageUrl] [nvarchar](300) NULL,
	[UpdatedAt] [datetime2](0) NULL,
 CONSTRAINT [PK_SetMeals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriptionPreferences]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionPreferences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[IsEmailSubscribed] [bit] NOT NULL,
	[IsPushSubscribed] [bit] NOT NULL,
	[UpdatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_SubscriptionPreferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tables]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tables](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](20) NOT NULL,
	[SeatCount] [int] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Tables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserNotifications]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserNotifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[ArticleId] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[IsRead] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
 CONSTRAINT [PK_UserNotifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2026/3/11 下午 10:31:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Account] [varchar](50) NOT NULL,
	[HashedPassword] [varchar](70) NOT NULL,
	[EmployeeNumber] [varchar](20) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Phone] [varchar](10) NOT NULL,
	[HireDate] [date] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[MustChangePassword] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ArticleCategories_Name]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ArticleCategories_Name] ON [dbo].[ArticleCategories]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ArticleCategories_Sort]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_ArticleCategories_Sort] ON [dbo].[ArticleCategories]
(
	[SortOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Articles_PublishDate]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Articles_PublishDate] ON [dbo].[Articles]
(
	[PublishDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Articles_Status]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Articles_Status] ON [dbo].[Articles]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Coupons_Code]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Coupons_Code] ON [dbo].[Coupons]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EmailQueues_Recipient]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_EmailQueues_Recipient] ON [dbo].[EmailQueues]
(
	[RecipientEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmailQueues_Status]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_EmailQueues_Status] ON [dbo].[EmailQueues]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Events_StartDate]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Events_StartDate] ON [dbo].[Events]
(
	[StartDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Events_Status]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Events_Status] ON [dbo].[Events]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Functions_FunctionName]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Functions_FunctionName] ON [dbo].[Functions]
(
	[FunctionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_MemberFavorites_Mem_Prod]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_MemberFavorites_Mem_Prod] ON [dbo].[MemberFavorites]
(
	[MemberId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Members_Account]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Members_Account] ON [dbo].[Members]
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Members_Email]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Members_Email] ON [dbo].[Members]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Members_IsDeleted]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Members_IsDeleted] ON [dbo].[Members]
(
	[IsDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_OrderAt]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Orders_OrderAt] ON [dbo].[Orders]
(
	[OrderAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_OrderNumber]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Orders_OrderNumber] ON [dbo].[Orders]
(
	[OrderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PasswordResetTokens_Token]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PasswordResetTokens_Token] ON [dbo].[PasswordResetTokens]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PasswordResetTokens_UserId]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_PasswordResetTokens_UserId] ON [dbo].[PasswordResetTokens]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_PreOrders_OrderAt]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_PreOrders_OrderAt] ON [dbo].[PreOrders]
(
	[OrderAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PreOrders_OrderNumber]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PreOrders_OrderNumber] ON [dbo].[PreOrders]
(
	[OrderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reservations_BookingNumber]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Reservations_BookingNumber] ON [dbo].[Reservations]
(
	[BookingNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reservations_Date]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_Reservations_Date] ON [dbo].[Reservations]
(
	[ReservationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleFunctions_Role_Func]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_RoleFunctions_Role_Func] ON [dbo].[RoleFunctions]
(
	[RoleId] ASC,
	[FunctionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Roles_RoleName]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_RoleName] ON [dbo].[Roles]
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tables_TableName]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tables_TableName] ON [dbo].[Tables]
(
	[TableName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserNotifications_IsRead]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_UserNotifications_IsRead] ON [dbo].[UserNotifications]
(
	[IsRead] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserNotifications_MemberId]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE NONCLUSTERED INDEX [IX_UserNotifications_MemberId] ON [dbo].[UserNotifications]
(
	[MemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserRoles_User_Role]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserRoles_User_Role] ON [dbo].[UserRoles]
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Account]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Account] ON [dbo].[Users]
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_EmployeeNumber]    Script Date: 2026/3/11 下午 10:31:03 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_EmployeeNumber] ON [dbo].[Users]
(
	[EmployeeNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ArticleCategories] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[ArticleCategories] ADD  DEFAULT ((1)) FOR [IsEnabled]
GO
ALTER TABLE [dbo].[Articles] ADD  DEFAULT ((0)) FOR [IsPinned]
GO
ALTER TABLE [dbo].[Articles] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((0)) FOR [DiscountType]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((0)) FOR [DiscountValue]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((0)) FOR [MinSpend]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT ((1)) FOR [IsTakeOut]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT ((0)) FOR [IsLimited]
GO
ALTER TABLE [dbo].[EmailQueues] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[EmailQueues] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [MinSpend]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [DiscountValue]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Functions] ADD  CONSTRAINT [DF_Functions_IsOwnerOnly]  DEFAULT ((0)) FOR [IsOwnerOnly]
GO
ALTER TABLE [dbo].[MemberCoupons] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[MemberFavorites] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Members] ADD  CONSTRAINT [DF__Members__IsBlack__10566F31]  DEFAULT ((0)) FOR [IsBlacklisted]
GO
ALTER TABLE [dbo].[Members] ADD  CONSTRAINT [DF__Members__Created__114A936A]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Members] ADD  CONSTRAINT [DF__Members__IsDelet__123EB7A3]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Members] ADD  CONSTRAINT [DF_Members_IsConfirmed]  DEFAULT ((0)) FOR [IsConfirmed]
GO
ALTER TABLE [dbo].[Members] ADD  CONSTRAINT [DF_Members_AvatarFileName]  DEFAULT (NULL) FOR [AvatarFileName]
GO
ALTER TABLE [dbo].[Members] ADD  CONSTRAINT [DF_Members_BlacklistReason]  DEFAULT (NULL) FOR [BlacklistReason]
GO
ALTER TABLE [dbo].[OrderDetails] ADD  DEFAULT ((1)) FOR [Qty]
GO
ALTER TABLE [dbo].[OrderDetails] ADD  DEFAULT ((0)) FOR [SubTotal]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [InOrOut]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderAt]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [DiscountAmount]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [TotalAmount]
GO
ALTER TABLE [dbo].[PasswordResetTokens] ADD  CONSTRAINT [DF_PasswordResetTokens_IsUsed]  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[PasswordResetTokens] ADD  CONSTRAINT [DF_PasswordResetTokens_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [PaidAt]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ((0)) FOR [DoneOrCancel]
GO
ALTER TABLE [dbo].[PreOrderDetails] ADD  DEFAULT ((1)) FOR [Qty]
GO
ALTER TABLE [dbo].[PreOrderDetails] ADD  DEFAULT ((0)) FOR [SubTotal]
GO
ALTER TABLE [dbo].[PreOrderDetails] ADD  DEFAULT ((0)) FOR [DoneOrCancel]
GO
ALTER TABLE [dbo].[PreOrders] ADD  DEFAULT ((0)) FOR [InOrOut]
GO
ALTER TABLE [dbo].[PreOrders] ADD  DEFAULT (getdate()) FOR [OrderAt]
GO
ALTER TABLE [dbo].[PreOrders] ADD  DEFAULT ((0)) FOR [DiscountAmount]
GO
ALTER TABLE [dbo].[PreOrders] ADD  DEFAULT ((0)) FOR [TotalAmount]
GO
ALTER TABLE [dbo].[PreOrders] ADD  DEFAULT ((0)) FOR [DoneOrCancel]
GO
ALTER TABLE [dbo].[Reservations] ADD  DEFAULT ((1)) FOR [AdultsCount]
GO
ALTER TABLE [dbo].[Reservations] ADD  DEFAULT ((0)) FOR [ChildrenCount]
GO
ALTER TABLE [dbo].[Reservations] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Reservations] ADD  DEFAULT (getdate()) FOR [ReservedAt]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SetMealItems] ADD  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[SetMealItems] ADD  DEFAULT ((0)) FOR [IsOptional]
GO
ALTER TABLE [dbo].[SetMealItems] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[SetMeals] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SetMeals] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SubscriptionPreferences] ADD  DEFAULT ((1)) FOR [IsEmailSubscribed]
GO
ALTER TABLE [dbo].[SubscriptionPreferences] ADD  DEFAULT ((1)) FOR [IsPushSubscribed]
GO
ALTER TABLE [dbo].[SubscriptionPreferences] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Tables] ADD  DEFAULT ((2)) FOR [SeatCount]
GO
ALTER TABLE [dbo].[Tables] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[UserNotifications] ADD  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[UserNotifications] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__CreatedAt__32AB8735]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__IsActive__339FAB6E]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__IsDeleted__3493CFA7]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_MustChangePassword]  DEFAULT ((0)) FOR [MustChangePassword]
GO
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[ArticleCategories] ([Id])
GO
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_Categories]
GO
ALTER TABLE [dbo].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_Events] FOREIGN KEY([EventId])
REFERENCES [dbo].[Events] ([Id])
GO
ALTER TABLE [dbo].[Articles] CHECK CONSTRAINT [FK_Articles_Events]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Parent] FOREIGN KEY([ParentCategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Parent]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [FK_Dishes_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [FK_Dishes_Categories]
GO
ALTER TABLE [dbo].[MemberCoupons]  WITH CHECK ADD  CONSTRAINT [FK_MemberCoupons_Coupons] FOREIGN KEY([CouponId])
REFERENCES [dbo].[Coupons] ([Id])
GO
ALTER TABLE [dbo].[MemberCoupons] CHECK CONSTRAINT [FK_MemberCoupons_Coupons]
GO
ALTER TABLE [dbo].[MemberCoupons]  WITH CHECK ADD  CONSTRAINT [FK_MemberCoupons_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO
ALTER TABLE [dbo].[MemberCoupons] CHECK CONSTRAINT [FK_MemberCoupons_Members]
GO
ALTER TABLE [dbo].[MemberFavorites]  WITH CHECK ADD  CONSTRAINT [FK_MemberFavorites_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO
ALTER TABLE [dbo].[MemberFavorites] CHECK CONSTRAINT [FK_MemberFavorites_Members]
GO
ALTER TABLE [dbo].[MemberFavorites]  WITH CHECK ADD  CONSTRAINT [FK_MemberFavorites_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[MemberFavorites] CHECK CONSTRAINT [FK_MemberFavorites_Products]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Products]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Coupons] FOREIGN KEY([CouponId])
REFERENCES [dbo].[Coupons] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Coupons]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Members]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Payments] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Payments]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_PreOrders] FOREIGN KEY([PreOrderId])
REFERENCES [dbo].[PreOrders] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_PreOrders]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Tables] FOREIGN KEY([TableId])
REFERENCES [dbo].[Tables] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Tables]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Users]
GO
ALTER TABLE [dbo].[PasswordResetTokens]  WITH CHECK ADD  CONSTRAINT [FK_PasswordResetTokens_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PasswordResetTokens] CHECK CONSTRAINT [FK_PasswordResetTokens_Users]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Orders] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Orders]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_PreOrders] FOREIGN KEY([PreOrderId])
REFERENCES [dbo].[PreOrders] ([Id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_PreOrders]
GO
ALTER TABLE [dbo].[PreOrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_PreOrderDetails_PreOrders] FOREIGN KEY([PreOrderId])
REFERENCES [dbo].[PreOrders] ([Id])
GO
ALTER TABLE [dbo].[PreOrderDetails] CHECK CONSTRAINT [FK_PreOrderDetails_PreOrders]
GO
ALTER TABLE [dbo].[PreOrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_PreOrderDetails_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[PreOrderDetails] CHECK CONSTRAINT [FK_PreOrderDetails_Products]
GO
ALTER TABLE [dbo].[PreOrders]  WITH CHECK ADD  CONSTRAINT [FK_PreOrders_Coupons] FOREIGN KEY([CouponId])
REFERENCES [dbo].[Coupons] ([Id])
GO
ALTER TABLE [dbo].[PreOrders] CHECK CONSTRAINT [FK_PreOrders_Coupons]
GO
ALTER TABLE [dbo].[PreOrders]  WITH CHECK ADD  CONSTRAINT [FK_PreOrders_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO
ALTER TABLE [dbo].[PreOrders] CHECK CONSTRAINT [FK_PreOrders_Members]
GO
ALTER TABLE [dbo].[PreOrders]  WITH CHECK ADD  CONSTRAINT [FK_PreOrders_Payments] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO
ALTER TABLE [dbo].[PreOrders] CHECK CONSTRAINT [FK_PreOrders_Payments]
GO
ALTER TABLE [dbo].[PreOrders]  WITH CHECK ADD  CONSTRAINT [FK_PreOrders_Tables] FOREIGN KEY([TableId])
REFERENCES [dbo].[Tables] ([Id])
GO
ALTER TABLE [dbo].[PreOrders] CHECK CONSTRAINT [FK_PreOrders_Tables]
GO
ALTER TABLE [dbo].[PreOrders]  WITH CHECK ADD  CONSTRAINT [FK_PreOrders_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PreOrders] CHECK CONSTRAINT [FK_PreOrders_Users]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Dishes] FOREIGN KEY([DishId])
REFERENCES [dbo].[Dishes] ([Id])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Dishes]
GO
ALTER TABLE [dbo].[RoleFunctions]  WITH CHECK ADD  CONSTRAINT [FK_RoleFunctions_Functions] FOREIGN KEY([FunctionId])
REFERENCES [dbo].[Functions] ([Id])
GO
ALTER TABLE [dbo].[RoleFunctions] CHECK CONSTRAINT [FK_RoleFunctions_Functions]
GO
ALTER TABLE [dbo].[RoleFunctions]  WITH CHECK ADD  CONSTRAINT [FK_RoleFunctions_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[RoleFunctions] CHECK CONSTRAINT [FK_RoleFunctions_Roles]
GO
ALTER TABLE [dbo].[SetMealItems]  WITH CHECK ADD  CONSTRAINT [FK_SetMealItems_Dishes] FOREIGN KEY([DishId])
REFERENCES [dbo].[Dishes] ([Id])
GO
ALTER TABLE [dbo].[SetMealItems] CHECK CONSTRAINT [FK_SetMealItems_Dishes]
GO
ALTER TABLE [dbo].[SetMealItems]  WITH CHECK ADD  CONSTRAINT [FK_SetMealItems_SetMeals] FOREIGN KEY([SetMealId])
REFERENCES [dbo].[SetMeals] ([Id])
GO
ALTER TABLE [dbo].[SetMealItems] CHECK CONSTRAINT [FK_SetMealItems_SetMeals]
GO
ALTER TABLE [dbo].[SubscriptionPreferences]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPreferences_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO
ALTER TABLE [dbo].[SubscriptionPreferences] CHECK CONSTRAINT [FK_SubscriptionPreferences_Members]
GO
ALTER TABLE [dbo].[UserNotifications]  WITH CHECK ADD  CONSTRAINT [FK_UserNotifications_Articles] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Articles] ([Id])
GO
ALTER TABLE [dbo].[UserNotifications] CHECK CONSTRAINT [FK_UserNotifications_Articles]
GO
ALTER TABLE [dbo].[UserNotifications]  WITH CHECK ADD  CONSTRAINT [FK_UserNotifications_Members] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Members] ([Id])
GO
ALTER TABLE [dbo].[UserNotifications] CHECK CONSTRAINT [FK_UserNotifications_Members]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [CK_Categories_DisplayOrder] CHECK  (([DisplayOrder]>=(0)))
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [CK_Categories_DisplayOrder]
GO
ALTER TABLE [dbo].[Coupons]  WITH CHECK ADD  CONSTRAINT [CK_Coupons_DiscountValue] CHECK  (([DiscountValue]>(0)))
GO
ALTER TABLE [dbo].[Coupons] CHECK CONSTRAINT [CK_Coupons_DiscountValue]
GO
ALTER TABLE [dbo].[Coupons]  WITH CHECK ADD  CONSTRAINT [CK_Coupons_MinSpend] CHECK  (([MinSpend]>=(0)))
GO
ALTER TABLE [dbo].[Coupons] CHECK CONSTRAINT [CK_Coupons_MinSpend]
GO
ALTER TABLE [dbo].[Coupons]  WITH CHECK ADD  CONSTRAINT [CK_Coupons_Type] CHECK  (([DiscountType]=(1) OR [DiscountType]=(0)))
GO
ALTER TABLE [dbo].[Coupons] CHECK CONSTRAINT [CK_Coupons_Type]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [CK_Dishes_Price] CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [CK_Dishes_Price]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [CK_Events_Dates] CHECK  (([EndDate]>[StartDate]))
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [CK_Events_Dates]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [CK_Events_MinSpend] CHECK  (([MinSpend]>=(0)))
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [CK_Events_MinSpend]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [CK_OrderDetails_Qty] CHECK  (([Qty]>(0)))
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [CK_OrderDetails_Qty]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [CK_OrderDetails_UnitPrice] CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [CK_OrderDetails_UnitPrice]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [CK_Orders_TotalAmount] CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [CK_Orders_TotalAmount]
GO
ALTER TABLE [dbo].[PreOrderDetails]  WITH CHECK ADD  CONSTRAINT [CK_PreOrderDetails_Qty] CHECK  (([Qty]>(0)))
GO
ALTER TABLE [dbo].[PreOrderDetails] CHECK CONSTRAINT [CK_PreOrderDetails_Qty]
GO
ALTER TABLE [dbo].[PreOrderDetails]  WITH CHECK ADD  CONSTRAINT [CK_PreOrderDetails_UnitPrice] CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[PreOrderDetails] CHECK CONSTRAINT [CK_PreOrderDetails_UnitPrice]
GO
ALTER TABLE [dbo].[PreOrders]  WITH CHECK ADD  CONSTRAINT [CK_PreOrders_TotalAmount] CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[PreOrders] CHECK CONSTRAINT [CK_PreOrders_TotalAmount]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [CK_Products_ProductType] CHECK  (([ProductType]='SetMeal' OR [ProductType]='Dish'))
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [CK_Products_ProductType]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [CK_Reservations_Adults] CHECK  (([AdultsCount]>(0)))
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [CK_Reservations_Adults]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [CK_Reservations_Children] CHECK  (([ChildrenCount]>=(0)))
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [CK_Reservations_Children]
GO
ALTER TABLE [dbo].[Reservations]  WITH CHECK ADD  CONSTRAINT [CK_Reservations_Status] CHECK  (([Status]>=(0) AND [Status]<=(3)))
GO
ALTER TABLE [dbo].[Reservations] CHECK CONSTRAINT [CK_Reservations_Status]
GO
ALTER TABLE [dbo].[SetMealItems]  WITH CHECK ADD  CONSTRAINT [CK_SetMealItems_DisplayOrder] CHECK  (([DisplayOrder]>=(0)))
GO
ALTER TABLE [dbo].[SetMealItems] CHECK CONSTRAINT [CK_SetMealItems_DisplayOrder]
GO
ALTER TABLE [dbo].[SetMealItems]  WITH CHECK ADD  CONSTRAINT [CK_SetMealItems_PickLimit] CHECK  (([PickLimit]>=(1)))
GO
ALTER TABLE [dbo].[SetMealItems] CHECK CONSTRAINT [CK_SetMealItems_PickLimit]
GO
ALTER TABLE [dbo].[SetMealItems]  WITH CHECK ADD  CONSTRAINT [CK_SetMealItems_Quantity] CHECK  (([Quantity]>=(1)))
GO
ALTER TABLE [dbo].[SetMealItems] CHECK CONSTRAINT [CK_SetMealItems_Quantity]
GO
ALTER TABLE [dbo].[SetMeals]  WITH CHECK ADD  CONSTRAINT [CK_SetMeals_DiscountType] CHECK  (([DiscountType]='percent' OR [DiscountType]='fixed'))
GO
ALTER TABLE [dbo].[SetMeals] CHECK CONSTRAINT [CK_SetMeals_DiscountType]
GO
ALTER TABLE [dbo].[SetMeals]  WITH CHECK ADD  CONSTRAINT [CK_SetMeals_DiscountValue] CHECK  (([DiscountValue]>=(0)))
GO
ALTER TABLE [dbo].[SetMeals] CHECK CONSTRAINT [CK_SetMeals_DiscountValue]
GO
ALTER TABLE [dbo].[SetMeals]  WITH CHECK ADD  CONSTRAINT [CK_SetMeals_SetPrice] CHECK  (([SetPrice]>=(0)))
GO
ALTER TABLE [dbo].[SetMeals] CHECK CONSTRAINT [CK_SetMeals_SetPrice]
GO
ALTER TABLE [dbo].[Tables]  WITH CHECK ADD  CONSTRAINT [CK_Tables_SeatCount] CHECK  (([SeatCount]>(0)))
GO
ALTER TABLE [dbo].[Tables] CHECK CONSTRAINT [CK_Tables_SeatCount]
GO
ALTER TABLE [dbo].[Tables]  WITH CHECK ADD  CONSTRAINT [CK_Tables_Status] CHECK  (([Status]>=(0) AND [Status]<=(2)))
GO
ALTER TABLE [dbo].[Tables] CHECK CONSTRAINT [CK_Tables_Status]
GO
USE [master]
GO
ALTER DATABASE [EatTogetherDB] SET  READ_WRITE 
GO
