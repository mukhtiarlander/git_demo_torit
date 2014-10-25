using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace RDN.Library.Classes.Utilities
{
    public static class EmailValidator
    {
        public static bool Validate(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            const string emailRegex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            var re = new Regex(emailRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (re.IsMatch(email))
                return (true);

            return (false);
        }
    }
    /// <summary>
    /// email validation attribute.
    /// </summary>
    public class EmailValidationAttribute : RegularExpressionAttribute
    {
        public EmailValidationAttribute() : base(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$") { }
    }
}


