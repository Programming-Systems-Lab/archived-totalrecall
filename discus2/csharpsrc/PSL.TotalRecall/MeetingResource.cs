using System;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for MeetingResource.
	/// </summary>
	public class MeetingResource:Resource
	{
		private enuResourceState m_resState= new enuResourceState();
		private string m_strOwner = "";
		private string m_strMeetingID = "";
		private Resource m_res = null;

		// TODO: Change this to internal when MeetingDAO moves over
		internal MeetingResource()
		{
		}

		public MeetingResource( Resource res, string strMeetingID, string strOwner )
		{
			if( res == null )
				throw new ArgumentNullException( "res", "Invalid Resource" );
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new ArgumentException( "Invalid meeting ID", "strOwner" );
			if( strOwner == null || strOwner.Length == 0 )
				throw new ArgumentException( "Invalid owner", "strOwner" );

			this.m_resState = enuResourceState.Shared;
			this.m_strOwner = strOwner;
			this.m_res = res;
			this.m_strMeetingID = strMeetingID;
		}

		public override string Name
		{
			get
			{
				if( this.m_res == null )
					return "";
				else return this.m_res.Name;
			}
			set
			{
				if( value == null || value.Length == 0 )
					return;

				if( this.m_res == null )
					return;
				else this.m_res.Name = value;
			}
		}
		
		public override string Url
		{
			get
			{
				if( this.m_res == null )
					return "";
				else return this.m_res.Url;
			}
			set
			{
				if( value == null || value.Length == 0 )
					return;

				if( this.m_res == null )
					return;

				else this.m_res.Url = value;
			}
		}
		
		public override string ID
		{
			get
			{
				if( this.m_res == null )
					return "";
				else return this.m_res.ID;
			}
			set
			{
				if( value == null || value.Length == 0 )
					return;

				if( this.m_res == null )
					return;
				else this.m_res.ID = value;
			}
		}
		


		public string Owner
		{
			get
			{ return this.m_strOwner; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strOwner = value;
			}
		}

		public enuResourceState State
		{
			get
			{ return this.m_resState; }
			set
			{ this.m_resState = value; }
		}

		public string MeetingID
		{
			get
			{ return this.m_strMeetingID; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strMeetingID = value;
			}
		}

		public Resource Resource
		{
			get
			{ return this.m_res; }
			set
			{
				if( value == null )
					return;
				this.m_res = value;
			}
		}
	}
}
