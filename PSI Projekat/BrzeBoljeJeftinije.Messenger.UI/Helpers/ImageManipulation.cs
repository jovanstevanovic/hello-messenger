/**
 * ImageManipulation.cs
 * Autor: Nikola Pavlović
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace BrzeBoljeJeftinije.Messenger.UI.Helpers
{
    /**
     * <summary>Helper klasa za manipulaciju slikama</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public static class ImageManipulation
    {
        public static readonly string OutputImageFormat = "image/jpeg";
        public static Image LoadImage(Stream img)
        {
            try
            {
                return Image.FromStream(img);
            }
            catch
            {
                return null;
            }
        }
        public static byte[] StoreImage(Image image)
        {
            using (var mStream = new MemoryStream())
            {
                image.Save(mStream, ImageFormat.Jpeg);
                return mStream.ToArray();
            }
        }
        public static Image ResizeImage(Image img, int maxWidth, int maxHeight)
        {
            if (img.Height < maxHeight && img.Width < maxWidth) return img;
            using (img)
            {
                Bitmap newImage = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(newImage))
                {
                    gr.Clear(Color.Transparent);
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    gr.DrawImage(img,
                        new Rectangle(0, 0, maxWidth, maxHeight),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                return newImage;
            }

        }
    }
}