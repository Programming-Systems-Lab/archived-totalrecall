using System;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// Summary description for RequiresTopicEvaluator.
	/// </summary>
	public class RequiresTopicEvaluator: IPolicyEvaluator
	{
		public const string TAG = "RequiresTopic";

		private XmlSerializer serializer = new XmlSerializer(typeof(RequiresTopic));

		public EvaluationResult evaluateExpression(XmlElement expressionDoc, IContext context) 
		{

			// first deserialize expression into a RequiresTopic instance
			RequiresTopic expression = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(expressionDoc);
				expression = (RequiresTopic) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize RequiresTopic node", e);
			}

			// check if we should do a direct or regular expression match
			bool isMatch;
			if (expression.IsRegexSpecified && expression.IsRegex) 
			{
				// do regex match
				Regex regex = new Regex(expression.Topic, RegexOptions.IgnoreCase);
				isMatch = regex.IsMatch(context.Topic, 0);
			} 
			else 
			{
				// do direct match
				isMatch = context.Topic.ToLower().Equals(expression.Topic.ToLower());
			}

			string match = "\"" + expression.Topic + "\"" + (expression.IsRegexSpecified && expression.IsRegex ? " (regex match)" : "");
			if (isMatch) 
			{
				return new EvaluationResult(TAG, true, "Context topic matches " + match);
			} 
			else 
			{
				return new EvaluationResult(TAG, false, "Context topic does NOT match " + match);
			}
		}

	}
}
