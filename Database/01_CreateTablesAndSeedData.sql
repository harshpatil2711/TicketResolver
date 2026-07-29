/*==============================================================
Database : TicketResolverDB
Part 1 : Master & Authentication Tables
==============================================================*/


/*==============================================================
Table : TicketResolverRole
==============================================================*/
CREATE TABLE TicketResolverRole
(
    RoleId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverRole PRIMARY KEY,

    RoleName NVARCHAR(100) NOT NULL,

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverRole_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverRole_IsActive
        DEFAULT(1),

    CONSTRAINT UQ_TicketResolverRole_RoleName
        UNIQUE(RoleName)
);
GO


/*==============================================================
Table : TicketResolverUser
==============================================================*/
CREATE TABLE TicketResolverUser
(
    UserId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverUser PRIMARY KEY,

    RoleId INT NOT NULL,

    FirstName NVARCHAR(100) NOT NULL,

    LastName NVARCHAR(100) NOT NULL,

    Email VARCHAR(200) NOT NULL,

    Mobile VARCHAR(20) NOT NULL,

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverUser_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverUser_IsActive
        DEFAULT(1),

    CONSTRAINT FK_TicketResolverUser_Role
        FOREIGN KEY(RoleId)
        REFERENCES TicketResolverRole(RoleId),

    CONSTRAINT UQ_TicketResolverUser_Email
        UNIQUE(Email),

    CONSTRAINT UQ_TicketResolverUser_Mobile
        UNIQUE(Mobile)
);
GO


/*==============================================================
Table : TicketResolverUserCredential
==============================================================*/
CREATE TABLE TicketResolverUserCredential
(
    CredentialId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverUserCredential PRIMARY KEY,

    UserId INT NOT NULL,

    PasswordHash VARCHAR(500) NOT NULL,

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverUserCredential_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverUserCredential_IsActive
        DEFAULT(1),

    CONSTRAINT FK_TicketResolverUserCredential_User
        FOREIGN KEY(UserId)
        REFERENCES TicketResolverUser(UserId),

    CONSTRAINT UQ_TicketResolverUserCredential_UserId
        UNIQUE(UserId)
);
GO


/*==============================================================
Table : TicketResolverRefreshToken
==============================================================*/
CREATE TABLE TicketResolverRefreshToken
(
    RefreshTokenId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverRefreshToken PRIMARY KEY,

    UserId INT NOT NULL,

    TokenHash VARCHAR(500) NOT NULL,

    ExpiryDate DATETIME NOT NULL,

    LastUsedDate DATETIME NULL,

    RevokedDate DATETIME NULL,

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverRefreshToken_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverRefreshToken_IsActive
        DEFAULT(1),

    CONSTRAINT FK_TicketResolverRefreshToken_User
        FOREIGN KEY(UserId)
        REFERENCES TicketResolverUser(UserId)
);
GO


/*==============================================================
Table : TicketResolverOtpVerification
==============================================================*/
CREATE TABLE TicketResolverOtpVerification
(
    OtpId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverOtpVerification PRIMARY KEY,

    UserId INT NULL,

    Email VARCHAR(200) NOT NULL,

    OtpCode VARCHAR(10) NOT NULL,

    Purpose VARCHAR(20) NOT NULL,

    ExpiryDate DATETIME NOT NULL,

    IsVerified BIT NOT NULL
        CONSTRAINT DF_TicketResolverOtpVerification_IsVerified
        DEFAULT(0),

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverOtpVerification_CreatedDate
        DEFAULT(GETDATE()),

    CONSTRAINT FK_TicketResolverOtpVerification_User
        FOREIGN KEY(UserId)
        REFERENCES TicketResolverUser(UserId),

    CONSTRAINT CK_TicketResolverOtpVerification_Purpose
        CHECK
        (
            Purpose IN
            (
                'Signup',
                'Login',
                'ForgotPassword'
            )
        )
);
GO


/*==============================================================
Table : TicketResolverTicketCategory
==============================================================*/
CREATE TABLE TicketResolverTicketCategory
(
    CategoryId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicketCategory PRIMARY KEY,

    CategoryName NVARCHAR(100) NOT NULL,

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicketCategory_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketCategory_IsActive
        DEFAULT(1),

    CONSTRAINT UQ_TicketResolverTicketCategory_CategoryName
        UNIQUE(CategoryName)
);
GO


/*==============================================================
Table : TicketResolverTicketPriority
==============================================================*/
CREATE TABLE TicketResolverTicketPriority
(
    PriorityId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicketPriority PRIMARY KEY,

    PriorityName NVARCHAR(50) NOT NULL,

    Sequence INT NOT NULL,

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicketPriority_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketPriority_IsActive
        DEFAULT(1),

    CONSTRAINT UQ_TicketResolverTicketPriority_PriorityName
        UNIQUE(PriorityName),

    CONSTRAINT CK_TicketResolverTicketPriority_Sequence
        CHECK (Sequence > 0)
);
GO


/*==============================================================
Table : TicketResolverTicketStatus
==============================================================*/
CREATE TABLE TicketResolverTicketStatus
(
    StatusId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicketStatus PRIMARY KEY,

    StatusName NVARCHAR(50) NOT NULL,

    IsTerminalState BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketStatus_IsTerminalState
        DEFAULT(0),

    CreatedBy INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicketStatus_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketStatus_IsActive
        DEFAULT(1),

    CONSTRAINT UQ_TicketResolverTicketStatus_StatusName
        UNIQUE(StatusName)
);
GO

/*==============================================================
Table : TicketResolverTicket
==============================================================*/
CREATE TABLE TicketResolverTicket
(
    TicketId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicket PRIMARY KEY,

    TicketNumber VARCHAR(9) NOT NULL,

    Subject NVARCHAR(200) NOT NULL,

    Description NVARCHAR(MAX) NOT NULL,

    CategoryId INT NOT NULL,

    PriorityId INT NOT NULL,

    StatusId INT NOT NULL
        CONSTRAINT DF_TicketResolverTicket_StatusId
        DEFAULT(1),

    CreatedBy INT NOT NULL,

    AssignedTo INT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicket_CreatedDate
        DEFAULT(GETDATE()),

    ResolvedDate DATETIME NULL,

    ClosedDate DATETIME NULL,

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicket_IsActive
        DEFAULT(1),

    CONSTRAINT FK_TicketResolverTicket_Category
        FOREIGN KEY(CategoryId)
        REFERENCES TicketResolverTicketCategory(CategoryId),

    CONSTRAINT FK_TicketResolverTicket_Priority
        FOREIGN KEY(PriorityId)
        REFERENCES TicketResolverTicketPriority(PriorityId),

    CONSTRAINT FK_TicketResolverTicket_Status
        FOREIGN KEY(StatusId)
        REFERENCES TicketResolverTicketStatus(StatusId),

    CONSTRAINT FK_TicketResolverTicket_AssignedTo
        FOREIGN KEY(AssignedTo)
        REFERENCES TicketResolverUser(UserId),

    CONSTRAINT UQ_TicketResolverTicket_TicketNumber
        UNIQUE(TicketNumber),

    CONSTRAINT CK_TicketResolverTicket_ResolvedDate
        CHECK
        (
            ResolvedDate IS NULL
            OR
            ResolvedDate >= CreatedDate
        ),

    CONSTRAINT CK_TicketResolverTicket_ClosedDate
        CHECK
        (
            ClosedDate IS NULL
            OR
            ClosedDate >= CreatedDate
        )
);
GO


/*==============================================================
Table : TicketResolverTicketComment
==============================================================*/
CREATE TABLE TicketResolverTicketComment
(
    CommentId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicketComment PRIMARY KEY,

    TicketId INT NOT NULL,

    UserId INT NOT NULL,

    CommentText NVARCHAR(MAX) NOT NULL,

    IsInternalNote BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketComment_IsInternalNote
        DEFAULT(0),

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicketComment_CreatedDate
        DEFAULT(GETDATE()),

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketComment_IsActive
        DEFAULT(1),

    CONSTRAINT FK_TicketResolverTicketComment_Ticket
        FOREIGN KEY(TicketId)
        REFERENCES TicketResolverTicket(TicketId),

    CONSTRAINT FK_TicketResolverTicketComment_User
        FOREIGN KEY(UserId)
        REFERENCES TicketResolverUser(UserId)
);
GO


/*==============================================================
Table : TicketResolverTicketAttachment
==============================================================*/
CREATE TABLE TicketResolverTicketAttachment
(
    AttachmentId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicketAttachment PRIMARY KEY,

    TicketId INT NOT NULL,

    CommentId INT NULL,

    OriginalFileName VARCHAR(255) NOT NULL,

    StoredFileName VARCHAR(255) NOT NULL,

    CreatedBy INT NOT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicketAttachment_CreatedDate
        DEFAULT(GETDATE()),

    ModifiedBy INT NULL,

    ModifiedDate DATETIME NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_TicketResolverTicketAttachment_IsActive
        DEFAULT(1),

    CONSTRAINT FK_TicketResolverTicketAttachment_Ticket
        FOREIGN KEY (TicketId)
        REFERENCES TicketResolverTicket(TicketId),

    CONSTRAINT FK_TicketResolverTicketAttachment_Comment
        FOREIGN KEY (CommentId)
        REFERENCES TicketResolverTicketComment(CommentId)
);
GO

/*==============================================================
Table : TicketResolverTicketStatusHistory
==============================================================*/
CREATE TABLE TicketResolverTicketStatusHistory
(
    HistoryId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverTicketStatusHistory PRIMARY KEY,

    TicketId INT NOT NULL,

    OldStatusId INT NULL,

    NewStatusId INT NOT NULL,

    PreviousAssignedTo INT NULL,

    CurrentAssignedTo INT NULL,

    ChangeReason NVARCHAR(500) NULL,

    CreatedBy INT NOT NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverTicketStatusHistory_CreatedDate
        DEFAULT(GETDATE()),

    CONSTRAINT FK_TicketResolverTicketStatusHistory_Ticket
        FOREIGN KEY(TicketId)
        REFERENCES TicketResolverTicket(TicketId),

    CONSTRAINT FK_TicketResolverTicketStatusHistory_OldStatus
        FOREIGN KEY(OldStatusId)
        REFERENCES TicketResolverTicketStatus(StatusId),

    CONSTRAINT FK_TicketResolverTicketStatusHistory_NewStatus
        FOREIGN KEY(NewStatusId)
        REFERENCES TicketResolverTicketStatus(StatusId),

    CONSTRAINT FK_TicketResolverTicketStatusHistory_PreviousAssignedTo
        FOREIGN KEY(PreviousAssignedTo)
        REFERENCES TicketResolverUser(UserId),

    CONSTRAINT FK_TicketResolverTicketStatusHistory_CurrentAssignedTo
        FOREIGN KEY(CurrentAssignedTo)
        REFERENCES TicketResolverUser(UserId)
);
GO

/*==============================================================
Table : TicketResolverLog
==============================================================*/
CREATE TABLE TicketResolverLog
(
    LogId INT IDENTITY(1,1)
        CONSTRAINT PK_TicketResolverLog PRIMARY KEY,

    LogLevel VARCHAR(20) NOT NULL,

    Source VARCHAR(200) NULL,

    Message NVARCHAR(MAX) NOT NULL,

    Exception NVARCHAR(MAX) NULL,

    StackTrace NVARCHAR(MAX) NULL,

    CreatedDate DATETIME NOT NULL
        CONSTRAINT DF_TicketResolverLog_CreatedDate
        DEFAULT(GETDATE())
);
GO


/*==============================================================
Seed Data : Roles
==============================================================*/

INSERT INTO TicketResolverRole
(
    RoleName,
    CreatedBy,
    CreatedDate,
    IsActive
)
VALUES
('Administrator',NULL,GETDATE(),1),
('Support Executive',NULL,GETDATE(),1),
('Employee',NULL,GETDATE(),1);
GO


/*==============================================================
Seed Data : Ticket Categories
==============================================================*/

INSERT INTO TicketResolverTicketCategory
(
    CategoryName,
    CreatedBy,
    CreatedDate,
    IsActive
)
VALUES
(N'Hardware',NULL,GETDATE(),1),
(N'Software',NULL,GETDATE(),1),
(N'Network',NULL,GETDATE(),1),
(N'Email',NULL,GETDATE(),1),
(N'Printer',NULL,GETDATE(),1),
(N'Other',NULL,GETDATE(),1);
GO


/*==============================================================
Seed Data : Ticket Priorities
==============================================================*/

INSERT INTO TicketResolverTicketPriority
(
    PriorityName,
    Sequence,
    CreatedBy,
    CreatedDate,
    IsActive
)
VALUES
(N'Low',1,NULL,GETDATE(),1),
(N'Medium',2,NULL,GETDATE(),1),
(N'High',3,NULL,GETDATE(),1),
(N'Critical',4,NULL,GETDATE(),1);
GO


/*==============================================================
Seed Data : Ticket Statuses
==============================================================*/

INSERT INTO TicketResolverTicketStatus
(
    StatusName,
    IsTerminalState,
    CreatedBy,
    CreatedDate,
    IsActive
)
VALUES
(N'New',0,NULL,GETDATE(),1),
(N'Assigned',0,NULL,GETDATE(),1),
(N'In Progress',0,NULL,GETDATE(),1),
(N'On Hold',0,NULL,GETDATE(),1),
(N'Resolved',0,NULL,GETDATE(),1),
(N'Closed',1,NULL,GETDATE(),1),
(N'Reopened',0,NULL,GETDATE(),1);
GO
