using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Interface exposed by information agents that uses complex types for both
	/// inputs and outputs
	/// </summary>
	public interface ICustodianInternal
	{
		MeetingResponseMsg JoinMeeting( MeetingRequestMsg req, string strVouchers );
		void InviteAgent( MeetingRequestMsg req, string strContactID );
		string SignMeetingRequest( MeetingRequestMsg req );
		void SendContextUpdate( ContextMsg ctxMsg, string strContactID );
		void ContextUpdate( ContextMsg ctxMsg );
		
		void RequestRecommendation( RecommendationRequestCtxMsg recReq );
		void Recommend( ResourceMsg resMsg );
		void AddResources( ResourceMsg resMsg );
		
		void RecallResources( ResourceMsg resMsg );
		void SendResources( ResourceMsg resMsg );
	}
}
