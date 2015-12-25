using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Store.Display;
using System.Web.Mvc;
using RDN.Library.Classes.Store.Classes;

namespace RDN.League.Models.Store
{
    public class StoreItemDisplayModel : StoreItem
    {
        public string StoreName { get; set; }
        public Guid MerchantId { get; set; }
        public Guid InternalId { get; set; }
        public Guid PrivateManagerId { get; set; }
        public SelectList ItemSizes { get; set; }
        public SelectList ItemTypeSelectList { get; set; }
        public SelectList ColorList { get; set; }
        public string ColorsSelected { get; set; }
        [AllowHtml]
        public string HtmlNote { get; set; }

        public bool HasExtraSmall { get; set; }
        public bool HasSmall { get; set; }
        public bool HasMedium { get; set; }
        public bool HasLarge { get; set; }
        public bool HasExtraLarge { get; set; }
        public bool HasXXLarge { get; set; }
        public bool HasXXXLarge { get; set; }

        public StoreItemDisplayModel()
        {

        }

    }
}
