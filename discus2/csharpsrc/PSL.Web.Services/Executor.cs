using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Microsoft.Web.Services;
using Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;

namespace PSL.Web.Services.DynamicInvoke
{
	/// <summary>
	/// Summary description for Executor.
	/// </summary>
	public class Executor:IExecutor
	{
		private ExecutorSettings m_settings = new ExecutorSettings();

		public Executor()
		{
		}

		public Executor( ExecutorSettings settings )
		{
			if( settings == null )
				throw new ArgumentNullException( "settings", "ExecutorSettings instance cannot be null" );

			this.m_settings = settings;
		}

		public ExecutorSettings Settings
		{
			get
			{ return this.m_settings; }
			set
			{
				if( value == null )
					return;

				this.m_settings = value;
			}
		}
		
		public object Execute( object objCtx )
		{
			ExecContext ctx = objCtx as ExecContext;
			return this.Execute( ctx );
		}

		public object Execute( ExecContext ctx )
		{
			if( ctx == null )
				throw new ArgumentNullException( "ctx", "Execution context cannot be null" );

			object objInvokeResult = null;
			string strRetVal = "";
			string strError = "";
			
			// Load Assembly containing web service proxy
			bool bProxyMethodHasParameters = false;
			Assembly a = Assembly.LoadFrom( ctx.Assembly );
			// Get the correct type (ProxyClass)
			Type ProxyType = a.GetType( ctx.ServiceName );
			// Create an instance of the Proxy Class
			Object objProxy = a.CreateInstance( ctx.ServiceName );
			if( objProxy == null || ProxyType == null )
			{
				strError = "Cannot create type/proxy instance ";
				strError += ctx.ServiceName;
				strError += " in assembly ";
				strError += ctx.Assembly;
				throw new Exception( strError );
			}

			// Check whether Proxy class has a property
			// called Url, valid for generated
			// proxies, if it does check its value
			// if value is empty fill in proxy access point
			PropertyInfo Url = ProxyType.GetProperty( "Url" );
			if( Url != null )
			{
				string strUrl = (string) Url.GetValue( objProxy, null );
				if( strUrl.Length == 0 )
					Url.SetValue( objProxy, ctx.AccessPointUrl, null );
			}
			
			// Once we have a Proxy Object and a Type instance
			// use reflection to get info on method to be
			// executed.
			MethodInfo mInfo = ProxyType.GetMethod( ctx.MethodName );
			if( mInfo == null )
			{
				strError = "Cannot find method ";
				strError += ctx.MethodName;
				strError += " of Proxy ";
				strError += ctx.ServiceName;
				strError += " loaded from assembly ";
				strError += ctx.Assembly;
				throw new System.Exception( strError );
			}

			// Get info on parameters expected by Proxy method
			ParameterInfo[] arrParamInfo = mInfo.GetParameters();
			if( arrParamInfo.Length > 0 )
				bProxyMethodHasParameters = true;
			
			Object[] param = null;

			if( bProxyMethodHasParameters )
			{
				// Number parameters passed not equal to number parameters expected
				if( ctx.Parameters.Count != arrParamInfo.Length )
					throw new Exception( "Wrong Number of Arguments Passed to Proxy ");
					
				// Create array to hold parameters
				param = new Object[arrParamInfo.Length];

				// Try deserialization
				for( int i = 0; i < ctx.Parameters.Count; i++ )
				{
					// Get the expected type
					Type paramType = arrParamInfo[i].ParameterType;
					// Create XmlSerializer
					XmlSerializer xs = new XmlSerializer( paramType );
					// Read in Xml doc representing parameter
					System.Xml.XmlReader xt = new XmlTextReader( (string) ctx.Parameters[i], XmlNodeType.Document, null );
					xt.Read(); 
					// Deserialize
					Object paramInst = xs.Deserialize( xt );
					// Store in parameter array
					param[i] = (Object) paramInst;
				}
			}// End if bProxyMethodHasParameters
			
			// Add digital signatures and/or encryption based on
			// the Executor settings
			if( m_settings.SignSoapMessage || m_settings.EncryptSoapMessage )
			{
				// Sign message
				X509SecurityToken signatureToken = new X509SecurityToken( this.Settings.SigningCertificate );
				// Ask proxy for RequestContext property
				PropertyInfo requestSoapContextProp = ProxyType.GetProperty( "RequestSoapContext" );
				SoapContext reqSoapCtx = null;

				// If property exists
				if( requestSoapContextProp != null )
				{
					// Get property value
					reqSoapCtx = (SoapContext) requestSoapContextProp.GetValue( objProxy, null );
					// Make changes to property value
					// Add security token to SOAP message header
					reqSoapCtx.Security.Tokens.Add( signatureToken );
					// Create a signature element from the token
					Signature sig = new Signature( signatureToken );
					// Sign the mesage body
					sig.SignatureOptions = SignatureOptions.IncludeSoapBody;
					// Add digital signature
					reqSoapCtx.Security.Elements.Add( sig );

					// Encrypt SOAP message
					if( m_settings.EncryptSoapMessage )
					{
						X509SecurityToken encryptionToken = new X509SecurityToken( this.m_settings.EncryptionCertificate );	
						// Add encryption token to SOAP message header
						reqSoapCtx.Security.Tokens.Add( encryptionToken );
						// Encrypt message
						EncryptedData enc = new EncryptedData( encryptionToken );
						reqSoapCtx.Security.Elements.Add( enc );
					}// End-if encrypting SOAP message
				}// End-if RequestSoapContext property exposed by proxy
			}//End-if SOAP message is to be digitally signed or encrypted
					
			if( bProxyMethodHasParameters )
				objInvokeResult = ProxyType.InvokeMember( ctx.MethodName,
					BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
					null,
					objProxy,
					param );
 
			else objInvokeResult = ProxyType.InvokeMember( ctx.MethodName,
					 BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static,
					 null,
					 objProxy,
					 null );

			// Check for digitally signed response
			if( this.m_settings.ExpectSignedResponse )
			{
				this.m_settings.ResponseCertificate = null;

				// Get the response certificate
				// Ask proxy for ResponseContext property
				PropertyInfo responseSoapContextProp = ProxyType.GetProperty( "ResponseSoapContext" );
				SoapContext respSoapCtx = null;

				// If property exists
				if( responseSoapContextProp != null )
				{
					// Get property value
					respSoapCtx = (SoapContext) responseSoapContextProp.GetValue( objProxy, null );
					// Inspect the response SOAP context
					if( respSoapCtx.Security.Elements.Count != 1 )
						throw new Exception( "Expected a single security element" );

					object objElement = respSoapCtx.Security.Elements[0];
					if( !(objElement is Microsoft.Web.Services.Security.Signature ) )
						throw new Exception( "Expected a digital signature element" );

					Microsoft.Web.Services.Security.Signature signature = objElement as Microsoft.Web.Services.Security.Signature;
					if( signature.SignatureOptions != SignatureOptions.IncludeSoapBody )
						throw new Exception( "Expected the body of the SOAP message to be signed" );

					// If the signature verifies set the response cert
					if( signature.CheckSignature() )
					{
						X509SecurityToken tok = (X509SecurityToken) signature.SecurityToken;
						this.m_settings.ResponseCertificate = tok.Certificate;
					}
				}
			}		
			
			if( objInvokeResult != null )
			{
				// Otherwise serialize results to XML
				// Get returned type
				Type returnType = objInvokeResult.GetType();
				// Create XmlSerializer
				XmlSerializer ser = new XmlSerializer( returnType );
				// Create a memory stream
				MemoryStream ms = new MemoryStream();
				// Serialize to stream ms
				ser.Serialize( ms, objInvokeResult );
				// Goto start of stream
				ms.Seek( 0, System.IO.SeekOrigin.Begin );
				// Create a stream reader
				TextReader reader = new StreamReader( ms );
				// Read entire stream, this is our return value
				strRetVal = reader.ReadToEnd();
				// Close reader
				reader.Close();
				// Close stream
				ms.Close();
			}

			return strRetVal;
		}
	}
}
