using System;
using System.Text;
using System.Collections;
using Microsoft.Data.Odbc;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for ParticipantDAO.
	/// </summary>
	public class ParticipantDAO:BaseDAO
	{
		public ParticipantDAO( string strDBConnect ):base( strDBConnect )
		{
		}
		
		public bool LeaveMeeting( string strMeetingID, MeetingParticipant participant )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( participant == null )
				throw new ArgumentNullException( "participant", "Invalid Meeting Participant" );

			// The Organizer CANNOT leave the meeting
			MeetingParticipant organizer = GetOrganizer( strMeetingID );
			if( organizer != null )
			{
				if( organizer.Name == participant.Name )
					throw new InvalidOperationException( "The Meeting Organizer CANNOT leave the meeting" );
			}

			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + enuMeetingParticipantRole.Inactive.ToString() + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( participant.Name ) + "'" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					bRetVal = true;
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;
		}
		
		public bool RejoinMeeting( string strMeetingID, MeetingParticipant participant )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( participant == null )
				throw new ArgumentNullException( "participant", "Invalid Meeting Participant" );

			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + enuMeetingParticipantRole.Participant.ToString() + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( participant.Name ) + "'" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					bRetVal = true;
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;
		}

		public bool IsInfoAgentInMeeting( string strMeetingID, string strIAUrl )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			// Quick error checks
			if( strIAUrl == null || strIAUrl.Length == 0 )
				throw new ArgumentException( "Invalid Info Agent Url", "strIAUrl" );

			bool bRetVal = false;
			OdbcDataReader dr = null;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strIAUrl ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount != 0 )
						bRetVal = true;
				}
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return bRetVal;			
		}

		public bool IsInMeeting( string strMeetingID, MeetingParticipant participant )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( participant == null )
				throw new ArgumentNullException( "participant", "Invalid Meeting Participant" );

			bool bRetVal = false;
			OdbcDataReader dr = null;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( participant.Name ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount != 0 )
						bRetVal = true;
				}
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return bRetVal;			
		}
		
		public bool IsInMeeting( string strMeetingID, string strContactID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentNullException( "strContactID", "Invalid Contact ID" );

			bool bRetVal = false;
			OdbcDataReader dr = null;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount != 0 )
						bRetVal = true;
				}
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return bRetVal;			
		}

		public bool AddMeetingParticipant( string strMeetingID, MeetingParticipant participant )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			// Prevent attempt to add a participant multiple times to a meeting
			if( IsInMeeting( strMeetingID, participant ) )
				return true;

			// Only one Meeting Organizer per meeting
			if( participant.Role == enuMeetingParticipantRole.Organizer && GetOrganizer( strMeetingID ) != null )
				throw new InvalidOperationException( "Only one Meeting Organizer per meeting" );
			
			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( participant.Name ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( participant.Role.ToString() ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( participant.Location ) + "'" );
				strQueryBuilder.Append( ")" );
				
				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					bRetVal = true;

			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;
		}

		public MeetingParticipant GetInfoAgent( string strMeetingID, string strInfoAgentUrl )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			MeetingParticipant particip = null;
			OdbcDataReader dr = null;
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strInfoAgentUrl ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
				{
					particip = new MeetingParticipant();
					particip.Location = (string) dr[Constants.PART_LOC];
					particip.Name = (string) dr[Constants.CONTACT_ID];
					particip.Role = (enuMeetingParticipantRole) enuMeetingParticipantRole.Parse( typeof(enuMeetingParticipantRole), (string) dr[Constants.PART_ROLE] , true );
				}
					
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return particip;
		}

		public MeetingParticipant GetOrganizer( string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			MeetingParticipant organizer = null;
			OdbcDataReader dr = null;
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + enuMeetingParticipantRole.Organizer.ToString() + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
				{
					organizer = new MeetingParticipant();
					organizer.Location = (string) dr[Constants.PART_LOC];
					organizer.Name = (string) dr[Constants.CONTACT_ID];
					organizer.Role = (enuMeetingParticipantRole) enuMeetingParticipantRole.Parse( typeof(enuMeetingParticipantRole), (string) dr[Constants.PART_ROLE] , true );
				}
					
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return organizer;
		}

		public string GetParticipantLocation( string strContactID )
		{
			// Quick error checks
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			
			OdbcDataReader dr = null;
			string strLocation = "";
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				
				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
					strLocation = (string) dr[Constants.PART_LOC];
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}
			
			return strLocation;
		}

		public ArrayList GetParticipants( string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			ArrayList lstParticipants = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					MeetingParticipant participant = new MeetingParticipant();
					participant.Location = (string) dr[Constants.PART_LOC];
					participant.Name = (string) dr[Constants.CONTACT_ID];
					participant.Role = (enuMeetingParticipantRole) enuMeetingParticipantRole.Parse( typeof(enuMeetingParticipantRole), (string) dr[Constants.PART_ROLE] , true );
					// Add participant to list
					lstParticipants.Add( participant );
				}
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return lstParticipants;
		}

		public bool UpdateParticipantRole( string strMeetingID, string strContactID, enuMeetingParticipantRole role )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
		
			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.PART_ROLE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + role.ToString() + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				
				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected >= 1 )
					bRetVal = true;
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;			
		}
		
		public bool UpdateParticpantLocation( string strMeetingID, string strContactID, string strLocation )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			if( strLocation == null || strLocation.Length == 0 )
				throw new ArgumentException( "Invalid contact location", "strLocation" );

			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.PARTICIPANTS_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strLocation ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				
				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected >= 1 )
					bRetVal = true;
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;			
		}
	}
}
