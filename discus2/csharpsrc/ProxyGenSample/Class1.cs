using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using PSL.Web.Services.DynamicProxy;
using PSL.Web.Services.DynamicInvoke;

namespace ProxyGenSample
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// Dynamic proxy generation...

			// Create a new proxy generation request
			ProxyGenRequest req = new ProxyGenRequest();
			// Set the location of the assembly to be generated
			// leave empty ("") for current directory
			req.ProxyPath = "";
			// Set the name of the assembly (dll) that will be generated
			req.ServiceName = "WeatherSvc";
			// Set the url of the web service's WSDL file
			req.WsdlUrl = "http://www.xmethods.net/sd/2001/TemperatureService.wsdl";  

			// Create a WSE mutator, generated proxies will expose the RequestContext
			// and ResponseContext properties
			ProxyPolicyMutator mutator = new ProxyPolicyMutator();
			// Set the name of the proxy class generated
			mutator.ProxyName = req.ServiceName;

			// Create the proxy generator
			ProxyGen pxyGen = new ProxyGen();
			// Set the proxy generator's mutator
			pxyGen.Mutator = mutator;
			// Generate the proxy
			string strAssembly = pxyGen.GenerateAssembly( req );
			
			// Dynamic invocation...
			
			// Set the parameter...note it MUST be in xml form
			string strZipcode = "<string>10025</string>";

			// Create an execution context
			ExecContext ctx = new ExecContext();
			// Set the fully qualified name of the web service proxy 
			// (FullQname = <Namespace>.<classname>"
			ctx.ServiceName = req.Namespace + "." + req.ServiceName;
			// Set the name of the assembly where the proxy class "lives"
			ctx.Assembly = strAssembly;
			// Set the name of the method to dynamically invoke
			ctx.MethodName = "getTemp";
			// Add any parameters
			ctx.AddParameter( strZipcode );
			
			// Create an Executor
			Executor exec = new Executor();
			// Execute using the context
			object objRes = exec.Execute( ctx );
			
			
			
		}
	}
}
