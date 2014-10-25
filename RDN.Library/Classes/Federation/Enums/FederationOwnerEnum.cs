using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Federation.Enums
{
    public enum FederationOwnerEnum
    {
        None = 0,
        //can do anything in the federation
        Owner = 1,
        //can add and edit members in federation.
        Manager = 2
    }
}
