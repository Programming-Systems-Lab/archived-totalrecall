using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ResourceCtxMsg : ContextMsg
	{
		ResourceMsg m_resMsg = null;

		public ResourceCtxMsg( ResourceMsg resMsg )
		{
			if( resMsg == null )
				throw new ArgumentNullException( "resMsg", "Resource message cannot be null" );
			
			this.m_resMsg = resMsg;

			if( this.m_resMsg.Type == enuResourceMsgType.Add )
				this.Type = enuContextMsgType.ResourceAdd;
			if( this.m_resMsg.Type == enuResourceMsgType.Update )
				this.Type = enuContextMsgType.ResourceAdd;
			if( this.m_resMsg.Type == enuResourceMsgType.Recall )
				this.Type = enuContextMsgType.ResourceRecall;
		}
		
		public ResourceMsg ResourceMessage
		{
			get
			{ return this.m_resMsg; }
			set
			{
				if( value == null )
					return;

				this.m_resMsg = value;
			}
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( value == enuContextMsgType.ResourceAdd || value == enuContextMsgType.ResourceRecall )
					this.m_type = value;
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
