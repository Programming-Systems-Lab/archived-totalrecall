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
		private string m_strMeetingID = "";
		private string m_strMeetingTopic = "";
		private string m_strSender = "";
		private string m_strSenderUri = "";

		public Message()
		{
		}

		[System.Xml.Serialization.XmlElement( "Sender", typeof(string) )]
		public string Sender
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

		[System.Xml.Serialization.XmlElement( "SenderUri", typeof(string) )]
		public string SenderUri
		{
			get
			{ return this.m_strSenderUri; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strSenderUri = value;
			}
		}
		
		[System.Xml.Serialization.XmlElement( "MeetingID", typeof(string) )]
		public string MeetingID
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
		
		[System.Xml.Serialization.XmlElement( "MeetingTopic", typeof(string) )]
		public string MeetingTopic
		{
			get
			{ return this.m_strMeetingTopic; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strMeetingTopic = value;
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
