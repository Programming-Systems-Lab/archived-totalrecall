using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.IO;

using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Configuration;
using System.Reflection;

// Microsoft.Web imports
using Microsoft.Web.Services;
using Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;
using Microsoft.Web.Services.Dime;
// PSL imports
using PSL.TotalRecall;
using PSL.TotalRecall.Util;
using PSL.Web.Services.DynamicInvoke;
using PSL.Web.Services.DynamicProxy;

namespace TotalRecall
{
	/// <summary>
	/// Summary description for InfoAgent.
	/// </summary>
	public class InfoAgent : System.Web.Services.WebService//, ICustodianExternal
	{
		// Constants
		public const string SIGNATURE_TAG = "Signature";
		public const string KEYINFO_TAG = "KeyInfo";
		public const string ID_TAG = "Id";
		public const string VOUCHER_TAG = "Voucher";
		public const string VOUCHERS_TAG = "Vouchers";
		public const string VOUCHER_REF = "#" + VOUCHER_TAG;
		public const string HTTP_PREFIX = "http://";
		public const string INFO_AGENT = "InfoAgent";
		public const string SOURCE = "InfoAgent";
				
		// Member variables
		private string m_strDBConnect = Constants.DBConnect;
		private string m_strSigningKeyName = "CN=Client";
		private MeetingParticipant me = new MeetingParticipant();
		private string m_strIAUrl = "/TotalRecall/InfoAgent.asmx?wsdl";
		private X509Certificate m_signingCert = null;
		private string m_strProxyCache = "C:\\temp\\PxyCache";

		public string ProxyCache
		{
			get
			{ return this.m_strProxyCache; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strProxyCache = value;
			}
		}
		
		public string IAUrl
		{
			get
			{ return this.m_strIAUrl; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strIAUrl = value;
			}
		}

		public X509Certificate SigningCert
		{
			get
			{ return this.m_signingCert; }
			set
			{
				if( value == null )
					return;
				this.m_signingCert = value;
			}
		}
		
		public string SigningKeyName
		{
			get
			{ return this.m_strSigningKeyName; }
			set
			{
				if( value == null || value.Length == 0 )
					return;
				this.m_strSigningKeyName = value;
			}
		}

		public string DBConnect
		{
			get
			{ return this.m_strDBConnect; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strDBConnect = value;
			}
		}

		
		public InfoAgent()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();

			if( !EventLog.SourceExists( SOURCE ) )
				EventLog.CreateEventSource( SOURCE, "Application" );
			
			// Get local path
			string strLocalPath = new Uri( Assembly.GetExecutingAssembly().CodeBase ).LocalPath;
			// Set current directory
			FileInfo fInfo = new FileInfo( strLocalPath );
			System.Environment.CurrentDirectory = fInfo.DirectoryName;
						
			// Set the signing key name from the config file
			this.SigningKeyName = ConfigurationSettings.AppSettings["SigningKeyName"];
			// Set the proxy cache location
			this.ProxyCache = ConfigurationSettings.AppSettings["ProxyCache"];
			// Set the database connection string
			this.DBConnect = ConfigurationSettings.AppSettings["ConnectionString"];
			// Set the info agent url
			this.IAUrl = ConfigurationSettings.AppSettings["InfoAgentUrl"];

			// Get the signing cert
			X509CertificateStore certStore = X509CertificateStore.LocalMachineStore( X509CertificateStore.MyStore );
			// Open cert store for reading
			certStore.OpenRead();
			X509CertificateCollection certColl = certStore.FindCertificateBySubjectName( this.SigningKeyName );
			if( certColl.Count != 1 )
				throw new SoapException( "Error finding signing certificate", SoapException.ServerFaultCode );
			
			// Get signing cert
			m_signingCert = certColl[0];
			
			// Create "me" meeting participant
			me.Name = this.SigningKeyName;
			me.Location = HTTP_PREFIX + GetIPAddress() + this.IAUrl;
			me.Role = enuMeetingParticipantRole.Participant;
		}
		
		[WebMethod][SoapRpcMethod]
		public string JoinMeeting( string strMeetingRequest, string strVouchers )
		{
			SoapContext reqCtx = HttpSoapContext.RequestContext;
			if( reqCtx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
						
			MeetingRequestMsg req = MeetingRequestMsg.FromXml( strMeetingRequest );
			
			MeetingResponseMsg resp = this.JoinMeeting( req, strVouchers, ref reqCtx );
			resp.MeetingID = req.MeetingID;
			resp.Sender = me.Name;
			resp.SenderUrl = me.Location;
			
			// Sign the response
			SoapContext respCtx = HttpSoapContext.ResponseContext;
			this.SignSoapContext( ref respCtx );
			
			return resp.ToXml();
		}

		[WebMethod][SoapRpcMethod]
		public string InviteAgent( string strMeetingRequest, string strIAUrl )
		{
			// Accept only SOAP requests
			SoapContext reqCtx = HttpSoapContext.RequestContext;
			if( reqCtx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
			MeetingRequestMsg req = MeetingRequestMsg.FromXml( strMeetingRequest );
			MeetingResponseMsg resp = this.InviteAgent( req, strIAUrl, ref reqCtx );
			
			// Sign the response
			SoapContext respCtx = HttpSoapContext.ResponseContext;
			this.SignSoapContext( ref respCtx );
			
			return resp.ToXml();
		}

		
		[WebMethod][SoapRpcMethod]
		public string SignMeetingRequest( string strMeetingRequest )
		{
			SoapContext reqCtx = HttpSoapContext.RequestContext;
			if( reqCtx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
			MeetingRequestMsg req = MeetingRequestMsg.FromXml( strMeetingRequest );
			string strSignedDoc = this.SignMeetingRequest( req, ref reqCtx );
			
			// Sign the response
			SoapContext respCtx = HttpSoapContext.ResponseContext;
			this.SignSoapContext( ref respCtx );
			// Returned signed document
			return strSignedDoc;
		}

		[WebMethod][SoapRpcMethod]
		public bool CreateMeeting( string strMeetingID, string strTopic )
		{
			// Accept only SOAP requests
			SoapContext reqCtx = HttpSoapContext.RequestContext;
			if( reqCtx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
			if( strMeetingID == null || strMeetingID.Length == 0 )
				throw new SoapException( "Invalid meeting ID", SoapException.ClientFaultCode );
			
			if( strTopic == null || strTopic.Length == 0 )
				throw new SoapException( "Invalid meeting topic", SoapException.ClientFaultCode );

			// Process the request to ensure the SOAP message
			// was signed etc. and return the certificate of the signer
			X509Certificate senderCert = ProcessRequest( ref reqCtx );

			// Only someone with "my" certificate can direct "me" to create
			// a meeting
			if( !senderCert.Equals( this.m_signingCert ) )
				throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );			
			
			// Sign the response
			SoapContext respCtx = HttpSoapContext.ResponseContext;
			this.SignSoapContext( ref respCtx );
			
			// Create a new meeting and add self as organizer
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			if( !mtgDAO.IsNewMeeting( strMeetingID ) )
				return true;
			
			bool bRetVal = mtgDAO.CreateNewMeeting( strMeetingID, strTopic );
			if( !bRetVal )
				throw new SoapException( "Error creating meeting", SoapException.ServerFaultCode );
			
			// Add self as contact if not in registry
			ContactDAO ctcDAO = new ContactDAO( this.DBConnect );
			ctcDAO.AddContact( me.Name, 1.0 );
			ctcDAO.AddContactCertificate( me.Name, this.SigningCert );

			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			// Record self as organizer for the meeting I create
			me.Role = enuMeetingParticipantRole.Organizer;
			participDAO.AddMeetingParticipant( strMeetingID, me );
			// Revert to participant role
			me.Role = enuMeetingParticipantRole.Participant;
			
			return bRetVal;
		}

		// Private support methods

		private string SignMeetingRequest( MeetingRequestMsg req, ref SoapContext reqCtx )
		{
			X509Certificate senderCert = ProcessRequest( ref reqCtx );
			
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			// If the meeting is new, then we are not a partcipant thus we
			// refuse to sign the meeting request
			if( mtgDAO.IsNewMeeting( req.MeetingID ) )
				throw new SoapException( "Signing request refused. Reason: IA not participating in this meeting", SoapException.ClientFaultCode );

			// If the meeting has ended, there is no reason to sign a meeting
			// request. Can't invite someone into an ended meeting.
			if( mtgDAO.GetMeetingState( req.MeetingID ) == enuMeetingState.Ended )
				throw new SoapException( "Signing request refused. Reason: Meeting has ended", SoapException.ClientFaultCode );

			// Only the meeting organizer can request a participant to sign a 
			// meeting request
			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			MeetingParticipant organizer = participDAO.GetOrganizer( req.MeetingID );
			if( organizer == null || organizer.Name != senderCert.GetName() )
				throw new SoapException( "Signing request refused. Only the meeting organizer can request participants to sign meeting requests", SoapException.ClientFaultCode );
			
			// Get the entire document and hash this
			byte[] arrData = Encoding.Unicode.GetBytes( req.ToXml() );
			
			HashAlgorithm hashAlg = new SHA512Managed();
			byte[] arrHashedValue = hashAlg.ComputeHash( arrData );
			// Convert hash to base64 string
			string strHashedValue = Convert.ToBase64String( arrHashedValue );
			
			// Create data document to be signed
			XmlDocument voucherDoc = new XmlDocument();
			XmlElement rootElement = voucherDoc.CreateElement( VOUCHER_TAG );
			XmlNode root = voucherDoc.ImportNode( rootElement, true );
			voucherDoc.AppendChild( root );

			// Create CDATA section to store hash as base64 string, 
			// using CDATA section since we have no control over what chars
			// will be in base64 string representation of hash
			XmlCDataSection hashData = voucherDoc.CreateCDataSection( strHashedValue );
			voucherDoc.ImportNode( hashData, true );
			voucherDoc.DocumentElement.AppendChild( hashData );

			// Check document structure
			string strData = voucherDoc.OuterXml;
			
			// Sign base64 string and return, sign using your Signing key
			// pull from certificate store
			SignedXml signedXml = new SignedXml();			
			System.Security.Cryptography.Xml.DataObject voucherInfo = new System.Security.Cryptography.Xml.DataObject();
			voucherInfo.Data = voucherDoc.ChildNodes;
			voucherInfo.Id = VOUCHER_TAG;
			signedXml.AddObject( voucherInfo );

			Reference reference = new Reference();
			reference.Uri = VOUCHER_REF;
			signedXml.AddReference( reference );

			// Once we have a voucher document we need to sign it and 
			// return this entire thing as the result
			
			// Get signing cert
			X509Certificate signingCert = this.m_signingCert;
			// Use x509 cert here
			signedXml.SigningKey = signingCert.Key;
			// Create an X509 data clause from this cert
			System.Security.Cryptography.Xml.KeyInfoX509Data x509DataClause = new System.Security.Cryptography.Xml.KeyInfoX509Data( signingCert );
			
			System.Security.Cryptography.Xml.KeyInfo keyInfo = new System.Security.Cryptography.Xml.KeyInfo();
			keyInfo.Id = signingCert.GetName();
			keyInfo.AddClause( new System.Security.Cryptography.Xml.RSAKeyValue( signingCert.PublicKey ) ); 
			// Add the x509 cert
			keyInfo.AddClause( x509DataClause );
			
			signedXml.KeyInfo = keyInfo;
						
			// Compute signature
			signedXml.ComputeSignature();
			
			return signedXml.GetXml().OuterXml;
		}

		private void SignSoapContext( ref SoapContext ctx )
		{
			X509SecurityToken tok = new X509SecurityToken( this.m_signingCert );
			ctx.Security.Tokens.Add( tok );
			// Create a signature element from the token
			Signature sig = new Signature( tok );
			// Sign the mesage body
			sig.SignatureOptions = SignatureOptions.IncludeSoapBody;
			// Add digital signature
			ctx.Security.Elements.Add( sig );
		}

		private string GetIPAddress()
		{
			// Get the IP Host entry
			IPHostEntry entry = Dns.GetHostByName( Dns.GetHostName() );
			// Determine whether we were able to get a list of addresses for this host
			if( entry.AddressList.Length == 0 )
				throw new Exception( "Unable to get IP Address of host" );
			// Create a new IP Address 
			IPAddress addr = new IPAddress( entry.AddressList[0].Address );
			return addr.ToString();
		}

		private Hashtable ProcessVouchers( MeetingRequestMsg req, string strVouchers )
		{
			Hashtable results = new Hashtable();
			
			if( strVouchers == null || strVouchers.Length == 0 )
				return results;
			
			HashAlgorithm hashAlg = new SHA512Managed();
			// Get bytes to hash over
			byte[] arrByteData = Encoding.Unicode.GetBytes( req.ToXml() );
			// Hash the bytes
			byte[] arrHashedValue = hashAlg.ComputeHash( arrByteData );
			// Convert hash to base64 string
			string strHashedValue = Convert.ToBase64String( arrHashedValue );

			// Load xml document of vouchers (signature document)
			XmlDocument voucherDoc = new XmlDocument();
			voucherDoc.LoadXml( strVouchers );
			XmlNodeList lstSignatures = voucherDoc.GetElementsByTagName( SIGNATURE_TAG );
			
			// For each signature returned...
			foreach( XmlElement signatureElement in lstSignatures )
			{
				// Verify the signature using KeyInfo data

				// Grab KeyInfo tag
				XmlNode keyInfoNode = signatureElement[KEYINFO_TAG];
				// Get the Id attribute
				string strKeyId = keyInfoNode.Attributes[ID_TAG].Value;

				// Get the X509 certificate of the signer
				SignedXml signedXml = new SignedXml();
				// Load the signature
				signedXml.LoadXml( signatureElement );
				// Verify the signature
				bool bVerified = signedXml.CheckSignature();

				if( !bVerified )
					continue; // Process next signature
			
				System.Security.Cryptography.Xml.KeyInfo keyInfo = signedXml.KeyInfo;
				IEnumerator it = keyInfo.GetEnumerator();
				while( it.MoveNext() )
				{
					object obj = it.Current;
					System.Security.Cryptography.Xml.KeyInfoX509Data x509Data = null;
					
					if( !( obj is System.Security.Cryptography.Xml.KeyInfoX509Data ) )
						continue; // Move on to next clause

					x509Data = (System.Security.Cryptography.Xml.KeyInfoX509Data) obj;
					// Get the cert
					X509Certificate cert = (X509Certificate) x509Data.Certificates[0];
					// Add the contact ID and the cert
					results.Add( cert.GetName(), cert );
				}
			}
			return results;
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion
		
		private X509Certificate ProcessRequest( ref SoapContext reqCtx )
		{
			// Get sender certificate
			if( reqCtx.Security.Elements.Count != 1 )
				throw new SoapException( "Expected a single security element", SoapException.ClientFaultCode );
			
			object objElement = reqCtx.Security.Elements[0];
			if( !(objElement is Microsoft.Web.Services.Security.Signature ) )
				throw new SoapException( "Expected a digital signature element", SoapException.ClientFaultCode );

			Microsoft.Web.Services.Security.Signature signature = objElement as Microsoft.Web.Services.Security.Signature;
			if( signature.SignatureOptions != SignatureOptions.IncludeSoapBody )
				throw new SoapException( "Expected signed SOAP message body", SoapException.ClientFaultCode );

			if( !signature.CheckSignature() )
				throw new SoapException( "Error verifying digital signature", SoapException.ClientFaultCode );

			X509SecurityToken tok = (X509SecurityToken) signature.SecurityToken;
			X509Certificate senderCert = tok.Certificate;
			
			return senderCert;
		}

		private MeetingResponseMsg JoinMeeting( MeetingRequestMsg req, string strVouchers, ref SoapContext reqCtx )
		{
			// Process the request to ensure the SOAP message
			// was signed etc. and return the certificate of the signer
			X509Certificate senderCert = ProcessRequest( ref reqCtx );

			MeetingResponseMsg resp = new MeetingResponseMsg();
			resp.Value = enuMeetingResponseValue.Accept;
			
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			if( !mtgDAO.IsNewMeeting( req.MeetingID ) )
				return resp;
			
			// Create this meeting in the database
			mtgDAO.CreateNewMeeting( req.MeetingID, req.MeetingTopic );

			Hashtable results = this.ProcessVouchers( req, strVouchers );

			// Add sender to contacts list
			ContactDAO ctcDAO = new ContactDAO( this.DBConnect );
			if( !ctcDAO.IsContactInRegistry( senderCert.GetName() ) )
				ctcDAO.AddContact( senderCert.GetName() );
						
			if( !ctcDAO.IsContactCertificateInStore( senderCert.GetName() ) )
				ctcDAO.AddContactCertificate( senderCert.GetName(), senderCert );
				
			// Note the organizer for the meeting
			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			IEnumerator it = req.m_lstParticipants.GetEnumerator();
			while( it.MoveNext() )
			{
				// For each participant...
				MeetingParticipant p = (MeetingParticipant) it.Current;			
				// Add as a contact
				if( !ctcDAO.IsContactInRegistry( p.Name ) )
					ctcDAO.AddContact( p.Name );
                // Add certficate
				X509Certificate participantCert = (X509Certificate) results[p.Name];
				if( participantCert != null )
				{	
					if( !ctcDAO.IsContactCertificateInStore( p.Name ) )
						ctcDAO.AddContactCertificate( p.Name, participantCert );
				}

				// Add as Meeting Participant
				participDAO.AddMeetingParticipant( req.MeetingID, p );
			}
			
			// Add self to contacts and as meeting participant			
			if( !ctcDAO.IsContactInRegistry( this.SigningKeyName ) )
				ctcDAO.AddContact( this.SigningKeyName, 1.0 );
			
			if( !participDAO.IsInMeeting( req.MeetingID, me ) )
				participDAO.AddMeetingParticipant( req.MeetingID, me );

			// Generate proxies to the organizer and participants
			// Do this operation using Async

			// Update contact cache 
			// Do this operation using Async
			
			return resp;
		}
		
		private MeetingResponseMsg InviteAgent( MeetingRequestMsg mtgReq, string strIAUrl, ref SoapContext reqCtx )
		{
			// Process the request to ensure the SOAP message
			// was signed etc. and return the certificate of the signer
			X509Certificate senderCert = ProcessRequest( ref reqCtx );

			// Only someone with "my" certificate can direct "me" to invite
			// another information agent
			if( !senderCert.Equals( this.m_signingCert ) )
				throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );

			// Only send out invitations for meetings we are participating in
			
			// IA Urls are unique every person has a single IA (per meeting), 
			// we can check whether this IA Url belongs to a contact already in the meeting
			// if it does then we can exit quickly (short circuit);
			// NOTE: We must convert the IA Urls to IP Addresses and then
			//       check the database
			Uri uri = new Uri( strIAUrl );
			IPHostEntry entry = Dns.Resolve( uri.Host );
			IPAddress addr = new IPAddress( entry.AddressList[0].Address );
			string strIP = addr.ToString();
			strIAUrl = uri.Scheme + Uri.SchemeDelimiter + strIP + uri.PathAndQuery;
		
			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			if( participDAO.IsInfoAgentInMeeting( mtgReq.MeetingID, strIAUrl ) )
			{
				MeetingResponseMsg response = new MeetingResponseMsg();
				response.MeetingID = mtgReq.MeetingID;
				response.Value = enuMeetingResponseValue.Accept;
				response.Sender = participDAO.GetInfoAgent( mtgReq.MeetingID, strIAUrl).Name;
				response.SenderUrl = strIAUrl;
				return response;
			}

			// When we are asked to invite an IA we may not "know" them (know = have their certificate or their
			// contact info in our registry). This will be determined for so after the invitee responds
			// For the time being assign the invitee a temporary contact ID
			string strContactID = Guid.NewGuid().ToString();
	
			ContactCacheDAO ctcCacheDAO = new ContactCacheDAO( this.DBConnect );
			Hashtable infoAgents = new Hashtable();
			Hashtable proxiesToGen = new Hashtable();
			proxiesToGen.Add( strContactID, strIAUrl );
			
			// Get the list of the participants (Do dbase queries Async)
			IEnumerator participIt = mtgReq.m_lstParticipants.GetEnumerator();
			while( participIt.MoveNext() )
			{
				// For each participant find out which ones we need to generate
				// proxies for. 
				MeetingParticipant p = (MeetingParticipant) participIt.Current;
				// Skip the sender "me"
				if( p.Name == this.SigningCert.GetName() )
					continue;
			
				// If the IA is not in the contact cache then we must generate
				// a proxy for it, otherwise add it to the list of infoAgents
				// this will be used later to get them to sign
				string strLoc = ctcCacheDAO.GetContactLocation( p.Name );
				if( strLoc == null || strLoc.Length == 0 )
					proxiesToGen.Add( p.Name, p.Location );
				else infoAgents.Add( p.Name, strLoc );
			}
			
			// Generate the IA proxies necessary (Do generation Async)
			IDictionaryEnumerator proxyIt = proxiesToGen.GetEnumerator();
			while( proxyIt.MoveNext() )
			{
				ProxyGenRequest pxyGenReq = new ProxyGenRequest();
				pxyGenReq.ProxyPath = this.ProxyCache;
				pxyGenReq.ServiceName = INFO_AGENT;
				pxyGenReq.WsdlUrl = (string) proxyIt.Value;
				string strAssembly = (string) this.GenerateWebServiceProxy( pxyGenReq );
				if( strAssembly != null || strAssembly.Length != 0 )
					infoAgents.Add( proxyIt.Key, strAssembly );
			}

            XmlDocument voucherDoc = new XmlDocument();
			XmlElement root = voucherDoc.CreateElement( VOUCHERS_TAG );
			voucherDoc.AppendChild( root );
			
			// Do invocation for getting participants to sign
			// except the meeting organizer, organizer will sign the
			// SOAP message
			IDictionaryEnumerator agentIt = infoAgents.GetEnumerator();
			while( agentIt.MoveNext() )
			{
				// Exclude the invitee
				if( (string) agentIt.Key == strContactID )
					continue;
				
				// Create an execution context
				ExecContext execCtx = new ExecContext();
				// Get the assembly
				execCtx.Assembly = (string) agentIt.Value;
				// Add the parameters...in this case the meeting request to sign
				execCtx.AddParameter( Serializer.Serialize( mtgReq.ToXml() ) );
				// Name of method to execute on the remote agent
				execCtx.MethodName = "SignMeetingRequest";
				// Fully qualified name of proxy class
				execCtx.ServiceName = ProxyGenRequest.DEFAULT_PROXY_NAMESPACE + "." + INFO_AGENT;
				// Create an executor to do invocation
				Executor exec = new Executor();
				// Create a settings instance
				ExecutorSettings settings = new ExecutorSettings();
				// Expect a signed response
				settings.ExpectSignedResponse = true;
				// Set the certificate to use to sign the outgoing message
				settings.SigningCertificate = this.SigningCert;
				// Set the executor settings instance
				exec.Settings = settings;
				// Do invocation
				object objRes = exec.Execute( execCtx );

				string strSignature = (string) Serializer.DeSerialize( (string) objRes, typeof(string) );
				if( strSignature.Length == 0 )
					continue;

				XmlDocument sigDoc = new XmlDocument();
				sigDoc.LoadXml( strSignature );
				XmlNode sigNode = voucherDoc.ImportNode( sigDoc.DocumentElement, true );
				root.AppendChild( sigNode );
			}
			
			// Do invocation for JoinMeeting
			ExecContext joinCtx = new ExecContext();
			joinCtx.MethodName = "JoinMeeting";
			joinCtx.ServiceName = ProxyGenRequest.DEFAULT_PROXY_NAMESPACE + "." + INFO_AGENT;
			joinCtx.Assembly = (string) infoAgents[strContactID];
			joinCtx.AddParameter( Serializer.Serialize( mtgReq.ToXml() ) );
			joinCtx.AddParameter( Serializer.Serialize( voucherDoc.OuterXml ) );
			ExecutorSettings joinSettings = new ExecutorSettings();
			joinSettings.ExpectSignedResponse = true;
			joinSettings.SigningCertificate = this.SigningCert;
			Executor joinExec = new Executor( joinSettings );
			string strResp = (string) Serializer.DeSerialize( (string) joinExec.Execute( joinCtx ), typeof(string) );

			MeetingResponseMsg resp = MeetingResponseMsg.FromXml( strResp );
			
			// Update contact cache data for all contacts except the new invitee
			proxyIt.Reset();
			while( proxyIt.MoveNext() )
			{
				if( (string) proxyIt.Key == strContactID )
					continue;

				ctcCacheDAO.AddContactLocation( (string) proxyIt.Key, (string) infoAgents[proxyIt.Key] );
			}

			// If the invitee accepted then add them as a meeting participant
			if( resp.Value == enuMeetingResponseValue.Accept )
			{
				MeetingParticipant invitee = new MeetingParticipant();
				invitee.Location = resp.SenderUrl;
				invitee.Name = joinSettings.ResponseCertificate.GetName();
				invitee.Role = enuMeetingParticipantRole.Participant;
				
				// Add contact data for invitee - invitee true name is on the response certificate
				ContactDAO ctcDAO = new ContactDAO( this.DBConnect );
				// Add invitee to contact registry
				ctcDAO.AddContact( invitee.Name );
				// Add invitee cert to cert store
				ctcDAO.AddContactCertificate( invitee.Name, joinSettings.ResponseCertificate );
				
				// Add invitee data to contact cache
				string strContactLoc = ctcCacheDAO.GetContactLocation( invitee.Name );
				if( strContactLoc == null || strContactLoc.Length == 0 )
					ctcCacheDAO.AddContactLocation( invitee.Name, (string) infoAgents[strContactID]  );
				else ctcCacheDAO.UpdateContactLocation( invitee.Name, (string) infoAgents[strContactID] );
				
				// Add invitee as a meeting participant				
				participDAO.AddMeetingParticipant( mtgReq.MeetingID, invitee );
			}

			return resp;
		}

		public object GenerateWebServiceProxy( object objCtx )
		{
			ProxyGenRequest pxyGenReq = objCtx as ProxyGenRequest;
			if( pxyGenReq == null )
				throw new ArgumentException( "Invalid proxy generation request" );

			ProxyPolicyMutator mutator = new ProxyPolicyMutator();
			mutator.ProxyName = pxyGenReq.ServiceName;
			
			// Ensure the name of the file generated is unique
			string strSuffix = "";
			int nCode = Guid.NewGuid().GetHashCode();
			if( nCode < 0 )
				nCode = nCode * -1;
			strSuffix = nCode.ToString();
			pxyGenReq.ServiceName = pxyGenReq.ServiceName + "_" + strSuffix;

			ProxyGen pxyGen = new ProxyGen();
			pxyGen.Mutator = mutator;

			return pxyGen.GenerateAssembly( pxyGenReq );
		}
				
		/*
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void SendContextUpdate( ContextMsg ctxMsg, string strContactID )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void ContextUpdate( ContextMsg ctxMsg )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void RequestRecommendation( RecommendationRequestMsg recReq )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void Recommend( ResourceMsg resMsg )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void AddResources( ResourceMsg resMsg )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void RecallResources( ResourceMsg resMsg )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]//[SoapRpcMethod(OneWay=true)]
		public void SendResources( ResourceMsg resMsg )
		{
			// Accept only SOAP requests
			SoapContext ctx = HttpSoapContext.RequestContext;
			if( ctx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
		}
		*/
	}
}
