using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Drawing;


using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;




namespace PlateRecog
{
    class TemplateRecogition
    {
        public static bool IsReady = false;
        public static OpenCvSharp.Size HOGWinSize = new OpenCvSharp.Size(96, 32);
        public static OpenCvSharp.Size HOGBlockSize = new OpenCvSharp.Size(16, 16);
        public static OpenCvSharp.Size HOGBlockStride = new OpenCvSharp.Size(8, 8);
        public static OpenCvSharp.Size HOGCellSize = new OpenCvSharp.Size(8, 8);
        public static int HOGNBits = 9;
        private static SVM svm = null;
        private static Random random = new Random();

        static TemplateRecogition()
        {
        }

        public static void SavePlateSample(PlateInfo plateInfo, string fileName)
        {
            plateInfo.OriginalMat.SaveImage(fileName);
        }

    }
}
