create table [Filter]
(
FilterID int identity not null primary key,
UserID int not null foreign key references [User](UserID),
Title nvarchar(100) not null,
Project nvarchar(max) null,
AssignedUser nvarchar(max) null,
Search varchar(200) null,
BugStatus nvarchar(max) null,
BugPriority nvarchar(max) null,
)