using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Diagnostics;
using Microsoft.Data.Odbc;


namespace PSL.TotalRecall.DataAccess
{
	/// <summary>
	/// Summary description for QueryService.
	/// </summary>
	public class QueryService
	{
		public const string CLASSNAME = "QueryService";

		public QueryService()
		{
		}

		public static string MakeQuotesafe( string strInput )
		{
			if( strInput == null || strInput.Length == 0 )
				throw new ArgumentException( "Invalid input string", "strInput" );
            
			return strInput.Replace( "'", "''" );
		}
		
		public static int ExecuteNonQuery( string strConnect, string strCmd )
		{
			if( strConnect == null || strConnect.Length == 0 )
				throw new ArgumentException( "Invalid connection string", "strConnect" );

			if( strCmd == null || strCmd.Length == 0 )
				throw new ArgumentException( "Invalid command", "strCmd" );
			            
			int nRowsAffected = 0;

			try
			{
				// Set the database connection string for the connection object
				OdbcConnection Conn = new OdbcConnection( strConnect );
				// Create a command, setting its command text
				OdbcCommand Command = new OdbcCommand( strCmd );
				// Set the command's database connection
				Command.Connection = Conn;
				// Open a connection to the database
				Conn.Open();
				// Execute the command and close the connection afterwards
				nRowsAffected = Command.ExecuteNonQuery();
				// Close connection
				Conn.Close();
			}
			catch( System.Exception e )
			{
				// Create string builder instance
				StringBuilder strStringBuilder = new StringBuilder( e.Message );
				// Report error
				strStringBuilder.Append( " LAST QUERY: " + strCmd );
				// Write error entry
				EventLog.WriteEntry( CLASSNAME, strStringBuilder.ToString(), EventLogEntryType.Error );
			}
			return nRowsAffected;
		}

		public static OdbcDataReader ExecuteReader( string strConnect, string strCmd )
		{
			if( strConnect == null || strConnect.Length == 0 )
				throw new ArgumentException( "Invalid connection string", "strConnect" );

			if( strCmd == null || strCmd.Length == 0 )
				throw new ArgumentException( "Invalid command", "strCmd" );
            			
			OdbcDataReader dr = null;
			try
			{
				// Set the database connection string for the connection object
				OdbcConnection Conn = new OdbcConnection( strConnect );
				// Create a command, setting its command text
				OdbcCommand Command = new OdbcCommand( strCmd );
				// Set the command's database connection
				Command.Connection = Conn;
				// Open a connection to the database
				Conn.Open();
				// Execute the command and close the connection afterwards
				dr = Command.ExecuteReader( CommandBehavior.CloseConnection );
			}
			catch( System.Exception e )
			{
				// Create string builder instance
				StringBuilder strStringBuilder = new StringBuilder( e.Message );
				// Report error
				strStringBuilder.Append( " LAST QUERY: " + strCmd );
				// Write error entry
				EventLog.WriteEntry( CLASSNAME, strStringBuilder.ToString(), EventLogEntryType.Error );
			}
			// Return the data reader
			return dr;
		}
	}
}
