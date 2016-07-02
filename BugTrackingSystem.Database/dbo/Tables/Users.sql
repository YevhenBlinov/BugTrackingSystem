create table [Users]
(
UserID int identity not null primary key,
FirstName nvarchar(35) not null,
LastName nvarchar(35) not null,
[Login] nvarchar(100) unique not null,
[Password] nvarchar(100) not null,
Email nvarchar(255) unique not null,
UserRoleID tinyint not null,
[Photo] nvarchar(1100) null
)