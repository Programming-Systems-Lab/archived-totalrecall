using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class RecommendationRequestCtxMsg : ContextMsg
	{
		private MeetingRequestMsg m_mtgReq = null;

		public MeetingRequestMsg MeetingRequest
		{
			get
			{ return this.m_mtgReq; }
			set
			{
				if( value == null )
					return;

				// Set the meeting ID
				this.MeetingID = value.MeetingID;
				this.m_mtgReq = value;
			}
		}
		
		public RecommendationRequestCtxMsg()
		{
			this.m_type = enuContextMsgType.RecommendationRequest;
		}

		public RecommendationRequestCtxMsg( MeetingRequestMsg mtgReq )
		{
			if( mtgReq == null )
				throw new ArgumentNullException( "mtgReq", "Invalid meeting request" );
			
			this.m_type = enuContextMsgType.RecommendationRequest;
			// Set the meeting ID
			this.MeetingID = mtgReq.MeetingID;
			this.m_mtgReq = mtgReq;
		}

		public new static RecommendationRequestCtxMsg FromXml( string strXml )
		{
			System.Type type =  typeof(RecommendationRequestCtxMsg);
			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return (RecommendationRequestCtxMsg) ser.Deserialize( xt );
		}

		public override enuContextMsgType Type
		{
			get
			{ return this.m_type; }
			set
			{
				if( value == enuContextMsgType.RecommendationRequest )
					this.m_type = value;
			}
		}
	}
}
