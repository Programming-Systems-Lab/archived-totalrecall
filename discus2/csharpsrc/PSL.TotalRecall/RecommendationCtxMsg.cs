using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class RecommendationCtxMsg : ContextMsg
	{
		MeetingRequestMsg m_mtgReq = null;

		public RecommendationCtxMsg( MeetingRequestMsg mtgReq )
		{
			if( mtgReq == null )
				throw new ArgumentNullException( "mtgReq", "Meeting request cannot be null" );

			this.m_mtgReq = mtgReq;
		}

		public MeetingRequestMsg MeetingRequest
		{
			get
			{ return this.m_mtgReq; }
			set
			{
				if( value == null )
					return;

				this.m_mtgReq = value;
			}
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( value == enuContextMsgType.RecommendationRequest || value == enuContextMsgType.RecommendationResponse )
					this.m_type = value;
			}
		}

		public new static RecommendationCtxMsg FromXml( string strXml )
		{
			System.Type type =  typeof(RecommendationCtxMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (RecommendationCtxMsg) ser.Deserialize( xt );
		}
	}
}
