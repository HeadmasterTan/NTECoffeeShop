using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NLog;
using Point = System.Drawing.Point;
using static NTECoffeeShop.CoffeeShop.ImageHandler;

namespace NTECoffeeShop.CoffeeShop
{
    // 带文字的模板更新为无需匹配，直接获取中心点。
    enum ETemplateName
    {
        LoginPageAnnouncement, // 登录界面公告图标
        LoginPageAnnouncementLight, // 登录界面公告图标
        MoonCard, // 月卡图标。
        EnterFKey,

        GameStartTips, // 游戏开始的提示，无需匹配模板。
        GameClearFlag,
        GameChapterTextArea,
        GameCompleteStar, // 游戏完成的提示。
        GameFailedStar, // 游戏失败的提示。
        GameResultEnergy, // 都市活力图标。
        GameResultEnergyEmpty,

        EndSceneBackButton,
        EndSceneConfirmButton,

        StartSceneOpen,
        StartSceneDarkStar,
        StartSceneStageList,
        StartSceneHalfStageList,
        StartSceneLastStage,
        StartSceneSecondToLastStage,
        StartSceneFirstStage,
        StartSceneStartButton,

        ExitButton, // 退出按钮，无需匹配模板。
        RestartOrConfirmButton, // 重新开始或确认按钮，无需匹配模板。

        EmptyCoffee,
        EmptyCroissant,
        EmptyCupCake,
        EmptyToast,
        CuttingBoard,
        CoffeeMachine, // 咖啡机。

        OriginToast, // 吐司，无需匹配模板。
        CutToast, // 切片吐司。
        OriginCroissant, // 可颂，无需匹配模板。
        CutCroissant, // 切片可颂。

        PipingBag, // 裱花袋，无需匹配模板。
        CupCake, // 杯子蛋糕。

        OriCoffee, // 原味咖啡。
        TeaCup, // 茶杯，无需匹配模板。
        GlassCup, // 玻璃杯，无需匹配模板。

        OrderArea, // 订单区域，无需匹配模板。

        BeafOnion, // 牛肉洋葱。
        CheeseHam, // 芝士火腿。
        CheesePomegranate, // 奶酪石榴。
        FriedChicken, // 炸鸡块。
        TomatoEgg, // 番茄鸡蛋。
        TomatoHam, // 番茄火腿。

        AppleSauce, // 苹果酱。
        CheeseSauce, // 奶酪酱。
        CandyStick, // 糖果棒。
        ChocolateChips, // 巧克力豆。
        StrawberryJam, // 草莓酱。

        CheeseMilk, // 奶酪牛奶。
        ChocolateCream, // 巧克力奶油。
        ChocolateJam, // 巧克力酱。
        CookieSugar, // 糖果饼干。
        DriedTangerine, // 橘子干。
        StrawberrySugar, // 草莓糖。
        StrawberryGuacamole, // 草莓牛油果酱。

        CheeseMilkCoffee, // 奶酪牛奶咖啡。
        ChocolateCreamCoffee, // 巧克力奶油咖啡。
        ChocolateJamCoffee, // 巧克力酱咖啡。
        CookieSugarCoffee, // 糖果饼干咖啡。
        StrawberrySugarCoffee, // 草莓糖咖啡。
        DriedTangerineCoffee, // 橘子干咖啡。
        StrawberryGuacamoleCoffee, // 草莓牛油果酱咖啡。

        AppleSauceCake, // 苹果酱蛋糕。
        CheeseSauceCake, // 奶酪酱蛋糕。
        CandyStickCake, // 糖果棒蛋糕。
        ChocolateChipsCake, // 巧克力豆蛋糕。
        StrawberryJamCake, // 草莓酱蛋糕。

        BeafOnionToast, // 牛肉洋葱吐司。
        CheeseHamCroissant, // 芝士火腿可颂。
        CheesePomegranateCroissant, // 奶酪石榴可颂。
        FriedChickenToast, // 炸鸡块吐司。
        TomatoEggCroissant, // 番茄鸡蛋可颂。
        TomatoHamToast, // 番茄火腿吐司。

        TuanSanLang, // 团三郎，无需匹配模板。
        Hammer,
    }

    internal class TemplateController
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private const string TEMPLATE_PATH = "./Resources/Images/";
        private static int _resolutionLevel;
        public static RECT _curWindowRect;

        // 以下是游戏内的几种分辨率。
        // 2560/1600 (1.6)
        // 1920/1080 (1.7)
        // 1792/768  (2.4)
        // 3840/1080 (3.5)
        private static double _curAspectRatio = 0;

        public static int ResolutionLevel => _resolutionLevel;

        public static void InitRatio(RECT windowRect)
        {
            _curWindowRect = windowRect;

            double width = windowRect.Right - windowRect.Left;
            double height = windowRect.Bottom - windowRect.Top;
            _curAspectRatio = width / height;

            Log.Info("【InitRatio】游戏当前分辨率：宽 = {0}, 高 = {1}", width, height);

            double ratio = GetSimilarityRatio();
            if (ratio == 0)
            {
                string msg = $"【InitRatio】游戏当前分辨率不符合要求\n请将游戏设置成[1920x1080]或[1280x720]分辨率";
                //string msg = $"【InitRatio】游戏当前分辨率不符合要求\n请选择符合[16:9] [16:10] [24:10] [35:10]宽高比的分辨率";
                Log.Error($"【InitRatio】游戏当前分辨率不符合要求");
                throw new Exception(msg);
            }
        }

        public static double GetSimilarityRatio()
        {
            double result = Math.Abs(_curAspectRatio - 1.6);
            if (result <= 0.1) return 1.6;

            result = Math.Abs(_curAspectRatio - 1.7);
            if (result <= 0.1) return 1.7;

            result = Math.Abs(_curAspectRatio - 2.4);
            if (result <= 0.1) return 2.4;

            result = Math.Abs(_curAspectRatio - 3.5);
            if (result <= 0.1) return 3.5;

            return 0;
        }

        private static Rectangle CalculateTemplateRect(double x, double y, double width, double height, double ratioWidth = 1920.0, double ratioHeight = 1080.0)
        {
            y -= 39; // 适配标题栏高度。

            int windowWidth = _curWindowRect.Right - _curWindowRect.Left;
            int windowHeight = _curWindowRect.Bottom - _curWindowRect.Top;
            int rectX = (int)(windowWidth * (x / ratioWidth));
            int rectY = (int)(windowHeight * (y / ratioHeight));
            int rectWidth = (int)(windowWidth * (width / ratioWidth));
            int rectHeight = (int)(windowHeight * (height / ratioHeight));

            return new Rectangle(rectX, rectY, rectWidth, rectHeight);
        }

        /// <summary>
        /// 分辨率16/9
        /// </summary>
        private static Rectangle GetNormalRatioRect(ETemplateName templateName)
        {
            switch (templateName)
            {
                case ETemplateName.EnterFKey:
                    return CalculateTemplateRect(1130, 555, 98, 79);
                case ETemplateName.GameStartTips:
                    return CalculateTemplateRect(915, 212, 88, 65);
                case ETemplateName.GameClearFlag:
                    return CalculateTemplateRect(1800, 197, 56, 179);
                case ETemplateName.GameChapterTextArea:
                    return CalculateTemplateRect(387, 160, 211, 87);
                case ETemplateName.GameCompleteStar:
                case ETemplateName.GameFailedStar:
                    return CalculateTemplateRect(866, 297, 190, 71);
                case ETemplateName.StartSceneOpen:
                    return CalculateTemplateRect(7, 54, 85, 70);
                case ETemplateName.StartSceneDarkStar:
                    return CalculateTemplateRect(91, 155, 160, 932);
                case ETemplateName.StartSceneStageList:
                    return CalculateTemplateRect(0, 118, 350, 967);
                case ETemplateName.StartSceneHalfStageList:
                    return CalculateTemplateRect(0, 157, 349, 887);
                case ETemplateName.StartSceneLastStage:
                    return CalculateTemplateRect(0, 925, 349, 169);
                case ETemplateName.StartSceneSecondToLastStage:
                    return CalculateTemplateRect(0, 806, 349, 169);
                case ETemplateName.StartSceneFirstStage:
                    return CalculateTemplateRect(0, 212, 202, 150);
                case ETemplateName.StartSceneStartButton:
                    return CalculateTemplateRect(1584, 1003, 271, 80);
                case ETemplateName.EndSceneBackButton:
                    return CalculateTemplateRect(605, 833, 311, 89);
                case ETemplateName.EndSceneConfirmButton:
                    return CalculateTemplateRect(1009, 827, 307, 96);
                case ETemplateName.ExitButton:
                    return CalculateTemplateRect(608, 831, 308, 89);
                case ETemplateName.RestartOrConfirmButton:
                    return CalculateTemplateRect(1007, 831, 308, 89);
                case ETemplateName.GameResultEnergy:
                    return CalculateTemplateRect(1071, 914, 200, 48);
                case ETemplateName.GameResultEnergyEmpty:
                    return CalculateTemplateRect(1038, 903, 247, 65);
                case ETemplateName.EmptyCoffee:
                    return CalculateTemplateRect(1631, 967, 280, 152);
                case ETemplateName.EmptyCupCake:
                    return CalculateTemplateRect(1122, 985, 307, 134);
                case ETemplateName.EmptyCroissant:
                    return CalculateTemplateRect(482, 768, 327, 156);
                case ETemplateName.EmptyToast:
                    return CalculateTemplateRect(0, 763, 308, 158);
                case ETemplateName.CuttingBoard:
                    return CalculateTemplateRect(323, 964, 213, 134);
                case ETemplateName.OriginToast:
                    return CalculateTemplateRect(28, 968, 202, 124);
                case ETemplateName.CutToast:
                    return CalculateTemplateRect(0, 784, 100, 97);
                case ETemplateName.OriginCroissant:
                    return CalculateTemplateRect(621, 973, 232, 139);
                case ETemplateName.CutCroissant:
                    return CalculateTemplateRect(503, 776, 133, 112);

                case ETemplateName.BeafOnion:
                case ETemplateName.CheesePomegranate:
                case ETemplateName.TomatoEgg:
                case ETemplateName.TomatoHam:
                case ETemplateName.CheeseHam:
                case ETemplateName.FriedChicken:
                    return CalculateTemplateRect(0, 615, 636, 145);
                case ETemplateName.PipingBag:
                    return CalculateTemplateRect(888, 966, 203, 142);
                case ETemplateName.CupCake:
                    return CalculateTemplateRect(1155, 1027, 103, 87);

                case ETemplateName.AppleSauce:
                case ETemplateName.CheeseSauce:
                case ETemplateName.StrawberryJam:
                case ETemplateName.CandyStick:
                case ETemplateName.ChocolateChips:
                    return CalculateTemplateRect(630, 619, 659, 138);
                case ETemplateName.CoffeeMachine:
                    return CalculateTemplateRect(1452, 972, 229, 143);
                case ETemplateName.OriCoffee:
                    return CalculateTemplateRect(1694, 987, 117, 116);
                case ETemplateName.TeaCup:
                    return CalculateTemplateRect(1150, 742, 195, 172);
                case ETemplateName.GlassCup:
                    return CalculateTemplateRect(1688, 729, 212, 205);

                case ETemplateName.CheeseMilk:
                case ETemplateName.ChocolateCream:
                case ETemplateName.ChocolateJam:
                case ETemplateName.CookieSugar:
                case ETemplateName.StrawberrySugar:
                case ETemplateName.DriedTangerine:
                case ETemplateName.StrawberryGuacamole:
                    return CalculateTemplateRect(1300, 620, 619, 132);

                case ETemplateName.CheeseMilkCoffee:
                case ETemplateName.ChocolateCreamCoffee:
                case ETemplateName.ChocolateJamCoffee:
                case ETemplateName.CookieSugarCoffee:
                case ETemplateName.StrawberrySugarCoffee:
                case ETemplateName.DriedTangerineCoffee:
                case ETemplateName.StrawberryGuacamoleCoffee:
                case ETemplateName.AppleSauceCake:
                case ETemplateName.CheeseSauceCake:
                case ETemplateName.StrawberryJamCake:
                case ETemplateName.CandyStickCake:
                case ETemplateName.ChocolateChipsCake:
                case ETemplateName.BeafOnionToast:
                case ETemplateName.TomatoHamToast:
                case ETemplateName.FriedChickenToast:
                case ETemplateName.CheesePomegranateCroissant:
                case ETemplateName.TomatoEggCroissant:
                case ETemplateName.CheeseHamCroissant:
                case ETemplateName.OrderArea:
                    return CalculateTemplateRect(316, 131, 1134, 401);
                case ETemplateName.TuanSanLang:
                    //return CalculateTemplateRect(365, 418, 1067, 108);
                    return CalculateTemplateRect(265, 307, 1205, 319);
                case ETemplateName.Hammer:
                    return CalculateTemplateRect(0, 409, 175, 159);
            }

            throw new Exception($"【GetNormalRatioRect】无法匹配当前 16/9 分辨率下的模板矩形: {templateName}");
        }

        /// <summary>
        /// 分辨率16/10
        /// </summary>
        private static Rectangle GetShortRatioRect(ETemplateName templateName)
        {
            //const double ratioWidth = 2560.0;
            //const double ratioHeight = 1600.0;

            throw new Exception($"【GetShortRatioRect】无法匹配当前 16/10 分辨率下的模板矩形: {templateName}");
        }

        /// <summary>
        /// 分辨率24/10
        /// </summary>
        private static Rectangle GetLongRatioRect(ETemplateName templateName)
        {
            //const double ratioWidth = 1792.0;
            //const double ratioHeight = 768.0;

            throw new Exception($"【GetLongRatioRect】无法匹配当前 24/10 分辨率下的模板矩形: {templateName}");
        }

        /// <summary>
        /// 分辨率35/10
        /// </summary>
        private static Rectangle GetUltraRatioRect(ETemplateName templateName)
        {
            //const double ratioWidth = 3840.0;
            //const double ratioHeight = 1080.0;

            throw new Exception($"【GetUltraRatioRect】无法匹配当前 35/10 分辨率下的模板矩形: {templateName}");
        }

        private static Rectangle MatchRatioRectangle(ETemplateName templateName)
        {
            switch (GetSimilarityRatio())
            {
                case 1.6:
                    return GetShortRatioRect(templateName);
                case 1.7:
                    return GetNormalRatioRect(templateName);
                case 2.4:
                    return GetLongRatioRect(templateName);
                case 3.5:
                    return GetUltraRatioRect(templateName);
            }

            throw new Exception("【MatchRatioRectangle】无法匹配当前分辨率\n请检查是否符合[16:9] [16:10] [24:10] [35:10]的分辨率");
        }

        public static Rectangle GetTemplateRect(ETemplateName templateName)
        {
            switch (templateName)
            {
                case ETemplateName.LoginPageAnnouncement:
                case ETemplateName.LoginPageAnnouncementLight:
                    return CalculateTemplateRect(1826, 140, 85, 85);
                case ETemplateName.MoonCard:
                    return CalculateTemplateRect(750, 260, 400, 650);

                default:
                    return MatchRatioRectangle(templateName);
            }
        }

        public static RECT GetTemplateRECT(ETemplateName templateName)
        {
            Rectangle rect;

            switch (templateName)
            {
                case ETemplateName.LoginPageAnnouncement:
                case ETemplateName.LoginPageAnnouncementLight:
                    rect = CalculateTemplateRect(1826, 140, 85, 85);
                    break;
                case ETemplateName.MoonCard:
                    rect = CalculateTemplateRect(750, 260, 400, 650);
                    break;
                default:
                    rect = MatchRatioRectangle(templateName);
                    break;
            }

            return new RECT
            {
                Left = _curWindowRect.Left + rect.X,
                Top = _curWindowRect.Top + rect.Y,
                Right = _curWindowRect.Left + rect.X + rect.Width,
                Bottom = _curWindowRect.Top + rect.Y + rect.Height
            };
        }

        private static Dictionary<string, Mat> _templateMatCache = null;

        /// <summary>
        /// 传入游戏窗口截图，程序句柄和需要比对的图片名
        /// 返回匹配结果，能够在窗口截图中找到比对图则返回比对图在整个桌面全屏模式下的绝对位置，否则返回null
        /// </summary>
        /// <param name="intPtrGame">程序句柄</param>
        /// <param name="tmplName">模板图名称</param>
        /// <param name="sourceImg">游戏截图</param>
        /// <param name="usePyramid">是否采用金字塔下采样</param>
        /// <param name="cvtChannel">转为HSV通道[B/G/R]</param>
        /// <returns>System.Drawing.Point?</returns>
        /// <exception cref="Exception"></exception>
        public static Point? MatchTemplateImgByName(IntPtr intPtrGame, ETemplateName tmplName, Bitmap sourceImg = null, bool usePyramid = true, EHsvChannel? cvtChannel = null)
        {
            string imgName = tmplName.ToString();

            string key = $"HTGame-{imgName}-{_resolutionLevel}";
            if (!_templateMatCache.ContainsKey(key))
            {
                _templateMatCache[key] = GetTemplateImage(imgName).ToMat();
            }
            Bitmap image = sourceImg ?? CaptureWindow(intPtrGame, tmplName);
            Rectangle rect = GetTemplateRect(tmplName);
            Point? loc = FindImageLocation(image, _templateMatCache[key], usePyramid, cvtChannel);

            if (loc != null)
            {
                RECT windowRect = _curWindowRect;

                // 将相对坐标转换为绝对坐标
                int absoluteX = windowRect.Left + rect.X + loc.Value.X;
                int absoluteY = windowRect.Top + rect.Y + loc.Value.Y;

                return new Point(absoluteX, absoluteY);
            }

            return null;
        }

        public static int GetRandomNumber(int num)
        {
            Random random = new Random();
            return random.Next(0, num + 1);
        }

        public static Point GetRectangleCenterPoint(ETemplateName tmplName)
        {
            Rectangle rect = GetTemplateRect(tmplName);
            RECT clientRect = _curWindowRect;

            return new Point(clientRect.Left + rect.X + rect.Width / 2, clientRect.Top + rect.Y + rect.Height / 2);
        }

        public static Point GetRectangleRandomPoint(IntPtr windowHandle, ETemplateName tmplName, bool half)
        {
            Rectangle rect = GetTemplateRect(tmplName);
            RECT clientRect = _curWindowRect;

            int x = clientRect.Left + rect.X + GetRandomNumber(rect.Width);
            int y = clientRect.Top + rect.Y + GetRandomNumber(rect.Height);

            if (half)
            {
                x = clientRect.Left + rect.X + rect.Width / 4 + GetRandomNumber(rect.Width / 2);
                y = clientRect.Top + rect.Y + rect.Height / 4 + GetRandomNumber(rect.Height / 2);
            }

            return new Point(x, y);
        }

        private static readonly Dictionary<string, Bitmap> TemplateImagesCache = new Dictionary<string, Bitmap>();

        private static void LoadTemplateImage(string path)
        {
            string imageName = Path.GetFileNameWithoutExtension(path);

            // 使用文件流读取，避免使用 new Bitmap(path) 可能导致文件锁定问题
            using (Stream stream = File.OpenRead(path))
            {
                Bitmap bmp = new Bitmap(stream);
                TemplateImagesCache[imageName] = bmp;
                _templateMatCache[imageName] = bmp.ToMat();
            }
        }

        public static void InitializeImages(IntPtr windowHandle)
        {
            if (!Directory.Exists(TEMPLATE_PATH))
            {
                throw new DirectoryNotFoundException($"【InitializeImages】未找到模板目录: {TEMPLATE_PATH}");
            }

            // 预防Mat未能加载。
            if (_templateMatCache == null)
            {
                _templateMatCache = new Dictionary<string, Mat>();
            }

            ClearCache();

            RECT rect = _curWindowRect;
            _resolutionLevel = GetResolutionLevel(rect.Bottom - rect.Top);

            string searchPattern = $"*-{_resolutionLevel}.png";
            // 获取目录下所有符合条件的文件路径
            string[] imagesFilePaths = Directory.GetFiles(TEMPLATE_PATH, searchPattern);

            Log.Info("【InitializeImages】开始初始化模板图片...");
            Log.Info($"【InitializeImages】{ string.Join(", ", imagesFilePaths) }");
            foreach (var path in imagesFilePaths)
            {
                try
                {
                    LoadTemplateImage(path);
                }
                catch (Exception ex)
                {
                    throw new Exception($"【InitializeImages】加载模板图片失败: {path}. \n错误信息: {ex.Message}");
                }
            }

            Log.Info("【InitializeImages】模板图片初始化完成");
        }

        public static Bitmap GetTemplateImage(string imageName, int resolutionLevel = 0)
        {
            if (resolutionLevel == 0)
            {
                resolutionLevel = _resolutionLevel;
            }

            string key = $"HTGame-{imageName}-{resolutionLevel}";

            if (TemplateImagesCache.TryGetValue(key, out Bitmap image))
            {
                return image;
            }

            string path = Path.Combine(TEMPLATE_PATH, $"{key}.png");
            if (File.Exists(path))
            {
                try
                {
                    LoadTemplateImage(path);
                    return TemplateImagesCache[imageName];
                }
                catch (Exception ex)
                {
                    throw new Exception($"【GetTemplateImage】加载模板图片失败: {path}. \n错误信息: {ex.Message}");
                }
            }

            throw new FileNotFoundException($"【GetTemplateImage】未找到模板图片: {imageName}");
        }

        public static void ClearCache()
        {
            foreach (Bitmap image in TemplateImagesCache.Values)
            {
                image?.Dispose();
            }
            TemplateImagesCache.Clear();
            Log.Info("【ClearCache】模板图片缓存已清空");

            if (_templateMatCache != null)
            {
                foreach (Mat mat in _templateMatCache.Values)
                {
                    mat?.Dispose();
                }
                _templateMatCache.Clear();
                Log.Info("【ClearCache】模板Mat缓存已清空");
            }
        }
    }
}
