using System.Collections.Generic;
using System.Windows;

namespace cbhk_environment.GeneralTools
{
    public class MosaicHelper
    {
        /// <summary>
        /// 马赛克处理
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="effectWidth"> 影响范围 每一个格子数 </param>
        public static List<ImageParticle> AdjustTobMosaic(System.Drawing.Bitmap bitmap, int effectWidth)
        {
            List<ImageParticle> particleList = new List<ImageParticle>();

            for (int heightOfffset = 0; heightOfffset < bitmap.Height; heightOfffset += effectWidth)
            {
                for (int widthOffset = 0; widthOffset < bitmap.Width; widthOffset += effectWidth)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    for (int x = widthOffset; (x < widthOffset + effectWidth && x < bitmap.Width); x++)
                    {
                        for (int y = heightOfffset; (y < heightOfffset + effectWidth && y < bitmap.Height); y++)
                        {
                            System.Drawing.Color pixel = bitmap.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    for (int x = widthOffset; (x < widthOffset + effectWidth && x < bitmap.Width); x += effectWidth)
                    {
                        for (int y = heightOfffset; (y < heightOfffset + effectWidth && y < bitmap.Height); y += effectWidth)
                        {
                            particleList.Add(new ImageParticle { Position = new Point(x, y), Size = CalculateSize(avgR, avgG, avgB, effectWidth) });
                        }
                    }
                }
            }
            return particleList;
        }

        /// <summary>
        /// 计算粒子大小
        /// </summary>
        public static double CalculateSize(int avgR, int avgG, int avgB, int effectWidth)
        {
            return (255 - (avgR * 0.299 + avgG * 0.587 + avgB * 0.114)) / 255 * effectWidth;
        }
    }

    public class ImageParticle
    {
        public Point Position;//位置
        public double Size;//尺寸
    }
}
