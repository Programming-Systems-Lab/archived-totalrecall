using System;
using System.Collections;

namespace PSL.TotalRecall.PolicyManager
{
	/// <summary>
	/// This interface provides method to get information about the context within which policies
	/// should be evaluated. A context should provide, for example, access to a list of meeting participants,
	/// the meeting topic, etc.
	/// </summary>
	public interface IContext 
	{
		/// <summary>
		/// The list of participants in this meeting.
		/// TODO: might want to make it a list or something else, and not necessarily of strings
		/// </summary>
		/// <returns></returns>
		Hashtable Participants 
		{
			get;
		}

		/// <summary>
		/// The topic of this meeting
		/// </summary>
		/// <returns></returns>
		string Topic 
		{
			get;
		}

	}
}
