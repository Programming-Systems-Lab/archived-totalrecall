using System;
using System.Collections;

namespace PSL.Web.Services.DynamicInvoke
{
	/// <summary>
	/// To invoke a method on a WS proxy one needs:
	/// The location of the service - assembly to load
	/// The service name - type to create
	/// The proxy url - in the event proxy generated without one
	/// The name of the method to execute
	/// Array of XML parameters to deserialize and pass for method invocation
	/// </summary>
	public sealed class ExecContext
	{
		private string m_strAssembly = "";
		private string m_strServiceName = "";
		private string m_strAccessPointUrl = "";
		private string m_strMethodName = "";
		private ArrayList m_params = new ArrayList();
		
		public ExecContext()
		{
		}

		public void Reset()
		{
			this.AccessPointUrl = "";
			this.Assembly = "";
			this.ServiceName = "";
			this.MethodName = "";
			this.ClearParameters();
		}
		
		// Properties
		public string Assembly
		{
			get
			{ return m_strAssembly; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;

				lock( m_strAssembly )
				{
					m_strAssembly = value; 
				}
			}
		}
		
		public string ServiceName
		{
			get
			{ return m_strServiceName; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				lock( m_strServiceName )
				{
					m_strServiceName = value; 
				}
			}
		}
		
		public string AccessPointUrl
		{
			get
			{ return m_strAccessPointUrl; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				lock( m_strAccessPointUrl )
				{
					m_strAccessPointUrl = value; 
				}
			}
		}

		public string MethodName
		{
			get
			{ return m_strMethodName; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				lock( m_strMethodName )
				{
					m_strMethodName = value;
				}
			}
		}

		public void AddParameter( object objParam )
		{
			lock( m_params.SyncRoot )
			{
				this.m_params.Add( objParam );
			}
		}

		public void ClearParameters()
		{
			lock( m_params.SyncRoot )
			{
				this.m_params.Clear();
			}
		}

		public ArrayList Parameters
		{
			get
			{ return this.m_params; }
			set
			{
				if( value == null || value.Count == 0 )
					return;
				
				m_params = value;
			}
		}
	}
}
