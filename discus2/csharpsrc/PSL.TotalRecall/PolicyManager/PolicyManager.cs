using System;
using System.IO;
using System.Collections;
using System.Configuration;
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
		/// The serializer to use for deserializing policy documents
		/// </summary>
		private static XmlSerializer serializer = new XmlSerializer(typeof(Policy));

		/// <summary>
		/// A table of mappings between namespaces of policy expression and the corresponding 
		/// IPolicyEvaluator class name
		/// </summary>
		//private static Hashtable evaluatorNames;
		private static IDictionary evaluatorMappings;

		/// <summary>
		/// A table of mappings between evaluator class names and an existing instantiation
		/// of the class (for caching instances).
		/// </summary>
		private static Hashtable evaluators;

		public const string TAG = "Policy";

		/// <summary>
		/// Used for testing. 
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Console.WriteLine("Enter name of policy XML doc to evaluate, or [Enter] to exit.");
			while (true) 
			{
				Console.Write("\n> ");
				string docName = Console.ReadLine();
				if (docName.Length == 0) 
				{
					break;
				}

				try 
				{
					StreamReader reader = new StreamReader(docName);
					string doc = reader.ReadToEnd();
					reader.Close();

					EvaluationResult result = PolicyManager.evaluatePolicy(doc, new TestContext());
					result.dumpResults(Console.Out);
				}
				catch (Exception e) 
				{
					Console.WriteLine("An error occurred: " + e.Message);
				}
				
			}
		}

		/// <summary>
		/// Static constructor, initializes evaluator tables.
		/// </summary>
		static PolicyManager() 
		{
			
			evaluatorMappings = (IDictionary) ConfigurationSettings.GetConfig("PolicyEvaluatorMappings");
			if (evaluatorMappings == null) 
			{
				throw new PolicyManagerException("Evaluator mappings not found in .config file");
			}

			evaluators = new Hashtable();

			
		}

		
		/// <summary>
		/// Default constructor is private since at this point all the methods exposed
		/// are static.
		/// </summary>
		private PolicyManager() 
		{
		}
		
		/// <summary>
		/// Evaluates a policy with respect to the given context.
		/// </summary>
		/// <param name="policy"></param>
		/// <param name="context"></param>
		/// <returns>
		///		An EvaluationResult indicating whether the policy is satisfied under the
		///		current context or not.
		///	</returns>
		///	<exception cref="PSL.TotalRecall.PolicyManager.PolicyManagerException">
		///		Thrown if there is a problem parsing the XML or analysing the policy.
		///	</exception>
		public static EvaluationResult evaluatePolicy(string policyDoc, IContext context)
		{
			
			if (policyDoc == null || context == null) 
			{
				throw new PolicyManagerException("Arguments cannot be null");
			}

			// first de-serialize the xml document
			Policy policy = null;
			try 
			{
				StringReader reader = new StringReader(policyDoc);
				policy = (Policy) serializer.Deserialize(reader);
				reader.Close();
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize policy document", e);
			}
			
			// now evaluate the one element in this policy
			return invokeEvaluator(policy.Any, context);

		}

		/// <summary>
		/// Evaluates a policy with respect to the given context.
		/// </summary>
		/// <param name="policy"></param>
		/// <param name="context"></param>
		/// <returns>
		///		An EvaluationResult indicating whether the policy is satisfied under the
		///		current context or not.
		///	</returns>
		///	<exception cref="PSL.TotalRecall.PolicyManager.PolicyManagerException">
		///		Thrown if there is a problem parsing the XML or analysing the policy.
		///	</exception>
		public static EvaluationResult evaluatePolicy(XmlDocument policyDoc, IContext context)
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
				reader.Close();
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize policy document", e);
			}
			
			// now evaluate the one element in this policy
			return invokeEvaluator(policy.Any, context);
			
		}
		
		/// <summary>
		/// Invokes an IPolicyEvaluator to evaluate the given XML element in the given
		/// context. An evaluator for the element must be registered in the PolicyManager 
		/// configuration (.config) file. Mappings are registered by element schema namespaces.
		/// </summary>
		/// <param name="element">The element to evaluate.</param>
		/// <param name="context">The context under which to evaluate this element</param>
		/// <returns>The EvaluationResult that the evaluator returns.</returns>
		public static EvaluationResult invokeEvaluator(XmlElement element, IContext context) 
		{
			EvaluationResult result = null;

			// get class that corresponds to this element
			// we use the namespace attribute for this tag
			string className = (string) evaluatorMappings[element.NamespaceURI];

			if (className == null) 
			{
				return new EvaluationResult(TAG, false, "Evaluator class not found for tag " +
					element.LocalName + " in namespace " + element.NamespaceURI);
			}
			

			// check if an evaluator for this class is already instantiated
			IPolicyEvaluator evaluator = (IPolicyEvaluator) evaluators[className];
			if (evaluator == null) 
			{
				// try to instantiate this evaluator
				Type type = Type.GetType(className);
				object o = Activator.CreateInstance(type);
				if (o == null || !(o is IPolicyEvaluator))
				{
					return new EvaluationResult(TAG, false, "Could not load evaluator for " + element.LocalName);
				}
				
				debug("instantiated evaluator for class " + className);

				evaluator = (IPolicyEvaluator) o;
				evaluators.Add(className, evaluator);
			}

			// now invoke the evaluator!
			result = evaluator.evaluateExpression(element, context);
			return result;
		}

		
		/// <summary>
		/// Returns the string to use to connect to the PolicyManager database.
		/// 
		/// Note: assumes one database used for all instances (possible problem)
		/// Reason: PolicyReferenceEvaluator, for example, needs to know the db
		/// connection string for its PolicyDAO object, but it gets instantiated 
		/// like all others evaluators, making it harder to use a particular constructor.
		/// Possible workaround: add an argument that identifies the object calling?
		/// </summary>
		/// <returns></returns>
		public static string DatabaseConnectionString 
		{
			get 
			{
				return ConfigurationSettings.AppSettings["DatabaseConnectionString"];	
			}
		}
		
		private static void debug(Object o) 
		{
#if DEBUG
			Console.WriteLine(o);
#endif
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
				return "My Topic";
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
