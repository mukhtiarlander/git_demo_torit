using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Scoreboard.Library.Util
{
    public class Imaging
    {
        /// <summary>
        /// converts image to Byte Array
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        /// <summary>
        /// converts Byte Array to image.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image ByteToImage(byte[] img)
        {
            ImageConverter converter = new ImageConverter();
            return (Image)converter.ConvertFrom(img);
        }
    }
}
