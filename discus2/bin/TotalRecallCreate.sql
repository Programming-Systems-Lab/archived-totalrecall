-- TotalRecall dbase create script
drop table MeetingResources
drop table ResourceCategories
drop table Resources
drop table ResourceStates
drop table Categories
drop table AccessPolicies
drop table Participants
drop table ContextMessageResponses
drop table ContextMessagesSent
drop table ContactCache
drop table Contacts
drop table Roles
drop table Meetings
drop table MeetingStates
drop table ContextMessageTypes

-- Table holds the constants that describe a meetings state e.g. Active, Ended 
create table MeetingStates
(
	MTG_STATE nvarchar(15) not null constraint PK_MeetingStates Primary key check(len(MTG_STATE) > 0),
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

-- Table holds the constants that describe a resources state e.g. Shared, Recalled
create table ResourceStates
(
	RES_STATE nvarchar(15) not null constraint PK_ResourceStates Primary key check(len(RES_STATE) > 0),
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

-- Table holds the constants that describe a participant's role in the meeting
-- Organizer, Participant
create table Roles
(
	ROLE_NAME nvarchar(25) not null constraint PK_ROLE_NAME Primary key check(len(ROLE_NAME) > 0),
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

-- Table holds the constants that describe the types of context messages that flow
-- among meeting participants IAEntry, IAExit, IALeaveMeeting etc.
create table ContextMessageTypes
(
	CTXMSG_TYPE nvarchar(100) not null constraint PK_ContextMessageTypes Primary key check(len(CTXMSG_TYPE) > 0),
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

-- Table holds data on people I know
create table Contacts
(
	CONTACT_ID nvarchar(100) not null constraint PK_Contacts Primary key check(len(CONTACT_ID) > 0),
	CONTACT_TRUST_SCORE real not null default 0.0 check(CONTACT_TRUST_SCORE >= 0.0),
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
	MODIFYDATE DATETIME NOT NULL DEFAULT GETDATE()
)

-- Table holds data and state of meetings
create table Meetings
(
	MTG_ID nvarchar(100) not null constraint PK_Meeting_ID Primary key check(len(MTG_ID) > 0),
	MTG_STATE nvarchar(15) not null constraint FK_Meeting_State Foreign key references MeetingStates(MTG_STATE),
	MTG_TOPIC ntext not null,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
	MODIFYDATE DATETIME NOT NULL DEFAULT GETDATE()
)

-- Table holds data on participants in meetings
create table Participants
(
	MTG_ID nvarchar(100) not null constraint FK_Participant_Meeting_ID Foreign key references Meetings(MTG_ID) on delete cascade,
	CONTACT_ID nvarchar(100) not null constraint FK_Participants_Contact_ID Foreign key references Contacts(CONTACT_ID) on delete no action,
	PART_ROLE nvarchar(25) not null constraint FK_Participant_Role_Name Foreign key references Roles(ROLE_NAME) on delete no action,
	PART_LOC nvarchar(256) not null check(len(PART_LOC) > 0),
	constraint PK_Participants Primary Key (MTG_ID,CONTACT_ID),
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
	MODIFYDATE DATETIME NOT NULL DEFAULT GETDATE()
)

-- Table holds information on the dynamically generated proxies for 
-- information agents we meet
create table ContactCache
(
	CONTACT_ID nvarchar(100) not null primary key constraint FK_ContactCache_Contact_ID Foreign key references Contacts(CONTACT_ID) on delete no action,	
	PART_LOC nvarchar(256) not null check(len(PART_LOC) > 0)		
)


-- Table holds the catalogue of all resources available to an IA
create table Resources
(
	RES_ID nvarchar(100) not null constraint PK_Resources Primary key check(len(RES_ID) > 0),
	RES_URL nvarchar(256) not null check(len(RES_URL) > 0) constraint UNIQUE_ResourceUrl unique,
	RES_NAME nvarchar(100) not null check(len(RES_NAME) > 0) constraint UNIQUE_ResourceName unique,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
	MODIFYDATE DATETIME NOT NULL DEFAULT GETDATE()
)

-- Table holds the access policies that govern the various categories under which
-- resources fall
create table AccessPolicies
(
	ACCPOL_ID nvarchar(100) not null constraint PK_Policies Primary key check(len(ACCPOL_ID) > 0),
	ACCPOL_NAME nvarchar(100) not null check(len(ACCPOL_NAME) > 0),
	ACCPOL_DOC ntext not null,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
	MODIFYDATE DATETIME NOT NULL DEFAULT GETDATE()
)

create table Categories
(
	CAT_NAME nvarchar(100) not null constraint PK_Categories Primary key check(len(CAT_NAME) > 0),
	ACCPOL_ID nvarchar(100) not null constraint FK_Categories_Accpol_ID Foreign key references AccessPolicies(ACCPOL_ID) on delete no action,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

create table ResourceCategories
(
	RES_ID nvarchar(100) not null constraint FK_ResourceCategories_Resource_ID Foreign key references Resources(RES_ID) on delete no action,
	CAT_NAME nvarchar(100) not null constraint FK_ResourceCategories_Category_Name Foreign key references Categories(CAT_NAME) on delete no action,
	Constraint PK_ResourcesCategories Primary key(RES_ID,CAT_NAME)
)

-- Table holds the subset of the Resources catalog that are/have been involved in a
-- meeting
create table MeetingResources
(
	MTG_ID nvarchar(100) not null constraint FK_Meeting_Resources_ID Foreign key references Meetings(MTG_ID) on delete cascade,
	RES_ID nvarchar(100) not null constraint FK_Resource_ID Foreign key references Resources(RES_ID) on delete no action,
	Constraint PK_MeetingResources Primary key(MTG_ID,RES_ID),
	RES_STATE nvarchar(15) not null constraint FK_Meeting_Resource_State Foreign key references ResourceStates(RES_STATE) on delete no action,
	RES_OWNER nvarchar(100) not null constraint FK_MeetingResources_ContactID Foreign key references Contacts(CONTACT_ID) on delete no action,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
	MODIFYDATE DATETIME NOT NULL DEFAULT GETDATE()
)

-- Table holds a log of the context messages sent
-- only used by meeting organizers
create table ContextMessagesSent
(
	MTG_ID nvarchar(100) not null constraint FK_Context_Messages_Sent_Meeting_ID Foreign key references Meetings(MTG_ID) on delete cascade,
	CTXMSG_ID nvarchar(100) not null check(len(CTXMSG_ID) > 0),
	CONTACT_ID nvarchar(100) not null constraint FK_Context_Messages_Sent_Contact_ID Foreign key references Contacts(CONTACT_ID) on delete no action,
	Constraint PK_ContextMessagesSent Primary key (MTG_ID,CTXMSG_ID,CONTACT_ID),
	CTXMSG_TYPE nvarchar(100) not null constraint FK_Context_Messages_Sent_CtxMsg_Type Foreign key references ContextMessageTypes(CTXMSG_TYPE) on delete no action,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

-- Table holds a log of context message reponses
-- only used by meeting organizers
create table ContextMessageResponses
(
	MTG_ID nvarchar(100) not null constraint FK_Context_Message_Responses_Meeting_ID Foreign key references Meetings(MTG_ID) on delete cascade,
	CTXMSG_ID nvarchar(100) not null check(len(CTXMSG_ID) > 0),
	CONTACT_ID nvarchar(100) not null constraint FK_Context_Message_Responses_Contact_ID Foreign key references Contacts(CONTACT_ID) on delete no action,
	Constraint PK_ContextMessageResponses Primary key (MTG_ID,CTXMSG_ID,CONTACT_ID),
	CTXMSG_TYPE nvarchar(100) not null constraint FK_Context_Message_Responses_CtxMsg_Type Foreign key references ContextMessageTypes(CTXMSG_TYPE) on delete no action,
	CREATEDATE DATETIME NOT NULL DEFAULT GETDATE(),
)

-- Create set of triggers for select tables (6)
-- Table list
-- Contacts
-- Meetings
-- Participants
-- Resources
-- MeetingResources
-- AccessPolicies

-- Create trigger for Contacts table
use TotalRecall
IF EXISTS (SELECT name FROM sysobjects
   WHERE name = 'trg_updateContacts' AND type = 'TR')
   DROP TRIGGER trg_updateContacts
Go
CREATE TRIGGER trg_updateContacts
ON Contacts
After Update As
Update Contacts set MODIFYDATE=GetDate() where CONTACT_ID in (select CONTACT_ID from inserted)
Go

-- Create trigger for Meetings table
use TotalRecall
if exists (select name from sysobjects where name='trg_updateMeetings' and type = 'TR')
	drop trigger trg_updateMeetings
go
create trigger trg_updateMeetings
on Meetings
After update as
Update Meetings set MODIFYDATE=GetDate() where MTG_ID in (select MTG_ID from inserted)
go

-- Create trigger for Participants table
use TotalRecall
if exists (select name from sysobjects where name='trg_updateParticipants' and type='TR')
	drop trigger trg_updateParticipants
go
create trigger trg_updateParticipants
on Participants
After update as
Update Participants set MODIFYDATE=GetDate() where CONTACT_ID in (select CONTACT_ID from inserted)
go

-- Create trigger for Resources table
use TotalRecall
if exists (select name from sysobjects where name='trg_updateResources' and type='TR')
	drop trigger trg_updateResources
go
create trigger trg_updateResources
on Resources
After update as 
Update Resources set MODIFYDATE=GetDate() where RES_ID in (select RES_ID from inserted)
go

-- Create trigger for MeetingResources table
use TotalRecall
if exists (select name from sysobjects where name='trg_updateMeetingResources' and type='TR')
	drop trigger trg_updateMeetingResources
go
create trigger trg_updateMeetingResources
on MeetingResources
After update as
Update MeetingResources set MODIFYDATE=GetDate() where RES_ID in (select RES_ID from inserted)
go

-- Create trigger for AccessPolicies table
use TotalRecall
if exists (select name from sysobjects where name='trg_updateAccessPolicies' and type='TR')
	drop trigger trg_updateAccessPolicies
go
create trigger trg_updateAccessPolicies
on AccessPolicies
After update as
Update AccessPolicies set MODIFYDATE=GetDate() where ACCPOL_NAME in (select ACCPOL_NAME from inserted)
go