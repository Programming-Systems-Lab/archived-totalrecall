using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class MeetingStatusCtxMsg : ContextMsg
	{
		public MeetingStatusCtxMsg()
		{
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( value == enuContextMsgType.MeetingResumed || ( value == enuContextMsgType.MeetingSuspended || value == enuContextMsgType.MeetingEnded ) )
					this.m_type = value;
			}
		}

		public new static MeetingStatusCtxMsg FromXml( string strXml )
		{
			System.Type type =  typeof(MeetingStatusCtxMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (MeetingStatusCtxMsg) ser.Deserialize( xt );
		}
	}
}
