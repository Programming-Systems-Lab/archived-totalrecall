using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Database constants
	/// </summary>
	public abstract class Constants
	{
		public const string DBConnect = "DSN=TotalRecall;UID=TotalRecallUser;PWD=[32totalRecall67;WSID=HERITAGE0;DATABASE=TotalRecall";
		public const string ME = "Me";
		public const string DEFAULT_CERT_PSWD = "discus";

		// Database table names
		public const string ACCESS_POLICIES_TABLENAME		= "AccessPolicies";
		public const string CATEGORIES_TABLENAME			= "Categories";
		public const string CONTACTS_TABLENAME				= "Contacts";
		public const string CONTEXT_MSG_RESPONSES_TABLENAME = "ContextMessageResponses";
		public const string CONTEXT_MSGS_SENT_TABLENAME		= "ContextMessagesSent";
		public const string CONTEXT_MSG_TYPES_TABLENAME		= "ContextMessageTypes";
		public const string MEETING_RESOURCES_TABLENAME		= "MeetingResources";
		public const string MEETINGS_TABLENAME				= "Meetings";
		public const string MEETING_STATES_TABLENAME		= "MeetingStates";
		public const string PARTICIPANTS_TABLENAME			= "Participants";
		public const string RESOURCES_TABLENAME				= "Resources";
		public const string RESOURCE_STATES_TABLENAME		= "ResourceStates";
		public const string ROLES_TABLENAME					= "Roles";
		public const string RESOURCE_CATEGORIES_TABLENAME	= "ResourceCategories";

		// Column names 
		// Meetings table
		public const string MTG_ID		= "MTG_ID";
		public const string MTG_STATE	= "MTG_STATE";
		public const string MTG_TOPIC	= "MTG_TOPIC";
		
		// MeetingParticipants table
		public const string CONTACT_ID	= "CONTACT_ID";
		public const string PART_ROLE	= "PART_ROLE";
		public const string PART_LOC	= "PART_LOC";

		public const string CONTACT_TRUST_SCORE	= "CONTACT_TRUST_SCORE";
		
		public const string ACCPOL_ID	= "ACCPOL_ID";
		public const string ACCPOL_NAME = "ACCPOL_NAME";
		public const string ACCPOL_DOC	= "ACCPOL_DOC";

		public const string RES_ID		= "RES_ID";
		public const string RES_NAME	= "RES_NAME";
		public const string RES_URL		= "RES_URL";
		public const string RES_STATE	= "RES_STATE";
		public const string RES_OWNER	= "RES_OWNER";

		public const string CAT_NAME	= "CAT_NAME";

		public Constants()
		{}
	}
}
