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
        public static Rect GetSafeRect(Rect rect, Mat mat)
        {
            if (rect.Left < 0)
                rect.Left = 0;
            else if (rect.Top < 0)
                rect.Top = 0;
            else if (rect.Width > mat.Width)
                rect.Width = mat.Width;
            else if (rect.Height > mat.Height)
                rect.Height = mat.Height;

            return rect;
        }

        //指数变换
        public static Mat IndexTransform(Mat mat)
        {

            if (mat.Empty())
                return mat;
            Mat result = Mat.Zeros(mat.Size(), mat.Type());


            return mat;
        }

        //对数变换
        public static Mat LogTransform(Mat mat)
        {
            int c = 5;

            if (mat.Empty())
                return mat;
            Mat result = Mat.Zeros(mat.Size(), mat.Type());
            //图像+1取对数
            Cv2.Add(mat, new Scalar(1.0), mat);
            //灰度归一化
            mat.ConvertTo(mat, MatType.CV_32F);
            Cv2.Log(mat, result);
            //图像增强
            result = c * result;
            //归一化到0~255，输入result，输出result_image
            Cv2.Normalize(result, result, 0, 255, NormTypes.MinMax);
            //转化成8bit图像显示
            Cv2.ConvertScaleAbs(result, result);

            return result;
        }

        //伽马变换
        public static Mat GammaTransform(Mat mat, float gammaFactor)
        {
            if (mat.Empty())
                return mat;

            mat.ConvertTo(mat, MatType.CV_64F, 1.0 / 255.0);

            Mat result = Mat.Zeros(mat.Size(), mat.Type());
            Cv2.Pow(mat, gammaFactor, result);

            //归一化到0~255，输入result，输出result_image
            Cv2.Normalize(result, result, 0, 255, NormTypes.MinMax);
            //转化成8bit图像显示
            Cv2.ConvertScaleAbs(result, result);

            return result;
        }

    }
}
