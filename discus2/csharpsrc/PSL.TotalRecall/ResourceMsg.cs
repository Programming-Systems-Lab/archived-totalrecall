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
	public class ResourceMsg : Message
	{
		public ResourceMsg()
		{
			this.Type = enuResourceMsgType.Add;
		}

		[System.Xml.Serialization.XmlArrayAttribute("Resources")]
		[System.Xml.Serialization.XmlArrayItem(typeof(Resource),ElementName="Resource")]
		public ArrayList m_lstResources = new ArrayList();

		public new static ResourceMsg FromXml( string strXml )
		{
			System.Type type =  typeof(ResourceMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (ResourceMsg) ser.Deserialize( xt );
		}

		[System.Xml.Serialization.XmlElement("Type")]
		public enuResourceMsgType Type;
	}
	
}
