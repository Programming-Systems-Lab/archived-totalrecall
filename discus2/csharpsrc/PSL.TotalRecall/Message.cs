using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public abstract class Message
	{
		protected string m_strMessageID = Guid.NewGuid().ToString();
		private string m_strMeetingID = "";
		private string m_strSender = "";
		private string m_strSenderUrl = "";

		public Message()
		{
		}

		[System.Xml.Serialization.XmlElement( "Sender", typeof(string) )]
		public virtual string Sender
		{
			get
			{ return this.m_strSender; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strSender = value;
			}
		}

		[System.Xml.Serialization.XmlElement( "SenderUrl", typeof(string) )]
		public virtual string SenderUrl
		{
			get
			{ return this.m_strSenderUrl; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strSenderUrl = value;
			}
		}
		
		[System.Xml.Serialization.XmlElement( "MeetingID", typeof(string) )]
		public virtual string MeetingID
		{
			get
			{ return this.m_strMeetingID; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strMeetingID = value;
			}
		}
		
		public virtual string MessageID
		{
			get
			{ return this.m_strMessageID; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strMessageID = value;
			}
		}

		public virtual string ToXml()
		{
			string strXml = "";
			
			// Specify set of prefix + namespace mappings
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add( "totalRecall", "http://www.psl.cs.columbia.edu/TotalRecall" );
			ns.Add( String.Empty, "" );

			// Create a new serializer
			XmlSerializer ser = new XmlSerializer( this.GetType() );
			// Create a memory stream
			MemoryStream ms = new MemoryStream();
			// Serialize to stream ms
			ser.Serialize( ms, this, ns );
			// Goto start of stream
			ms.Seek( 0, System.IO.SeekOrigin.Begin );
			// Create a stream reader
			TextReader reader = new StreamReader( ms );
			// Read entire stream, this is our return value
			strXml = reader.ReadToEnd();
			// Close reader
			reader.Close();
			// Close stream
			ms.Close();
		
			return strXml;
		}

		public static Message FromXml( string strXml )
		{
			throw new NotSupportedException( "Method not supported by current implementation" );
		}
	}
}
