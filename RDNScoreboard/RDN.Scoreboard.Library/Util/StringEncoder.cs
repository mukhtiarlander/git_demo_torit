using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.Util
{
   public  class StringEncoder
    {
        /// <summary>
        /// This method creates a Base64 encoded string from an input 
        /// parameter string.
        /// </summary>
        /// <param name="m_enc">The String containing the characters 
        /// to be encoded.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64UTF8(string m_enc)
        {
            byte[] toEncodeAsBytes =
            System.Text.Encoding.UTF8.GetBytes(m_enc);
            string returnValue =
            System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        /// <summary>
        /// This method will Decode a Base64 string.
        /// </summary>
        /// <param name="m_enc">The String containing the characters 
        /// to be decoded.</param>
        /// <returns>A String containing the results of decoding the
        /// specified sequence of bytes.</returns>
        public static string DecodeFrom64(string m_enc)
        {
            byte[] encodedDataAsBytes =
            System.Convert.FromBase64String(m_enc);
            string returnValue =
            System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
