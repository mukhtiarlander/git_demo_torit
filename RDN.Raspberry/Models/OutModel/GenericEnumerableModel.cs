using System.Collections.Generic;

namespace RDN.Raspberry.Models.OutModel
{
    public class GenericEnumerableModel <T> : Base
    {
        public IEnumerable<T> Model { get; set; } 
    }
}