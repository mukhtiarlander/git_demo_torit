﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace RDNScoreboard.Code
{
    public class EnumValuesExtension : MarkupExtension
    {
        private readonly Type _enumType;

        public EnumValuesExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum)
                throw new ArgumentException("Argument enumType must derive from type Enum.");

            _enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_enumType);
        }
    }
}
