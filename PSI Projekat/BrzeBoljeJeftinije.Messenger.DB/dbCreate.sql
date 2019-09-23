/****** Object:  StoredProcedure [dbo].[sp_UpdateUser]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_UpdateUser]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateGroup]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_UpdateGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateFriendRequest]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_UpdateFriendRequest]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateAdminUser]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_UpdateAdminUser]
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreUser]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_StoreUser]
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreReadMessage]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_StoreReadMessage]
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreMessageCryptoMaterial]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_StoreMessageCryptoMaterial]
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreMessage]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_StoreMessage]
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreAttachment]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_StoreAttachment]
GO
/****** Object:  StoredProcedure [dbo].[sp_StoreAdminUser]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_StoreAdminUser]
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveUsersFromGroup]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_RemoveUsersFromGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetMessagesForGroup]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_GetMessagesForGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteUser]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_DeleteUser]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteGroup]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_DeleteGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteFriendship]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_DeleteFriendship]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteFriendRequest]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_DeleteFriendRequest]
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateGroup]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_CreateGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateFriendRequest]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_CreateFriendRequest]
GO
/****** Object:  StoredProcedure [dbo].[sp_AddUsersToGroup]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_AddUsersToGroup]
GO
/****** Object:  StoredProcedure [dbo].[sp_AddFriendship]    Script Date: 01-6-2018 01:34:39 ******/
DROP PROCEDURE IF EXISTS [dbo].[sp_AddFriendship]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageRead]') AND type in (N'U'))
ALTER TABLE [dbo].[MessageRead] DROP CONSTRAINT IF EXISTS [Relationship_User->MessageRead]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageRead]') AND type in (N'U'))
ALTER TABLE [dbo].[MessageRead] DROP CONSTRAINT IF EXISTS [Relationship_Message->MessageRead]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]') AND type in (N'U'))
ALTER TABLE [dbo].[MessageCryptoMaterial] DROP CONSTRAINT IF EXISTS [Relationship_User->MessageCryptoMaterial]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]') AND type in (N'U'))
ALTER TABLE [dbo].[MessageCryptoMaterial] DROP CONSTRAINT IF EXISTS [Relationship_Message->MessageCryptoMaterial]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Message]') AND type in (N'U'))
ALTER TABLE [dbo].[Message] DROP CONSTRAINT IF EXISTS [Relationship_User->Message]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Message]') AND type in (N'U'))
ALTER TABLE [dbo].[Message] DROP CONSTRAINT IF EXISTS [Relationship_Group->Message]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GroupMember]') AND type in (N'U'))
ALTER TABLE [dbo].[GroupMember] DROP CONSTRAINT IF EXISTS [Relationship_User->GroupMember]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GroupMember]') AND type in (N'U'))
ALTER TABLE [dbo].[GroupMember] DROP CONSTRAINT IF EXISTS [Relationship_Group->GroupMember]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Friendship]') AND type in (N'U'))
ALTER TABLE [dbo].[Friendship] DROP CONSTRAINT IF EXISTS [Relationship_User(User2)->Friendship]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Friendship]') AND type in (N'U'))
ALTER TABLE [dbo].[Friendship] DROP CONSTRAINT IF EXISTS [Relationship_User(User1)->Friendship]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FriendRequest]') AND type in (N'U'))
ALTER TABLE [dbo].[FriendRequest] DROP CONSTRAINT IF EXISTS [Relationship_User(Sender)->FriendRequest]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FriendRequest]') AND type in (N'U'))
ALTER TABLE [dbo].[FriendRequest] DROP CONSTRAINT IF EXISTS [Relationship_User(Receiver)->FriendRequest]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attachment]') AND type in (N'U'))
ALTER TABLE [dbo].[Attachment] DROP CONSTRAINT IF EXISTS [Realtionship_Message->Attachment]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GroupMember]') AND type in (N'U'))
ALTER TABLE [dbo].[GroupMember] DROP CONSTRAINT IF EXISTS [IsAdmin_Default_Value]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Group]') AND type in (N'U'))
ALTER TABLE [dbo].[Group] DROP CONSTRAINT IF EXISTS [IsBinary_Default_Value]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FriendRequest]') AND type in (N'U'))
ALTER TABLE [dbo].[FriendRequest] DROP CONSTRAINT IF EXISTS [Seen_Default_Value]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FriendRequest]') AND type in (N'U'))
ALTER TABLE [dbo].[FriendRequest] DROP CONSTRAINT IF EXISTS [Resolved_Default_Value]
GO
/****** Object:  Table [dbo].[User]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[User]
GO
/****** Object:  Table [dbo].[MessageRead]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[MessageRead]
GO
/****** Object:  Table [dbo].[MessageCryptoMaterial]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[MessageCryptoMaterial]
GO
/****** Object:  Table [dbo].[Message]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[Message]
GO
/****** Object:  Table [dbo].[GroupMember]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[GroupMember]
GO
/****** Object:  Table [dbo].[Group]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[Group]
GO
/****** Object:  Table [dbo].[Friendship]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[Friendship]
GO
/****** Object:  Table [dbo].[FriendRequest]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[FriendRequest]
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[Attachment]
GO
/****** Object:  Table [dbo].[AdminUser]    Script Date: 01-6-2018 01:34:39 ******/
DROP TABLE IF EXISTS [dbo].[AdminUser]
GO
/****** Object:  UserDefinedTableType [dbo].[ListOfUsers]    Script Date: 01-6-2018 01:34:39 ******/
DROP TYPE IF EXISTS [dbo].[ListOfUsers]
GO
/****** Object:  UserDefinedTableType [dbo].[ListOfUsers]    Script Date: 01-6-2018 01:34:39 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'ListOfUsers' AND ss.name = N'dbo')
CREATE TYPE [dbo].[ListOfUsers] AS TABLE(
	[User] [int] NOT NULL
)
GO
/****** Object:  Table [dbo].[AdminUser]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminUser]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AdminUser](
	[AdminUserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Password] [varchar](100) NOT NULL,
 CONSTRAINT [XPKAdminUser] PRIMARY KEY CLUSTERED 
(
	[AdminUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attachment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Attachment](
	[AttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](100) NOT NULL,
	[FileExtension] [varchar](100) NOT NULL,
	[Content] [varbinary](max) NOT NULL,
	[MessageId] [int] NOT NULL,
 CONSTRAINT [XPKAttachment] PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[FriendRequest]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FriendRequest]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FriendRequest](
	[SenderId] [int] NOT NULL,
	[ReceiverId] [int] NOT NULL,
	[Resolved] [bit] NOT NULL,
	[Seen] [bit] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [XPKFriendRequest] PRIMARY KEY CLUSTERED 
(
	[SenderId] ASC,
	[ReceiverId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Friendship]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Friendship]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Friendship](
	[User1] [int] NOT NULL,
	[User2] [int] NOT NULL,
 CONSTRAINT [XPKFriendship] PRIMARY KEY CLUSTERED 
(
	[User2] ASC,
	[User1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Group]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Group]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Group](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Picture] [varbinary](max) NULL,
	[PictureType] [varchar](100) NULL,
	[IsBinary] [bit] NOT NULL,
 CONSTRAINT [XPKGroup] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[GroupMember]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GroupMember]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[GroupMember](
	[GroupId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [XPKGroupMember] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Message]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Message]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Message](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NOT NULL,
	[SenderId] [int] NOT NULL,
	[Datetime] [datetime] NOT NULL,
	[Text] [varchar](255) NOT NULL,
 CONSTRAINT [XPKMessage] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[MessageCryptoMaterial]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MessageCryptoMaterial](
	[MessageCryptoMaterialId] [int] IDENTITY(1,1) NOT NULL,
	[Material] [varchar](1536) NULL,
	[MessageId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [XPKMessageCryptoMaterial] PRIMARY KEY CLUSTERED 
(
	[MessageCryptoMaterialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[MessageRead]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageRead]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MessageRead](
	[MessageId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [XPKMessageRead] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[User]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CertHash] [varchar](100) NOT NULL,
	[Certificate] [varbinary](8000) NOT NULL,
	[Picture] [varbinary](max) NULL,
	[PictureType] [varchar](100) NULL,
	[RtId] [varchar](200) NULL,
	[BannedUntil] [datetime] NULL,
 CONSTRAINT [XPKUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [CertHash_Unique] UNIQUE NONCLUSTERED 
(
	[CertHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Resolved_Default_Value]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FriendRequest] ADD  CONSTRAINT [Resolved_Default_Value]  DEFAULT ((0)) FOR [Resolved]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Seen_Default_Value]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[FriendRequest] ADD  CONSTRAINT [Seen_Default_Value]  DEFAULT ((0)) FOR [Seen]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsBinary_Default_Value]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Group] ADD  CONSTRAINT [IsBinary_Default_Value]  DEFAULT ((1)) FOR [IsBinary]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsAdmin_Default_Value]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[GroupMember] ADD  CONSTRAINT [IsAdmin_Default_Value]  DEFAULT ((0)) FOR [IsAdmin]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Realtionship_Message->Attachment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Attachment]'))
ALTER TABLE [dbo].[Attachment]  WITH CHECK ADD  CONSTRAINT [Realtionship_Message->Attachment] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Realtionship_Message->Attachment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Attachment]'))
ALTER TABLE [dbo].[Attachment] CHECK CONSTRAINT [Realtionship_Message->Attachment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(Receiver)->FriendRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[FriendRequest]'))
ALTER TABLE [dbo].[FriendRequest]  WITH CHECK ADD  CONSTRAINT [Relationship_User(Receiver)->FriendRequest] FOREIGN KEY([ReceiverId])
REFERENCES [dbo].[User] ([UserId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(Receiver)->FriendRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[FriendRequest]'))
ALTER TABLE [dbo].[FriendRequest] CHECK CONSTRAINT [Relationship_User(Receiver)->FriendRequest]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(Sender)->FriendRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[FriendRequest]'))
ALTER TABLE [dbo].[FriendRequest]  WITH CHECK ADD  CONSTRAINT [Relationship_User(Sender)->FriendRequest] FOREIGN KEY([SenderId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(Sender)->FriendRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[FriendRequest]'))
ALTER TABLE [dbo].[FriendRequest] CHECK CONSTRAINT [Relationship_User(Sender)->FriendRequest]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(User1)->Friendship]') AND parent_object_id = OBJECT_ID(N'[dbo].[Friendship]'))
ALTER TABLE [dbo].[Friendship]  WITH CHECK ADD  CONSTRAINT [Relationship_User(User1)->Friendship] FOREIGN KEY([User1])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(User1)->Friendship]') AND parent_object_id = OBJECT_ID(N'[dbo].[Friendship]'))
ALTER TABLE [dbo].[Friendship] CHECK CONSTRAINT [Relationship_User(User1)->Friendship]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(User2)->Friendship]') AND parent_object_id = OBJECT_ID(N'[dbo].[Friendship]'))
ALTER TABLE [dbo].[Friendship]  WITH CHECK ADD  CONSTRAINT [Relationship_User(User2)->Friendship] FOREIGN KEY([User2])
REFERENCES [dbo].[User] ([UserId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User(User2)->Friendship]') AND parent_object_id = OBJECT_ID(N'[dbo].[Friendship]'))
ALTER TABLE [dbo].[Friendship] CHECK CONSTRAINT [Relationship_User(User2)->Friendship]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Group->GroupMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[GroupMember]'))
ALTER TABLE [dbo].[GroupMember]  WITH CHECK ADD  CONSTRAINT [Relationship_Group->GroupMember] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([GroupId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Group->GroupMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[GroupMember]'))
ALTER TABLE [dbo].[GroupMember] CHECK CONSTRAINT [Relationship_Group->GroupMember]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->GroupMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[GroupMember]'))
ALTER TABLE [dbo].[GroupMember]  WITH CHECK ADD  CONSTRAINT [Relationship_User->GroupMember] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->GroupMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[GroupMember]'))
ALTER TABLE [dbo].[GroupMember] CHECK CONSTRAINT [Relationship_User->GroupMember]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Group->Message]') AND parent_object_id = OBJECT_ID(N'[dbo].[Message]'))
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [Relationship_Group->Message] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([GroupId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Group->Message]') AND parent_object_id = OBJECT_ID(N'[dbo].[Message]'))
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [Relationship_Group->Message]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->Message]') AND parent_object_id = OBJECT_ID(N'[dbo].[Message]'))
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [Relationship_User->Message] FOREIGN KEY([SenderId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->Message]') AND parent_object_id = OBJECT_ID(N'[dbo].[Message]'))
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [Relationship_User->Message]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Message->MessageCryptoMaterial]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]'))
ALTER TABLE [dbo].[MessageCryptoMaterial]  WITH CHECK ADD  CONSTRAINT [Relationship_Message->MessageCryptoMaterial] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Message->MessageCryptoMaterial]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]'))
ALTER TABLE [dbo].[MessageCryptoMaterial] CHECK CONSTRAINT [Relationship_Message->MessageCryptoMaterial]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->MessageCryptoMaterial]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]'))
ALTER TABLE [dbo].[MessageCryptoMaterial]  WITH CHECK ADD  CONSTRAINT [Relationship_User->MessageCryptoMaterial] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->MessageCryptoMaterial]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageCryptoMaterial]'))
ALTER TABLE [dbo].[MessageCryptoMaterial] CHECK CONSTRAINT [Relationship_User->MessageCryptoMaterial]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Message->MessageRead]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRead]'))
ALTER TABLE [dbo].[MessageRead]  WITH CHECK ADD  CONSTRAINT [Relationship_Message->MessageRead] FOREIGN KEY([MessageId])
REFERENCES [dbo].[Message] ([MessageId])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_Message->MessageRead]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRead]'))
ALTER TABLE [dbo].[MessageRead] CHECK CONSTRAINT [Relationship_Message->MessageRead]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->MessageRead]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRead]'))
ALTER TABLE [dbo].[MessageRead]  WITH CHECK ADD  CONSTRAINT [Relationship_User->MessageRead] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Relationship_User->MessageRead]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRead]'))
ALTER TABLE [dbo].[MessageRead] CHECK CONSTRAINT [Relationship_User->MessageRead]
GO
/****** Object:  StoredProcedure [dbo].[sp_AddFriendship]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_AddFriendship]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_AddFriendship] AS' 
END
GO
ALTER Procedure [dbo].[sp_AddFriendship]
(
	@User1 integer,
	@User2 integer
)
As
Begin
	Insert Into [dbo].[Friendship](User1, User2)
	Values (@User1, @User2);
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_AddUsersToGroup]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_AddUsersToGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_AddUsersToGroup] AS' 
END
GO
ALTER Procedure [dbo].[sp_AddUsersToGroup]
(
	@list ListOfUsers READONLY,
	@GroupId integer,
	@IsAdmin bit,
	@Iterator integer
)
As
Begin
	DECLARE @UserId integer

	WHILE (@Iterator >= 0)
	BEGIN
		SET @UserId = (Select top 1 [User]
					  From	(Select ROW_NUMBER() Over (Order By [User] DESC) as rownumber, [User] From @list) as foo
					  Where rownumber = @Iterator + 1)
					  
		Insert Into [dbo].[GroupMember](UserId, GroupId, IsAdmin)
		Values (@UserId, @GroupId, @IsAdmin)
		Set @Iterator = @Iterator - 1
	END; 
End


GO
/****** Object:  StoredProcedure [dbo].[sp_CreateFriendRequest]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateFriendRequest]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_CreateFriendRequest] AS' 
END
GO
ALTER Procedure [dbo].[sp_CreateFriendRequest]
(
	@SenderId integer,
	@ReceiverId integer,
	@Resolved bit,
	@Seen bit,
	@TimeStamp datetime
)
As
Begin
	Insert Into [dbo].[FriendRequest](SenderId, ReceiverId, Resolved, Seen, [TimeStamp])
	Values (@SenderId, @ReceiverId, @Resolved, @Seen, @TimeStamp);
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_CreateGroup]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_CreateGroup] AS' 
END
GO
ALTER Procedure [dbo].[sp_CreateGroup]
(
	@Name nvarchar(100)  = null,
	@TimeStamp datetime,
	@Picture varbinary(max) = null,
	@PictureType varchar(100) = null,
	@IsBinary bit
)
As
Begin
	Insert Into [dbo].[Group](Name, [TimeStamp], Picture, PictureType, [IsBinary])
	Values (@Name, @TimeStamp, @Picture, @PictureType, @IsBinary);
	SELECT SCOPE_IDENTITY();
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteFriendRequest]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteFriendRequest]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_DeleteFriendRequest] AS' 
END
GO
ALTER Procedure [dbo].[sp_DeleteFriendRequest]
(
	@SenderId integer,
	@ReceiverId integer
)
As
Begin
	Delete From [dbo].[FriendRequest]
	Where SenderId = @SenderId
	And ReceiverId = @ReceiverId
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteFriendship]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteFriendship]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_DeleteFriendship] AS' 
END
GO
ALTER Procedure [dbo].[sp_DeleteFriendship]
(
	@User1Id integer,
	@User2Id integer
)
As
Begin
	DECLARE @GroupId integer

	Set @GroupId = (Select G.GroupId
				   From [dbo].[Group] G
				   Join [dbo].[GroupMember] M ON G.GroupId = M.GroupId
				   WHERE M.UserId = @User1Id AND G.IsBinary = 1 AND G.GroupId IN (Select G2.GroupId
																				    From [dbo].[Group] G2
																				    Join [dbo].[GroupMember] M2
																				    ON G2.GroupId = M2.GroupId
																				    WHERE M2.UserId = @User2Id AND G2.IsBinary = 1
																				   )
					)
																				
	Delete From [dbo].[Group]
	Where GroupId = @GroupId
	
	Delete From [dbo].[Friendship]
	Where (User1 = @User1Id AND User2 = @User2Id) or (User1 = @User2Id AND User2 = @User1Id)
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteGroup]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_DeleteGroup] AS' 
END
GO
ALTER Procedure [dbo].[sp_DeleteGroup]
(
	@GroupId integer
)
As
Begin
	Delete From [dbo].[Group]
	Where GroupId = @GroupId
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteUser]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_DeleteUser] AS' 
END
GO
ALTER Procedure [dbo].[sp_DeleteUser]
(
	@UserId integer
)
As
Begin
	Delete From dbo.[Group]
	WHERE IsBinary=1 
		AND EXISTS
		(SELECT * FROM dbo.[GroupMember] WHERE dbo.GroupMember.UserId=@UserId AND dbo.GroupMember.GroupId=dbo.[Group].GroupId);
	Delete From [dbo].[FriendRequest]
	Where ReceiverId = @UserId
	
	Delete From [dbo].[Friendship]
	Where User2 = @UserId OR User1=@UserId
	
	Delete From [dbo].[MessageCryptoMaterial]
	Where UserId = @UserId
	
	Delete From [dbo].[MessageRead]
	Where UserId = @UserId

	Delete From [dbo].[User]
	Where UserId = @UserId
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_GetMessagesForGroup]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetMessagesForGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_GetMessagesForGroup] AS' 
END
GO
ALTER Procedure [dbo].[sp_GetMessagesForGroup]
(
	@GroupId integer,
	@StartRow integer,
	@EndRow integer,
	@User integer=NULL
)
As
Begin
	IF @User is not NULL
	BEGIN
		INSERT INTO dbo.MessageRead(MessageId, UserId)
		SELECT dbo.Message.MessageId, @User
		FROM dbo.Message LEFT OUTER JOIN dbo.MessageRead ON (dbo.Message.MessageId=dbo.MessageRead.MessageId AND dbo.MessageRead.UserId=@User)
		WHERE GroupId=@GroupId AND dbo.MessageRead.UserId is NULL
		ORDER BY [Datetime] DESC
		OFFSET @StartRow ROWS
		FETCH NEXT (@EndRow-@StartRow) ROWS ONLY;
	END
	SELECT MessageId, GroupId, SenderId, [Datetime], [Text]
	FROM dbo.Message
	WHERE GroupId=@GroupId
	ORDER BY [Datetime] DESC
	OFFSET @StartRow ROWS
	FETCH NEXT (@EndRow-@StartRow) ROWS ONLY;
End


GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveUsersFromGroup]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RemoveUsersFromGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_RemoveUsersFromGroup] AS' 
END
GO
ALTER Procedure [dbo].[sp_RemoveUsersFromGroup]
(
	@list ListOfUsers READONLY,
	@GroupId integer,
	@Iterator integer
)
As
Begin	
	DECLARE @UserId integer

	WHILE (@Iterator >= 0)
	BEGIN
		SET @UserId = (Select top 1 [User]
					  From	(Select ROW_NUMBER() Over (Order By [User] DESC) as rownumber, [User] From @list) as foo
					  Where rownumber = @Iterator + 1)
		Delete From [dbo].[GroupMember]
		Where UserId = @UserId
		And GroupId = @GroupId

		SET @Iterator = @Iterator - 1
	END; 

	If ((Select Count(*) From [dbo].[GroupMember] Where GroupId = @GroupId) <= 1)
		Delete From [dbo].[Group]
		Where GroupId = @GroupId
End


GO
/****** Object:  StoredProcedure [dbo].[sp_StoreAdminUser]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreAdminUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_StoreAdminUser] AS' 
END
GO
ALTER Procedure [dbo].[sp_StoreAdminUser]
(
	@Username nvarchar(100),
	@Password varchar(100)
)
As
Begin
	Insert Into [dbo].[AdminUser](Username, [Password])
	Values (@Username, @Password);
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_StoreAttachment]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreAttachment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_StoreAttachment] AS' 
END
GO
ALTER Procedure [dbo].[sp_StoreAttachment]
(
	@FileName nvarchar(100),
	@FileExtension varchar(100),
	@Content varbinary(max),
	@MessageId integer
)
As
Begin
	Insert Into [dbo].[Attachment](FileName, FileExtension, Content, MessageId)
	Values (@FileName, @FileExtension, @Content, @MessageId);
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_StoreMessage]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_StoreMessage] AS' 
END
GO
ALTER Procedure [dbo].[sp_StoreMessage]
(
	@GroupId integer,
	@SenderId integer,
	@Datetime datetime,
	@Text varchar(255)
)
As
Begin
	Insert Into [dbo].[Message](GroupId, SenderId, [DateTime], [Text])
	Values (@GroupId, @SenderId, @DateTime, @Text);
	SELECT SCOPE_IDENTITY();
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_StoreMessageCryptoMaterial]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreMessageCryptoMaterial]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_StoreMessageCryptoMaterial] AS' 
END
GO
ALTER Procedure [dbo].[sp_StoreMessageCryptoMaterial]
(
	@Material varchar(1536),
	@MessageId integer,
	@UserId integer
)
As
Begin
	Insert Into [dbo].[MessageCryptoMaterial](Material, MessageId, UserId)
	Values (@Material, @MessageId, @UserId);
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_StoreReadMessage]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreReadMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_StoreReadMessage] AS' 
END
GO
ALTER Procedure [dbo].[sp_StoreReadMessage]
(
	@UserId integer,
	@MessageId integer
)
As
Begin
	Insert Into [dbo].[MessageRead](UserId, MessageId)
	Values (@UserId, @MessageId);
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_StoreUser]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_StoreUser] AS' 
END
GO
ALTER Procedure [dbo].[sp_StoreUser]
(
	@Name nvarchar(100),
	@CertHash varchar(100),
	@Certificate binary(8000),
	@Picture varbinary(max) = null,
	@PictureType varchar(100) = null,
	@RtId varchar(200) = null,
	@BannedUntil datetime = null
)
As
Begin
	Insert Into [dbo].[User](Name, CertHash, [Certificate], Picture, PictureType, RtId, BannedUntil)
	Values (@Name, @CertHash, @Certificate, @Picture, @PictureType, @RtId, @BannedUntil);
	SELECT SCOPE_IDENTITY();
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateAdminUser]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateAdminUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_UpdateAdminUser] AS' 
END
GO
ALTER Procedure [dbo].[sp_UpdateAdminUser]
(
	@AdminUserId integer,
	@Username nvarchar(100),
	@Password varchar(100)
)
As
Begin
	Update [dbo].[AdminUser]
	Set Username = @Username, [Password] = @Password
	Where AdminUserId = @AdminUserId
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateFriendRequest]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateFriendRequest]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_UpdateFriendRequest] AS' 
END
GO
ALTER Procedure [dbo].[sp_UpdateFriendRequest]
(
	@SenderId integer,
	@ReceiverId integer,
	@Resolved bit,
	@Seen bit
)
As
Begin
	Update [dbo].[FriendRequest]
	Set Resolved = @Resolved, Seen = @Seen
	Where SenderId = @SenderId
	And ReceiverId = @ReceiverId
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateGroup]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_UpdateGroup] AS' 
END
GO
ALTER Procedure [dbo].[sp_UpdateGroup]
(
	@GroupId integer,
	@Name nvarchar(100),
	@TimeStamp datetime,
	@Picture varbinary(max) = null,
	@PictureType varchar(100) = null,
	@IsBinary bit,
	@AlsoUpdatePicture bit
)
As
Begin
	IF @AlsoUpdatePicture = 1
		Update [dbo].[Group]
		Set [Name] = @Name, 
			[TimeStamp] = @TimeStamp, 
			IsBinary = @IsBinary,
			Picture = @Picture, 
			PictureType = @PictureType
		Where GroupId = @GroupId;
	
	ELSE
		Update [dbo].[Group]
		Set [Name] = @Name, 
			[TimeStamp] = @TimeStamp,
			IsBinary = @IsBinary
		Where GroupId = @GroupId;
End;


GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUser]    Script Date: 01-6-2018 01:34:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[sp_UpdateUser] AS' 
END
GO
ALTER Procedure [dbo].[sp_UpdateUser]
(
	@UserId integer,
	@Name nvarchar(100),
	@CertHash varchar(100),
	@Certificate binary(8000),
	@Picture varbinary(max) = null,
	@PictureType varchar(100) = null,
	@RtId varchar(200) = null,
	@BannedUntil datetime = null
)
As
Begin

	IF @Picture is not null
		Update [dbo].[User]
		Set Name = @Name, 
			CertHash = @CertHash, 
			[Certificate] = @Certificate, 
			Picture = @Picture, 
			PictureType = @PictureType,
			RtId = @RtId, 
			BannedUntil = @BannedUntil
		Where UserId = @UserId;

	ELSE
		Update [dbo].[User]
		Set Name = @Name, 
			CertHash = @CertHash, 
			[Certificate] = @Certificate, 
			RtId = @RtId, 
			BannedUntil = @BannedUntil
		Where UserId = @UserId

End;
GO
INSERT into dbo.AdminUser(Username, Password) VALUES ('root', '$');
GO