create table [ProjectUsers]
(
UserID int not null foreign key references [Users](UserID),
ProjectID int not null foreign key references [Projects](ProjectID)
)