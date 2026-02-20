USE [YiQiEat_DB]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 2026/2/20 下午 03:44:31 ******/
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
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([Id], [CategoryName], [IsActive], [CreatedAt], [ParentCategoryId], [DisplayOrder], [ImageUrl], [UpdatedAt]) VALUES (1, N'主餐', 1, CAST(N'2026-02-20T12:37:53.0000000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Categories] ([Id], [CategoryName], [IsActive], [CreatedAt], [ParentCategoryId], [DisplayOrder], [ImageUrl], [UpdatedAt]) VALUES (2, N'飲料', 1, CAST(N'2026-02-20T12:37:53.0000000' AS DateTime2), NULL, 2, NULL, NULL)
INSERT [dbo].[Categories] ([Id], [CategoryName], [IsActive], [CreatedAt], [ParentCategoryId], [DisplayOrder], [ImageUrl], [UpdatedAt]) VALUES (3, N'甜點', 1, CAST(N'2026-02-20T12:37:53.0000000' AS DateTime2), NULL, 3, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Categories] ADD  CONSTRAINT [DF_Categories_DisplayOrder]  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_ParentCategoryId] FOREIGN KEY([ParentCategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_ParentCategoryId]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [CK_Categories_DisplayOrder] CHECK  (([DisplayOrder]>=(0)))
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [CK_Categories_DisplayOrder]
GO
