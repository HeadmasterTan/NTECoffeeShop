using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;

using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace NTECoffeeShop.CoffeeShop
{
    enum EHsvChannel
    {
        H,
        S,
        V
    }

    internal class ImageHandler
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Left == r2.Left &&
                       r1.Top == r2.Top &&
                       r1.Right == r2.Right &&
                       r1.Bottom == r2.Bottom;
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !(r1 == r2);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RGB
        {
            public int R;
            public int G;
            public int B;
            public RGB(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        /// <summary>
        /// 获取纯净的游戏内容窗口坐标ClientRect
        /// 不包含边框、标题栏、阴影等
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="rect"></param>
        public static bool GetPureClientRect(IntPtr hWnd, out RECT rect)
        {
            try
            {
                GetClientRect(hWnd, out RECT clientRect);

                POINT topLeft = new POINT { X = 0, Y = 0 };
                ClientToScreen(hWnd, ref topLeft);

                rect = new RECT
                {
                    Top = topLeft.Y,
                    Left = topLeft.X,
                    Bottom = topLeft.Y + clientRect.Bottom,
                    Right = topLeft.X + clientRect.Right
                };
            }
            catch
            {
                rect = new RECT();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取指定窗口的屏幕截图，并返回一个Bitmap对象。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="tmplName"></param>
        /// <returns></returns>
        public static Bitmap CaptureWindow(IntPtr hWnd, ETemplateName? tmplName = null)
        {
            IntPtr foregroundWindow = ProcessHandler.GetForegroundWindow();
            if (hWnd != foregroundWindow)
            {
                Log.Warn("【CaptureWindow】游戏窗口不在前台，正在切换到前台");

                ProcessHandler.SetForegroundWindow(hWnd);
                System.Threading.Thread.Sleep(200); // 等待窗口切换完成
            }

            RECT rect;

            // 获取窗口在屏幕上的坐标和大小
            if (tmplName.HasValue)
            {
                rect = TemplateController.GetTemplateRECT(tmplName.Value);
            }
            else
            {
                rect = TemplateController._curWindowRect;
            }

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            // 创建一个与窗口大小相同的Bitmap对象，用于存储屏幕截图
            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 从屏幕上指定的区域复制图像到Bitmap对象中
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
            }

            return bmp;
        }

        /// <summary>
        /// 传入窗口高度，以判断返回游戏设置的分辨率
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public static int GetResolutionLevel(int height)
        {
            if (height < 900)
            {
                return 720;
            }
            if (height < 1439)
            {
                return 1080;
            }
            if (height < 2160)
            {
                return 1440;
            }
            return 2160;
        }

        /// <summary>
        /// 根据传入的HSV颜色值，计算出一个范围，用于在图像处理中进行颜色过滤。
        /// </summary>
        /// <param name="hsv">输入的HSV颜色值</param>
        /// <param name="rangeType">范围 0-小  1-中  2-大</param>
        /// <returns>返回一个包含下限和上限的元组，用于颜色过滤</returns>
        public static (Scalar lower, Scalar upper) GetHsvRange(Scalar hsv, int rangeType = 0)
        {
            int h = (int)hsv.Val0;
            int s = (int)hsv.Val1;
            int v = (int)hsv.Val2;

            // ±5度色调，±10饱和度和亮度
            int hRange = 5;
            int sRange = 10;
            int vRange = 10;

            switch (rangeType)
            {
                case 1:
                    hRange = 7;
                    sRange = 20;
                    vRange = 20;
                    break;
                case 2:
                    hRange = 10;
                    sRange = 40;
                    vRange = 40;
                    break;
            }

            Scalar lower = new Scalar(
                Math.Max(0, h - hRange),
                Math.Max(0, s - sRange),
                Math.Max(0, v - vRange));

            Scalar upper = new Scalar(
                Math.Min(179, h + hRange),
                Math.Min(255, s + sRange),
                Math.Min(255, v + vRange));

            return (lower, upper);
        }

        private static Mat _convertHsvPixel = new Mat();
        private static Dictionary<string, Scalar> _hsvScalarCache = new Dictionary<string, Scalar>();

        /// <summary>
        /// 将RGB颜色值转换为HSV颜色值。
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static Scalar ConvertRgbToHsv(RGB rgb)
        {
            string rgbString = $"({rgb.R}, {rgb.G}, {rgb.B})";
            if (_hsvScalarCache.ContainsKey(rgbString))
            {
                return _hsvScalarCache[rgbString];
            }

            using (Mat rgbPixel = new Mat(1, 1, MatType.CV_8UC3, new Scalar(rgb.B, rgb.G, rgb.R)))
            {
                Cv2.CvtColor(rgbPixel, _convertHsvPixel, ColorConversionCodes.BGR2HSV);

                Vec3b hsv = _convertHsvPixel.At<Vec3b>(0, 0);
                _hsvScalarCache[rgbString] = new Scalar(hsv.Item0, hsv.Item1, hsv.Item2);
                return _hsvScalarCache[rgbString];
            }
        }

        private static Mat _detectHsv = new Mat();
        private static Mat _detectMask = new Mat();
        private static Mat _structuringElement =
            Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(20, 15));

        /// <summary>
        /// 根据给定的截图，以及rgb值，检测图像中是否存在符合条件的颜色区域，并返回该区域的中心点坐标。
        /// </summary>
        /// <param name="screenFrame">传入的截图</param>
        /// <param name="rgb">RGB颜色值</param>
        /// <param name="rangeType">范围 0-小  1-中  2-大</param>
        /// <returns>返回检测到的颜色区域的中心点坐标，如果未检测到则返回null</returns>
        public static OpenCvSharp.Point[] DetectAreaByRgb(Bitmap screenFrame, RGB? rgb = null, int rangeType = 0)
        {
            using (Mat frame = screenFrame.ToMat())
            {
                Cv2.CvtColor(frame, _detectHsv, ColorConversionCodes.BGR2HSV);

                Scalar lowerColor;
                Scalar upperColor;

                if (rgb.HasValue)
                {
                    (lowerColor, upperColor) = GetHsvRange(ConvertRgbToHsv(rgb.Value), rangeType);
                }
                else
                {
                    (lowerColor, upperColor) = GetHsvRange(ConvertRgbToHsv(new RGB(36, 206, 170)));
                }

                Cv2.InRange(_detectHsv, lowerColor, upperColor, _detectMask);

                Cv2.MorphologyEx(
                    _detectMask,
                    _detectMask,
                    MorphTypes.Close,
                    _structuringElement);

                // 查找轮廓
                Cv2.FindContours(
                    _detectMask,
                    out OpenCvSharp.Point[][] contours,
                    out HierarchyIndex[] hierachy,
                    RetrievalModes.External,
                    ContourApproximationModes.ApproxSimple);

                if (contours.Length > 0)
                {
                    var largestContour = contours.OrderByDescending(c => Cv2.ContourArea(c)).First();

                    //if (largestContour.Length > 1)
                    //{
                    //    _detectMask.SaveImage($"./Resources/{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.png");
                    //}

                    return largestContour;
                    //var rec = Cv2.BoundingRect(largestContour);
                    //return new Point(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
                }
            }

            return null;
        }

        public static Point? DetectAreaCenterPointByRgb(Bitmap screenFrame, RGB? rgb = null, int rangeType = 0)
        {
            var largestContour = DetectAreaByRgb(screenFrame, rgb, rangeType);
            if (largestContour == null)
            {
                return null;
            }

            var rec = Cv2.BoundingRect(largestContour);

            return new Point(rec.X + rec.Width / 2, rec.Y + rec.Height / 2);
        }

        /// <summary>
        /// 图像金字塔（Pyramid 下采样）
        /// 缩小原图和模板图，先在缩小的图像上进行快速的粗匹配，找到一个大致位置后，再回到原图上进行精确匹配。
        /// 注意：因为会转为灰度图，所以如果匹配的时候图形相同但颜色不同的，要谨慎使用。
        /// </summary>
        /// <param name="refMat"></param>
        /// <param name="tplMat"></param>
        /// <param name="bestMaxVal"></param>
        /// <param name="bestLoc"></param>
        private static void MatcherPyramid(Mat refMat, Mat tplMat, out double bestMaxVal, out OpenCvSharp.Point bestLoc)
        {
            OpenCvSharp.Size templateSize = tplMat.Size();

            Mat grayTemplate = new Mat();
            Cv2.CvtColor(tplMat, grayTemplate, ColorConversionCodes.BGR2GRAY);

            // 下采样模板图像，缩小2倍（如果需要缩小4倍，则再调用一次PyrDown）
            Mat smallTemplate = new Mat();
            Cv2.PyrDown(grayTemplate, smallTemplate);

            Mat graySrc = new Mat();
            Mat smallSrc = new Mat();
            Mat resultSmall = new Mat();
            Mat resultExact = new Mat();

            // 将原始图像转换为灰度图像
            Cv2.CvtColor(refMat, graySrc, ColorConversionCodes.BGR2GRAY);

            // 下采样原始图像，同上。
            Cv2.PyrDown(graySrc, smallSrc);

            // 在低分辨率小图上进行【粗匹配】
            // 计算匹配结果矩阵的大小，应该是（W - w + 1, H - h + 1）
            int resSmallWidth = smallSrc.Cols - smallTemplate.Cols + 1;
            int resSmallHeight = smallSrc.Rows - smallTemplate.Rows + 1;

            // 确保结果矩阵的大小正确，如果不正确则重新创建
            if (resultSmall.Cols != resSmallWidth || resultSmall.Rows != resSmallHeight)
            {
                resultSmall.Create(resSmallHeight, resSmallWidth, MatType.CV_32FC1);
            }

            Cv2.MatchTemplate(smallSrc, smallTemplate, resultSmall, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(resultSmall, out _, out double smallMaxVal, out _, out OpenCvSharp.Point smallMaxLoc);

            // 坐标映射，将小图找到的坐标放大 2 倍，映射回原图的坐标系
            OpenCvSharp.Point roughLocInOriginal = new OpenCvSharp.Point(smallMaxLoc.X * 2, smallMaxLoc.Y * 2);

            // 在原图上截取局部区域进行【精匹配】
            // ROI 的搜索范围：大致坐标周围向外扩展10-20像素，确保包含正确位置，同时又不会太大导致匹配效率降低。
            int padding = 16;
            int roiX = Math.Max(0, roughLocInOriginal.X - padding);
            int roiY = Math.Max(0, roughLocInOriginal.Y - padding);

            // 确保ROI不会超出原图边界
            int roiW = Math.Min(refMat.Cols - roiX, templateSize.Width + 2 * padding);
            int roiH = Math.Min(refMat.Rows - roiY, templateSize.Height + 2 * padding);

            Rect roiRect = new Rect(roiX, roiY, roiW, roiH);

            // 使用子矩阵（共享内存，不复制数据）
            Mat srcRoi = new Mat(graySrc, roiRect);

            // 精匹配结果矩阵大小
            int resExactWidth = srcRoi.Cols - grayTemplate.Cols + 1;
            int resExactHeight = srcRoi.Rows - grayTemplate.Rows + 1;

            if (resultExact.Cols != resExactWidth || resultExact.Rows != resExactHeight)
            {
                resultExact.Create(resExactHeight, resExactWidth, MatType.CV_32FC1);
            }

            Cv2.MatchTemplate(srcRoi, grayTemplate, resultExact, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(resultExact, out _, out bestMaxVal, out _, out OpenCvSharp.Point exactMaxLoc);

            // 最终计算出原图上的精确坐标。
            bestLoc = new OpenCvSharp.Point(roiX + exactMaxLoc.X, roiY + exactMaxLoc.Y);

            grayTemplate?.Dispose();
            smallTemplate?.Dispose();
            graySrc?.Dispose();
            smallSrc?.Dispose();
            resultSmall?.Dispose();
            resultExact?.Dispose();
            srcRoi?.Dispose();
        }

        //private static Mat _matchResizedTplMat = new Mat();
        //private static Mat _matchTmplResult = new Mat();

        private static void MatchImageTemplate(Mat refMat, Mat tplMat, double scale, ref double bestMaxVal, ref OpenCvSharp.Point bestLoc, ref double bestScale, bool usePyramid = true)
        {
            using (Mat matchResizedTplMat = new Mat())
            {
                double maxVal;
                OpenCvSharp.Point maxLoc;

                // 根据当前的缩放比例调整模板图像的大小
                Cv2.Resize(tplMat, matchResizedTplMat, new OpenCvSharp.Size(tplMat.Cols * scale, tplMat.Rows * scale));

                if (matchResizedTplMat.Cols > refMat.Cols || matchResizedTplMat.Rows > refMat.Rows)
                {
                    return; // 跳过模板图像大于屏幕截图的情况
                }

                if (usePyramid)
                {
                    // 直接调用金字塔匹配方法，获取匹配结果
                    MatcherPyramid(refMat, matchResizedTplMat, out maxVal, out maxLoc);
                }
                else
                {
                    using (Mat matchTmplResult = new Mat())
                    {
                        // 使用归一化相关系数匹配方法进行模板匹配，并获取匹配结果
                        Cv2.MatchTemplate(refMat, matchResizedTplMat, matchTmplResult, TemplateMatchModes.CCoeffNormed);
                        Cv2.MinMaxLoc(matchTmplResult, out _, out maxVal, out _, out maxLoc);
                    }
                }

                if (maxVal > bestMaxVal)
                {
                    bestMaxVal = maxVal;
                    bestLoc = maxLoc;
                    bestScale = scale;
                }
            }
        }

        public static (Mat srcMat, Mat tplMat) ConvertToHsvChannel(Mat source, Mat template, EHsvChannel channel = EHsvChannel.H)
        {
            Mat hsvSrc = new Mat();
            Mat hsvTemplate = new Mat();

            Cv2.CvtColor(source, hsvSrc, ColorConversionCodes.BGR2HSV);
            Cv2.CvtColor(template, hsvTemplate, ColorConversionCodes.BGR2HSV);

            Mat[] srcChannels = Cv2.Split(hsvSrc);
            Mat[] matChannels = Cv2.Split(hsvTemplate);
            Mat srcMat;
            Mat tplMat;

            switch (channel)
            {
                case EHsvChannel.H:
                    srcMat = srcChannels[0];
                    tplMat = matChannels[0];
                    break;
                case EHsvChannel.S:
                    srcMat = srcChannels[1];
                    tplMat = matChannels[1];
                    break;
                case EHsvChannel.V:
                    srcMat = srcChannels[2];
                    tplMat = matChannels[2];
                    break;
                default:
                    throw new Exception("【ConvertToHsvChannel】转化HSV通道异常。");
            }

            //srcMat.SaveImage($"./Resources/{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.png");

            return (srcMat, tplMat);
        }

        private const double MinScale = 0.8;
        private const double MaxScale = 1.2;
        private const double MinSimilarity = 0.8;

        public static Point? FindImageLocation(Bitmap screenSource, Mat templateImg, bool usePyramid = true, EHsvChannel? cvtChannel = null)
        {
            Mat tplMat;
            Mat refMat;

            if (cvtChannel == null)
            {
                tplMat = templateImg;
                refMat = screenSource.ToMat();
            }
            else
            {
                (refMat, tplMat) = ConvertToHsvChannel(screenSource.ToMat(), templateImg, cvtChannel.Value);
            }

            double bestMaxVal = 0;
            OpenCvSharp.Point bestLoc = new OpenCvSharp.Point();
            double bestScale = 1.0;

            // 直接匹配原始大小的模板图像
            MatchImageTemplate(refMat, tplMat, 1.0, ref bestMaxVal, ref bestLoc, ref bestScale, usePyramid);

            if (bestMaxVal <= MinSimilarity)
            {
                // 尝试缩放比例，步长为0.1，来匹配模板图像
                for (double scale = MinScale; scale <= MaxScale; scale += 0.1)
                {
                    if (scale == 1.0) continue; // 已经匹配过原始大小的模板图像，跳过

                    MatchImageTemplate(refMat, tplMat, scale, ref bestMaxVal, ref bestLoc, ref bestScale, usePyramid);
                    if (bestMaxVal > MinSimilarity)
                    {
                        break; // 找到匹配度足够高的结果，跳出循环
                    }
                }
            }

            if (bestMaxVal > MinSimilarity)
            {
                int centerX = bestLoc.X + (int)(tplMat.Cols * bestScale / 2);
                int centerY = bestLoc.Y + (int)(tplMat.Rows * bestScale / 2);
                return new Point(centerX, centerY);
            }

            refMat?.Dispose();
            return null;
        }

        /// <summary>
        /// 裁剪图片，返回一个新的Bitmap对象，包含原图中cropRect指定的区域。
        /// </summary>
        /// <param name="sourceImg">原始图像</param>
        /// <param name="cropRect">裁剪区域</param>
        /// <returns>裁剪后的图像</returns>
        public static Bitmap CropImageByRect(Bitmap sourceImg, Rectangle cropRect)
        {
            // 创建一个新的 Bitmap 对象来存储裁剪后的图像
            Bitmap croppedImg = new Bitmap(cropRect.Width, cropRect.Height);

            // 使用 Graphics 对象从原始图像中裁剪指定区域并绘制到新的 Bitmap 上
            using (Graphics g = Graphics.FromImage(croppedImg))
            {
                g.DrawImage(sourceImg, new Rectangle(0, 0, croppedImg.Width, croppedImg.Height), cropRect, GraphicsUnit.Pixel);
            }

            return croppedImg;
        }

        public static Bitmap HandleOcrSourceImage(Bitmap image, double scale = 2.0)
        {
            //const int scale = 2;
            Mat resized = new Mat();

            using (Mat src = BitmapConverter.ToMat(image))
            using (Mat gray = new Mat())
            using (Mat binary = new Mat())
            {
                // 灰度化
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // 高斯模糊（去噪，防止二值化后边缘出现大量锯齿颗粒）
                Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(3, 3), 0);

                // 大律法（Otsu）自适应二值化，彻底变成纯黑白，踢出灰色杂质背景
                Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

                // 放大
                Cv2.Resize(binary, resized, new OpenCvSharp.Size(binary.Width * scale, binary.Height * scale), 0, 0, InterpolationFlags.Cubic);
            }

            return BitmapConverter.ToBitmap(resized);
        }
    }
}
