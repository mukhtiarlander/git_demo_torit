using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using RDN.Utilities.Paging;

namespace RDN.Utilities.Paging
{
    public static class PagingExtensions
    {
        #region HtmlHelper extensions

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, null);
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, string controllerName)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, controllerName, null, null);
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, null);
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, object values)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, new RouteValueDictionary(values));
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, object values)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, actionName, new RouteValueDictionary(values));
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            return Pager(htmlHelper, pageSize, currentPage, totalItemCount, null, valuesDictionary);
        }

        public static string Pager(this HtmlHelper htmlHelper, int pageSize, int currentPage, int totalItemCount, string actionName, string controllerName, string routeName, RouteValueDictionary valuesDictionary)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }
            if (actionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }
                valuesDictionary.Add("action", actionName);
            }
            if (controllerName != null)
            {
                if (valuesDictionary.ContainsKey("controller"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "controllerName");
                }
                valuesDictionary.Add("controller", controllerName);
            }
            var pager = new Pager(htmlHelper.ViewContext, pageSize, currentPage, totalItemCount, routeName, valuesDictionary);
            return pager.RenderHtml();
        }

        #endregion

        #region IQueryable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

        #region IEnumerable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}