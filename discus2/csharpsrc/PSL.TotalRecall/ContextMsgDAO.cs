using System;
using System.Collections;
using System.Text;
using Microsoft.Data.Odbc;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public class ContextMsgDAO : BaseDAO
	{
		public ContextMsgDAO( string strDBConnect ):base( strDBConnect )
		{
		}
		
		// And messages we send out or the responses sent out
		// are recorded in the context messages sent table
		public bool SendContextMessage( ContextMsg ctxMsg )
		{
			// Quick error checks
			if( ctxMsg == null )
				throw new ArgumentNullException( "ctxMsg", "Invalid context message" );

			// Put the destination data in the Sender, SenderUrl fields of
			// the context message
			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.CONTEXT_MSGS_SENT_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG_TYPE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CONTACT_LOC );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MeetingID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MessageID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.Dest ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.Type.ToString() ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.DestUrl ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.ToXml() ) + "'" );
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
		
		// A context message goes into the responses table if there is
		// a sent message with a matching message id. Otherwise it goes into the
		// context messages received table
		public bool ReceiveContextMessage( ContextMsg ctxMsg, bool bMarkAsNewMsg )
		{
			// Quick error checks
			if( ctxMsg == null )
				throw new ArgumentNullException( "ctxMsg", "Invalid context message" );

			bool bRetVal = false;
			bool bIsResponse = false;

			string strTablename = Constants.CONTEXT_MSGS_RECEIVED_TABLENAME;
			if( IsContextMessageResponse( ctxMsg ) )
			{
				strTablename = Constants.CONTEXT_MSG_RESPONSES_TABLENAME;
				bIsResponse = true;
			}

			// If a context message is not a response then it is a regular
			// received message
			if( !bIsResponse )
			{
				// Check whether we have received this context message twice
				if( this.IsContextMessageReceivedTwice( ctxMsg ) )
					return true;
			}

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( strTablename );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG_TYPE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CONTACT_LOC );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG );
				if( !bIsResponse )
				{
					strQueryBuilder.Append( "," );
					strQueryBuilder.Append( Constants.NEWMSG );
				}
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MeetingID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MessageID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.Sender ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.Type.ToString() ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.SenderUrl ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.ToXml() ) + "'" );
				if( !bIsResponse )
				{
					strQueryBuilder.Append( "," );
					if( bMarkAsNewMsg == true )
						strQueryBuilder.Append( 1 );
					else strQueryBuilder.Append( 0 );
				}
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

		public ArrayList GetNewReceivedContextMessages()
		{
			ArrayList lstMessages = new ArrayList();
			OdbcDataReader dr = null;

			try
			{
				// Only Recommendation request messages should be the "new" messages in the 
				// context messages received, all other messages can be handled when received and
				// thus should be put in the context messages received table marked as read (not new msg)
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.CTXMSG_TYPE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CTXMSG );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.CONTEXT_MSGS_RECEIVED_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.NEWMSG );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( 1 );
				strQueryBuilder.Append( " ORDER BY " );
				strQueryBuilder.Append( Constants.CREATEDATE );
				strQueryBuilder.Append( " ASC " );
				
				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					// Get the context message type
					enuContextMsgType type = (enuContextMsgType) enuContextMsgType.Parse( typeof(enuContextMsgType), (string) dr[Constants.CTXMSG_TYPE], true ); 
					string strCtxMsg = (string) dr[Constants.CTXMSG];
					ContextMsg ctxMsg = null;
					switch( type )
					{
						case enuContextMsgType.InfoAgentJoined: 
						case enuContextMsgType.InfoAgentLeft: ctxMsg = InfoAgentCtxMsg.FromXml( strCtxMsg );
															  break;
						
						case enuContextMsgType.MeetingEnded:
						case enuContextMsgType.MeetingResumed:
						case enuContextMsgType.MeetingSuspended: ctxMsg = MeetingStatusCtxMsg.FromXml( strCtxMsg );
																 break;
						
						case enuContextMsgType.RecommendationRequest: ctxMsg = RecommendationCtxMsg.FromXml( strCtxMsg );
																	  break;		
					}

					if( ctxMsg != null )
						lstMessages.Add( ctxMsg );
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
			
			return lstMessages;
		}

		public bool IsContextMessageReceivedTwice( ContextMsg ctxMsg )
		{
			// Quick error checks
			if( ctxMsg == null )
				throw new ArgumentNullException( "ctxMsg", "Invalid context message" );

			bool bRetVal = false;
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.CONTEXT_MSGS_RECEIVED_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CTXMSG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MessageID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );
				
				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount >= 1 )
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


		// A context msg is treated as a response if there is a match for
		// the message id in the context messages sent table
		public bool IsContextMessageResponse( ContextMsg ctxMsg )
		{
			// Quick error checks
			if( ctxMsg == null )
				throw new ArgumentNullException( "ctxMsg", "Invalid context message" );

			bool bRetVal = false;
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.CONTEXT_MSGS_SENT_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CTXMSG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MessageID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );
				
				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount >= 1 )
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

		public bool MarkReceivedContextMessages( ArrayList lstContextMessages )
		{
			if( lstContextMessages == null || lstContextMessages.Count == 0 )
				return true;

			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.CONTEXT_MSGS_RECEIVED_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.NEWMSG );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( 1 );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CTXMSG_ID );
				strQueryBuilder.Append( " IN " );
				strQueryBuilder.Append( "(" );
				
				IEnumerator it = lstContextMessages.GetEnumerator();
				while( it.MoveNext() )
				{
					ContextMsg ctxMsg = (ContextMsg) it.Current;
					strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( ctxMsg.MessageID ) + "'" );
					strQueryBuilder.Append( "," );
				}
				
				// Remove the last ",", there will be one since the collection must
				// have at least one message in it to reach this point in the method
				strQueryBuilder.Remove( strQueryBuilder.Length - 1, 1 );
				strQueryBuilder.Append( ")" );
								
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
