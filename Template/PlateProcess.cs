using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace Template
{
    public enum PlateCategory
    {
        普通车牌=0,
        非车牌=1,
        普通车牌_两行=2,
        
    }
    public enum PlateColor
    {
        蓝牌 = 0,
        黄牌 = 1,
        未知 = 6
    }
    public enum PlateChar
    {
        非字符 = 0,
        穗 = 1,
        A = 2,
        B = 3,
        C = 4,
        D = 5,
        E = 6,
        F = 7,
        G = 8,
        H = 9,
        I = 10,
        J = 11,
        K = 12,
        L = 13,
        M = 14,
        N = 15,
        O = 16,
        P = 17,
        Q = 18,
        R = 19,
        S = 20,
        T = 21,
        U = 22,
        V = 23,
        W = 24,
        X = 25,
        Y = 26,
        Z = 27,
        _0 = 28,
        _1 = 29,
        _2 = 30,
        _3 = 31,
        _4 = 32,
        _5 = 33,
        _6 = 34,
        _7 = 35,
        _8 = 36,
        _9 = 37,
        点 = 38,
        京 = 39,
        津 = 40,
        沪 = 41,
        渝 = 42,
        蒙 = 43,
        新 = 44,
        藏 = 45,
        宁 = 46,
        桂 = 47,
        港 = 48,
        澳 = 49,
        黑 = 50,
        吉 = 51,
        辽 = 52,
        晋 = 53,
        冀 = 54,
        青 = 55,
        鲁 = 56,
        豫 = 57,
        苏 = 58,
        皖 = 59,
        浙 = 60,
        闽 = 61,
        赣 = 62,
        湘 = 63,
        鄂 = 64,
        粤 = 65,
        琼 = 66, 
        甘 = 67,
        陕 = 68,
        贵 = 69,
        云 = 70,
        川 = 71,
        
    }
    public enum PlateLocateMethod
    {
        未知 = 0,
        颜色法_蓝黑 = 1,
        颜色法_黄白 = 2,
        颜色法 = 3,
        Sobel法 = 4,
        MSER法 = 5
    }
    public enum CharSplitMethod
    {
        未知 = 0,
        原图 = 1,
        伽马 = 2,
        指数 = 3,
        对数 = 4
    }

    //管理字符图片的“类”
    public struct CharImage
    {
        public string FileName;
        public string Name;
        public PlateChar PlateChar;
        public OpenCvSharp.Size MatSize;
        public CharImage(string fileName, string name, PlateChar plateChar, OpenCvSharp.Size matSize)
        {
            this.FileName = fileName;
            this.Name = name;
            this.PlateChar = plateChar;
            this.MatSize = matSize;
        }
    }

    //管理车牌文件的“类”
    public struct PlateImage
    {
        public string FileName;
        public string Name;
        public PlateCategory PlateCategory;
        public OpenCvSharp.Size MatSize;
        public PlateImage(string fileName, string name, PlateCategory plateCategory, OpenCvSharp.Size
        matSize)
        {
            this.FileName = fileName;
            this.Name = name;
            this.PlateCategory = plateCategory;
            this.MatSize = matSize;
        }
    }
    public class CharInfo
    {
        public PlateChar PlateChar;
        public Mat OriginalMat;
        public Rect OriginalRect;
        public PlateLocateMethod PlateLocateMethod;
        public CharSplitMethod CharSplitMethod;
        public string Info
        {
            get
            {
                return string.Format("字符:{0} \r\n宽:{1} \r\n⾼高:{2} \r\n宽⾼高⽐比:{3:0.00} \r\n左:{4} \r\n右:{5} \r\n上:{6}\r\n下:{7} \r\n⻋车牌定位:{8} \r\n字符切分:{9} \r\n",
               this.PlateChar,
               this.OriginalRect.Width,
               this.OriginalRect.Height,
               (float)this.OriginalRect.Width / this.OriginalRect.Height,
               this.OriginalRect.Left,
               this.OriginalRect.Right,
               this.OriginalRect.Top,
               this.OriginalRect.Bottom,
               this.PlateLocateMethod,
               this.CharSplitMethod);
            }
        }
        public CharInfo() { }
        public CharInfo(PlateChar plateChar, Mat originalMat, Rect originalRect,
        PlateLocateMethod plateLocateMethod,
        CharSplitMethod charSplitMethod)
        {
            this.PlateChar = plateChar;
            this.OriginalMat = originalMat;
            this.OriginalRect = originalRect;
            this.PlateLocateMethod = plateLocateMethod;
            this.CharSplitMethod = charSplitMethod;
        }
        public override string ToString()
        {
            return this.PlateChar.ToString().Replace("_", ""); ;
        }
    }
    public class PlateInfo
    {
        public PlateCategory PlateCategory;
        public PlateColor PlateColor = PlateColor.未知;
        public RotatedRect RotatedRect;
        public Rect OriginalRect;
        public Mat OriginalMat;
        public PlateLocateMethod PlateLocateMethod;
        public List<CharInfo> CharInfos;
        public string Info
        {
            get
            {
                return string.Format("类型:{0} \r\n颜⾊色:{1} \r\n字符:{2} \r\n宽:{3} \r\n⾼高:{4} \r\n宽⾼高⽐比:{5:0.00} \r\n左:{6} \r\n右: {7} \r\n上: {8} \r\n下: {9} \r\n⻋车牌定位: {10}\r\n",
                this.PlateCategory,
                this.PlateColor,
                this.ToString(),
                this.OriginalRect.Width,
                this.OriginalRect.Height,
                (float)this.OriginalRect.Width / this.OriginalRect.Height,
                this.OriginalRect.Left,
                this.OriginalRect.Right,
                this.OriginalRect.Top,
                this.OriginalRect.Bottom,
                this.PlateLocateMethod);
            }
        }
        public PlateInfo() { }
        public PlateInfo(PlateCategory plateCategory,
        Rect originalRect, Mat originalMat,
        List<CharInfo> charInfos,
        PlateLocateMethod plateLocateMethod)
        {
            this.PlateCategory = plateCategory;
            this.OriginalRect = originalRect;
            this.OriginalMat = originalMat;
            this.CharInfos = charInfos;
            this.PlateLocateMethod = plateLocateMethod;
        }
        public override string ToString()
        {
            if (this.CharInfos == null)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append(this.PlateCategory.ToString());
            //stringBuilder.Append(" ");
            foreach (CharInfo charInfo in this.CharInfos)
            {
                stringBuilder.Append(charInfo.ToString());
            }
            string result = stringBuilder.ToString();
            result = result.Replace("⾮非字符", "");
            return result;
        }
    }
}