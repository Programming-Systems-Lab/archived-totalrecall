using System;
using Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;

namespace PSL.Web.Services.DynamicInvoke
{
	/// <summary>
	/// Summary description for ExecutorSettings.
	/// </summary>
	public class ExecutorSettings
	{
		private bool m_bSignSoapMessage = false;
		private bool m_bEncryptSoapMessage = false;
		private X509Certificate m_signingCert = null;
		private X509Certificate m_encCert = null;
		private bool m_bExpectSignedResponse = false;
		private X509Certificate m_responseCert = null;
		
		public ExecutorSettings()
		{
		}
		
		public bool SignSoapMessage
		{
			get
			{ return this.m_bSignSoapMessage; }
		}

		public bool EncryptSoapMessage
		{
			get
			{ return this.m_bEncryptSoapMessage; }
		}

		public X509Certificate SigningCertificate
		{
			get
			{ return this.m_signingCert; }
			set
			{
				if( value == null )
				{
					this.m_bSignSoapMessage = false;
					this.m_signingCert = value;
					return;
				}
				
				if( !value.IsCurrent || ( !value.SupportsDigitalSignature || value.Key == null ) )
					throw new ArgumentException( "Invalid X509 Certificate for Digital Signatures" );

				this.m_signingCert = value;
				this.m_bSignSoapMessage = true;
			}
		}
		
		public X509Certificate EncryptionCertificate
		{
			get
			{ return this.m_encCert; }
			set
			{
				// If we are going to encrypt a message, we MUST sign it also
				if( this.m_signingCert == null )
					throw new InvalidOperationException( "Cannot set encryption certificate before setting a signature certificate" );
				
				if( value == null )
				{
					this.m_bEncryptSoapMessage = false;
					this.m_encCert = value;
					return;
				}
				
				if( !value.IsCurrent || (!value.SupportsDataEncryption ) )
					throw new ArgumentException( "Invalid X509 Certificate for Encryption" );

				this.m_encCert = value;
				this.m_bEncryptSoapMessage = true;
			}
		}

		public X509Certificate ResponseCertificate
		{
			get
			{ return this.m_responseCert; }
			set
			{
				this.m_responseCert = value;
			}
		}

		public bool ExpectSignedResponse
		{
			get
			{ return this.m_bExpectSignedResponse; }
			set
			{
				this.m_bExpectSignedResponse = value;
			}
		}
	}
}
