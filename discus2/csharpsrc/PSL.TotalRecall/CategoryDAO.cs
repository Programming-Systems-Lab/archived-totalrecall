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

		public void AddCategory(Category category) 
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

				QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				
			}
			catch( Exception e )
			{
				Console.WriteLine("Exception in AddCategory(): " + e);
			}
			finally
			{
			}

			
		}

		public ArrayList GetAllCategories()
		{
			// Quick error checks
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