using System;

namespace RDN.Library.DataModels.Base
{
    public abstract class InheritDb
    {        
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }        

        protected InheritDb()
        {
            Created = DateTime.UtcNow;            
        }
    }
}
