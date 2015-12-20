using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.League.Models.Helpers
{
    public static class EnumerationHelper
    {
        public static IDictionary<int, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
                throw new ArgumentException("Enumeration type is expected.");

            var dictionary = new Dictionary<int, string>();

            foreach (int value in System.Enum.GetValues(enumerationType))
            {
                var name = System.Enum.GetName(enumerationType, value);
                dictionary.Add(value, name);
            }

            return dictionary;
        }

    }
}