using System;
using System.Xml;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// This interface should be implemented by all "policy plugins" that can evaluate
	/// a particular XML policy expression tag.
	/// </summary>
	public interface IPolicyEvaluator
	{
		/// <summary>
		/// Evaluates the given expression with respect to the given context.
		/// NOTE: implementations of this method should be thread-safe! That is,
		/// one instance could be shared among several threads.
		/// </summary>
		EvaluationResult evaluateExpression(XmlElement expr, IContext context);	
		
	}
}
