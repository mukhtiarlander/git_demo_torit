using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace RDN.EmailServer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			Library.DatabaseInitializers.EmailServerInitializer.Initialize();

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new EmailServer() 
			};
			ServiceBase.Run(ServicesToRun);
		}        
	}
}
