using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Summary description for Util.
	/// </summary>
	public class Util
	{
		public static XmlElement getXmlElement(Object o)
		{
			XmlSerializer ser = new XmlSerializer(o.GetType());
			MemoryStream ms = new MemoryStream();
			ser.Serialize(ms, o);
			ms.Seek(0,SeekOrigin.Begin);	// reset position

			XmlDocument doc = new XmlDocument();
			doc.Load(ms);

			return doc.DocumentElement;
	
		}
		
		public static void serialize(Object o, TextWriter writer) 
		{
			XmlSerializer ser = new XmlSerializer(o.GetType());
			
			ser.Serialize(writer, o);
		}
	
	}
}
