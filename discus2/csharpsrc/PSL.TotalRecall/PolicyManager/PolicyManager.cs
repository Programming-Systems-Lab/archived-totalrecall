using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// The PolicyManager's main purpose is to analyze policy documents with respect to a given context,
	/// and indicate whether the policy is satisfied by the current context or not.
	/// 
	/// This class is not thread-safe (because the shared serializer is not thread-safe).
	/// </summary>
	public class PolicyManager
	{
		/// <summary>
		/// The assembly used to load plugin classes
		/// </summary>
		private static Assembly pluginAssembly = Assembly.GetExecutingAssembly();	// TODO: might need to modify this
		
		/// <summary>
		/// The serializer to use for deserializing policy documents
		/// </summary>
		private XmlSerializer serializer = new XmlSerializer(typeof(Policy));

		/// <summary>
		/// A table of mappings between namespaces of policy expression and the corresponding 
		/// IPolicyEvaluator class name
		/// </summary>
		private static Hashtable evaluatorNames;

		public const string TAG = "Policy";

		/// <summary>
		/// Used for testing. Pass a policy XML file as the first argument.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			PolicyManager manager = new PolicyManager();

			XmlDocument doc = new XmlDocument();
			doc.Load(args[0]);
			EvaluationResult result = manager.evaluatePolicy(doc, new TestContext());
			result.dumpResults(Console.Out);
			debug("done");
		}

		static PolicyManager() 
		{
			// initialize table 
			// TODO: probably read this from some config file
			evaluatorNames = new Hashtable();
			
			evaluatorNames.Add("http://psl.cs.columbia.edu/discus2/All",
				"PSL.TotalRecall.PolicyManager.AllEvaluator");
			
			evaluatorNames.Add("http://psl.cs.columbia.edu/discus2/RequiresParticipants",
				"PSL.TotalRecall.PolicyManager.RequiresParticipantsEvaluator");

		}

		/// <summary>
		/// Evaluates a policy with respect to the given context.
		/// </summary>
		/// <param name="policy"></param>
		/// <param name="context"></param>
		/// <returns>
		///		An EvaluationResult indiciating whether the policy is satisfied under the
		///		current context or not.
		///	</returns>
		///	<exception cref="PSL.TotalRecall.PolicyManager.PolicyManagerException">
		///		Thrown if there is a problem parsing the XML or analysing the policy.
		///	</exception>
		public EvaluationResult evaluatePolicy(XmlDocument policyDoc, IContext context)
		{
			
			if (policyDoc == null || context == null) 
			{
				throw new PolicyManagerException("Arguments cannot be null");
			}

			// first de-serialize the XmlDocument
			Policy policy = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(policyDoc);
				policy = (Policy) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize policy document", e);
			}
			
			// now evaluate the one element in this policy
			return invokeEvaluator(policy.Any, context);
			
		}
		
		public static EvaluationResult invokeEvaluator(XmlElement element, IContext context) 
		{
			EvaluationResult result = null;

			// get class that corresponds to this element
			// we use the namespace attribute for this tag
			string className = (string) evaluatorNames[element.NamespaceURI];
			if (className == null) 
			{
				result = new EvaluationResult(TAG, false, "Evaluator class not found for tag " +
					element.LocalName + " in namespace " + element.NamespaceURI);
			}
			else 
			{

				object o = pluginAssembly.CreateInstance(className, true);
				if (o == null || !(o is IPolicyEvaluator))
				{
					result = new EvaluationResult(TAG, false, "Could not load plugin for " + element.LocalName);
				}
				else 
				{
					// invoke the evaluator!
					IPolicyEvaluator evaluator = (IPolicyEvaluator) o;
					result = evaluator.evaluateExpression(element, context);
				}
			}

			return result;
		}

		private static void debug(Object o) 
		{
			Console.WriteLine(o);
		}
	}

	public class PolicyManagerException : Exception 
	{
		public PolicyManagerException() : base() 
		{
		}

		public PolicyManagerException(string s) : base(s) 
		{
		}

		public PolicyManagerException(string s, Exception e) : base(s,e) 
		{
		}
	}

	public class TestContext : IContext 
	{
		Hashtable participantTable;

		public TestContext() 
		{
			participantTable = new Hashtable();
			participantTable.Add("joe","joe schmoe");
			participantTable.Add("bob","bob schumaher");

		}

		public Hashtable Participants 
		{
			get 
			{
				return participantTable;
			}
		}

		public string Topic 
		{
			get 
			{
				return "MyTopic";
			}
		}
	}

	public class TestPlugin : IPolicyEvaluator 
	{
		public EvaluationResult evaluateExpression(XmlElement expression, IContext context) 
		{
			return new EvaluationResult("test", true, "");
		}
	}
	
}
