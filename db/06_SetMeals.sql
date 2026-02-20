USE [YiQiEat_DB]
GO
/****** Object:  Table [dbo].[SetMeals]    Script Date: 2026/2/20 下午 03:44:31 ******/
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
SET IDENTITY_INSERT [dbo].[SetMeals] ON 

INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (1, N'雙人分享餐', N'fixed', CAST(100.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:40.0000000' AS DateTime2), CAST(899.00 AS Decimal(10, 2)), N'兩人一起吃剛剛好', NULL, NULL)
INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (2, N'全家分享餐', N'percent', CAST(10.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T12:38:40.0000000' AS DateTime2), CAST(1999.00 AS Decimal(10, 2)), N'家庭聚餐首選', NULL, NULL)
INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (3, N'經典義起餐', N'fixed', CAST(80.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T15:26:11.0000000' AS DateTime2), CAST(599.00 AS Decimal(10, 2)), N'店內招牌組合', NULL, NULL)
INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (4, N'商業午餐餐', N'fixed', CAST(50.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T15:26:11.0000000' AS DateTime2), CAST(399.00 AS Decimal(10, 2)), N'平日快速午餐套餐', NULL, NULL)
INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (5, N'情人節限定餐', N'fixed', CAST(150.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T15:26:11.0000000' AS DateTime2), CAST(1299.00 AS Decimal(10, 2)), N'浪漫雙人套餐', NULL, NULL)
INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (6, N'過年限定餐', N'percent', CAST(15.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T15:26:11.0000000' AS DateTime2), CAST(2680.00 AS Decimal(10, 2)), N'春節團圓豪華餐', NULL, NULL)
INSERT [dbo].[SetMeals] ([Id], [SetMealName], [DiscountType], [DiscountValue], [IsActive], [CreatedAt], [SetPrice], [Description], [ImageUrl], [UpdatedAt]) VALUES (7, N'聖誕節歡樂餐', N'fixed', CAST(200.00 AS Decimal(10, 2)), 1, CAST(N'2026-02-20T15:26:11.0000000' AS DateTime2), CAST(2399.00 AS Decimal(10, 2)), N'聖誕限定豪華套餐', NULL, NULL)
SET IDENTITY_INSERT [dbo].[SetMeals] OFF
GO
ALTER TABLE [dbo].[SetMeals] ADD  CONSTRAINT [DF_SetMeals_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SetMeals] ADD  CONSTRAINT [DF_SetMeals_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SetMeals]  WITH CHECK ADD  CONSTRAINT [CK_SetMeals_DiscountType] CHECK  (([DiscountType]='percent' OR [DiscountType]='fixed'))
GO
ALTER TABLE [dbo].[SetMeals] CHECK CONSTRAINT [CK_SetMeals_DiscountType]
GO
ALTER TABLE [dbo].[SetMeals]  WITH CHECK ADD  CONSTRAINT [CK_SetMeals_DiscountValue] CHECK  (([DiscountValue]>=(0)))
GO
ALTER TABLE [dbo].[SetMeals] CHECK CONSTRAINT [CK_SetMeals_DiscountValue]
GO
ALTER TABLE [dbo].[SetMeals]  WITH CHECK ADD  CONSTRAINT [CK_SetMeals_SetPrice] CHECK  (([SetPrice] IS NULL OR [SetPrice]>=(0)))
GO
ALTER TABLE [dbo].[SetMeals] CHECK CONSTRAINT [CK_SetMeals_SetPrice]
GO
