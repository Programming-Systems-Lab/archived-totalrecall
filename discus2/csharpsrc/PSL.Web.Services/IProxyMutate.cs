using System;
using System.CodeDom;

namespace PSL.Web.Services.DynamicProxy
{
	/// <summary>
	/// Summary description for IProxyMutate
	/// </summary>
	public interface IProxyMutate
	{
		// Mutate operation context specific
		void Mutate( ref CodeNamespace cnSpace );
	}
}
