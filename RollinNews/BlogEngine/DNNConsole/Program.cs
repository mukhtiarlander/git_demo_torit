using BlogEngine.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            log4net.Config.XmlConfigurator.Configure();

            new DNNScraper().ScanAllPages();

        }
    }
}
