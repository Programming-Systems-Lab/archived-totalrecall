using System;
using System.IO;
using System.Text;
using System.Collections;
using Microsoft.Web.Services;
using Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;
using Microsoft.Data.Odbc;
using CAPICOM;
using System.Runtime.InteropServices;
using PSL.TotalRecall.DataAccess;

namespace PSL.TotalRecall
{
	/// <summary>
	/// Summary description for ContactDAO.
	/// </summary>
	public class ContactDAO:BaseDAO
	{
		public ContactDAO( string strDBConnect ):base( strDBConnect )
		{
		}

		public bool IsContactInRegistry( string strContactID )
		{
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			
			bool bRetVal = false;
			OdbcDataReader dr = null;

			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " SELECT COUNT(*) " );
				strQueryBuilder.Append( " FROM " );
				strQueryBuilder.Append( Constants.CONTACTS_TABLENAME );
				strQueryBuilder.Append( " WHERE " );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "=" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );

				dr =  QueryService.ExecuteReader( this.DBConnect, strQueryBuilder.ToString() );
				
				if( dr == null )
					throw new Exception( "Null data reader returned from query" );

				// Advance data reader to first record
				if( dr.Read() )
				{
					int nCount = -1;
					if( !dr.IsDBNull( 0 ) )
						nCount = dr.GetInt32( 0 );
					
					if( nCount == 1 )
						bRetVal = true;
				}
			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
				if( dr != null )
					dr.Close();
			}

			return bRetVal;
		}

		public bool IsContactCertificateInStore( string strContactID )
		{
			bool bRetVal = false;

			X509CertificateStore certStore = X509CertificateStore.LocalMachineStore( X509CertificateStore.MyStore );
			if( certStore == null )
				throw new Exception( "Error opening Local Machine Store" );
			
			if( certStore.OpenRead() )
			{
				X509CertificateCollection certColl = certStore.FindCertificateBySubjectName( strContactID );
				if( certColl.Count == 0 )
					bRetVal = false;
				else bRetVal = true;
			}
			
			// Close the certificate store
			certStore.Close();

			return bRetVal;
		}

		public bool AddContact( string strContactID )
		{
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			
			// Prevent attempt to add a contact multiple times
			if( IsContactInRegistry( strContactID ) )
				return true;

			bool bRetVal = false;
				
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.CONTACTS_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				strQueryBuilder.Append( ")" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					bRetVal = true;

			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;
		}

		public bool AddContact( string strContactID, double dblTrustScore )
		{
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			
			// Prevent attempt to add a contact multiple times
			if( IsContactInRegistry( strContactID ) )
				return true;
			
			if( dblTrustScore <= 0.0 )
				dblTrustScore = 0.0;

			bool bRetVal = false;
				
			try
			{
				StringBuilder strQueryBuilder = new StringBuilder();
				strQueryBuilder.Append( " INSERT INTO " );
				strQueryBuilder.Append( Constants.CONTACTS_TABLENAME );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( Constants.CONTACT_ID );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( Constants.CONTACT_TRUST_SCORE );
				strQueryBuilder.Append( ")" );
				strQueryBuilder.Append( " VALUES " );
				strQueryBuilder.Append( "(" );
				strQueryBuilder.Append( "'" + QueryService.MakeQuotesafe( strContactID ) + "'" );
				strQueryBuilder.Append( "," );
				strQueryBuilder.Append( dblTrustScore.ToString() );
				strQueryBuilder.Append( ")" );

				int nRowsAffected = QueryService.ExecuteNonQuery( this.DBConnect, strQueryBuilder.ToString() );
				if( nRowsAffected == 1 )
					bRetVal = true;

			}
			catch( Exception /*e*/ )
			{
			}
			finally
			{
			}

			return bRetVal;
		}
				
		public bool AddContactCertificate( string strContactID, X509Certificate cert )
		{
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			if( cert == null )
				throw new ArgumentNullException( "cert", "Invalid Contact X509 Certificate" );

			// Prevent any attempt to add multiple certificates for a contact
			if( IsContactCertificateInStore( strContactID ) )
				return true;

			bool bRetVal = true;
			
			// Use CAPICOM (v2.0) support to add certificate
			StoreClass store = new StoreClass();
			store.Open( CAPICOM_STORE_LOCATION.CAPICOM_LOCAL_MACHINE_STORE,
						X509CertificateStore.MyStore,
						CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_EXISTING_ONLY |
						CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED
						);
			
			// Store certificate
			// Convert cert to base64 string
			string strBase64Cert = cert.ToBase64String();
			// Save to temp file
			string strTempFile = Guid.NewGuid().ToString();
			StreamWriter sw = new StreamWriter( File.Create( strTempFile ) );
			sw.Write( strBase64Cert );
			sw.Flush();
			sw.Close();
			// Load cert from temp file
			store.Load( strTempFile, Constants.DEFAULT_CERT_PSWD, CAPICOM_KEY_STORAGE_FLAG.CAPICOM_KEY_STORAGE_DEFAULT | CAPICOM_KEY_STORAGE_FLAG.CAPICOM_KEY_STORAGE_EXPORTABLE );
			// Delete temp file
			File.Delete( strTempFile );
			// Close store
			store.CloseHandle( store.StoreHandle );
			// Return true		
			return bRetVal;
		}

		public bool RemoveContactCertificate( string strContactID )
		{
			if( strContactID == null || strContactID.Length == 0 )
				throw new ArgumentException( "Invalid contact ID", "strContactID" );
			
			// Prevent any attempt to add multiple certificates for a contact
			if( !IsContactCertificateInStore( strContactID ) )
				return true;
			
			bool bRetVal = true;

			// Use CAPICOM (v2.0) support to remove certificate
			StoreClass store = new StoreClass();
			store.Open( CAPICOM_STORE_LOCATION.CAPICOM_LOCAL_MACHINE_STORE,
				X509CertificateStore.MyStore,
				CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_EXISTING_ONLY |
				CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED
				);
			
			// Remove "CN=" prefix from contact ID
			// Hack because CAPICOM Find by Subject name will not ignore the "CN="
			// unlike Microsoft.Web.Services.Security.X509.X509CertificateStore
			int nStart = strContactID.LastIndexOf( "=" );
			strContactID = strContactID.Substring( nStart + 1 );
			// Find the cert to remove
			Certificates certCol = ((Certificates) store.Certificates).Find(CAPICOM_CERTIFICATE_FIND_TYPE.CAPICOM_CERTIFICATE_FIND_SUBJECT_NAME, strContactID, false );
			IEnumerator it = certCol.GetEnumerator();
			while( it.MoveNext() )
			{
				store.Remove( (Certificate) it.Current );
			}
			
			// Close store
			store.CloseHandle( store.StoreHandle );
			return bRetVal;
		}
		
		public X509Certificate GetContactCertificate( string strContactID )
		{
			X509CertificateStore certStore = X509CertificateStore.LocalMachineStore( X509CertificateStore.MyStore );
			if( certStore == null )
				throw new Exception( "Error opening Local Machine Store" );
			
			X509Certificate cert = null;

			if( certStore.OpenRead() )
			{
				X509CertificateCollection certColl = certStore.FindCertificateBySubjectName( strContactID );
				if( certColl.Count == 1 )
					cert = certColl[0];
			}
			
			// Close the certificate store
			certStore.Close();
			return cert;
		}
	}
}
