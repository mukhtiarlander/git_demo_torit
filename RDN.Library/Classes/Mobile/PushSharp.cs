using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Mobile
{
    public class PushSharpServer
    {
        static PushSharpServer instance = new PushSharpServer();

        public PushSharp.PushBroker Broker { get; set; }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PushSharpServer()
        { }

        public static PushSharpServer Instance
        {
            get
            {
                return instance;
            }
        }


    }
}
