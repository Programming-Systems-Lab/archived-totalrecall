using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for ContextMsg.
	/// </summary>
	public abstract class ContextMsg:Message
	{
		protected enuContextMsgType m_type = enuContextMsgType.Unknown;
		protected string m_strDest = "";
		protected string m_strDestUrl = "";
        
		public ContextMsg()
		{
		}

		public virtual string Dest
		{
			get
			{ return this.m_strDest; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strDest = value;
			}
		}

		public virtual string DestUrl
		{
			get
			{ return this.m_strDestUrl; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strDestUrl = value;
			}
		}

		[System.Xml.Serialization.XmlElement("Type")]
		public virtual enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				this.m_type = value; 
			}
		}
	}
}
