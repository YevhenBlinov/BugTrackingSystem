create table [BugAttachments]
(
BugAttachmentID int identity not null primary key,
BugID int not null foreign key references Bugs(BugID),
Attachment nvarchar(1100) null
)