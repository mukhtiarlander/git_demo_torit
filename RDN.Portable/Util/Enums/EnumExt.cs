using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Util.Enums
{


    public static class EnumExt
    {
        public static string ToFreindlyName(this Enum enumTo)
        {
            return enumTo.ToString().Replace("_", " ");
        }
        public static string ToFreindlyName(string enumTo)
        {
            return enumTo.Replace("_", " ");
        }
        public static List<EnumClass> ToList<T>()
        {
            var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
            var array2 = Enum.GetNames(typeof(T)).ToArray<string>();
            List<EnumClass> lst = null;
            for (int i = 0; i < array.Length; i++)
            {
                if (lst == null)
                    lst = new List<EnumClass>();
                string name = array2[i];
                T value = array[i];
                lst.Add(new EnumClass { Name = name, Value = value });
            }
            return lst;
        }
        public static IEnumerable<T> GetValues<T>()
        {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException("Type must be enumeration type.");

            return GetValues_impl<T>();
        }

        private static IEnumerable<T> GetValues_impl<T>()
        {
            return from field in typeof(T).GetFields()
                   where field.IsLiteral && !string.IsNullOrEmpty(field.Name)
                   select (T)field.GetValue(null);
        }
    }
}
