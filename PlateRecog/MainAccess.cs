using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;

namespace PlateRecog
{
    class MainAccess
    {
        static void Main(string[] args)
        {
            //训练车牌识别库
            //mainFunction.TrainSVMData();
            //Console.ReadKey();
            //利用训练库对测试集进行测试（效果貌似还不错）
            PlateCategory_SVM.Load(@"E:\工作文件夹（workplace）\VSworkplace\PlateRecog\plateRecog.xml");
            //string[] batchImgFiles = DataPreparingForSVM.getImgFiles(@"C:\Users\faiz\Desktop\AI\车牌-字符样本\车牌-字符样本\plates\测试集");

            //for (int index=0;index<batchImgFiles.Length;index++)
            //{
            //    Console.WriteLine("{0}", PlateCategory_SVM.Test(batchImgFiles[index]));
            //}
            //Console.ReadKey();
            //训练字符识别库
            //DataPreparingForSVM.TrainSVMDataForCharRecog();
            //Console.ReadKey();
            //利用字符训练库对测试集经行测试
            PlateChar_SVM.Load(@"E:\工作文件夹（workplace）\VSworkplace\PlateRecog\charRecog.xml");
            //string[] batchImgFiles = DataPreparingForSVM.getImgFiles(@"C:\Users\faiz\Desktop\AI\车牌-字符样本\车牌-字符样本\chars\测试集");

            //for (int index = 0; index < batchImgFiles.Length; index++)
            //{
            //    Console.WriteLine("{0}", PlateChar_SVM.Test(batchImgFiles[index]));
            //}
            //Console.ReadKey();
            //对最终的测试进行测试
            List<string> finalResult = PlateRevogTool.GetRecogFinalResult(@"C:\Users\faiz\Desktop\AI\img_source\12867_2019-03-28-08-09-51-086918.jpg");
            Console.WriteLine("hello");
            for(int index=0;index<finalResult.Count;index++)
            {
                Console.WriteLine("{0}", finalResult[index]);
            }
            Console.ReadKey();
        }




    }
}
