create table [Bugs]
(
BugID int identity not null primary key,
ProjectID int not null foreign key references [Projects](ProjectID),
AssignedUserID int null foreign key references [Users](UserID),
[Subject] nvarchar(200) not null, 
Number int not null,
CreationDate datetime2 not null,
ModificationDate datetime2 not null,
[StatusID] tinyint not null,
[PriorityID] tinyint not null,
[Description] nvarchar(max) not null,
Comments nvarchar(1100) null
)