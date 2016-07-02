create table [Bug]
(
BugID int identity not null primary key,
ProjectID int not null foreign key references [Project](ProjectID),
AssignedUserID int null foreign key references [User](UserID),
[Subject] nvarchar(200) not null, 
CreationDate datetime2 not null,
ModificationDate datetime2 not null,
[StatusID] int not null foreign key references BugStatus(BugStatusID),
[PriorityID] int not null foreign key references BugPriority(BugPriorityID),
[Description] nvarchar(max) not null,
Comments nvarchar(1100) null
)