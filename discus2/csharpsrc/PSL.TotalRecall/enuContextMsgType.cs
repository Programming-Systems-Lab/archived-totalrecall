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

		ResourceShared,
		ResourceSharedResponse,
		ResourceRecalled,
		ResourceRecalledResponse,
		ResourceUpdated,
		ResourceUpdatedResponse,

		MeetingEnded,
		MeetingSuspended,
		MeetingResumed,
		MeetingSuspendedResponse,
		MeetingResumedResponse,
		MeetingEndedResponse,

		Unknown,
	}
}
