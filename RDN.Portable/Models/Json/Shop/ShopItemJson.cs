using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using RDN.Portable.Classes.Store.Enums;
using System.Text.RegularExpressions;

namespace RDN.Portable.Models.Json.Shop
{
    [DataContract]
    public class ShopItemJson
    {
        [DataMember]
        public int StoreItemId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        const string HTML_TAG_PATTERN = "<.*?>";
        public string NotesNonHtml
        {
            get
            {
                if (!String.IsNullOrEmpty(Notes))
                    return Regex.Replace(Notes, HTML_TAG_PATTERN, string.Empty);

                return string.Empty;
            }
        }
        [DataMember]
        public string Notes { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal Shipping { get; set; }
        [DataMember]
        public bool IsPublished { get; set; }
        [DataMember]
        public StoreItemTypeEnum ItemType { get; set; }
        [DataMember]
        public StoreItemShirtSizesEnum ItemSize { get; set; }
        [DataMember]
        public int QuantityInStock { get; set; }
        [DataMember]
        public bool CanRunOutOfStock { get; set; }
        public string SoldByHuman { get { return "by " + SoldBy; } }
        [DataMember]
        public string SoldBy { get; set; }
        [DataMember]
        public string SoldById { get; set; }

        [DataMember]
        public bool AcceptsPayPal { get; set; }
        [DataMember]
        public bool AcceptsStripe { get; set; }
        [DataMember]
        public string Views { get; set; }

        private string _firstPhotoUrl;
        public string FirstPhotoUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_firstPhotoUrl))
                    _firstPhotoUrl = PhotoUrlsThumbs.FirstOrDefault();
                return _firstPhotoUrl;
            }
            set { _firstPhotoUrl = value; }
        }
        [DataMember]
        public List<string> PhotoUrls { get; set; }
        [DataMember]
        public List<string> PhotoUrlsThumbs { get; set; }
        [DataMember]
        public List<string> Colors { get; set; }
        [DataMember]
        public string RDNUrl { get; set; }

        public ShopItemJson()
        {
            PhotoUrls = new List<string>();
            PhotoUrlsThumbs = new List<string>();
            Colors = new List<string>();
        }

    }
}
