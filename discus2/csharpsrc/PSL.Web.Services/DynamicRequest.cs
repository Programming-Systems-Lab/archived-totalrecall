using System;

// DISCUS Dynamic Proxy Package
namespace PSL.Web.Services.DynamicProxy
{
	/// <summary>
	/// Encapsulates a Dynamic Request to a Web Service
	/// </summary>
	public class ProxyGenRequest
	{
		private string m_strWsdlUrl = ""; // URL to WSDL file
		private string m_strServiceName = ""; // Name of service (class)
		private string m_strNamespace = DEFAULT_PROXY_NAMESPACE; // Namespace to use
		private string m_strProtocol = SOAPProtocol; // Protocol to use (SOAP, HTTP Get or HTTP Post )
		private string m_strProxyPath = ""; // Path where proxy generated
		
		public const string DEFAULT_PROXY_NAMESPACE = "DynamicPxy";
		// Request protocols, Default = SOAPProtocol
		public const string SOAPProtocol = "SOAP";
		public const string HTTPGetProtocol = "HTTP GET";
		public const string HTTPPostProtocol = "HTTP POST";

		public ProxyGenRequest()
		{
		}
		
		// Properties (gets and sets)
		public string WsdlUrl
		{
			get { return m_strWsdlUrl; }
			set { m_strWsdlUrl = value; }
		}

		public string ServiceName
		{
			get { return m_strServiceName; }
			set { m_strServiceName = value; }
		}
	
		public string Namespace
		{
			get { return m_strNamespace; }
			set { m_strNamespace = value; }
		}
		
		public string ProxyPath
		{
			get { return m_strProxyPath; }
			set { m_strProxyPath = value; }
		}

		public string Protocol
		{
			get { return m_strProtocol; }
			set { m_strProtocol = value; }
		}
	}// End ProxyGenRequest
}
