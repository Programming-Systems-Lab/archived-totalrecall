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
using PSL.TotalRecall.PolicyManager;
namespace TotalRecall
{
	public class Context:IContext
	{
		private Hashtable m_participants = new Hashtable();
		private string m_strTopic = "";

		public Context()
		{}
			
		public Hashtable Participants
		{ 
			get
			{ return this.m_participants; }
		}

		public string Topic
		{
			get
			{ return this.m_strTopic; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strTopic = value;
			}
		}

		public void AddParticipant( string strContactID )
		{
			if( strContactID == null || strContactID.Length == 0 )
				return;

			this.m_participants.Add( strContactID, strContactID );
		}

		public void ClearParticipants()
		{
			this.m_participants.Clear();
		}
	}
	
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
		private string m_strProxyCache = "PxyCache";
		private string m_strCurrentDir = "";

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
		
		public string CurrentDir
		{
			get
			{ return this.m_strCurrentDir; }
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
			this.m_strCurrentDir = fInfo.DirectoryName;
			
			// Set the signing key name from the config file
			this.SigningKeyName = ConfigurationSettings.AppSettings["SigningKeyName"];
			// Set the proxy cache location
			this.ProxyCache = ConfigurationSettings.AppSettings["ProxyCache"];
			// Set the database connection string
			this.DBConnect = ConfigurationSettings.AppSettings["DatabaseConnectionString"];
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
			// Initialze proxy cache
			this.InitializeProxyCacheDir();
		}
		
		[WebMethod][SoapRpcMethod]
		public string JoinMeeting( string strMeetingRequest, string strVouchers )
		{
			SoapContext reqCtx = HttpSoapContext.RequestContext;
			if( reqCtx == null )
				throw new ApplicationException( "Non-SOAP message!!!" );
			
			// Process the request to ensure the SOAP message
			// was signed etc. and return the certificate of the signer
			X509Certificate senderCert = ProcessRequest( ref reqCtx );

			MeetingRequestMsg req = null;

			try
			{
				req = MeetingRequestMsg.FromXml( strMeetingRequest );
			}
			catch( Exception /*e*/ )
			{
				throw new SoapException( "Invalid meeting request", SoapException.ClientFaultCode );
			}
			
			MeetingResponseMsg resp = this.JoinMeeting( req, strVouchers, senderCert );
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
			
			// Process the request to ensure the SOAP message
			// was signed etc. and return the certificate of the signer
			X509Certificate senderCert = ProcessRequest( ref reqCtx );

			// Only someone with "my" certificate can direct "me" to invite
			// another information agent
			if( !senderCert.Equals( this.m_signingCert ) )
				throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );

			MeetingRequestMsg req = null;
			
			try
			{	
				req = MeetingRequestMsg.FromXml( strMeetingRequest );
			}
			catch( Exception /*e*/ )
			{
				throw new SoapException( "Invalid meeting request", SoapException.ClientFaultCode );
			}
			
			MeetingResponseMsg resp = this.InviteAgent( req, strIAUrl );
			
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
			
			X509Certificate senderCert = ProcessRequest( ref reqCtx );

			MeetingRequestMsg req = null;

			try
			{
				req = MeetingRequestMsg.FromXml( strMeetingRequest );
			}
			catch( Exception /*e*/ )
			{
				throw new SoapException( "Invalid meeting request", SoapException.ClientFaultCode );
			}

			// Only the meeting organizer can request a participant to sign a 
			// meeting request
			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			MeetingParticipant organizer = participDAO.GetOrganizer( req.MeetingID );
			if( organizer == null || organizer.Name != senderCert.GetName() )
				throw new SoapException( "Signing request refused. Only the meeting organizer can request participants to sign meeting requests", SoapException.ClientFaultCode );
						
			string strSignedDoc = this.SignMeetingRequest( req );

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

		
		// One way methods, must handle all exceptions since they return nothing
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void InfoAgentContextUpdate( string strIACtxMsg )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}
				
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strIACtxMsg == null || strIACtxMsg.Length == 0 )
					throw new SoapException( "Invalid Info Agent Context Message", SoapException.ClientFaultCode );
				
				// Process the request to ensure the SOAP message
				// was signed etc. and return the certificate of the signer
				X509Certificate senderCert = ProcessRequest( ref reqCtx );
			
				InfoAgentCtxMsg iaCtxMsg = null;
				try
				{	
					iaCtxMsg = InfoAgentCtxMsg.FromXml( strIACtxMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Info Agent Context Msg", SoapException.ClientFaultCode );
				}

				ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
				// Only the meeting organizer can send these context messages
				if( senderCert.GetName() != participDAO.GetOrganizer( iaCtxMsg.MeetingID ).Name )
					throw new SoapException( "Unauthorized operation, only the Meeting Organizer can send IA Agent Joined/Left context messages", SoapException.ClientFaultCode );

				InfoAgentContextUpdate( iaCtxMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void MeetingContextUpdate( string strMtgCtxMsg )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}

				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strMtgCtxMsg == null || strMtgCtxMsg.Length == 0 )
					throw new SoapException( "Invalid Meeting Status Context Message", SoapException.ClientFaultCode );
				
				// Process the request to ensure the SOAP message
				// was signed etc. and return the certificate of the signer
				X509Certificate senderCert = ProcessRequest( ref reqCtx );
			
				MeetingStatusCtxMsg mtgCtxMsg = null;
				try
				{	
					mtgCtxMsg = MeetingStatusCtxMsg.FromXml( strMtgCtxMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Meeting Status Context Msg", SoapException.ClientFaultCode );
				}

				ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
				// Only the meeting organizer can send these context messages
				if( senderCert.GetName() != participDAO.GetOrganizer( mtgCtxMsg.MeetingID ).Name )
					throw new SoapException( "Unauthorized operation, only the Meeting Organizer can send IA Agent Joined/Left context messages", SoapException.ClientFaultCode );

				MeetingContextUpdate( mtgCtxMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void ContextUpdate( string strCtxResponse )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strCtxResponse == null || strCtxResponse.Length == 0 )
					throw new SoapException( "Invalid Context Message Response", SoapException.ClientFaultCode );
				
				// Process the request to ensure the SOAP message
				// was signed etc. and return the certificate of the signer
				X509Certificate senderCert = ProcessRequest( ref reqCtx );
			
				ContextMsgResponse respMsg = null;
				try
				{	
					respMsg = ContextMsgResponse.FromXml( strCtxResponse );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Context Message Response", SoapException.ClientFaultCode );
				}

				// Anyone can send a response, just save it
				ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
				ctxMsgDAO.ReceiveContextMessage( respMsg, false );
				// Process context message response
				ProcessContextMessageResponse( respMsg, senderCert );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}
		
		private void ProcessContextMessageResponse( ContextMsgResponse respMsg, X509Certificate senderCert )
		{
			// If the response is of type resource recalled then
			// get the context messages sent that matches
			// recreate the message sent, scroll thru the list of
			// resource ids recalled and update our database to reflect that
			if( respMsg.Type == enuContextMsgType.ResourceRecalledResponse )
			{
				ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
				ContextMsg ctxMsg = ctxMsgDAO.GetContextMsgSent( respMsg.MeetingID, respMsg.MessageID, respMsg.Sender );
				if( ctxMsg == null )
					return;

				if( ctxMsg is ResourceCtxMsg && ctxMsg.Type == enuContextMsgType.ResourceRecalled )
				{
					ResourceDAO resDAO = new ResourceDAO( this.DBConnect );
					ResourceCtxMsg resCtxMsg = (ResourceCtxMsg) ctxMsg;
					IEnumerator it = resCtxMsg.ResourceIDs.GetEnumerator();
					while( it.MoveNext() )
					{
						resDAO.RecallResource( (string) it.Current );
					}
				}
			}
		}

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void SendContextUpdate( string strCtxResponse, string strContactID, string strIAUrl )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}

				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strCtxResponse == null || strCtxResponse.Length == 0 )
					throw new SoapException( "Invalid Context Message Response", SoapException.ClientFaultCode );
				if( strContactID == null || strContactID.Length == 0 )
					throw new SoapException( "Invalid contact ID", SoapException.ClientFaultCode );
				if( strIAUrl == null || strIAUrl.Length == 0 )
					throw new SoapException( "Invalid Info Agent Url", SoapException.ClientFaultCode );
				
				X509Certificate senderCert = this.ProcessRequest( ref reqCtx );
				// Only someone with "my" certificate can direct "me" to send 
				// a context update
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );			
				
				ContextMsgResponse respMsg = null;
				try
				{	
					respMsg = ContextMsgResponse.FromXml( strCtxResponse );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Context Message Response", SoapException.ClientFaultCode );
				}
				
				SendContextUpdate( respMsg, strContactID, strIAUrl );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void SendInfoAgentContextUpdate( string strIACtxMsg, string strContactID, string strIAUrl )
		{
			try
			{
				// Accept only SOAP requests
				// NOTE: Since we are a one-way method, we won't have access to the
				//		 static HttpSoapContext.RequestContext method hence this property
				//		 will always be null. We need to try get the Request SOAP Context
				//		 another way - from the raw Http request stream. Once the
				//		 request stream contains a SOAP envelope we can reconstruct it
				//		 and try to put the security items (tokens, signatures etc.)
				//		 where they should be in the Request SOAP context
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}
				
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
						
				if( strIACtxMsg == null || strIACtxMsg.Length == 0 )
					throw new SoapException( "Invalid Info Agent Context Message", SoapException.ClientFaultCode );
				if( strContactID == null || strContactID.Length == 0 )
					throw new SoapException( "Invalid contact ID", SoapException.ClientFaultCode );
				if( strIAUrl == null || strIAUrl.Length == 0 )
					throw new SoapException( "Invalid Info Agent Url", SoapException.ClientFaultCode );
				
				X509Certificate senderCert = this.ProcessRequest( ref reqCtx );
				// Only someone with "my" certificate can direct "me" to send
				// an info agent context update
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );			
				
				InfoAgentCtxMsg iaCtxMsg = null;
				try
				{	
					iaCtxMsg = InfoAgentCtxMsg.FromXml( strIACtxMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Info Agent Context Msg", SoapException.ClientFaultCode );
				}
				
				SendContextUpdate( iaCtxMsg, strContactID, strIAUrl );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void SendMeetingContextUpdate( string strMtgCtxMsg, string strContactID, string strIAUrl )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}			
				
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strMtgCtxMsg == null || strMtgCtxMsg.Length == 0 )
					throw new SoapException( "Meeting Status Context Message", SoapException.ClientFaultCode );
				if( strContactID == null || strContactID.Length == 0 )
					throw new SoapException( "Invalid contact ID", SoapException.ClientFaultCode );
				if( strIAUrl == null || strIAUrl.Length == 0 )
					throw new SoapException( "Invalid Info Agent Url", SoapException.ClientFaultCode );
				
				X509Certificate senderCert = this.ProcessRequest( ref reqCtx );
				// Only someone with "my" certificate can direct "me" to send
				// a meeting context update
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );			
				
				MeetingStatusCtxMsg mtgCtxMsg = null;
				try
				{	
					mtgCtxMsg = MeetingStatusCtxMsg.FromXml( strMtgCtxMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Meeting Status Context Message", SoapException.ClientFaultCode );
				}
				
				SendContextUpdate( mtgCtxMsg, strContactID, strIAUrl );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}
		
		
		// Private support methods

		private string SignMeetingRequest( MeetingRequestMsg req )
		{
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			// If the meeting is new, then we are not a partcipant thus we
			// refuse to sign the meeting request
			if( mtgDAO.IsNewMeeting( req.MeetingID ) )
				throw new SoapException( "Signing request refused. Reason: IA not participating in this meeting", SoapException.ClientFaultCode );

			// If the meeting has ended, there is no reason to sign a meeting
			// request. Can't invite someone into an ended meeting.
			if( mtgDAO.GetMeetingState( req.MeetingID ) == enuMeetingState.Ended )
				throw new SoapException( "Signing request refused. Reason: Meeting has ended", SoapException.ClientFaultCode );

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
					continue; // Process next signature (ignore invalid signatures)
			
				// Validate meeting request hash
				XmlNode voucherNode = signatureElement["Object"].FirstChild;
				string strVoucherData = voucherNode.InnerText;
				// If meeting request hash differs from content of voucher then
				// ignore this signature
				if( strVoucherData != strHashedValue )
					continue;
				
				System.Security.Cryptography.Xml.KeyInfo keyInfo = signedXml.KeyInfo;
				IEnumerator it = keyInfo.GetEnumerator();
				while( it.MoveNext() )
				{
					object obj = it.Current;
					System.Security.Cryptography.Xml.KeyInfoX509Data x509Data = null;
					
					if( !( obj is System.Security.Cryptography.Xml.KeyInfoX509Data ) )
						continue; // Move on to next clause

					x509Data = (System.Security.Cryptography.Xml.KeyInfoX509Data) obj;
					// Get the System.Security... version of the cert
					System.Security.Cryptography.X509Certificates.X509Certificate ssCert = (System.Security.Cryptography.X509Certificates.X509Certificate) x509Data.Certificates[0];
					// Build a Microsoft.Web.Services.Security... version of the cert from the raw ssCert data
					X509Certificate cert = new X509Certificate( ssCert.GetRawCertData() );
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

		private MeetingResponseMsg JoinMeeting( MeetingRequestMsg req, string strVouchers, X509Certificate senderCert )
		{
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
		
			return resp;
		}
		
		private MeetingResponseMsg InviteAgent( MeetingRequestMsg mtgReq, string strIAUrl )
		{
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

				InfoAgentCtxMsg iaMsg = new InfoAgentCtxMsg();
				iaMsg.Certificate = joinSettings.ResponseCertificate;
				iaMsg.MeetingID = mtgReq.MeetingID;
				iaMsg.ContactID = invitee.Name;
				iaMsg.InfoAgentUrl = invitee.Location;
				iaMsg.Sender = me.Name;
				iaMsg.SenderUrl = me.Location;
				iaMsg.Type = enuContextMsgType.InfoAgentJoined;

				ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );

				// Get the list of participants and send an InfoAgentCtxMsg
				// to let them know an IA has joined the meeting
				participIt.Reset();
				while( participIt.MoveNext() )
				{
					MeetingParticipant p = (MeetingParticipant) participIt.Current;
					// Skip the sender "me"
					if( p.Name == this.SigningCert.GetName() )
						continue;
					
					// Set the destination for the IA Ctx Msg
					iaMsg.Dest = p.Name;
					iaMsg.DestUrl = p.Location;
					// Save the message we are about to send
					ctxMsgDAO.SendContextMessage( iaMsg );		
					this.SendContextUpdate( iaMsg, p.Name, p.Location );
				}
			}

			return resp;
		}

		private object GenerateWebServiceProxy( object objCtx )
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
				
		private void InfoAgentContextUpdate( InfoAgentCtxMsg iaCtxMsg )
		{
			// Ignore any messages for meetings we are not participating in.
			// We don't check the meeting status in case we want to resume an
			// ended meeting at a later date
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			if( mtgDAO.IsNewMeeting( iaCtxMsg.MeetingID ) )
				return;

			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			// Create a context message response
			ContextMsgResponse ctxResp = new ContextMsgResponse();
			ctxResp.MeetingID = iaCtxMsg.MeetingID;
			ctxResp.MessageID = iaCtxMsg.MessageID;
			ctxResp.Ack = true;
			ctxResp.Sender = me.Name;
			ctxResp.SenderUrl = me.Location;
			// IA Ctx msgs either IA joined or IA left
			if( iaCtxMsg.Type == enuContextMsgType.InfoAgentJoined )
			{
				// Set the type of the response
				ctxResp.Type = enuContextMsgType.InfoAgentJoinedResponse;
				// Add IA as a contact and add their certificate if necessary
				ContactDAO ctcDAO = new ContactDAO( this.DBConnect );
				ctcDAO.AddContact( iaCtxMsg.ContactID );
				ctcDAO.AddContactCertificate( iaCtxMsg.ContactID, iaCtxMsg.Certificate );
				// Add IA as a meeting participant
				MeetingParticipant particip = new MeetingParticipant();
				particip.Name = iaCtxMsg.ContactID;
				particip.Location = iaCtxMsg.InfoAgentUrl;
				particip.Role = enuMeetingParticipantRole.Participant;
				if( !participDAO.IsInfoAgentInMeeting( iaCtxMsg.MeetingID, iaCtxMsg.InfoAgentUrl ) )
					participDAO.AddMeetingParticipant( iaCtxMsg.MeetingID, particip );
				else participDAO.UpdateParticipantRole( iaCtxMsg.MeetingID, iaCtxMsg.ContactID, enuMeetingParticipantRole.Participant );
			}
			else 
			{
				// Set the type of the response
				ctxResp.Type = enuContextMsgType.InfoAgentLeftResponse;
				// Update participant role to inactive
				participDAO.UpdateParticipantRole( iaCtxMsg.MeetingID, iaCtxMsg.ContactID, enuMeetingParticipantRole.Inactive );
			}
			
			// Send the response
			ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
			ctxMsgDAO.ReceiveContextMessage( iaCtxMsg, false );
			// Save the response (ack) if necessary
			ctxMsgDAO.SendContextMessage( ctxResp );
			// Send a response (ack) if necessary
			this.SendContextUpdate( ctxResp, iaCtxMsg.Sender, iaCtxMsg.SenderUrl );
		}

		private void MeetingContextUpdate( MeetingStatusCtxMsg mtgCtxMsg )
		{
			// Ignore any messages for meetings we are not participating in.
			// We don't check the meeting status in case we want to resume an
			// ended meeting at a later date
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			if( mtgDAO.IsNewMeeting( mtgCtxMsg.MeetingID ) )
				return;

			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			// Create a context message response
			ContextMsgResponse ctxResp = new ContextMsgResponse();
			ctxResp.MeetingID = mtgCtxMsg.MeetingID;
			ctxResp.MessageID = mtgCtxMsg.MessageID;
			ctxResp.Ack = true;
			ctxResp.Sender = me.Name;
			ctxResp.SenderUrl = me.Location;
				
			bool bMeetingInactive = false;
			enuMeetingState meetingState = enuMeetingState.Active;
			// Set the type of response
			if( mtgCtxMsg.Type == enuContextMsgType.MeetingEnded )
			{
				ctxResp.Type = enuContextMsgType.MeetingEndedResponse;
				bMeetingInactive = true;
				meetingState = enuMeetingState.Ended;
			}
			else if( mtgCtxMsg.Type == enuContextMsgType.MeetingSuspended )
			{
				ctxResp.Type = enuContextMsgType.MeetingSuspendedResponse;
				bMeetingInactive = true;
				meetingState = enuMeetingState.Suspended;
			}
								
			if( bMeetingInactive )
			{
				// Change the meeting status to ended and mark all participants
				// as inactive (except the organizer)
				mtgDAO.UpdateMeetingState( mtgCtxMsg.MeetingID, meetingState );
					
				ArrayList lstParticipants = participDAO.GetParticipants( mtgCtxMsg.MeetingID );
				IEnumerator it = lstParticipants.GetEnumerator();
				while( it.MoveNext() )
				{
					MeetingParticipant p = (MeetingParticipant) it.Current;
					if( p.Role == enuMeetingParticipantRole.Organizer )
						continue;

					participDAO.UpdateParticipantRole( mtgCtxMsg.MeetingID, p.Name, enuMeetingParticipantRole.Inactive );
				}
			}
			else if( mtgCtxMsg.Type == enuContextMsgType.MeetingResumed )
			{
				// Set the type of response
				ctxResp.Type = enuContextMsgType.MeetingResumedResponse;
				// Change meeting state to active
				mtgDAO.UpdateMeetingState( mtgCtxMsg.MeetingID, meetingState );
				// Change my status to participant
				participDAO.UpdateParticipantRole( mtgCtxMsg.MeetingID, me.Name, enuMeetingParticipantRole.Participant );
			}
			
			// Send response
			ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
			ctxMsgDAO.ReceiveContextMessage( mtgCtxMsg, false );
			// Save the response (ack) if necessary
			ctxMsgDAO.SendContextMessage( ctxResp );
			// Send a response (ack) if necessary
			this.SendContextUpdate( ctxResp, mtgCtxMsg.Sender, mtgCtxMsg.SenderUrl );
		}

		private SoapContext GetSoapContextFromHttpRequest( ref HttpRequest httpReq )
		{
			// Go to the beginning of the input stream
			httpReq.InputStream.Seek( 0, SeekOrigin.Begin );
			// Create a stream reader to pull in the SOAP envelope
			StreamReader sr = new StreamReader( httpReq.InputStream );
			string strSoapEnv = sr.ReadToEnd();
			// Re-construct the SOAP envelope
			SoapEnvelope env = new SoapEnvelope();
			env.LoadXml( strSoapEnv );
			// Now that we have the SOAP envelope we will want to make sure
			// any security tokens, signatures etc. are placed in the 
			// appropriate collections in the RequestSoapContext.
			// To do this, we need to mimic what the WSE pipeline does.
			// When it receives a SOAP envelope it passes it thru a sequence of
			// filters. Each filter may modify the SOAP envelope. We need to
			// Create apply a Security Input filter so that security data gets
			// "put" where it should be in the RequestSoapContext
			Pipeline pipe = new Pipeline();
			pipe.InputFilters.Add( new SecurityInputFilter() );
			pipe.ProcessInputMessage( env );
			// Return the context of the modified SOAP envelope
			return env.Context;
		}

		private void SendContextUpdate( ContextMsg ctxMsg, string strContactID, string strIAUrl )
		{
			string strMethodName = "";
			if( ctxMsg is ContextMsgResponse )
				strMethodName = "ContextUpdate";
			else if( ctxMsg is InfoAgentCtxMsg )
				strMethodName = "InfoAgentContextUpdate";
			else if( ctxMsg is MeetingStatusCtxMsg )
				strMethodName = "MeetingContextUpdate";
			else if( ctxMsg is ContextMsgResponse )
				strMethodName = "ContextUpdate";

			if( strMethodName.Length == 0 )
				throw new Exception( "Unknown/Unexpected context message to send" );

			// Check the contact cache if we have an assembly
			// then load an invoke
			// if we don't have one then generate one then invoke
			ContactCacheDAO ctcCacheDAO = new ContactCacheDAO( this.DBConnect );
			string strAssembly = ctcCacheDAO.GetContactLocation( strContactID );
			if( strAssembly == null || strAssembly.Length == 0 )
			{
				// Generate a web service proxy
				ProxyGenRequest pxyGenReq = new ProxyGenRequest();
				pxyGenReq.ProxyPath = this.ProxyCache;
				pxyGenReq.ServiceName = INFO_AGENT;
				pxyGenReq.WsdlUrl = strIAUrl;
				strAssembly = (string) this.GenerateWebServiceProxy( pxyGenReq );
				// Update contact cache data
				ctcCacheDAO.AddContactLocation( strContactID, strAssembly );
			}

			// Create an execution context
			ExecContext execCtx = new ExecContext();
			// Get the assembly
			execCtx.Assembly = strAssembly;
			// Add the parameters...in this case the context message to sign
			execCtx.AddParameter( Serializer.Serialize( ctxMsg.ToXml() ) );
			// Name of method to execute on the remote agent
			execCtx.MethodName = strMethodName;
			// Fully qualified name of proxy class
			execCtx.ServiceName = ProxyGenRequest.DEFAULT_PROXY_NAMESPACE + "." + INFO_AGENT;
			// Create an executor to do invocation
			Executor exec = new Executor();
			// Create a settings instance
			ExecutorSettings settings = new ExecutorSettings();
			// DO NOT expect a signed response since we are executing a one way method
			settings.ExpectSignedResponse = false;
			// Set the certificate to use to sign the outgoing message
			settings.SigningCertificate = this.SigningCert;
			// Set the executor settings instance
			exec.Settings = settings;
			// Do invocation (expect null/"" returned)
			object objRes = exec.Execute( execCtx );
		}
		
		private bool InitializeProxyCacheDir()
		{
			bool bRetVal = false;
			
			try
			{
				// Determine if directory name of the proxy cache
				// read from the system config file is NOT Fully 
				// Quallified i.e <drive letter>:\<path>
				// If not Fully Qualified then we must prefix
				// with Current Directory
				if( this.ProxyCache.IndexOf( ":" ) == -1 )
				{
					string strPath = this.CurrentDir;
					// Append proxy cache sub dir
					strPath += "\\";
					strPath += this.ProxyCache;
					this.ProxyCache = strPath;
				}
				
				// Try to access DirectoryInfo of ProxyCache
				DirectoryInfo dirInfo = new DirectoryInfo( this.ProxyCache );
				// If the directory does not exist then try creating it
				if( !dirInfo.Exists )
					Directory.CreateDirectory( dirInfo.FullName );
				
				bRetVal = true;
			}
			catch( System.IO.DirectoryNotFoundException /*e*/ )
			{
				Directory.CreateDirectory( this.ProxyCache ); 
				bRetVal = true;
			}
			return bRetVal;
		}


		// Resource related methods

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void RequestRecommendation( string strRecommendationRequest )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}

				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strRecommendationRequest == null || strRecommendationRequest.Length == 0 )
					throw new SoapException( "Invalid Recommendation Request Message", SoapException.ClientFaultCode );
				
				X509Certificate senderCert = this.ProcessRequest( ref reqCtx );
				// Only someone with "my" certificate can direct "me" to request
				// a recommendation
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, recommendation request directive refused", SoapException.ClientFaultCode );			
				
				RecommendationRequestCtxMsg recReqMsg = null;
				try
				{	
					recReqMsg = RecommendationRequestCtxMsg.FromXml( strRecommendationRequest );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Recommendation Request Message", SoapException.ClientFaultCode );
				}
				
				// Fill in the sender info
				recReqMsg.Sender = me.Name;
				recReqMsg.SenderUrl = me.Location;
				RequestRecommendation( recReqMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}		

		private void RequestRecommendation( RecommendationRequestCtxMsg recReqMsg )
		{
			// For the time being just put this message into the database

			// In the actual system we would send msg to our Memento Analysis Agent
			
			// *Send* context message to MAA, currently we stick the msg in our
			// database where a faked MAA will check it
			ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
			bool bRes = ctxMsgDAO.ReceiveContextMessage( recReqMsg, true );
			
			// Record that we sent the msg
			recReqMsg.Dest = me.Name;
			recReqMsg.DestUrl = me.Location;
			bRes = ctxMsgDAO.SendContextMessage( recReqMsg );
		}
		
		// Response to a recommendation request
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void Recommend( string strRecommendationResponse )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}

				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strRecommendationResponse == null || strRecommendationResponse.Length == 0 )
					throw new SoapException( "Invalid Recommendation Response Message", SoapException.ClientFaultCode );
				
				X509Certificate senderCert = this.ProcessRequest( ref reqCtx );
				// Only someone with my certificate can recommend something to me
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, recommend directive refused", SoapException.ClientFaultCode );			
				
				RecommendationResponseCtxMsg recRespMsg = null;
				try
				{	
					recRespMsg = RecommendationResponseCtxMsg.FromXml( strRecommendationResponse );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Recommendation Response Message", SoapException.ClientFaultCode );
				}
				
				// Fill in the sender info
				recRespMsg.Sender = me.Name;
				recRespMsg.SenderUrl = me.Location;
				Recommend( recRespMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}
		
		private void Recommend( RecommendationResponseCtxMsg recRespMsg )
		{
			// Send the resource message on to the meeting organizer
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			// Ignore messages associated with meetings we no nothing aboue
			if( mtgDAO.IsNewMeeting( recRespMsg.MeetingID ) )
				return;
			// Only send recommendations to active meetings
			if( mtgDAO.GetMeetingState( recRespMsg.MeetingID ) != enuMeetingState.Active )
				return;

			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			ResourceDAO resDAO = new ResourceDAO( this.DBConnect );
			Context ctx = new Context();
			ctx.Topic = mtgDAO.GetMeetingTopic( recRespMsg.MeetingID );
			ArrayList lstParticipants = participDAO.GetParticipants( recRespMsg.MeetingID );
			IEnumerator participIt = lstParticipants.GetEnumerator();
			while( participIt.MoveNext() )
			{
				MeetingParticipant p = (MeetingParticipant) participIt.Current;
				ctx.AddParticipant( p.Name );
			}

			MeetingParticipant organizer = participDAO.GetOrganizer( recRespMsg.MeetingID );
			
			IEnumerator it = recRespMsg.ResourceMessage.m_lstResources.GetEnumerator();
			while( it.MoveNext() )
			{
				Resource res = (Resource) it.Current;
				// For each resource recommended, clear it with the policy manager
				// Get the policies governing a resource
				ArrayList lstPolicy = resDAO.GetResourcePolicies( res.ID );
				if( lstPolicy.Count == 0 )
				{
					MeetingResource mtgRes = new MeetingResource( res, recRespMsg.MeetingID, me.Name );
					// Here is where we would ask the policy manager if it's ok to
					// share this resource
					resDAO.AddMeetingResource( mtgRes.MeetingID, mtgRes );
					continue; // Move on to next resource
				}
				
				// Get the policies governing each resource
				IEnumerator polIt = lstPolicy.GetEnumerator();
				while( polIt.MoveNext() )
				{
					// Determine what can be shared
					EvaluationResult evalRes = PolicyManager.evaluatePolicy( (string) polIt.Current, ctx );
					if( evalRes.Result == true )
					{
						MeetingResource mtgRes = new MeetingResource( res, recRespMsg.MeetingID, me.Name );
						// Here is where we would ask the policy manager if it's ok to
						// share this resource
						resDAO.AddMeetingResource( mtgRes.MeetingID, mtgRes );
						break; // Accept the first policy that is satisfied
					}
				}
			}
			
			// If I'm not the organizer then I have to send a context message
			// to the organizer
			if( organizer.Name != me.Name )
			{
				// Create a context msg 
				ResourceCtxMsg resCtxMsg = new ResourceCtxMsg();
				resCtxMsg.Type = enuContextMsgType.ResourceShared;
				resCtxMsg.MessageID = recRespMsg.ResourceMessage.MessageID;
				// Set the destination
				resCtxMsg.Dest = organizer.Name;
				resCtxMsg.DestUrl = organizer.Location;
				resCtxMsg.Sender = me.Name;
				resCtxMsg.SenderUrl = me.Location;
				resCtxMsg.MeetingID = recRespMsg.MeetingID;
	
				IEnumerator resIt = recRespMsg.ResourceMessage.m_lstResources.GetEnumerator();
				while( resIt.MoveNext() )
				{
					Resource res = (Resource) resIt.Current;
					resCtxMsg.AddResourceID( res.ID );
				}

				if( resCtxMsg.ResourceIDs.Count > 0 )
				{
					ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
					ctxMsgDAO.SendContextMessage( resCtxMsg );
				}

				// Set sender info on inner resource message
				recRespMsg.ResourceMessage.Sender = me.Name;
				recRespMsg.ResourceMessage.SenderUrl = me.Location;
				recRespMsg.Type = enuContextMsgType.ResourceShared;
				recRespMsg.Dest = organizer.Name;
				recRespMsg.DestUrl = organizer.Location;
				// Send resources to meeting organizer, the organizer is responsible
				// for showing these resources (here we actually push the resources 
				// using DIME)
				this.SendResources( recRespMsg.ResourceMessage, recRespMsg.Dest, recRespMsg.DestUrl );
			}
		}
		
		private void SendResources( ResourceMsg resMsg, string strContactID, string strIAUrl )
		{
			// Check the contact cache if we have an assembly
			// then load an invoke
			// if we don't have one then generate one then invoke
			ContactCacheDAO ctcCacheDAO = new ContactCacheDAO( this.DBConnect );
			string strAssembly = ctcCacheDAO.GetContactLocation( strContactID );
			if( strAssembly == null || strAssembly.Length == 0 )
			{
				// Generate a web service proxy
				ProxyGenRequest pxyGenReq = new ProxyGenRequest();
				pxyGenReq.ProxyPath = this.ProxyCache;
				pxyGenReq.ServiceName = INFO_AGENT;
				pxyGenReq.WsdlUrl = strIAUrl;
				strAssembly = (string) this.GenerateWebServiceProxy( pxyGenReq );
				// Update contact cache data
				ctcCacheDAO.AddContactLocation( strContactID, strAssembly );
			}

			// Create an execution context
			ExecContext execCtx = new ExecContext();
			// Get the assembly
			execCtx.Assembly = strAssembly;
			// Add the parameters...in this case the context message to sign
			execCtx.AddParameter( Serializer.Serialize( resMsg.ToXml() ) );
			// Name of method to execute on the remote agent
			execCtx.MethodName = "AddResources";
			// Fully qualified name of proxy class
			execCtx.ServiceName = ProxyGenRequest.DEFAULT_PROXY_NAMESPACE + "." + INFO_AGENT;
			// Create an executor to do invocation
			Executor exec = new Executor();
			// Create a settings instance
			ExecutorSettings settings = new ExecutorSettings();
			// DO NOT expect a signed response since we are executing a one way method
			settings.ExpectSignedResponse = false;
			// Set the certificate to use to sign the outgoing message
			settings.SigningCertificate = this.SigningCert;
			// Set the executor settings instance
			exec.Settings = settings;
			// Do invocation (expect null/"" returned)
			object objRes = exec.Execute( execCtx );
		}

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void SendResources( string strResMsg, string strContactID, string strIAUrl )
		{
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}

				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strResMsg == null || strResMsg.Length == 0 )
					throw new SoapException( "Invalid Resource Message", SoapException.ClientFaultCode );
				if( strContactID == null || strContactID.Length == 0 )
					throw new SoapException( "Invalid contact ID", SoapException.ClientFaultCode );
				if( strIAUrl == null || strIAUrl.Length == 0 )
					throw new SoapException( "Invalid Info Agent Url", SoapException.ClientFaultCode );
				
				X509Certificate senderCert = this.ProcessRequest( ref reqCtx );
				// Only someone with "my" certificate can direct "me" to create
				// a meeting
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, invitation directive refused", SoapException.ClientFaultCode );			
				
				ResourceMsg resMsg = null;
				try
				{	
					resMsg = ResourceMsg.FromXml( strResMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Resource Message", SoapException.ClientFaultCode );
				}
				
				this.SendResources( resMsg, strContactID, strIAUrl );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void AddResources( string strResMsg )
		{
			// Receives resources
			// Sender must be a meeting participant
			// IA receiving must be the organizer
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strResMsg == null || strResMsg.Length == 0 )
					throw new SoapException( "Invalid Resource Message Response", SoapException.ClientFaultCode );
				
				// Process the request to ensure the SOAP message
				// was signed etc. and return the certificate of the signer
				X509Certificate senderCert = ProcessRequest( ref reqCtx );
			
				ResourceMsg resMsg = null;
				try
				{	
					resMsg = ResourceMsg.FromXml( strResMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Resource Message", SoapException.ClientFaultCode );
				}

				// Sender must be in this meeting
				ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
				if( !participDAO.IsInMeeting( resMsg.MeetingID, senderCert.GetName() ) )
					throw new SoapException( "Sender " + senderCert.GetName() + " is not in meeting " + resMsg.MeetingID, SoapException.ClientFaultCode );
				
				// Only meeting organizer can receive these resource messages
				MeetingParticipant organizer = participDAO.GetOrganizer( resMsg.MeetingID );
				if( organizer != null )
				{
					if( organizer.Name != me.Name )
						throw new SoapException( "IA not organizer for meeting " + resMsg.MeetingID, SoapException.ClientFaultCode );
				}

				this.AddResources( resMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}

		private void AddResources( ResourceMsg resMsg )
		{
			ResourceDAO resDAO = new ResourceDAO( this.DBConnect );
			IEnumerator it = resMsg.m_lstResources.GetEnumerator();
			while( it.MoveNext() )
			{
				Resource res = (Resource) it.Current;
				if( !resDAO.IsResourceInCatalog( res.ID ) )
					resDAO.AddNewResource( res );
				if( !resDAO.IsResourceInMeeting( res.ID, resMsg.MeetingID ) )
				{
					MeetingResource mtgRes = new MeetingResource( res, resMsg.MeetingID, resMsg.Sender );
					resDAO.AddMeetingResource( mtgRes.MeetingID, mtgRes );
				}
				else
				{
					if( resDAO.GetMeetingResourceState( res.ID ) != enuResourceState.Shared )
						resDAO.ShareResource( res.ID );
				}
			}
			if( resMsg.m_lstResources.Count > 0 )
			{
				ResourceCtxMsg resCtxMsg = new ResourceCtxMsg( resMsg );
				resCtxMsg.Type = enuContextMsgType.ResourceShared;
				resCtxMsg.Dest = me.Name;
				resCtxMsg.DestUrl = me.Location;
				ContextMsgDAO ctxDAO = new ContextMsgDAO( this.DBConnect );
				ctxDAO.ReceiveContextMessage( resCtxMsg, false );
								
				// Send a response
				ContextMsgResponse ctxRespMsg = new ContextMsgResponse();
				ctxRespMsg.MessageID = resMsg.MessageID;
				ctxRespMsg.MeetingID = resMsg.MeetingID;
				ctxRespMsg.Ack = true;
				ctxRespMsg.Sender = me.Name;
				ctxRespMsg.SenderUrl = me.Location;
				ctxRespMsg.Type = enuContextMsgType.ResourceSharedResponse;
				this.SendContextUpdate( ctxRespMsg, resMsg.Sender, resMsg.SenderUrl );
			}
		}
		
		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void RecallMyResources( string strResCtxMsg )
		{
			// Sender must be a meeting participant
			// IA receiving must be the organizer
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strResCtxMsg == null || strResCtxMsg.Length == 0 )
					throw new SoapException( "Invalid Resource Context Message Response", SoapException.ClientFaultCode );
				
				// Process the request to ensure the SOAP message
				// was signed etc. and return the certificate of the signer
				X509Certificate senderCert = ProcessRequest( ref reqCtx );
				// Only someone with my cert can direct me to recall resources
				// I've shared
				if( !senderCert.Equals( this.m_signingCert ) )
					throw new SoapException( "Unauthorized operation, recall directive refused", SoapException.ClientFaultCode );			
								
				ResourceCtxMsg resCtxMsg = null;
				try
				{	
					resCtxMsg = ResourceCtxMsg.FromXml( strResCtxMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Resource Context Message", SoapException.ClientFaultCode );
				}

				this.RecallMyResources( resCtxMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}
		
		private void RecallMyResources( ResourceCtxMsg resCtxMsg )
		{
			MeetingDAO mtgDAO = new MeetingDAO( this.DBConnect );
			// Ignore messages about meetings we know nothing about
			if( mtgDAO.IsNewMeeting( resCtxMsg.MeetingID ) )
				return;
			
			// Get the meeting organizer, we tell the organizer to remove the
			// resources we've shared from the meeting
			ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
			MeetingParticipant organizer = participDAO.GetOrganizer( resCtxMsg.MeetingID );
			if( organizer == null )
				return;

			// If I'm the organizer then remove these resources (mark them as recalled)
			if( organizer.Name == me.Name )
			{
				ResourceDAO resDAO = new ResourceDAO( this.DBConnect );
				// Get resources shared by owner
				Hashtable resources = resDAO.GetSharedResources( resCtxMsg.MeetingID, resCtxMsg.Sender );
				IEnumerator it = resCtxMsg.ResourceIDs.GetEnumerator();
				while( resources.Count > 0 && it.MoveNext() )
				{
					string strResID = (string) it.Current;
					// Recall the resources specified from the set shared
					if( resources[strResID] != null )
						resDAO.RecallResource( strResID );
				}
			}
			else
			{
				// Call recall resources on the meeting organizer
				// Record that we sent the context message
				resCtxMsg.Sender = me.Name;
				resCtxMsg.SenderUrl = me.Location;
				resCtxMsg.Dest = organizer.Name;
				resCtxMsg.DestUrl = organizer.Location;
				resCtxMsg.Type = enuContextMsgType.ResourceRecalled;
				ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
				ctxMsgDAO.SendContextMessage( resCtxMsg );
				// Actually send the context message
				this.SendRecallResources( resCtxMsg, resCtxMsg.Dest, resCtxMsg.DestUrl );
			}
		}

		private void SendRecallResources( ResourceCtxMsg resCtxMsg, string strContactID, string strIAUrl )
		{
			// Check the contact cache if we have an assembly
			// then load an invoke
			// if we don't have one then generate one then invoke
			ContactCacheDAO ctcCacheDAO = new ContactCacheDAO( this.DBConnect );
			string strAssembly = ctcCacheDAO.GetContactLocation( strContactID );
			if( strAssembly == null || strAssembly.Length == 0 )
			{
				// Generate a web service proxy
				ProxyGenRequest pxyGenReq = new ProxyGenRequest();
				pxyGenReq.ProxyPath = this.ProxyCache;
				pxyGenReq.ServiceName = INFO_AGENT;
				pxyGenReq.WsdlUrl = strIAUrl;
				strAssembly = (string) this.GenerateWebServiceProxy( pxyGenReq );
				// Update contact cache data
				ctcCacheDAO.AddContactLocation( strContactID, strAssembly );
			}

			// Create an execution context
			ExecContext execCtx = new ExecContext();
			// Get the assembly
			execCtx.Assembly = strAssembly;
			// Add the parameters...in this case the context message to sign
			execCtx.AddParameter( Serializer.Serialize( resCtxMsg.ToXml() ) );
			// Name of method to execute on the remote agent
			execCtx.MethodName = "RecallResources";
			// Fully qualified name of proxy class
			execCtx.ServiceName = ProxyGenRequest.DEFAULT_PROXY_NAMESPACE + "." + INFO_AGENT;
			// Create an executor to do invocation
			Executor exec = new Executor();
			// Create a settings instance
			ExecutorSettings settings = new ExecutorSettings();
			// DO NOT expect a signed response since we are executing a one way method
			settings.ExpectSignedResponse = false;
			// Set the certificate to use to sign the outgoing message
			settings.SigningCertificate = this.SigningCert;
			// Set the executor settings instance
			exec.Settings = settings;
			// Do invocation (expect null/"" returned)
			object objRes = exec.Execute( execCtx );
		}

		[SoapDocumentMethod(OneWay=true)]
		[WebMethod]
		public void RecallResources( string strResCtxMsg )
		{
			// Sender must be a meeting participant
			// IA receiving must be the organizer
			try
			{
				// Accept only SOAP requests
				SoapContext reqCtx = HttpSoapContext.RequestContext;
				if( reqCtx == null )
				{
					// Read in the entire Soap envelope and try to get a SoapContext that
					// way
					try
					{
						// Get the Http request
						HttpRequest httpReq = this.Context.Request;
						// Try to get a Soap Context from it
						reqCtx = this.GetSoapContextFromHttpRequest( ref httpReq );		
					}
					catch( Exception /*e*/ )
					{}
				}
				if( reqCtx == null )
					throw new ApplicationException( "Non-SOAP message!!!" );
				
				if( strResCtxMsg == null || strResCtxMsg.Length == 0 )
					throw new SoapException( "Invalid Resource Context Message Response", SoapException.ClientFaultCode );
				
				// Process the request to ensure the SOAP message
				// was signed etc. and return the certificate of the signer
				X509Certificate senderCert = ProcessRequest( ref reqCtx );
			
				ResourceCtxMsg resCtxMsg = null;
				try
				{	
					resCtxMsg = ResourceCtxMsg.FromXml( strResCtxMsg );
				}
				catch( Exception /*e*/ )
				{
					throw new SoapException( "Invalid Resource Context Message", SoapException.ClientFaultCode );
				}

				// Sender must be in this meeting
				ParticipantDAO participDAO = new ParticipantDAO( this.DBConnect );
				if( !participDAO.IsInMeeting( resCtxMsg.MeetingID, senderCert.GetName() ) )
					throw new SoapException( "Sender " + senderCert.GetName() + " is not in meeting " + resCtxMsg.MeetingID, SoapException.ClientFaultCode );
				
				// Only meeting organizer can receive these resource messages
				MeetingParticipant organizer = participDAO.GetOrganizer( resCtxMsg.MeetingID );
				if( organizer != null )
				{
					if( organizer.Name != me.Name )
						throw new SoapException( "IA not organizer for meeting " + resCtxMsg.MeetingID, SoapException.ClientFaultCode );
				}

				this.RecallResources( resCtxMsg );
			}
			catch( Exception e )
			{
				// Log exception
				EventLog.WriteEntry( SOURCE, e.Message, EventLogEntryType.Error );
			}
		}

		private void RecallResources( ResourceCtxMsg resCtxMsg )
		{
			ContextMsgDAO ctxMsgDAO = new ContextMsgDAO( this.DBConnect );
			ctxMsgDAO.ReceiveContextMessage( resCtxMsg, false );
			
			// Only the owner and the meeting organizer know the resource ids
			ResourceDAO resDAO = new ResourceDAO( this.DBConnect );
			IEnumerator it = resCtxMsg.ResourceIDs.GetEnumerator();
			Hashtable resources = resDAO.GetSharedResources( resCtxMsg.MeetingID, resCtxMsg.Sender );
			while( resources.Count > 0 && it.MoveNext() )
			{
				string strResID = (string) it.Current;
				if( resources[strResID] != null )
					resDAO.RecallResource( strResID );
			}

			// Send a response
			ContextMsgResponse ctxRespMsg = new ContextMsgResponse();
			ctxRespMsg.MessageID = resCtxMsg.MessageID;
			ctxRespMsg.MeetingID = resCtxMsg.MeetingID;
			ctxRespMsg.Ack = true;
			ctxRespMsg.Sender = me.Name;
			ctxRespMsg.SenderUrl = me.Location;
			ctxRespMsg.Type = enuContextMsgType.ResourceRecalledResponse;
			this.SendContextUpdate( ctxRespMsg, resCtxMsg.Sender, resCtxMsg.SenderUrl );
		}
	}
}
