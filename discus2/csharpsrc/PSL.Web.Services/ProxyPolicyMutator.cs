using System;
using System.CodeDom;
using System.Collections;

namespace PSL.Web.Services.DynamicProxy
{
	/// <summary>
	/// Summary description for ProxyPolicyMutator.
	/// </summary>
	public class ProxyPolicyMutator:ProxyMutator
	{
		// Set base class
		public const string PROXY_BASE_CLASS = "Microsoft.Web.Services.WebServicesClientProtocol";
		// Set compiler params
		public const string PROXY_COMPILER_PARAMS = "Microsoft.Web.Services.dll";
		public const string PROXY_WSE_IMPORT = "Microsoft.Web.Services";
		public const string PROXY_WSE_SECURITY_IMPORT = "Microsoft.Web.Services.Security";

		private string m_strProxyName = "";
		private string m_strCodeSnippet = "";

		public ProxyPolicyMutator():base()
		{
		}

		public string ProxyName
		{
			get
			{ return m_strProxyName; }
			set
			{ 
				if( value == null || value == "" )
					return;

				m_strProxyName = value; 
			}
		}

		public string CodeSnippet
		{
			get
			{ return this.m_strCodeSnippet; }
				set
				{
					if( value == null || value == "" )
						return;
					
					this.m_strCodeSnippet = value;
				}
		}
		
		public override void Mutate( ref CodeNamespace cnSpace )
		{
			// Call Base class implementation first to do general preperation
			// for mutation
			base.Mutate( ref cnSpace );
			
			if( m_cnSpace == null )
				throw new Exception( "CodeNamespace is null, either parameter cnSpace is null or base.Mutate(...) not called" );

			// Add namespaces
			m_cnSpace.Imports.Add( new CodeNamespaceImport(	PROXY_WSE_IMPORT ) );
			m_cnSpace.Imports.Add( new CodeNamespaceImport(	PROXY_WSE_SECURITY_IMPORT ) );
			// Add Compiler parameters
			m_lstCompilerParameters.Add( PROXY_COMPILER_PARAMS );

			// Change proxy name if nec
			if( ProxyName.Length > 0 )
			{
				ProxyNameMutator nameMutator = new ProxyNameMutator();
				nameMutator.ProxyName = m_strProxyName;
				nameMutator.Mutate( ref m_cnSpace );
			}
			
			// Change base class
			ProxyBaseClassMutator baseClassMutator = new ProxyBaseClassMutator();
			baseClassMutator.BaseClass = PROXY_BASE_CLASS;
			baseClassMutator.Mutate( ref m_cnSpace );
			
			// Augment proxy methods
			if( this.CodeSnippet.Length > 0 )
			{
				ProxyMethodMutator methodMutator = new ProxyMethodMutator();
				methodMutator.CodeSnippet = this.CodeSnippet;
				methodMutator.Mutate( ref m_cnSpace );
			}
		}
	}
}
