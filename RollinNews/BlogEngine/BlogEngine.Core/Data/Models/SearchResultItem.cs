using System;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Models
{
    /// <summary>
    /// Category item
    /// </summary>
    public class SearchResultItem
    {
        public string PhotoUrl { get; set; }
        public string IdOfItem { get; set; }
        public string Title { get; set; }
        public string UrlOfItem { get; set; }
        public string ItemType { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public SearchResultItem()
        {
            Properties = new Dictionary<string, string>();
        }
    }
}
