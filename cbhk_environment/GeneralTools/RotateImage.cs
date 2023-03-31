using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace cbhk_environment.GeneralTools
{
    public class RotateImage
    {
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public static void BitmapMirror(Bitmap curBitmap, int width, int height, int direction)
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = curBitmap.LockBits(rect, ImageLockMode.ReadWrite, curBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = width * height * 4;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            byte temp;
            if (direction == 0)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < halfWidth; j++)
                    {
                        int index1 = i * width * 4 + 4 * j;               // B
                        int index2 = (i + 1) * width * 4 - (1 + j) * 4;
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                        index1 = i * width * 4 + 4 * j + 1;               // G
                        index2 = (i + 1) * width * 4 - (1 + j) * 4 + 1;
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                        index1 = i * width * 4 + 4 * j + 2;               // R
                        index2 = (i + 1) * width * 4 - (1 + j) * 4 + 2;
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                        index1 = i * width * 4 + 4 * j + 3;               // A
                        index2 = (i + 1) * width * 4 - (1 + j) * 4 + 3;
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                    }
                }
            }
            else
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < halfHeight; j++)
                    {
                        int index1 = j * width * 4 + i * 4;
                        int index2 = (height - j - 1) * width * 4 + i * 4;    // B
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                        index1 = j * width * 4 + i * 4 + 1;
                        index2 = (height - j - 1) * width * 4 + i * 4 + 1;    // G
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                        index1 = j * width * 4 + i * 4 + 2;
                        index2 = (height - j - 1) * width * 4 + i * 4 + 2;    // R
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                        index1 = j * width * 4 + i * 4 + 3;
                        index2 = (height - j - 1) * width * 4 + i * 4 + 3;    // A
                        temp = rgbValues[index1];
                        rgbValues[index1] = rgbValues[index2];
                        rgbValues[index2] = temp;
                    }
                }
            }
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            curBitmap.UnlockBits(bmpData);
        }
    }
}
