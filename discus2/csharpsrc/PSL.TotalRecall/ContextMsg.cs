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
		protected XmlElement m_DataElem = null;
		protected string m_strMessageID = Guid.NewGuid().ToString();
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

		public virtual string MessageID
		{
			get
			{ return this.m_strMessageID; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strMessageID = value;
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
		
		[System.Xml.Serialization.XmlAnyElement()]
		public virtual XmlElement DataElem
		{
			get
			{ return this.m_DataElem; }
			set
			{} // Do nothing setter
		}

		[System.Xml.Serialization.XmlIgnore()]
		public virtual string DataXml
		{
			get
			{ 
				if( this.m_DataElem == null )
					return "";

				return this.m_DataElem.OuterXml;
			}
			set
			{
				if( value == null || value.Length == 0 )
					return;

				XmlDocument innerDoc = new XmlDocument();
				innerDoc.LoadXml( value );
				this.m_DataElem = innerDoc.DocumentElement;
			}
		}
	}
}
