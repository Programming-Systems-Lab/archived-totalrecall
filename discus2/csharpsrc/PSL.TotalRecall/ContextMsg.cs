using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for ContextMsg.
	/// </summary>
	public class ContextMsg:Message
	{
		private XmlElement m_DataElem = null;

		public ContextMsg()
		{
		}

		[System.Xml.Serialization.XmlElement("Type")]
		public enuContextMsgType Type = enuContextMsgType.Unknown;

		[System.Xml.Serialization.XmlAnyElement()]
		public XmlElement DataElem
		{
			get
			{ return this.m_DataElem; }
			set
			{} // Do nothing setter
		}

		[System.Xml.Serialization.XmlIgnore()]
		public string DataXml
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

	public enum enuContextMsgType
	{
		IAJoined,
		IALeft,
		IALeaving,
		Unknown
	}
}
