using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Test
	{
		
		public static void Main(String[] args) 
		{
			RequiresTopic rt = new RequiresTopic();
			rt.Topic = "foobar";

			RequiresParticipants rp = new RequiresParticipants();
			rp.Participant = new String[] {"joe", "bob"};

			All all = new All();
			all.Any = new XmlElement[] {Util.getXmlElement(rt), Util.getXmlElement(rp)};

			Policy policy = new Policy();
			policy.Any = Util.getXmlElement(all);

			StreamWriter writer = new StreamWriter("testpolicy2.xml");

			Util.serialize(policy, writer);
			writer.Close();
			Console.Out.WriteLine("done");
				

		}
		
	}
}
