using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Classes.Enums
{
    public static class EnumExt
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            var values = (from TEnum e in Enum.GetValues(typeof(TEnum))
                          select new { Id = e, Name = e.ToString().Replace("_", " ") }).OrderBy(x => x.Name);

            return new SelectList(values, "Id", "Name", enumObj);
        }
        public static SelectList ToSelectListValue<TEnum>(this TEnum enumObj)
        {
            var values = (from TEnum e in Enum.GetValues(typeof(TEnum))
                          select new { Id = Convert.ToInt32(e), Name = e.ToString().Replace("_", " ") }).OrderBy(x => x.Name);

            return new SelectList(values, "Id", "Name", enumObj);
        }
    
        public static SelectList ToSelectListDropDown<TEnum>(this TEnum enumObj)
        {
            var values = (from TEnum e in Enum.GetValues(typeof(TEnum))
                          select new { Id = e.ToString(), Name = e.ToString().Replace("_", " ") }).OrderBy(x => x.Name);
            var val = values.Where(x => x.Id == enumObj.ToString()).FirstOrDefault();
            
            var list =  new SelectList(values, "Id", "Name", val);
            return list;
        }
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, TEnum[] notInList)
        {
            var values = (from TEnum e in Enum.GetValues(typeof(TEnum))
                          select new { Id = e, Name = e.ToString().Replace("_", " ") }).OrderBy(x => x.Name);
            var final = values.Where(x => !notInList.Contains(x.Id));

            return new SelectList(final, "Id", "Name", enumObj.ToString());
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