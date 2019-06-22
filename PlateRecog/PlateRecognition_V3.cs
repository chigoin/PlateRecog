using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
namespace PlateRecog
{
    public class PlateRecognition_V3
    {
        public static List<PlateInfo> Recognite(Mat matSource)
        {
            List<PlateInfo> result = new List<PlateInfo>();
            List<PlateInfo> plateInfosLocate = PlateLocator_V3.LocatePlates(matSource);
            for (int index = 0; index < plateInfosLocate.Count; index++)
            {
                PlateInfo plateInfo = plateInfosLocate[index];
                PlateInfo plateInfoOfHandled = GetPlateInfoByMutilMethodAndMutilColor(plateInfo);
                if (plateInfoOfHandled != null)
                {
                    plateInfoOfHandled.PlateCategory = plateInfo.PlateCategory;
                    result.Add(plateInfoOfHandled);
                }
            }
            return result;
        }
        public static PlateInfo GetPlateInfoByMutilMethodAndMutilColor(PlateInfo plateInfo)
        {
            PlateInfo result = null;
            if (plateInfo.OriginalMat == null) return result;
            PlateInfo plateInfo_Blue = GetPlateInfoByMutilMethod(plateInfo, PlateColor.蓝牌);
            if (JudgePlateRightful(plateInfo_Blue) == true)
            {
                plateInfo_Blue.PlateColor = PlateColor.蓝牌;
                return plateInfo_Blue;
            }
            PlateInfo plateInfo_Yello = GetPlateInfoByMutilMethod(plateInfo, PlateColor.黄牌);
            if (JudgePlateRightful(plateInfo_Yello) == true)
            {
                plateInfo_Yello.PlateColor = PlateColor.黄牌;
                return plateInfo_Yello;
            }
            return result;
        }
        public static bool JudgePlateRightful(PlateInfo plateInfo)
        {
            if (plateInfo.CharInfos == null || plateInfo.CharInfos.Count == 0) return false;
            if (plateInfo.PlateColor == PlateColor.未知) return false;
            int charCount = 0;
            foreach (var charInfo in plateInfo.CharInfos)
            {
                if (charInfo.PlateChar != PlateChar.非字符)
{
                    charCount++;
                }
            }
            return (charCount >= 5);
        }
        public static PlateInfo GetPlateInfoByMutilMethod(PlateInfo plateInfo, PlateColor plateColor)
        {
            PlateInfo plateInfoByOriginal = GetPlateInfo(plateInfo, plateColor, CharSplitMethod.原图);
            PlateInfo plateInfoByGamma = GetPlateInfo(plateInfo, plateColor, CharSplitMethod.伽马);
            PlateInfo plateInfoByIndex = GetPlateInfo(plateInfo, plateColor, CharSplitMethod.指数);
            PlateInfo plateInfoByLog = GetPlateInfo(plateInfo, plateColor, CharSplitMethod.对数);
            List<PlateInfo> plateInfos = new List<PlateInfo>();
            if (plateInfoByOriginal.CharInfos != null && plateInfoByOriginal.CharInfos.Count != 0)
            {
                plateInfos.Add(plateInfoByOriginal);
            }
            if (plateInfoByGamma.CharInfos != null && plateInfoByGamma.CharInfos.Count != 0)
            {
                plateInfos.Add(plateInfoByGamma);
            }
            if (plateInfoByIndex.CharInfos != null && plateInfoByIndex.CharInfos.Count != 0)
            {
                plateInfos.Add(plateInfoByIndex);
            }
            if (plateInfoByLog.CharInfos != null && plateInfoByLog.CharInfos.Count != 0)
            {
                plateInfos.Add(plateInfoByLog);
            }
            if (plateInfos.Count == 0) return new PlateInfo();
            plateInfos.Sort(new PlateInfoComparer_DESC());
            PlateInfo result = plateInfos[0];
            return result;
        }
        public static PlateInfo GetPlateInfo(PlateInfo plateInfo, PlateColor plateColor, CharSplitMethod
        splitMethod)
        {
            PlateInfo result = new PlateInfo();
            result.PlateCategory = plateInfo.PlateCategory;
            result.OriginalMat = plateInfo.OriginalMat;
            result.OriginalRect = plateInfo.OriginalRect;
            result.PlateLocateMethod = plateInfo.PlateLocateMethod;
            result.PlateColor = plateColor;
            List<CharInfo> charInfos = new List<CharInfo>();
            //switch (splitmethod)
            //{
            //    case charsplitmethod.伽马:
            //        charinfos = charsegment_v3.spliteplatebygammatransform(plateinfo.originalmat,
            //        platecolor);
            //        break;
            //    case charsplitmethod.指数:
            //        charinfos = charsegment_v3.spliteplatebyindextransform(plateinfo.originalmat, platecolor);
            //        break;
            //    case charsplitmethod.对数:
            //        charinfos = charsegment_v3.spliteplatebylogtransform(plateinfo.originalmat, platecolor);
            //        break;
            //    case charsplitmethod.原图:
            //    default:
            //        charinfos = charsegment_v3.spliteplatebyoriginal(plateinfo.originalmat,
            //        plateinfo.originalmat, platecolor);
            //        break;
            //}
            for (int index = charInfos.Count - 1; index >= 0; index--)
            {
                CharInfo charInfo = charInfos[index];
                PlateChar plateChar = PlateChar_SVM.Test(charInfo.OriginalMat);
                if (plateChar == PlateChar.非字符)
{
                charInfos.RemoveAt(index);
            }
            charInfo.PlateChar = plateChar;
        }
        result.CharInfos = charInfos;
CheckLeftAndRightToRemove(result);
        CheckPlateColor(result);
return result;
}
    private static void CheckLeftAndRightToRemove(PlateInfo plateInfo)
    {
        if (plateInfo.PlateCategory == PlateCategory.非车牌) return;
        if (plateInfo.CharInfos == null) return;
        if (plateInfo.CharInfos.Count < 4) return;
        int charCount = plateInfo.CharInfos.Count;
        //两头的先除开，去掉中间的汉字
        for (int index = charCount - 1 - 1; index > 0 + 1; index--)
        {
            CharInfo charInfo = plateInfo.CharInfos[index];
            int charInfoValue = (int)charInfo.PlateChar;
            if (charInfoValue >= (int)PlateChar.京 && charInfoValue <= (int)PlateChar.川)
            {
                plateInfo.CharInfos.RemoveAt(index);
            }
        }
        charCount = plateInfo.CharInfos.Count;
        CharInfo second = plateInfo.CharInfos[1];
        int secondValue = (int)second.PlateChar;
        CharInfo lastSecond = plateInfo.CharInfos[charCount - 2];
        int lastSecondValue = (int)lastSecond.PlateChar;
        switch (plateInfo.PlateCategory)
        {
            case PlateCategory.普通车牌:
                    break;
            case PlateCategory.普通车牌_两行:
                    break;

            
        }
        charCount = plateInfo.CharInfos.Count;
        if (charCount < 7) return;
        CharInfo first = plateInfo.CharInfos[0];
        int firstValue = (int)first.PlateChar;
        second = plateInfo.CharInfos[1];
        secondValue = (int)second.PlateChar;
        CharInfo lastFirst = plateInfo.CharInfos[charCount - 1];
        int lastFirstValue = (int)lastFirst.PlateChar;
        switch (plateInfo.PlateCategory)
        {
            case PlateCategory.普通车牌:
                if (lastFirstValue >= (int)PlateChar.京 && lastFirstValue <= (int)PlateChar.川)
                {
                    plateInfo.CharInfos.RemoveAt(charCount - 1); //去掉最后⼀一位汉字
                }
                if (firstValue <= (int)PlateChar.点)
                {
                    plateInfo.CharInfos.Remove(first); //如果第⼀一位为⾮非汉字，删除
                }
                if (secondValue >= (int)PlateChar._0 && secondValue <= (int)PlateChar._9)
                {
                    plateInfo.CharInfos.Remove(second); //如果第⼆二位为数字，删除
                }
                break;
        }
    }
    private static void CheckPlateColor(PlateInfo plateInfo)
    {
        if (plateInfo.PlateCategory == PlateCategory.非车牌) return;
        switch (plateInfo.PlateCategory)
        {
            case PlateCategory.普通车牌:
                break;
            case PlateCategory.普通车牌_两行:
                plateInfo.PlateColor = PlateColor.黄牌;
                break;
        }
    }
    private class PlateInfoComparer_DESC : IComparer<PlateInfo>
    {
        public int Compare(PlateInfo x, PlateInfo y)
        {
            return y.CharInfos.Count.CompareTo(x.CharInfos.Count);
        }
    }
}
}