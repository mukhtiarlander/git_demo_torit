using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Drawing
{
    public class Images
    {
        public static Image ScaleDownImage(Image image, int maxWidth, int maxHeight)
        {
            if (image.Height > maxHeight || image.Width > maxWidth)
            {
                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
                return newImage;
            }
            return image;
        }
    }
}
