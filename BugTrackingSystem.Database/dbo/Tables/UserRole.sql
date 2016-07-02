create table [UserRole]
(
UserRoleID int identity not null primary key,
RoleName nvarchar(20) unique not null 
)