create table [BugPriority]
(
BugPriorityID int identity not null primary key,
PriorityName nvarchar(20) unique not null
)