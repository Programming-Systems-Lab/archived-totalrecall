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

	public enum enuResourceMsgType
	{
		Add,
		Recall,
		Update
	}
	
	public class Resource
	{
		private string m_strID = "";
		private string m_strName = "";
		private string m_strUrl = "";
		
		public string ID
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

		public string Url
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
	}
}
