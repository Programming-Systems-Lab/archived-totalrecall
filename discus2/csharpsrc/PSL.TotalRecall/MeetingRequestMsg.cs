using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for MeetingRequestMsg.
	/// </summary>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall", IsNullable=false)]
	public class MeetingRequestMsg:Message
	{
		[System.Xml.Serialization.XmlArrayAttribute("Participants")]
		[System.Xml.Serialization.XmlArrayItem(typeof(MeetingParticipant),ElementName="Participant")]
		public ArrayList m_lstParticipants = new ArrayList();
		
		private string m_strMeetingTopic = "";

		public MeetingRequestMsg()
		{
		}

		public new static MeetingRequestMsg FromXml( string strXml )
		{
			System.Type type =  typeof(MeetingRequestMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (MeetingRequestMsg) ser.Deserialize( xt );
		}

		[System.Xml.Serialization.XmlElement( "MeetingTopic", typeof(string) )]
		public string MeetingTopic
		{
			get
			{ return this.m_strMeetingTopic; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strMeetingTopic = value;
			}
		}
	}
	
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall", IsNullable=false)]
	public class MeetingParticipant
	{
		private string m_strName = "";
		private string m_strLoc = "";
		[System.Xml.Serialization.XmlElement( "Role" )]
		public enuMeetingParticipantRole Role = enuMeetingParticipantRole.Participant;

		[System.Xml.Serialization.XmlElement( "Name", typeof(string) )]
		public string Name
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

		[System.Xml.Serialization.XmlElement( "Location", typeof(string) )]
		public string Location
		{
			get
			{ return this.m_strLoc; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strLoc = value;
			}
		}
	}
}
