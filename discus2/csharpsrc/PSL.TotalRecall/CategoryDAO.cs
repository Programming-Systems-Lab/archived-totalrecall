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
	public class CategoryDAO:BaseDAO
	{
		public CategoryDAO( string strDBConnect ):base( strDBConnect )
		{
		}

		public bool AddCategory(Category category) 
		{
			//Quick error checks
			if( category == null )
				throw new ArgumentNullException( "category cannot be null" );
			
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
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( category.Name ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( category.PolicyID ) + "'" );
				strQueryBuilder.Append( ")" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				return ( nRowsAffected == 1 );
				
			}
			catch( Exception e )
			{
				Console.WriteLine("Exception in AddCategory(): " + e);
				return false;
			}
			
			
		}

		public bool UpdateCategory(string categoryName, string policyId) 
		{
			return UpdateCategory(categoryName, categoryName, policyId);
		}

		/// <summary>
		/// Updates the name and policy id for a given category.
		///
		/// </summary>
		/// <param name="categoryName">The current name of the category</param>
		/// <param name="newName">The new name for the category</param>
		/// <param name="policyId">The new policyId for the category</param>
		/// <returns></returns>
		public bool UpdateCategory(string categoryName, string newName, string policyId) 
		{
			//Quick error checks
			if( categoryName == null || policyId == null)
				throw new ArgumentNullException( "category and policyId cannot be null" );
			
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " UPDATE " );
				strQueryBuilder.Append( Constants.CATEGORIES_TABLENAME );
				strQueryBuilder.Append( " SET " );
				strQueryBuilder.Append( Constants.CAT_NAME );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( newName ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.ACCPOL_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( policyId ) + "'" );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CAT_NAME );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( categoryName ) + "'" );
				
				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				return ( nRowsAffected == 1 );
				
			}
			catch( Exception e )
			{
				Console.WriteLine("Exception in AddCategory(): " + e);
				return false;
			}
			
			
		}


		public ArrayList GetAllCategories()
		{
			ArrayList categories = new ArrayList();
			OdbcDataReader dr = null;
						
			try
			{
				string query = "SELECT * FROM " + Constants.CATEGORIES_TABLENAME;
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
				Console.WriteLine("Exception in GetAllCategories(): " + e);
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return categories;
		}

		
	}
}