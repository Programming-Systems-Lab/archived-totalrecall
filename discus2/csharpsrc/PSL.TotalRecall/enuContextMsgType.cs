using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for enuContextMsgType.
	/// </summary>
	public enum enuContextMsgType
	{
		InfoAgentJoined,
		InfoAgentLeft,
		InfoAgentJoinedResponse,
		InfoAgentLeftResponse,

		RecommendationRequest,
		RecommendationResponse,

		ResourceAdd,
		ResourceRecall,
		ResourceAddResponse,
		ResourceRecallResponse,

		MeetingEnded,
		MeetingSuspended,
		MeetingResumed,
		MeetingSuspendedResponse,
		MeetingResumedResponse,
		MeetingEndedResponse,

		Unknown,
	}
}
