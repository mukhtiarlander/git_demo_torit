using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Utilities
{
    /// <summary>
    /// used to gather ids from the admins in order to delete accounts/leagues etc.
    /// </summary>
    public class IdModel
    {
        public string Id { get; set; }
        public string Id2 { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAttached { get; set; }
        public bool IsSuccess { get; set; }
    }
}