using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;

//将老师源码中能够用到的API进行封装
namespace PlateRecog
{
    class PlateRevogTool
    {  

        //
        public static List<string> GetRecogFinalResult(string imgPath)
        {
            Mat imgMat = Cv2.ImRead(imgPath, ImreadModes.Color);
            //Cv2.ImShow("debug", imgMat);
            //Cv2.WaitKey();
           if(imgMat==null)
            {
                Console.WriteLine("载入图片出现问题");
            }
            List<PlateInfo> allPlateInfos = RecogPlateAndChar(imgMat);
            List<string> allPlateContent = PrintPlateCharsAfterRecog(allPlateInfos);
            return allPlateContent;
        }
        //返回图片处理后的后识别的车牌内容
        public static List<string> PrintPlateCharsAfterRecog(List<PlateInfo> plateInfoList)
        {
            List<string> plateCharsInfo = new List<string>();
            for(int index=0;index<plateInfoList.Count;index++)
            {
                List<CharInfo> charInfos = plateInfoList[index].CharInfos;
                string plateContent = "";
                plateContent = CharInfo2String(charInfos);
                plateCharsInfo.Add(plateContent);

            }
            return plateCharsInfo;
        }
        //将车牌中的字符信息转为字符串（便于打印操作）
        public static string CharInfo2String(List<CharInfo> charInfos)
        {
            string plateContent = "";
            
            for(int index=0;index<charInfos.Count;index++)
            {
                plateContent = plateContent + charInfos[index].PlateChar;
                
            }
            return plateContent;
        }

        
        public static List<PlateInfo> RecogPlateAndChar(Mat matSource)
        {
            List<PlateInfo> PlateInfoList = PlateRecognition_V3.Recognite(matSource);
            if(PlateInfoList.Count==0)
            {
                 PlateCategory PlateCategory=PlateCategory.非车牌;
                 
                 Rect OriginalRect=new Rect();
                 Mat OriginalMatnew = new Mat();
                 PlateLocateMethod PlateLocateMethod=PlateLocateMethod.未知;
                 List<CharInfo> CharInfos=null;

                 PlateInfo noPlateInfo = new PlateInfo(PlateCategory,OriginalRect,OriginalMatnew,CharInfos,PlateLocateMethod);
                 PlateInfoList.Add(noPlateInfo);
                 return PlateInfoList; 
                 
            }
            else
            {
                return PlateInfoList;

            }

        }


     
    }
}
