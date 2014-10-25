using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace RDN.Utilities.Paging
{
    public class Pager
    {
        private ViewContext viewContext;
        private readonly int pageSize;
        private readonly int currentPage;
        private readonly int totalItemCount;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;
        private readonly string routeName;
        private readonly string javascript;

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, string routeName, RouteValueDictionary valuesDictionary)
        {
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.linkWithoutPageValuesDictionary = valuesDictionary;
            this.routeName = routeName;
        }
        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, string javascript)
        {
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.javascript = javascript;
        }

        public string RenderHtml(bool isJavaScript)
        {
            int pageCount = (int)Math.Ceiling(this.totalItemCount / (double)this.pageSize);
            int nrOfPagesToDisplay = 10;

            var sb = new StringBuilder();

            // Previous
            if (this.currentPage > 1)
            {
                if (!isJavaScript)
                    sb.Append(GeneratePageLink("&lt;", this.currentPage - 1, routeName, javascript));
                else
                    sb.Append("<span class=\"disabled\">&lt;</span>");
            }
            else
            {
                sb.Append("<span class=\"disabled\">&lt;</span>");
            }

            int start = 1;
            int end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                int middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                int below = (this.currentPage - middle);
                int above = (this.currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }

            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1, routeName, javascript));
                sb.Append(GeneratePageLink("2", 2, routeName, javascript));
                sb.Append("...");
            }
            for (int i = start; i <= end; i++)
            {
                if (i == this.currentPage)
                {
                    if (!isJavaScript)
                        sb.AppendFormat("<span class=\"current\">{0}</span>", i);
                    else
                    {
                        string linkFormat = "<span  onclick=\"{0}\">{1}</span>";
                        sb.AppendFormat(linkFormat, javascript, i);
                    }
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i, routeName, javascript));
                }
            }
            if (end < (pageCount - 3))
            {
                sb.Append("...");
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1, routeName, javascript));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount, routeName, javascript));
            }

            // Next
            if (this.currentPage < pageCount)
            {
                if (!isJavaScript)
                    sb.Append(GeneratePageLink("&gt;", (this.currentPage + 1), routeName, javascript));
                else
                    sb.Append("<span class=\"disabled\">&gt;</span>");
            }
            else
            {
                sb.Append("<span class=\"disabled\">&gt;</span>");
            }
            return sb.ToString();
        }

        private string GeneratePageLink(string linkText, int pageNumber, string routeName, string javascript)
        {

            //var virtualPathData = this.viewContext.RouteData.Route.GetVirtualPath(this.viewContext, pageLinkValueDictionary);
            if (!String.IsNullOrEmpty(routeName))
            {
                var pageLinkValueDictionary = new RouteValueDictionary(this.linkWithoutPageValuesDictionary);
                pageLinkValueDictionary.Add("page", pageNumber);
                var virtualPathData = RouteTable.Routes.GetVirtualPath(this.viewContext.RequestContext, pageLinkValueDictionary);
                if (routeName != null)
                    virtualPathData = RouteTable.Routes.GetVirtualPath(this.viewContext.RequestContext, routeName, pageLinkValueDictionary);


                if (virtualPathData != null)
                {
                    string linkFormat = "<a href=\"{0}\">{1}</a>";
                    return String.Format(linkFormat, virtualPathData.VirtualPath, linkText);
                }
                else
                {
                    return null;
                }
            }
            else if (!String.IsNullOrEmpty(javascript))
            {
                string linkFormat = "<span class=\"pagerJS\" onclick=\"{0}\">{1}</span>";
                return String.Format(linkFormat, javascript, linkText);
            }
            return null;
        }
    }
}