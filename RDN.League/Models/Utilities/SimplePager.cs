using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.Utilities
{
    public class SimplePager<T>
    {
        public List<T> Items { get; set; }
        public List<SelectListItem> Pages { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
        public int NumberOfRecords { get; set; }
        public int ItemToDelete { get; set; }
        public bool DeleteAll { get; set; }

        public SimplePager()
        {
            Pages = new List<SelectListItem>();
        }
    }
    /// <summary>
    /// the modified pager has an item to approve and changes the data type of item to delete.
    /// Item to approve also added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleModPager<T>
    {
        public List<T> Items { get; set; }
        public List<SelectListItem> Pages { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
        public int NumberOfRecords { get; set; }
        public string ItemToDelete { get; set; }
        public bool DeleteAll { get; set; }
        public string ItemToApprove { get; set; }

        public SimpleModPager()
        {
            Pages = new List<SelectListItem>();
        }
    }
}