using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Web.Services.Security.X509;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class InfoAgentCtxMsg : ContextMsg
	{
		private string m_strContactID = "";
		private string m_strInfoAgentUrl = "";
		private string m_strBase64Cert = "";

		public InfoAgentCtxMsg()
		{
		}

		public string ContactID
		{
			get
			{ return this.m_strContactID; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strContactID = value;
			}
		}
		
		public string InfoAgentUrl
		{
			get
			{ return this.m_strInfoAgentUrl; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strInfoAgentUrl = value;
			}
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( value == enuContextMsgType.InfoAgentJoined || value == enuContextMsgType.InfoAgentLeft )
					this.m_type = value;
			}
		}

		[XmlIgnore]
		public X509Certificate Certificate
		{
			get
			{ 
				if( this.m_strBase64Cert.Length == 0 )
					return null;
				else return X509Certificate.FromBase64String( this.m_strBase64Cert );
			}
			set
			{
				if( value == null )
					return;
				
				this.m_strBase64Cert = value.ToBase64String();
			}
		}

		public string Base64Cert
		{
			get
			{ return this.m_strBase64Cert; }

			set
			{
				if( value == null || value.Length == 0 )
					return;
				
				// Determine whether string value is actually a valid X509Certificate
				// if we fail to create a cert from the string then we exit
				try
				{
					X509Certificate innerCert = X509Certificate.FromBase64String( value );
				}
				catch( Exception /*e*/ )
				{
					return;
				}

				this.m_strBase64Cert = value;
			}
		}

		public new static InfoAgentCtxMsg FromXml( string strXml )
		{
			System.Type type =  typeof(InfoAgentCtxMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (InfoAgentCtxMsg) ser.Deserialize( xt );
		}
	}
}
