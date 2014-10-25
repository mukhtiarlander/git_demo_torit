using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Store.Classes.Enums
{
    public static class EnumExt
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            var values = (from TEnum e in Enum.GetValues(typeof(TEnum))
                          select new { Id = e, Name = e.ToString().Replace("_", " ") }).OrderBy(x => x.Name);

            return new SelectList(values, "Id", "Name", enumObj);
        }
        public static SelectList ToSelectListId<TEnum>(this TEnum enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = Convert.ToInt32(e), Name = Convert.ToInt32(e) };

            return new SelectList(values, "Id", "Name", enumObj);
        }
        public static SelectList ToSelectListIdAndName<TEnum>(this TEnum enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = Convert.ToInt32(e), Name = Convert.ToInt32(e) + " - " + e.ToString() };

            return new SelectList(values, "Id", "Name", enumObj);
        }
    }
}