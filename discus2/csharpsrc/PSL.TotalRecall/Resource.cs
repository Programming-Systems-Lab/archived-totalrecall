using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for Resource.
	/// </summary>
	public class Resource
	{
		protected string m_strID = "";
		protected string m_strName = "";
		protected string m_strUrl = "";
		
		public virtual string ID
		{
			get
			{ return this.m_strID; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strID = value;
			}
		}

		public virtual string Name
		{
			get
			{ return this.m_strName; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strName = value;
			}
		}

		public virtual string Url
		{
			get
			{ return this.m_strUrl; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strUrl = value;
			}
		}
	}
}
