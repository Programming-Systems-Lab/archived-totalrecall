-- TotalRecall dbase populate script
delete from Roles
delete from MeetingStates
delete from ResourceStates
delete from ContextMessageTypes

-- Create the possible roles for participants
-- Inactive role added for archive/history purposes
-- when a participant leaves a meeting they become Inactive
insert into Roles(ROLE_NAME) values ('Organizer')
insert into Roles(ROLE_NAME) values ('Participant')
insert into Roles(ROLE_NAME) values ('Inactive')

-- Create the possible meeting states
insert into MeetingStates(MTG_STATE) values ('Active')
insert into MeetingStates(MTG_STATE) values ('Ended')
insert into MeetingStates(MTG_STATE) values ('Suspended')

-- Create the possible resource states
insert into ResourceStates(RES_STATE) values ('Shared')
insert into ResourceStates(RES_STATE) values ('Recalled')

-- Create the possible context message types
insert into ContextMessageTypes(CTXMSG_TYPE) values ('InfoAgentJoined')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('InfoAgentLeft')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('InfoAgentJoinedResponse')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('InfoAgentLeftResponse')

insert into ContextMessageTypes(CTXMSG_TYPE) values ('ResourceAdd')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('ResourceRecall')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('ResourceAddResponse')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('ResourceRecallResponse')

insert into ContextMessageTypes(CTXMSG_TYPE) values ('RecommendationRequest')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('RecommendationResponse')

insert into ContextMessageTypes(CTXMSG_TYPE) values ('MeetingSuspended')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('MeetingResumed')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('MeetingSuspendedResponse')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('MeetingResumedResponse')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('MeetingEnded')
insert into ContextMessageTypes(CTXMSG_TYPE) values ('MeetingEndedResponse')

insert into ContextMessageTypes(CTXMSG_TYPE) values ('Unknown')
