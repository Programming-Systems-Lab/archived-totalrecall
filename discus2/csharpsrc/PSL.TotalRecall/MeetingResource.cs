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

		// TODO: Change this to internal when MeetingDAO moves over
		internal MeetingResource()
		{
		}

		public MeetingResource( Resource res, string strOwner )
		{
			if( res == null )
				throw new ArgumentNullException( "res", "Invalid Resource" );
			if( strOwner == null || strOwner.Length == 0 )
				throw new ArgumentException( "Invalid owner", "strOwner" );

			this.Name = res.Name;
			this.Url = res.Url;
			this.ID = res.ID;
			this.m_resState = enuResourceState.Shared;
			this.m_strOwner = strOwner;
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
	}
}
