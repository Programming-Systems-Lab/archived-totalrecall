using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.Util
{
	/// <summary>
	/// Summary description for Serializer.
	/// </summary>
	public abstract class Serializer
	{
		public Serializer()
		{
		}

		public static object DeSerialize( string strXml, Type type )
		{
			if( strXml == null || strXml.Length == 0 )
				throw new ArgumentException( "Invalid XML to deserialize", "strXml" );
			if( type == null )
				throw new ArgumentNullException( "type", "Deserialization type cannot be null" );

			XmlSerializer ser = new XmlSerializer( type );
			XmlReader xt = new XmlTextReader( strXml, XmlNodeType.Document, null );
			xt.Read();
			return ser.Deserialize( xt );
		}

		public static string Serialize( object obj )
		{
			if( obj == null )
				throw new ArgumentNullException( "obj", "Object to serialize cannot be null" );
			
			string strXml = "";
			
			// Create a new serializer
			XmlSerializer ser = new XmlSerializer( obj.GetType() );
			// Create a memory stream
			MemoryStream ms = new MemoryStream();
			// Serialize to stream ms
			ser.Serialize( ms, obj );
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
	}
}
