using System;
using System.Text;
using System.Collections;
using Microsoft.Data.Odbc;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for ResourceDAO.
	/// </summary>
	public class ResourceDAO:BaseDAO
	{
		public ResourceDAO( string strDBConnect ):base( strDBConnect )
		{
		}

		public ArrayList GetAllResources()
		{
			// Quick error checks
			ArrayList lstResources = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				string query = "SELECT * FROM " + Constants.RESOURCES_TABLENAME;
				dr =  QueryService.ExecuteReader( this.DBConnect, query );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					Resource res =  new Resource();
					res.ID = (string) dr[Constants.RES_ID];
					res.Name = (string) dr[Constants.RES_NAME];
					res.Url = (string) dr[Constants.RES_URL];
					lstResources.Add( res );
				}
			}
			catch( Exception e )
			{
				Console.WriteLine("Error in GetAllResources(): " + e);
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return lstResources;
		}


		public ArrayList GetMeetingResources( string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			
			ArrayList lstResources = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME + "." + Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME + "." + Constants.RES_OWNER );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME + "." + Constants.RES_STATE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME + "." + Constants.RES_URL );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME + "." + Constants.RES_NAME );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME + "." + Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME + "." + Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME + "." + Constants.RES_ID );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					MeetingResource mtgRes =  new MeetingResource();
					mtgRes.ID = (string) dr[Constants.RES_ID];
					mtgRes.Name = (string) dr[Constants.RES_NAME];
					mtgRes.Owner = (string) dr[Constants.RES_OWNER];
					mtgRes.State = (enuResourceState) enuResourceState.Parse( typeof(enuResourceState), (string) dr[Constants.RES_STATE], true );
					mtgRes.Url = (string) dr[Constants.RES_URL];
					lstResources.Add( mtgRes );
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

			return lstResources;
		}

		public ArrayList GetResourcePolicies( string strResourceID )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );
			
			ArrayList lstPolicies = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				// Create two string builders, one for inner select and one for
				// outer select
				
				// Inner query, returns category names for resource
				StringBuilder strInnerQuery = new StringBuilder();
				strInnerQuery.Append( " SELECT " );
				strInnerQuery.Append( Constants.RESOURCE_CATEGORIES_TABLENAME + "." + Constants.CAT_NAME );
				strInnerQuery.Append( " FROM " );
				strInnerQuery.Append( Constants.RESOURCE_CATEGORIES_TABLENAME );
				strInnerQuery.Append( " WHERE " );
				strInnerQuery.Append( Constants.RESOURCE_CATEGORIES_TABLENAME + "." + Constants.RES_ID );
				strInnerQuery.Append( "=" );
				strInnerQuery.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );
				
				// Outer query
				StringBuilder strOuterQuery = new StringBuilder();
				strOuterQuery.Append( " SELECT " );
				strOuterQuery.Append( Constants.ACCESS_POLICIES_TABLENAME + "." + Constants.ACCPOL_DOC );
				strOuterQuery.Append( "," );
				strOuterQuery.Append( Constants.CATEGORIES_TABLENAME + "." + Constants.CAT_NAME );
				strOuterQuery.Append( " FROM " );
				strOuterQuery.Append( Constants.ACCESS_POLICIES_TABLENAME );
				strOuterQuery.Append( "," );
				strOuterQuery.Append( Constants.CATEGORIES_TABLENAME );
				strOuterQuery.Append( " WHERE " );
				strOuterQuery.Append( Constants.ACCESS_POLICIES_TABLENAME + "." + Constants.ACCPOL_ID );
				strOuterQuery.Append( "=" );
				strOuterQuery.Append( Constants.CATEGORIES_TABLENAME + "." + Constants.ACCPOL_ID );
				strOuterQuery.Append( " AND " );
				strOuterQuery.Append( Constants.CATEGORIES_TABLENAME + "." + Constants.CAT_NAME );
				strOuterQuery.Append( " IN " );
				strOuterQuery.Append( "(" );
				strOuterQuery.Append( strInnerQuery.ToString() );
				strOuterQuery.Append( ")" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strOuterQuery.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					string strPolicy = (string) dr[Constants.ACCPOL_DOC];
					lstPolicies.Add( strPolicy );
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

			return lstPolicies;
		}

		// Returns the GUID res ID
		public string AddNewResource( Resource res )
		{
			//Quick error checks
			if( res == null )
				throw new ArgumentNullException( "res", "Invalid resource to add" );
			
			// Generate a new resource ID if none specified in the resource
			if( res.ID.Length == 0 )
				res.ID = Guid.NewGuid().ToString();
			
			if( this.IsResourceInCatalog( res.ID ) )
				return res.ID;
			
			string strRetVal = "";

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_URL );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_NAME );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( res.ID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( res.Url ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( res.Name ) + "'" );
				strQueryBuilder.Append( ")" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					strRetVal = res.ID;
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return strRetVal;
		}

		public Resource GetResourceByID( string strResourceID )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );

			Resource retVal = null;
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_NAME );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_URL );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
				{
					retVal = new Resource();
					retVal.ID = (string) dr[Constants.RES_ID];
					retVal.Name = (string) dr[Constants.RES_NAME];
					retVal.Url = (string) dr[Constants.RES_URL];
				}
				
			}
			catch( Exception /*e*/ )
			{
				retVal = null;
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Returns an ArrayList of Categories associated with this resource
		/// </summary>
		/// <param name="resourceId"></param>
		/// <returns></returns>
		public ArrayList GetResourceCategories(string resourceId)
		{
			ArrayList categories = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				string query = " SELECT " + Constants.CATEGORIES_TABLENAME + ".*" +
					" FROM " + Constants.CATEGORIES_TABLENAME + ", " + Constants.RESOURCE_CATEGORIES_TABLENAME +
					" WHERE " + Constants.RES_ID + "=" + "'" + QueryService.MakeQuotesafe( resourceId ) + "'" +
					" AND " + Constants.RESOURCE_CATEGORIES_TABLENAME + "." + Constants.CAT_NAME + "=" +
							  Constants.CATEGORIES_TABLENAME + "." + Constants.CAT_NAME;

				dr =  QueryService.ExecuteReader( this.DBConnect, query );
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				while( dr.Read() )
				{
					Category cat =  new Category();
					cat.Name = (string) dr[Constants.CAT_NAME];
					cat.PolicyID = (string) dr[Constants.ACCPOL_ID];
					categories.Add( cat );
				}
			}
			catch( Exception e )
			{
				Console.WriteLine("Exception in GetResourceCategories(): " + e);
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return categories;
		}

		
		public Resource GetResourceByName( string strResourceName )
		{
			// Quick error checks
			if( strResourceName == null || strResourceName.Length == 0 )
				throw new ArgumentException( "Invalid resource name", "strResourceName" );

			Resource retVal = null;
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_NAME );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_URL );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_NAME );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceName ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
				{
					retVal = new Resource();
					retVal.ID = (string) dr[Constants.RES_ID];
					retVal.Name = (string) dr[Constants.RES_NAME];
					retVal.Url = (string) dr[Constants.RES_URL];
				}
			}
			catch( Exception /*e*/ )
			{
				retVal = null;
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return retVal;
		}

		public Resource GetResourceByUrl( string strResourceUrl )
		{
			// Quick error checks
			if( strResourceUrl == null || strResourceUrl.Length == 0 )
				throw new ArgumentException( "Invalid resource Url", "strResourceUrl" );

			Resource retVal = null;
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_NAME );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_URL );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_URL );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceUrl ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
				{
					retVal = new Resource();
					retVal.ID = (string) dr[Constants.RES_ID];
					retVal.Name = (string) dr[Constants.RES_NAME];
					retVal.Url = (string) dr[Constants.RES_URL];
				}
			}
			catch( Exception /*e*/ )
			{
				retVal = null;
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return retVal;
		}

		public bool AddMeetingResource( string strMeetingID, MeetingResource mtgRes )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( mtgRes == null )
				throw new ArgumentNullException( "mtgRes", "Invalid resource to add" );
			
			if( this.IsResourceInMeeting( mtgRes.ID, strMeetingID ) )
				return true;

			bool bRetVal = false;
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_STATE );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.RES_OWNER );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( mtgRes.ID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( mtgRes.State.ToString() ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( mtgRes.Owner ) + "'" );
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

		public bool IsResourceInCatalog( string strResourceID )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );

			bool bRetVal = false;
			
			OdbcDataReader dr = null;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

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

		public bool IsResourceInMeeting( string strResourceID, string strMeetingID )
		{
			// Quick error checks
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strMeetingID" );
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );

			bool bRetVal = false;
			
			OdbcDataReader dr = null;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.MTG_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strMeetingID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

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

		public bool UpdateResource( string strResourceID, string strName, string url )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );
			if( strName == null || strName.Length == 0 )
				throw new ArgumentException( "Invalid resource name", "strName" );

			bool bRetVal =  false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.RESOURCES_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.RES_NAME );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strName ) + "'" );
				strQueryBuilder.Append( " , " );
				strQueryBuilder.Append( Constants.RES_URL );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( url ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

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

		

		public bool ShareResource( string strResourceID )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );
		
			bool bRetVal =  false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.RES_STATE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( enuResourceState.Shared.ToString() ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

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

		public bool RecallResource( string strResourceID )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );
		
			bool bRetVal =  false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.RES_STATE );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( enuResourceState.Recalled.ToString() ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

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

		public enuResourceState GetMeetingResourceState( string strResourceID )
		{
			// Quick error checks
			if( strResourceID == null || strResourceID.Length == 0 )
				throw new ArgumentException( "Invalid resource ID", "strResourceID" );
		
			OdbcDataReader dr = null;
			enuResourceState state = new enuResourceState();
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.RES_STATE );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.MEETING_RESOURCES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strResourceID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Scroll thru list returned
				if( dr.Read() )
					state = (enuResourceState) enuResourceState.Parse( typeof(enuResourceState), (string) dr[Constants.RES_STATE], true );
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

		public bool AddResourceToCategory( Resource res, string categoryName )
		{
			// Quick error checks
			if( res == null )
				throw new ArgumentNullException( "res", "Invalid resource to add" );
			if( categoryName == null || categoryName.Length == 0 )
				throw new ArgumentException( "Invalid category name", "strName" );
			
			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.RESOURCE_CATEGORIES_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CAT_NAME );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( res.ID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( categoryName ) + "'" );
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

		public bool RemoveResourceFromCategory( Resource res, string categoryName )
		{
			// Quick error checks
			if( res == null )
				throw new ArgumentNullException( "res", "Invalid resource to add" );
			if( categoryName == null || categoryName.Length == 0 )
				throw new ArgumentException( "Invalid category name", "strName" );
			
			bool bRetVal = false;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " DELETE FROM " );
				strQueryBuilder.Append( Constants.RESOURCE_CATEGORIES_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.RES_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( res.ID ) + "'" );
				strQueryBuilder.Append( " AND " );
				strQueryBuilder.Append( Constants.CAT_NAME );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( categoryName ) + "'" );
				
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
