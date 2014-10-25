using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.EmailServer.Enums;

namespace RDN.EmailServerTester
{
    class Program
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("EmailServerLogger");
        
        static void Main(string[] args)
        {
            Library.DatabaseInitializers.EmailServerInitializer.Initialize();

            var s = new EmailServer.EmailServer(); 
            s.Init();
            
            s.Start();
            Console.ReadLine();
        }
    }
}
