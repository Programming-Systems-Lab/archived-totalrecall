using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class RecommendationResponseCtxMsg : ContextMsg
	{
		private RecommendationRequestCtxMsg m_recReq = null;
		// List of resources
		private ResourceMsg m_resourceMsg = null;

		public RecommendationResponseCtxMsg( RecommendationRequestCtxMsg recReq )
		{
			if( recReq == null )
				throw new ArgumentNullException( "recReq", "Invalid recommendation request" );
			
			this.m_recReq = recReq;
			this.m_type = enuContextMsgType.RecommendationResponse;
			this.MeetingID = m_recReq.MeetingID;
			this.MessageID = m_recReq.MessageID;
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( value == enuContextMsgType.RecommendationResponse )
					this.m_type = value;
			}
		}

		public new static RecommendationResponseCtxMsg FromXml( string strXml )
		{
			System.Type type =  typeof(RecommendationResponseCtxMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (RecommendationResponseCtxMsg) ser.Deserialize( xt );
		}

		public ResourceMsg Resource
		{
			get
			{ return this.m_resourceMsg; }
			set
			{
				if( value == null )
					return;
				this.m_resourceMsg = value;
			}
		}
	}
}
