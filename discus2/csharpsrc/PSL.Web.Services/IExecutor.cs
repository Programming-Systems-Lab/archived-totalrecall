using System;

namespace PSL.Web.Services.DynamicInvoke
{
	/// <summary>
	/// Summary description for IExecutor.
	/// </summary>
	public interface IExecutor
	{
		object Execute( ExecContext ctx );
	}
}
