using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// This class is used to return the result of an evaluation by an IPolicyExpression class
	/// </summary>
	public class EvaluationResult 
	{
		private string tag;			
		private bool result;		
		private string message;
		private IEnumerable nestedResults;

		private static IEnumerable EMPTY_ENUMERABLE = new ArrayList();

		public EvaluationResult(string tag, bool result) : 
			this(tag, result, null) 
		{
		}

		public EvaluationResult(string tag, bool result, string message) : 
			this(tag, result, message, EMPTY_ENUMERABLE)
		{
		}
		
		public EvaluationResult(string tag, bool result, string message, IEnumerable nestedResults) 
		{
			this.tag = tag;
			this.result = result;
			this.message = message;
			this.nestedResults = nestedResults;
		}

		/// <summary>
		/// The policy expression tag for this evaluation. Read-only property.
		/// </summary>
		public string Tag 
		{
			get 
			{
				return tag;
			}
		}

		/// <summary>
		/// The actual result of this evaluation. Read-only property.
		/// </summary>
		public bool Result
		{
			get 
			{
				return result;
			}
		}

		/// <summary>
		/// An optional message, for example describing why the evaluation result is false. Read-only property.
		/// </summary>
		public string Message
		{
			get 
			{
				return message;
			}
		}

		/// <summary>
		/// An evaluation result can optionally contain nested evaluation results. 
		/// For example, a policy that is composed of multiple expressions can return all these
		/// evaluation results so that the initial caller can now more specifically where an evaluation
		/// failed.
		/// Read-only property.
		/// </summary>
		public IEnumerable EvaluationResults 
		{
			get 
			{
				return nestedResults;
			}
		}
		
		public override string ToString() 
		{
			return "[EvaluationResult: tag=" + tag + ", result=" + result + ", message=" + message + "]";
		}

		/// <summary>
		/// Prints out the result, message, and any nested EvaluationResults
		/// </summary>
		/// <param name="writer"></param>
		public void dumpResults(TextWriter writer) 
		{	
			dumpResults(writer, "");
		}

		private void dumpResults(TextWriter writer, string prefix) 
		{
			writer.WriteLine(prefix + "Tag: " + tag);
			writer.WriteLine(prefix + "Evaluated to " + result + ": " + message + "\n");
			
			if (nestedResults.GetEnumerator().MoveNext()) 
			{
				writer.WriteLine("------------------------ nested results ------------------------");
				foreach (EvaluationResult evalResult in nestedResults) 
				{
					evalResult.dumpResults(writer, prefix + "  ");
				}
				writer.WriteLine("----------------------------------------------------------------");
			}
		}

	}
}
