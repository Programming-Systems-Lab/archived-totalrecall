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

		public AccessPolicy AddNewPolicy( string strPolicyName, string strPolicy )
		{
			// Quick error checks
			if( strPolicy == null || strPolicy.Length == 0 )
				throw new ArgumentException( "Invalid policy", "strPolicy" );
			if( strPolicyName == null || strPolicyName.Length == 0 )
				throw new ArgumentException( "Invalid policy name", "strPolicyName" );
			
			AccessPolicy policy = null;
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
				{
					policy = new AccessPolicy();
					policy.Name = strPolicyName;
					policy.Id = strPolicyID;
					policy.Document = strPolicy;
				}
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}
			
			return policy;
		}

		public bool UpdatePolicy( string strPolicyID, string strPolicy, string name )
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
				strQueryBuilder.Append( " , " );
				strQueryBuilder.Append( Constants.ACCPOL_NAME);
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( name ) + "'" );
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

		public AccessPolicy GetPolicy( string strPolicyID )
		{
			// Quick error checks
			if( strPolicyID == null || strPolicyID.Length == 0 )
				throw new ArgumentException( "Invalid policy ID", "strPolicyID" );
			
			OdbcDataReader dr = null;
			AccessPolicy policy = null;
						
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( "*" );
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
				{
					policy = new AccessPolicy();
					policy.Name = (string) dr[Constants.ACCPOL_NAME];
					policy.Id = (string) dr[Constants.ACCPOL_ID];
					policy.Document = (string) dr[Constants.ACCPOL_DOC];
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
			
			return policy;
		}

		public ArrayList GetAllPolicies()
		{
			OdbcDataReader dr = null;
			AccessPolicy policy = null;
				
			ArrayList policies = new ArrayList();

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT " );
				strQueryBuilder.Append( "*" );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.ACCESS_POLICIES_TABLENAME);
				
				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				while ( dr.Read() ) 
				{
					policy = new AccessPolicy();
					policy.Name = (string) dr[Constants.ACCPOL_NAME];
					policy.Id = (string) dr[Constants.ACCPOL_ID];
					policy.Document = (string) dr[Constants.ACCPOL_DOC];

					policies.Add(policy);
				}
					
			}
			catch( Exception e )
			{
				Console.WriteLine("Exception in GetAllPolicies(): " + e);
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}
			
			return policies;
		}
		
		
	}
}
