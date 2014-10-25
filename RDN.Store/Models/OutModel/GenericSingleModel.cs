using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Store.Models.OutModel
{
    public class GenericSingleModel<T> : Base
    {
        public GenericSingleModel()
        {

        }

        public GenericSingleModel(T model)
        {
            Model = model;
        }

        public T Model { get; set; }
    }
}