using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for BaseDAO.
	/// </summary>
	public abstract class BaseDAO
	{
		protected string m_strDBConnect = "";
		
		public BaseDAO( string strDBConnect )
		{
			if( strDBConnect == null || strDBConnect.Length == 0 )			
				throw new ArgumentException( "Invalid database connection string", strDBConnect );

			this.m_strDBConnect = strDBConnect;
		}
		
		public virtual string DBConnect
		{
			get
			{ return this.m_strDBConnect; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strDBConnect = value;
			}
		}
	}
}
