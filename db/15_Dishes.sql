USE [YiQiEat_DB]
GO
/****** Object:  Table [dbo].[Dishes]    Script Date: 2026/2/20 下午 03:44:31 ******/
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
SET IDENTITY_INSERT [dbo].[Dishes] ON 

INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (1, 1, N'經典瑪格麗特披薩', CAST(320.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'番茄與莫札瑞拉的經典組合', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (2, 1, N'奶油培根義大利麵', CAST(280.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'濃郁奶油搭配煙燻培根', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (3, 1, N'青醬雞肉燉飯', CAST(300.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'香濃羅勒青醬', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (4, 2, N'可樂', CAST(60.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'冰涼暢快', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (5, 2, N'檸檬紅茶', CAST(70.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'清爽解膩', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (6, 2, N'拿鐵咖啡', CAST(90.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'義式濃縮咖啡', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (7, 3, N'提拉米蘇', CAST(120.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'經典義式甜點', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (8, 3, N'焦糖布丁', CAST(100.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'香甜滑嫩', NULL, 1, 0, NULL, NULL, NULL)
INSERT [dbo].[Dishes] ([Id], [CategoryId], [DishName], [Price], [IsActive], [CreatedAt], [Description], [ImageUrl], [IsTakeOut], [IsLimited], [StartDate], [EndDate], [UpdatedAt]) VALUES (9, 3, N'巧克力熔岩蛋糕', CAST(150.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:13.0000000' AS DateTime2), N'濃郁爆漿', NULL, 1, 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Dishes] OFF
GO
ALTER TABLE [dbo].[Dishes] ADD  CONSTRAINT [DF_Dishes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Dishes] ADD  CONSTRAINT [DF_Dishes_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Dishes] ADD  CONSTRAINT [DF_Dishes_IsTakeOut]  DEFAULT ((1)) FOR [IsTakeOut]
GO
ALTER TABLE [dbo].[Dishes] ADD  CONSTRAINT [DF_Dishes_IsLimited]  DEFAULT ((0)) FOR [IsLimited]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [FK_Dishes_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [FK_Dishes_CategoryId]
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [CK_Dishes_Price] CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [CK_Dishes_Price]
GO
