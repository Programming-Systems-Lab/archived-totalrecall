using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for enuContextMsgType.
	/// </summary>
	public enum enuContextMsgType
	{
		MeetingRequest,
		MeetingRequestAccept,
		MeetingRequestRefuse,
		InfoAgentJoined,
		InfoAgentLeft,
		InfoAgentLeaving,
		RecommendationRequest,
		RecommendationResponse,
		Unknown
	}
}
