using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// An implementation to evaluate a PolicyReference policy expression tag.
	/// The policy referenced in this expression is loaded and then evaluated, and the
	/// EvaluationResults is returned
	/// </summary>
	public class PolicyReferenceEvaluator : IPolicyEvaluator
	{
		public const string TAG = "PolicyReference";

		private XmlSerializer serializer = new XmlSerializer(typeof(PolicyReference));

		public EvaluationResult evaluateExpression(XmlElement expressionDoc, IContext context)
		{

			// first deserialize expression into a PolicyReference instance
			PolicyReference expression = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(expressionDoc);
				expression = (PolicyReference) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize PolicyReference node", e);
			}

			// TODO: load policy reference from DB

			XmlElement policy = null;

			return PolicyManager.invokeEvaluator(policy, context);
			
		}

	}
}
