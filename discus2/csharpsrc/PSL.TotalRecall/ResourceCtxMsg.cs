using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ResourceCtxMsg : ContextMsg
	{
		private ArrayList m_lstResID = new ArrayList();
		private string m_strNewName = "";
		private string m_strNewUrl = "";
		
		public ResourceCtxMsg()
		{
		}
		
		public void AddResourceID( string strResID )
		{
			if( strResID == null || strResID.Length == 0 )
				return;

			this.m_lstResID.Add( strResID );
		}

		public void Clear()
		{
			this.m_lstResID.Clear();
		}

		public ArrayList ResourceIDs
		{
			get
			{ return this.m_lstResID; }
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( ( value == enuContextMsgType.ResourceShared || value == enuContextMsgType.ResourceRecalled ) || value == enuContextMsgType.ResourceUpdated )
					this.m_type = value;
			}
		}

		public string NewName
		{
			get
			{ return this.m_strNewName; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strNewName = value;
			}
		}

		public string NewUrl
		{
			get
			{ return this.m_strNewUrl; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strNewUrl = value;
			}
		}
				
		public new static ResourceCtxMsg FromXml( string strXml )
		{
			System.Type type =  typeof(ResourceCtxMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (ResourceCtxMsg) ser.Deserialize( xt );
		}
	}
}
