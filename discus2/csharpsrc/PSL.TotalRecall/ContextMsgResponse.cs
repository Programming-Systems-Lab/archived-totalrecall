using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public class ContextMsgResponse : ContextMsg
	{
		private bool m_bAck = true;

		public ContextMsgResponse()
		{
		}

		public bool Ack
		{
			get
			{ return this.m_bAck; }
			set
			{ this.m_bAck = value; }
		}

		public new static ContextMsgResponse FromXml( string strXml )
		{
			System.Type type =  typeof(ContextMsgResponse);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (ContextMsgResponse) ser.Deserialize( xt );
		}
	}
}
