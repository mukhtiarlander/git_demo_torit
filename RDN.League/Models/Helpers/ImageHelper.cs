using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.Helpers
{
    public class ImageHelper
    {
        public static string MemberImage(Guid id)
        {
            return MemberImage(id, 0, 0, "");
        }

        public static string MemberImage(Guid id, int width, int height)
        {
            return MemberImage(id, width, height, "");
        }

        public static string MemberImage(Guid id, int width, int height, string alt)
        {
            var url = "/Utilities/MemberPhoto/" + id.ToString();
            var heightOutput = height == 0 ? "" : "height: " + height + "px;";
            var widthOutput = width == 0 ? "" : "width: " + width + "px;";
            return string.Format("<img src=\"{0}\" style=\"{2}{3}\" alt=\"{1}\" />", url, alt, heightOutput, widthOutput);
        }
    } 
}