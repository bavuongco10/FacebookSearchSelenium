USE [TestBD1]
GO
/****** Object:  Table [dbo].[Comment]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comment](
	[CommentId] [int] IDENTITY(1,1) NOT NULL,
	[CommentOwner] [nvarchar](200) NULL,
	[CommentContent] [nvarchar](4000) NULL,
	[CommentDateTime] [text] NULL,
	[CommentLikesCount] [nchar](10) NULL,
	[CommentShareCount] [nchar](10) NULL,
	[PostLink] [nvarchar](400) NULL,
 CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED 
(
	[CommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FamilyRole]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FamilyRole](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[FacebookID1] [nvarchar](100) NULL,
	[FacebookID2] [nvarchar](max) NULL,
	[Role] [nvarchar](100) NULL,
 CONSTRAINT [PK_FamilyRole] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[People]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[People](
	[PeopleId] [int] IDENTITY(1,1) NOT NULL,
	[FacebookId] [nvarchar](100) NULL,
	[FacebookName] [nvarchar](100) NULL,
	[Skills] [nvarchar](max) NULL,
	[Phone] [nvarchar](100) NULL,
	[AddressName] [nvarchar](max) NULL,
	[AddressMapLink] [nvarchar](max) NULL,
	[Screennames] [nvarchar](max) NULL,
	[Website] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Birthday] [nvarchar](100) NULL,
	[Gender] [nvarchar](100) NULL,
	[InterestedIn] [nvarchar](max) NULL,
	[Languages] [nvarchar](max) NULL,
	[ReligousViews] [nvarchar](max) NULL,
	[ReligousViewsLink] [nvarchar](max) NULL,
	[PoliticalViews] [nvarchar](max) NULL,
	[PoliticalViewsLink] [nvarchar](max) NULL,
	[Relationship] [nvarchar](max) NULL,
	[CommentOwner] [nvarchar](200) NULL,
 CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED 
(
	[PeopleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[People_test]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[People_test](
	[PeopleId] [int] IDENTITY(1,1) NOT NULL,
	[FacebookId] [nvarchar](100) NULL,
	[FacebookName] [nvarchar](100) NULL,
	[Skills] [nvarchar](max) NULL,
	[Phone] [nvarchar](100) NULL,
	[AddressName] [nvarchar](max) NULL,
	[AddressMapLink] [nvarchar](max) NULL,
	[Screennames] [nvarchar](max) NULL,
	[Website] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Birthday] [nvarchar](100) NULL,
	[Gender] [nvarchar](100) NULL,
	[InterestedIn] [nvarchar](max) NULL,
	[Languages] [nvarchar](max) NULL,
	[ReligousViews] [nvarchar](max) NULL,
	[ReligousViewsLink] [nvarchar](max) NULL,
	[PoliticalViews] [nvarchar](max) NULL,
	[PoliticalViewsLink] [nvarchar](max) NULL,
	[Relationship] [nvarchar](max) NULL,
	[CommentOwner] [nvarchar](200) NULL,
 CONSTRAINT [PK_People_test] PRIMARY KEY CLUSTERED 
(
	[PeopleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PeoplePlace]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeoplePlace](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Url] [nvarchar](max) NULL,
	[Role] [nvarchar](max) NULL,
	[FacebookID] [nvarchar](100) NULL,
 CONSTRAINT [PK_PeoplePlace] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PeopleWorkEdu]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeopleWorkEdu](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FacebookID] [nvarchar](100) NULL,
	[Link] [nvarchar](max) NULL,
	[Level] [nvarchar](max) NULL,
	[DateTime] [nvarchar](max) NULL,
	[Role] [nvarchar](100) NULL,
 CONSTRAINT [PK_PeopleWorkEdu] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Post]    Script Date: 5/13/2016 4:28:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Post](
	[PostId] [int] IDENTITY(1,1) NOT NULL,
	[PostOwner] [text] NULL,
	[PostLink] [nvarchar](400) NULL,
	[PostDateTime] [text] NULL,
	[PostContent] [nvarchar](4000) NULL,
	[SearchString] [text] NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
