using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

using PSL.TotalRecall;

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
		private PolicyDAO policyDAO;

		public PolicyReferenceEvaluator() 
		{
			policyDAO = new PolicyDAO(PolicyManager.DatabaseConnectionString);

		}

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
			try 
			{
				string policyDoc = policyDAO.GetPolicy(expression.policyId);

				if (policyDoc == null || policyDoc.Length == 0) 
				{
					return new EvaluationResult(TAG, false, "Policy with id " + expression.policyId + " not found in database.");
				}
				else 
				{
					EvaluationResult result = PolicyManager.evaluatePolicy(policyDoc, context);
					ArrayList list = new ArrayList();
					list.Add(result);
					return new EvaluationResult(TAG, result.Result, "Evaluated policy id " + expression.policyId, list);
				}
			}
			catch (PolicyManagerException /*e*/) 
			{
				return new EvaluationResult(TAG, false, "Could not evaluate policy id " + expression.policyId + ": " + e);
			}
			
		}

	}
}
