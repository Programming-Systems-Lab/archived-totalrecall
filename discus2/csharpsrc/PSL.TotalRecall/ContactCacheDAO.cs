using System;
using System.Collections;
using System.Text;
using Microsoft.Data.Odbc;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for ContactCacheDAO.
	/// </summary>
	public class ContactCacheDAO:BaseDAO
	{
		public ContactCacheDAO( string strDBConnect ):base( strDBConnect )
		{
		}

		public bool AddContactLocation( string strContactID, string strLocation )
		{
			// Quick error checks
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			if( strLocation == null || strLocation.Length == 0 )
				throw new ArgumentException( "Invalid contact location", "strLocation" );

			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.CONTACT_CACHE_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strLocation ) + "'" );
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

		public string GetContactLocation( string strContactID )
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
				strQueryBuilder.Append( Constants.CONTACT_CACHE_TABLENAME );
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

		public bool ClearContactLocation( string strContactID )
		{
			// Quick error checks
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			
			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " DELETE FROM " );
				strQueryBuilder.Append( Constants.CONTACT_CACHE_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				
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

		public bool UpdateContactLocation( string strContactID, string strLocation )
		{
			// Quick error checks
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			if( strLocation == null || strLocation.Length == 0 )
				throw new ArgumentException( "Invalid contact location", "strLocation" );

			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.CONTACT_CACHE_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.PART_LOC );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strLocation ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				
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
	}
}
