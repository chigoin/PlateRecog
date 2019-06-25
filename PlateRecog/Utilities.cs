using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenCvSharp;

namespace PlateRecog
{
    class Utilities
    {
        static Utilities()
        {
        }

        //得到安全的矩形
        public static Rect GetSafeRect(Rect rect, Mat m)
        {
            Rect roi = rect;
            Rect nullRect = new Rect();
            nullRect.Width = 0;
            nullRect.Height = 0;
            nullRect.Left = 0;
            nullRect.Top = 0;
            if (!(0 <= roi.X && 0 <= roi.Width && roi.X + roi.Width <= m.Cols && 0 <= roi.Y && 0 <= roi.Height && roi.Y + roi.Height <= m.Rows))
            {
                return nullRect;
            }
            
            if (roi.Left < 0)
                roi.Left = 0;
            if (roi.Top < 0)
                roi.Top = 0;
            if (roi.Right > m.Cols)
                roi.Width = m.Cols - roi.Left;
            if (roi.Bottom > m.Rows)
                roi.Height = m.Rows - roi.Top;
            return roi;
        }

        public static List<Rect> GetSafeRects(Mat m, List<Rect> rects)
        {

            List<Rect> result = new List<Rect>();
            for (int index = 0; index < rects.Count; index++)
            {
                Rect roi = rects[index];
                if (!(0 <= roi.X && 0 <= roi.Width && roi.X + roi.Width <= m.Cols && 0 <= roi.Y && 0 <= roi.Height && roi.Y + roi.Height <= m.Rows))
                {
                    continue;
                }
                if (roi.Left < 0)
                    roi.Left = 0;
                if (roi.Top < 0)
                    roi.Top = 0;
                if (roi.Right > m.Cols)
                    roi.Width = m.Cols - roi.Left;
                if (roi.Bottom > m.Rows)
                    roi.Height = m.Rows - roi.Top;

                result.Add(roi);
            }
            return result;
        }

        //拉普拉斯增强
        public static Mat LaplaceTransform(Mat source)
        {
            Mat kernel = new Mat(3, 3, MatType.CV_32FC1);
            kernel.Set<float>(0, 0, 0);
            kernel.Set<float>(0, 1, -1);
            kernel.Set<float>(0, 2, 0);
            kernel.Set<float>(1, 0, 0);
            kernel.Set<float>(1, 1, 5);
            kernel.Set<float>(1, 2, 0);
            kernel.Set<float>(2, 0, 0);
            kernel.Set<float>(2, 1, -1);
            kernel.Set<float>(2, 2, 0);

            Mat result = source.Filter2D(MatType.CV_8UC3, kernel);

            return result;
        }

        //指数变换
        public static Mat IndexTransform(Mat source)
        {
            Mat result = new Mat(source.Size(), source.Type());
            int rows = source.Rows;
            int cols = source.Cols;

            double k = 1 / 255f;
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < cols; colIndex++)
                {
                    Vec3b color = source.At<Vec3b>(rowIndex, colIndex);
                    byte B = color[0];
                    byte G = color[1];
                    byte R = color[2];

                    B = (byte)(k * B * B);
                    G = (byte)(k * G * G);
                    R = (byte)(k * R * R);
                    color = new Vec3b(B, G, R);
                    result.Set<Vec3b>(rowIndex, colIndex, color);
                }
            }
            return result;
        }

        //对数变换
        public static Mat LogTransform(Mat source)
        {
            Mat result = new Mat(source.Size(), source.Type());
            int rows = source.Rows;
            int cols = source.Cols;

            double k = 255 / Math.Log10(256.0);
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < cols; colIndex++)
                {
                    Vec3b color = source.At<Vec3b>(rowIndex, colIndex);
                    byte B = color[0];
                    byte G = color[1];
                    byte R = color[2];

                    B = (byte)(k * Math.Log10(B + 1));
                    G = (byte)(k * Math.Log10(G + 1));
                    R = (byte)(k * Math.Log10(R + 1));
                    color = new Vec3b(B, G, R);
                    result.Set<Vec3b>(rowIndex, colIndex, color);
                }
            }

            return result;
        }


        //伽马变换
        public static Mat GammaTransform(Mat source, float gammaFactor = 0.4f)
        {
            int[] lut = new int[256];
            for (int index = 0; index < 256; index++)
            {
                float f = (index + 0.5f) / 255;
                f = (float)Math.Pow(f, gammaFactor);
                lut[index] = (int)(f * 255.0f - 0.5f);
                if (lut[index] > 255) lut[index] = 255;
            }

            Mat result = source.Clone();
            if (source.Channels() == 1)
            {
                for (int rowIndex = 0; rowIndex < result.Rows; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < result.Cols; colIndex++)
                    {
                        int temp = result.At<int>(rowIndex, colIndex);
                        result.Set<int>(rowIndex, colIndex, lut[temp]);
                    }
                }
            }
            else
            {
                for (int rowIndex = 0; rowIndex < result.Rows; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < result.Cols; colIndex++)
                    {
                        Vec3b temp = result.At<Vec3b>(rowIndex, colIndex);
                        result.Set<Vec3b>(rowIndex, colIndex, new Vec3b(
                         (byte)lut[temp[0]],
                         (byte)lut[temp[1]],
                         (byte)lut[temp[2]]));
                    }
                }
            }

            return result;
        }


    }
}
