using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Evaluates a Not policy expression, which returns the inverse of the result of the one
	/// nested policy expression
	/// </summary>
	public class NotEvaluator : IPolicyEvaluator
	{
		public const string TAG = "Not";

		private XmlSerializer serializer = new XmlSerializer(typeof(Not));

		public EvaluationResult evaluateExpression(XmlElement expressionDoc, IContext context) 
		{
			
			// first deserialize expression into a Not instance
			Not expression = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(expressionDoc);
				expression = (Not) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize Not node", e);
			}

			EvaluationResult result = PolicyManager.invokeEvaluator(expression.Any, context);
			
			return new EvaluationResult(TAG, !result.Result, "Nested expression evaluated " + 
				result.Result);
				
			
		}
	
	}
}
