using System;
using System.Text;
using System.Collections;
using Microsoft.Data.Odbc;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for PolicyDAO.
	/// </summary>
	public class PolicyDAO:BaseDAO
	{
		public PolicyDAO( string strDBConnect ):base( strDBConnect )
		{
		}

		public string AddNewPolicy( string strPolicyName, string strPolicy )
		{
			// Quick error checks
			if( strPolicy == null || strPolicy.Length == 0 )
				throw new ArgumentException( "Invalid policy", "strPolicy" );
			if( strPolicyName == null || strPolicyName.Length == 0 )
				throw new ArgumentException( "Invalid policy name", "strPolicyName" );
			
			string strRetVal = "";
			string strPolicyID = Guid.NewGuid().ToString();
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.ACCESS_POLICIES_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.ACCPOL_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.ACCPOL_NAME );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.ACCPOL_DOC );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicyID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicyName ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicy ) + "'" );
				strQueryBuilder.Append( ")" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					strRetVal = strPolicyID;
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}
			
			return strRetVal;
		}

		public bool UpdatePolicy( string strPolicyID, string strPolicy )
		{
			// Quick error checks
			if( strPolicy == null || strPolicy.Length == 0 )
				throw new ArgumentException( "Invalid policy", "strPolicy" );
			if( strPolicyID == null || strPolicyID.Length == 0 )
				throw new ArgumentException( "Invalid policy ID", "strPolicyID" );
			
			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.ACCESS_POLICIES_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.ACCPOL_DOC );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicy ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.ACCPOL_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicyID ) + "'" );

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

		public string GetPolicy( string strPolicyID )
		{
			// Quick error checks
			if( strPolicyID == null || strPolicyID.Length == 0 )
				throw new ArgumentException( "Invalid policy ID", "strPolicyID" );
			
			OdbcDataReader dr = null;
			string strPolicy = "";
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( Constants.ACCPOL_DOC );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.ACCESS_POLICIES_TABLENAME);
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.ACCPOL_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicyID ) + "'" );
				
				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Take first entry returned
				if( dr.Read() )
					strPolicy = (string) dr[Constants.ACCPOL_DOC];
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}
			
			return strPolicy;
		}
		
		public bool AddPolicyCategory( string strPolicyID, string strCategoryName )
		{
			// Quick error checks
			if( strCategoryName == null || strCategoryName.Length == 0 )
				throw new ArgumentException( "Invalid category name", "strCategory" );
			if( strPolicyID == null || strPolicyID.Length == 0 )
				throw new ArgumentException( "Invalid policy ID", "strPolicyID" );

			bool bRetVal = false;
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.CATEGORIES_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.CAT_NAME );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.ACCPOL_ID );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strCategoryName ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strPolicyID ) + "'" );
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
	}
}
