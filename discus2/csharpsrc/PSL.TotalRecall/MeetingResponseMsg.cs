using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for MeetingResponseMsg.
	/// </summary>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall", IsNullable=false)]
	public class MeetingResponseMsg:Message
	{
		public MeetingResponseMsg()
		{
		}
		
		public new static MeetingResponseMsg FromXml( string strXml )
		{
			System.Type type =  typeof(MeetingResponseMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (MeetingResponseMsg) ser.Deserialize( xt );
		}

		[System.Xml.Serialization.XmlElement( "Value" )]
		public enuMeetingResponseValue Value;
	}
}
