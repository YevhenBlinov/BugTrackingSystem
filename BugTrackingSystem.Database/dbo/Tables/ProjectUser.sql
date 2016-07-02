create table [ProjectUser]
(
UserID int not null foreign key references [User](UserID),
ProjectID int not null foreign key references [Project](ProjectID)
)