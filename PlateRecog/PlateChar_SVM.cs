using System;
using System.Drawing;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;

namespace PlateRecog
{   
    //定义字符识别类，使用SVM对字符进行识别
    public class PlateChar_SVM
    {
        public static bool IsReady = false;
        public static OpenCvSharp.Size HOGWinSize = new OpenCvSharp.Size(32, 96);
        public static OpenCvSharp.Size HOGBlockSize = new OpenCvSharp.Size(16, 16);
        public static OpenCvSharp.Size HOGBlockStride = new OpenCvSharp.Size(8, 8);
        public static OpenCvSharp.Size HOGCellSize = new OpenCvSharp.Size(8, 8);
        public static int HOGNBits = 9;
        private static SVM svm = null;
        private static Random random = new Random();
        static PlateChar_SVM()
        {
        }
        //通过HOG完成对图片特征的提取
        public static float[] ComputeHogDescriptors(Mat image)
        {
            Mat matToHog = image.Resize(HOGWinSize);
            HOGDescriptor hog = new HOGDescriptor(HOGWinSize,
            HOGBlockSize,
            HOGBlockStride,
            HOGCellSize,
            HOGNBits);
            return hog.Compute(matToHog, new OpenCvSharp.Size(1, 1), new OpenCvSharp.Size(0, 0));
        }
        //设置SVM对图片进行训练
        public static bool Train(Mat samples, Mat responses)
        {
            svm = SVM.Create();
            svm.Type = SVM.Types.CSvc;
            svm.KernelType = SVM.KernelTypes.Linear;
            svm.TermCriteria = new TermCriteria(CriteriaType.MaxIter, 10000, 1e-10);
            IsReady = true;
            return svm.Train(samples, SampleTypes.RowSample, responses);
        }
        //保存训练样本库
        public static void Save(string fileName)
        {
            if (IsReady == false || svm == null) return;
            svm.Save(fileName);
        }
        //加载字符识别库
        public static void Load(string fileName)
        {
            try
            {
                svm = SVM.Load(fileName);
                IsReady = true;
            }
            catch (Exception)
            {
                throw new Exception("字符识别库加载异常，请检查存放路路径");
            }
        }
        //
        public static bool IsCorrectTrainngDirectory(string path)
        {
            bool isCorrect = true;
            string[] plateCharNames = Enum.GetNames(typeof(PlateChar));
            for (int index = 0; index < plateCharNames.Length; index++)
            {
                string plateChar = plateCharNames[index];
                string charDirectory = string.Format(@"{0}\{1}", path, plateChar);
                isCorrect = Directory.Exists(charDirectory);
                if (isCorrect == false) break;
            }
            return isCorrect;
        }

        //
        public static PlateChar Test(Mat matTest)
        {
            //try
            //{
            if (IsReady == false || svm == null)
            {
                throw new Exception("训练数据为空，请重新训练字符或加载数据");
            }
            PlateChar result = PlateChar.非字符;
            float[] descriptor = ComputeHogDescriptors(matTest);
            Mat testDescriptor = Mat.Zeros(1, descriptor.Length, MatType.CV_32FC1);
            for (int index = 0; index < descriptor.Length; index++)
            {
                testDescriptor.Set<float>(0, index, descriptor[index]);
            }
            float predict = svm.Predict(testDescriptor);
            result = (PlateChar)((int)predict);
            return result;
            //}
            //catch (Exception ex)
            //{
            // throw ex;
            //}
        }
        public static PlateChar Test(string fileName)
        {
            Mat matTest = Cv2.ImRead(fileName, ImreadModes.Grayscale);
            return Test(matTest);
        }
        //保存字符样例
        public static void SaveCharSample(CharInfo charInfo, string libPath)
        {
            DateTime now = DateTime.Now;
            string time = string.Format("{0}-{1:00}-{2:00}-{3:00}-{4:00}-{5:00}-{6:000000}",
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second,
            random.Next(100000));
            string shortFileNameNoExt = string.Format("{0}_{1}_{2}",
            charInfo.PlateLocateMethod,
            charInfo.CharSplitMethod,
            time);
            SaveCharSample(charInfo.OriginalMat, charInfo.PlateChar, libPath, shortFileNameNoExt);
        }
        public static void SaveCharSample(Mat charMat, PlateChar plateChar, string libPath, string
        shortFileNameNoExt)
        {
            string fileName = string.Format(@"{0}\chars\{1}\{2}.jpg", libPath, plateChar, shortFileNameNoExt);
            charMat = charMat.Resize(HOGWinSize);
            charMat.SaveImage(fileName);
        }
        
        public static void SaveCharSample(Mat charMat, PlateChar plateChar, string libPath)
        {
            CharInfo charInfo = new CharInfo();
            charInfo.OriginalMat = charMat;
            charInfo.PlateChar = plateChar;
            SaveCharSample(charInfo, libPath);
        }
        //为字符串准备存储目录
        public static bool PrepareCharTrainningDirectory(string path)
        {
            bool success = true;
            try
            {
                success = Directory.Exists(path);
                string charsDirectory = path + @"\chars";
                if (Directory.Exists(charsDirectory) == false) Directory.CreateDirectory(charsDirectory);
                string[] plateCharNames = Enum.GetNames(typeof(PlateChar));
                for (int index_PlateChar = 0; index_PlateChar < plateCharNames.Length; index_PlateChar++)
                {
                    string plateChar = plateCharNames[index_PlateChar];
                    string plateCharDirectory = charsDirectory + @"\" + plateChar;
                    if (Directory.Exists(plateCharDirectory) == false)
                        Directory.CreateDirectory(plateCharDirectory);
                }
            }
            catch (IOException)
            {
                success = false;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
    }
}