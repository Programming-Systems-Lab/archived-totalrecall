using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.psl.cs.columbia.edu/TotalRecall", IsNullable=false)]
	public class RecommendationRequestMsg : Message
	{
		[System.Xml.Serialization.XmlArrayAttribute("Participants")]
		[System.Xml.Serialization.XmlArrayItem(typeof(MeetingParticipant),ElementName="Participant")]
		public ArrayList m_lstParticipants = new ArrayList();
		
		public RecommendationRequestMsg()
		{
		}

		public new static RecommendationRequestMsg FromXml( string strXml )
		{
			System.Type type =  typeof(RecommendationRequestMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (RecommendationRequestMsg) ser.Deserialize( xt );
		}
	}
}
