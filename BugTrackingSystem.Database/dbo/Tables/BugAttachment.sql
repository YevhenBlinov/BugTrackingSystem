create table [BugAttachment]
(
BugAttachmentID int identity not null primary key,
BugID int not null foreign key references Bug(BugID),
Attachment nvarchar(1100) null
)