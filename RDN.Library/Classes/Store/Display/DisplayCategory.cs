using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Store.Classes;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Store.Display
{
    public class DisplayCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<StoreItem> StoreItems { get; set; }

        public DisplayCategory()
        {
            StoreItems = new List<StoreItem>();
        }

    }
}
