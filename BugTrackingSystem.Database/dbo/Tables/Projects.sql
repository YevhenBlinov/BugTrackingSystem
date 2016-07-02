create table [Projects]
(
ProjectID int identity not null primary key,
Name nvarchar(50) unique not null,
Prefix nchar(3) unique not null
)