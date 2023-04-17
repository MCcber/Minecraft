using System.Drawing;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;

namespace cbhk_environment.GeneralTools
{
    public class ChangeBitmapSize
    {
        /// <summary>
        /// 放大图形
        /// </summary>
        /// <param name="p_Bitmap">图形</param>
        /// <param name="p_Width">放大后的宽</param>
        /// <param name="p_Height">放大后高</param>
        /// <param name="p_ZoomType">放大类型 true为插值</param>
        /// <returns>放大后的图形</returns>
        public static Bitmap Magnifier(Bitmap srcbitmap, int multiple)
        {
            Bitmap bitmap = new(srcbitmap.Size.Width * multiple, srcbitmap.Size.Height * multiple);
            if (multiple <= 0) { multiple = 0; return srcbitmap; }
            BitmapData srcbitmapdata = srcbitmap.LockBits(new Rectangle(new Point(0, 0), srcbitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* srcbyte = (byte*)(srcbitmapdata.Scan0.ToPointer());
                byte* sourcebyte = (byte*)(bitmapdata.Scan0.ToPointer());
                for (int y = 0; y < bitmapdata.Height; y++)
                {
                    for (int x = 0; x < bitmapdata.Width; x++)
                    {
                        long index = (x / multiple) * 4 + (y / multiple) * srcbitmapdata.Stride;
                        sourcebyte[0] = srcbyte[index];
                        sourcebyte[1] = srcbyte[index + 1];
                        sourcebyte[2] = srcbyte[index + 2];
                        sourcebyte[3] = srcbyte[index + 3];
                        sourcebyte += 4;
                    }
                }
            }

            srcbitmap.UnlockBits(srcbitmapdata);
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }
    }
}
