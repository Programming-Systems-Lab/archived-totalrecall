using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Interface exposed by information agents that uses simple types for both
	/// inputs and outputs
	/// </summary>
	public interface ICustodianExternal
	{
		// Method exposed by information agents to receive invitations into a meeting
		string JoinMeeting( string strMeetingRequest, string strVourchers );
		// Method exposed by information agents to invite other information agents
		// into a meeting
		string InviteAgent( string strMeetingRequest, string strInfoAgentUri );
		
		// Method exposed by information agents to allow them to sign a meeting
		// request (invitation)
		string SignMeetingRequest( string strMeetingRequest );
		
		// Method exposed by information agents to send a context message to other
		// information agents 
		string SendContextUpdate( string strContextMessage, string strInfoAgentUri );
		// Method exposed by information agents to receive context messages
		// (may trigger a call to SendResources or RecallResources)
		string ContextUpdate( string strContextMessage );

		// Method exposed by information agents to request recommendations from the
		// local Memento agent (local Memento agent uri set in config file)
		string RequestRecommendation( string strRecommendationRequest );
		// Method exposed by information agents to receive recommendations from the
		// local Memento agent (calls SendResources)
		string Recommend( string strRecommendationResponse );

		// Method exposed by information agents to receive resources sent by meeting
		// participants
		string AddResources( string strResourceAddMessage );
		// Method exposed by information agents to recall (mark as tainted) resources
		// previously supplied to a meeting.
		// Meeting organizer information agent Uri stored in data source
		// indexed by meeting ID
		string RecallResources( string strResourceRecallMessage );
		// Method exposed by information agents to supply resources to a meeting.
		// Meeting organizer information agent Uri stored in data source
		// indexed my meeting ID (call AddResource on meeting organizer information
		// agent)
		string SendResources( string strResourceAddMessage );
		
		// Method exposed by information agents to be the meeting organizer
		// for a new meeting
		string InitiateMeeting( string strMeetingInit );
	}
}
