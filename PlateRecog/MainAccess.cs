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
            MainAccess mainFunction = new MainAccess();
            //mainFunction.TrainSVMData();
            //Console.ReadKey();
            PlateCategory_SVM.Load(@"E:\工作文件夹（workplace）\VSworkplace\PlateRecog\plateRecog.xml");
            string[] batchImgFiles = mainFunction.getImgFiles(@"C:\Users\faiz\Desktop\AI\车牌-字符样本\车牌-字符样本\plates\测试集");
            
            for (int index=0;index<batchImgFiles.Length;index++)
            {
                Console.WriteLine("{0}", PlateCategory_SVM.Test(batchImgFiles[index]));
            }
            

            Console.ReadKey();

        }
        //获得当前路径下的所有文件
        string[] getImgFiles(string path)
        {
           
           string[] dirs = Directory.GetFiles(path, "*.JPG");
           return dirs;

        }
        //工具函数，用于将list转为array
         List<string> StringList2Array(string[] strs)
        {
            List<string> arrayStr = new List<string>();
            for (int i = 0; i < strs.Length; i++)
            {
                arrayStr.Add(strs[i]);
            }
            return arrayStr;
        }
        //得到所有正样本图片文件
         List<string> getPostiveSampleFiles(string path)
        {
            string[] imgFiles = getImgFiles(path);
            return StringList2Array(imgFiles);
         }
        //得到所有负样本图片文件
        List<string> getNegtiveSampleFiles(string path)
        {
            string[] imgFiles = getImgFiles(path);
            return StringList2Array(imgFiles);
        }
        //定义正负样本结构体，用于给图片打上标签
        struct TrainStruct
        {
            public string file;//正负样本路径
            public int label;
        }

        //得到图片的HOG特征
        Mat GetSvmHOGFeatures(Mat img)
        {
           
            float[] descriptor = PlateCategory_SVM.ComputeHogDescriptors(img);
            Mat feature = Float2Mat(descriptor);
            return feature;

        }

        void TrainSVMData()
        {
            Console.WriteLine("preparing for training data!!!!");
            List<TrainStruct> svmData = new List<TrainStruct>();
            Mat srcMat;
            List<string> posImgFiles = getPostiveSampleFiles(@"E:\工作文件夹（workplace）\VSworkplace\PlateRecog\PlateRecog\普通车牌");
            List<string> negImgFiles = getNegtiveSampleFiles(@"E:\工作文件夹（workplace）\VSworkplace\PlateRecog\PlateRecog\非车牌");
            //添加将正样本数据
            for (int index=0;index<posImgFiles.Count;index++)
            {
                TrainStruct trainData;
                trainData.file = posImgFiles[index];
                trainData.label = 1;
                svmData.Add(trainData);
            }
            //添加负样本数据
            for (int index = 0; index < negImgFiles.Count; index++)
            {
                TrainStruct trainData;
                trainData.file = negImgFiles[index];
                trainData.label = -1;
                svmData.Add(trainData);
            }
            Mat samples=new Mat();
            Mat responses = new Mat();
            //读取数据并进行处理
            for(int index=0;index<svmData.Count;index++)
            {
                //以灰度的方式读取图片
                Mat img = Cv2.ImRead(svmData[index].file, ImreadModes.Grayscale);
                //剔除无用数据
                if(img.Data==null)
                {
                    Console.WriteLine("failed to load image {0}", svmData[index].file);
                }
                //对图片进行二值化
                Mat dst = new Mat();
                Cv2.Threshold(img, dst, 0, 255, ThresholdTypes.Otsu);
                Mat feature = GetSvmHOGFeatures(dst);
                //获取HOG特征
                feature = feature.Reshape(1, 1);
                samples.PushBack(feature);
                responses.PushBack(Int2Mat(svmData[index].label));
            }
            // 训练数据的格式，OpenCV规定 samples 中的数据都是需要32位浮点型
            // 因为TrainData::create 第一个参数是规定死的要cv_32F
            samples.ConvertTo(samples, MatType.CV_32F);
            // samples 将图片和样本标签合并成为一个训练集数据
            // 第二个参数的原因是，我们的samples 中的每个图片数据的排列都是一行
            if (PlateCategory_SVM.Train(samples, responses))
            {
                Console.WriteLine("Traing!!!");
                PlateCategory_SVM.Save(@"E:\工作文件夹（workplace）\VSworkplace\PlateRecog\plateRecog.xml");
            }
            else
            {
                Console.WriteLine("failed to train data");

            }


        }
        //float 数组转为mat
        Mat Float2Mat(float[] list)
        {
            Mat resultMat = Mat.Zeros(1,list.Length, MatType.CV_32FC1);
            for (int index = 0; index < list.Length; index++)
            {
                resultMat.Set<float>(0, index, list[index]);
            }
            return resultMat;
        }
        
        Mat Int2Mat(int integer)
        {
            Mat resultMat = Mat.Zeros(1,1 , MatType.CV_32SC1);
            
            resultMat.Set<int>(0, 0, integer);
            return resultMat;
        }
    

    }
}
