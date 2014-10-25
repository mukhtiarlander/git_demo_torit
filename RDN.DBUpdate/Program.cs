using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Imports.Rinxter;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Federation;
using RDN.Library.Classes.Federation.Enums;

namespace RDN.DBUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            var dc = new ManagementContext();
            //Console.WriteLine(string.Format("Email verifications count: {0}", dc.EmailVerifications.Count()));            
            //Console.WriteLine(string.Format("Leagues count: {0}", dc.Leagues.Count()));
            //Console.WriteLine(string.Format("Members count: {0}", dc.Members.Count()));
            //Console.WriteLine(string.Format("Teams count: {0}", dc.Teams.Count()));
            //Console.WriteLine(string.Format("Contact leagues count: {0}", dc.ContactLeagues.Count()));

            //RinxterImportFactory riW = new RinxterImportFactory();
            //riW.Initialize(FederationsEnum.WFTDA).RunRinxterImports();

            //RinxterImportFactory riM = new RinxterImportFactory();
            //riM.Initialize(FederationsEnum.MRDA).RunRinxterImports();

            Console.ReadLine();
        }
    }
}
