create table [BugStatuses]
(
BugStatusID int identity not null primary key,
StatusName nvarchar(20) unique not null
)