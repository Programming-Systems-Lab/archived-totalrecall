using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Evaluates "Some" policy expressions, that represent "m of n" requirements, that is a 
	/// minimum of m policy expressions have to evaluate to true. (Note that it is not *exactly* m)
	/// </summary>
	public class SomeEvaluator : IPolicyEvaluator
	{
		public const string TAG = "Some";

		private XmlSerializer serializer = new XmlSerializer(typeof(Some));

		public EvaluationResult evaluateExpression(XmlElement expressionDoc, IContext context) 
		{
			
			// first deserialize expression into a Some instance
			Some expression = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(expressionDoc);
				expression = (Some) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize Some node", e);
			}

			ArrayList results = new ArrayList();
			int count = 0;	// expressions that evaluate to true
			foreach (XmlElement element in expression.Any) 
			{
				EvaluationResult result = PolicyManager.invokeEvaluator(element, context);
				results.Add(result);
				
				if (result.Result == true) 
				{
					count++;
				}

				if (count == expression.min) 
				{
					return new EvaluationResult(TAG, true, "At least " + expression.min + " expressions evaluated true", results);
				}
			}

			return new EvaluationResult(TAG, false, "Not enough expressions evaluated true. " + 
				"Required " + expression.min + ", got " + count, results);
			
		}
	
	}
}
