using System;
using System.Collections;
using System.Text;
using Microsoft.Data.Odbc;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for MeetingDAO.
	/// </summary>
	public class MeetingDAO:BaseDAO
	{
		public MeetingDAO( string strDBConnect ):base( strDBConnect )
		{
		}
				
		public bool IsNewMeeting( string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			bool bRetVal = false;
			OdbcDataReader dr = null;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				
				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount == 0 )
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

		public bool CreateNewMeeting( string strMeetingID, string strTopic )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( strTopic == null || strTopic.Length == 0 )
				throw new ArgumentException( "Invalid topic", "strTopic" );
			
			// Prevent attempt to create the same meeting multiple times
			if( !IsNewMeeting( strMeetingID ) )
				return true;

			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.MTG_TOPIC );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.MTG_STATE );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strTopic ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + enuMeetingState.Active.ToString() + "'" );
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

		public bool UpdateMeetingTopic( string strMeetingID, string strTopic )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( strTopic == null || strTopic.Length == 0 )
				throw new ArgumentException( "Invalid topic", "strTopic" );
			
			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.MTG_TOPIC );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strTopic ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				
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

		public bool UpdateMeetingState( string strMeetingID, enuMeetingState state )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.MTG_STATE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + state.ToString() + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				
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
		
		public enuMeetingState GetMeetingState( string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			OdbcDataReader dr = null;
			enuMeetingState state = new enuMeetingState();
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.MTG_STATE );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				if( dr.Read() )
					state = (enuMeetingState) enuMeetingState.Parse( typeof(enuMeetingState), (string) dr[Constants.MTG_STATE], true );
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return state;
		}

		public string GetMeetingTopic( string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			OdbcDataReader dr = null;
			string strRetVal = "";
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.MTG_TOPIC );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				if( dr.Read() )
					strRetVal = (string)dr[Constants.MTG_TOPIC];
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return strRetVal;
		}

		public ArrayList GetMeetingIDs( enuMeetingState state )
		{
			ArrayList lstMeetings = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETINGS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_STATE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( state.ToString() ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					// Add participant to list
					lstMeetings.Add( (string) dr[Constants.MTG_ID] );
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

			return lstMeetings;
		}

		// Methods that build on basic data access
		public bool ResumeMeeting( string strMeetingID )
		{
			return UpdateMeetingState( strMeetingID, enuMeetingState.Active );
		}

		public bool SuspendMeeting( string strMeetingID )
		{
			return UpdateMeetingState( strMeetingID, enuMeetingState.Suspended );
		}

		public bool EndMeeting( string strMeetingID )
		{
			return UpdateMeetingState( strMeetingID, enuMeetingState.Ended );
		}
	}
}
