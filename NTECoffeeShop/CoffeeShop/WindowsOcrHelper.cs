using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using NLog;

namespace NTECoffeeShop.CoffeeShop
{
    internal class WindowsOcrHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static async Task<OcrResult> ExtractText(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            // 将 System.Drawing.Bitmap 转换为 WinRT 的 SoftwareBitmap
            SoftwareBitmap softwareBitmap;
            using (MemoryStream stream = new MemoryStream())
            {
                // 将图片保存到内存流（使用PNG无损格式）
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                // 转换为 WinRT 的随机访问流
                using (IRandomAccessStream winRtStream = stream.AsRandomAccessStream())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(winRtStream);
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                }
            }

            // 初始化 OCR 引擎（默认使用系统用户语言）
            //OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
            var englishLanguage = new Windows.Globalization.Language("en-US");
            OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(englishLanguage);
            if (ocrEngine == null)
            {
                ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                Log.Warn("【FindStringFromImageAsync】当前系统未能找到语言包 [en-US]");
            }
            if (ocrEngine == null)
            {
                Log.Info("【FindStringFromImageAsync】当前系统并非Win 10/11，或未安装/不支持 OCR 语言包");
                throw new InvalidOperationException("【FindStringFromImageAsync】当前系统并非Win 10/11，或未安装/不支持 OCR 语言包");
            }

            // 执行识别
            OcrResult ocrResult = await ocrEngine.RecognizeAsync(softwareBitmap);

            softwareBitmap.Dispose();
            return ocrResult;
        }

        public static async Task<List<string>> GetStringFromImageAsync(Bitmap bitmap, double scale = 1.0)
        {
            // 执行识别
            OcrResult ocrResult = await ExtractText(bitmap);
            List<string> resList = new List<string>();

            foreach (var line in ocrResult.Lines)
            {
                string lineText = line.Text.Replace(" ", "");
                resList.Add(lineText);
            }

            return resList;
        }

        /// <summary>
        /// 在Bitmap中查找指定字符串并返回Rectangle
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Rectangle?> FindStringFromImageAsync(Bitmap bitmap, string searchString, double scale = 1.0)
        {
            // 执行识别
            OcrResult ocrResult = await ExtractText(bitmap);

            //Console.WriteLine("=================================FindString=================================");
            // 遍历识别结果寻找目标字符串
            foreach (var line in ocrResult.Lines)
            {
                string lineText = line.Text.Replace(" ", ""); // 移除空格方便比对
                //Console.WriteLine($"{lineText}");

                if (lineText.Contains(searchString))
                {
                    // 整行文本完美匹配
                    if (lineText == searchString)
                    {
                        var rect = line.Words[0].BoundingRect; // 粗略取首字或通过下述循环精确合并
                        return CalculateRectangle(rect.X, rect.Y, rect.Width, rect.Height, scale);
                    }

                    // 目标字符串是行内的一部分（合并词组坐标）
                    for (int i = 0; i < line.Words.Count; i++)
                    {
                        string combinedText = "";
                        double minX = double.MaxValue, minY = double.MaxValue;
                        double maxX = 0, maxY = 0;

                        for (int j = i; j < line.Words.Count; j++)
                        {
                            combinedText += line.Words[j].Text;

                            var box = line.Words[j].BoundingRect;
                            minX = Math.Min(minX, box.Left);
                            minY = Math.Min(minY, box.Top);
                            maxX = Math.Max(maxX, box.Right);
                            maxY = Math.Max(maxY, box.Bottom);

                            // 返回识别到的矩形坐标
                            if (combinedText.Contains(searchString))
                            {
                                return CalculateRectangle(minX, minY, maxX - minX, maxY - minY, scale);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static Rectangle CalculateRectangle(double x, double y, double width, double height, double scale)
        {
            if (scale != 1.0)
            {
                x /= scale;
                y /= scale;
                width /= scale;
                height /= scale;
            }

            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }
    }
}
