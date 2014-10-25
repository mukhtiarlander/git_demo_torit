using RDN.Portable.Classes.Controls.Dues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RDN.League.Models.Dues
{
    public class DuesModel : DuesPortableModel
    {
        [AllowHtml]
        public string DuesEmailDisplayText { get; set; }
        public SelectList CurrencyList { get; set; }
    }
}