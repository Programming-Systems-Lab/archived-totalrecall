using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Summary description for AllEvaluator.
	/// </summary>
	public class AllEvaluator : IPolicyEvaluator
	{
		public const string TAG = "All";

		private XmlSerializer serializer = new XmlSerializer(typeof(All));

		public EvaluationResult evaluateExpression(XmlElement expressionDoc, IContext context) 
		{

			// first deserialize expression into a All instance
			All expression = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(expressionDoc);
				expression = (All) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize All node", e);
			}

			ArrayList results = new ArrayList();
			foreach (XmlElement element in expression.Any) 
			{
				EvaluationResult result = PolicyManager.invokeEvaluator(element, context);
				results.Add(result);
				
				if (!result.Result) 
				{
					return new EvaluationResult(TAG, false, "At least one expression evaluated false", results);
				}
			}

			return new EvaluationResult(TAG, true, "All expressions evaluated true", results);
		}
	}
}
