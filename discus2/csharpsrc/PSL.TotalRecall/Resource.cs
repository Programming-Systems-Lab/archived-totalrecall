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

		public override string ToString()
		{
			return m_strName + " [url=" + m_strUrl + ", id=" + m_strID + "]";

		}

		public override bool Equals(object o) 
		{
			Resource other = (Resource) o;
			return other.ID.Equals(ID) && 
				other.Name.Equals(Name) &&
				other.Url.Equals(Url);
		}

		public override int GetHashCode() 
		{
			return ID.GetHashCode() + 29 * Name.GetHashCode() + 29 * Url.GetHashCode();
		}
	}
}
