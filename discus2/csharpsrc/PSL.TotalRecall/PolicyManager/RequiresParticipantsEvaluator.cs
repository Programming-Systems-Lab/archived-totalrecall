using System;
using System.Xml;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// An implemenation to evaluate a RequiresParticipants policy expression tag
	/// </summary>
	public class RequiresParticipantsEvaluator : IPolicyEvaluator
	{
		
		private XmlSerializer serializer = new XmlSerializer(typeof(RequiresParticipants));

		public EvaluationResult evaluateExpression(XmlElement expressionDoc, IContext context) 
		{

			// first deserialize expression into a RequiresParticipants instance
			RequiresParticipants expression = null;
			try 
			{
				XmlNodeReader reader = new XmlNodeReader(expressionDoc);
				expression = (RequiresParticipants) serializer.Deserialize(reader);
			}
			catch (Exception e) 
			{
				throw new PolicyManagerException("Could not deserialize RequiresParticipants node", e);
			}

			// go through and check if all participants listed in the policy are in the meeting
			
			foreach (string participant in expression.Participant) 
			{
				if (context.Participants[participant] == null) 
				{
					// return "false" as the evaluation result
					return new EvaluationResult(false, "Participant " + participant + " not in context");
				}

			}

			// if we got here, it means all participants were found
			return new EvaluationResult(true, "All participants found in context");
		}

	}
}
