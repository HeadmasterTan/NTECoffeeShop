using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using static NTECoffeeShop.CoffeeShop.ImageHandler;
using static NTECoffeeShop.CoffeeShop.TemplateController;
using Point = System.Drawing.Point;

namespace NTECoffeeShop.CoffeeShop
{
    internal class ManagerScene
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly RGB RgbWhite = new RGB(255, 255, 255);

        private static readonly RGB RgbProgressBar = new RGB(246, 187, 23); // 黄色滚动条
        private static readonly RGB RgbStartTips = new RGB(239, 137, 79); // 开始提示

        private static DataModel _dataModel = new DataModel();
        private static StoreManager _storeManager = StoreManager.GetInstance();
        private static List<ETemplateName> _orderTypeList = new List<ETemplateName>();

        public static async Task InitializeStageData()
        {
            Log.Info("【InitializeStageData】开始初始化关卡信息...");

            _orderTypeList.Clear();
            string stage = await GetCurrentStage();
            string stageMsg = "";

            if (_dataModel.DefaultStageProducts.TryGetValue(stage, out var products))
            {
                _orderTypeList.AddRange(products);
                stageMsg = stage;
            }

            if (_orderTypeList.Count == 0)
            {
                _orderTypeList.AddRange(DataModel.Stage_All);
                stageMsg = "全部";
            }

            Log.Info($"【InitializeStageData】初始化完毕，当前使用的是 {stageMsg} 关卡信息。");
        }

        public static async Task<string> GetCurrentStage()
        {
            // 默认直接使用重复关卡
            if (_storeManager.ExecuteType == EExecuteType.Repeat)
            {
                return _storeManager.RepeatStage;
            }

            string pattern = @"(\d+)-(\d+)";
            Match match;

            double scale = 2.0;
            Bitmap image = CaptureWindow(_storeManager.IntPtrGame, ETemplateName.GameChapterTextArea);
            List<string> lineList =
                await WindowsOcrHelper.GetStringFromImageAsync(HandleOcrSourceImage(image, scale), scale);

            string matchStage = "";
            string defaultStage = "99-99";

            Log.Info("====================GetCurrentStage====================");
            foreach (var line in lineList)
            {
                Log.Info("提取信息：" + line);
                match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    matchStage = match.Value;
                    Log.Info("匹配信息：" + matchStage);
                    break;
                }
            }
            Log.Info("====================GetCurrentStage====================");

            if (matchStage == "")
            {
                Log.Error("【GetCurrentStage】提取关卡失败！");
                matchStage = defaultStage;
            }

            return matchStage;
        }

        public static Point? GetProgressBarPoint()
        {
            Bitmap image = CaptureWindow(_storeManager.IntPtrGame, ETemplateName.OrderArea);
            Point? locBar = DetectAreaCenterPointByRgb(image, RgbProgressBar, 1);
            return locBar;
        }

        public static bool IsStartTipsVisible()
        {
            Bitmap image = CaptureWindow(_storeManager.IntPtrGame, ETemplateName.GameStartTips);
            Point? locStartTips = DetectAreaCenterPointByRgb(image, RgbStartTips, 1);
            Point? locWhite = DetectAreaCenterPointByRgb(image, RgbWhite, 1);

            return locStartTips.HasValue && locWhite.HasValue;
        }

        public static bool IsTuanSanLang()
        {
            ETemplateName tuanSanLang = ETemplateName.TuanSanLang;

            Bitmap sourceImg = CaptureWindow(_storeManager.IntPtrGame, tuanSanLang);

            string tmplImgPath =
                $"./Resources/Images/HTGame-{tuanSanLang.ToString()}-{TemplateController.ResolutionLevel}.png";
            Point2f? point = CharacterHandler.FindCharacter(sourceImg, tmplImgPath);

            //if (point.HasValue)
            //{
            //    Console.WriteLine($"x = {point.Value.X}, y = {point.Value.Y}");
            //}

            return point.HasValue;
        }

        public static bool CheckIsEmpty(ETemplateName type)
        {
            Point? point = MatchTemplateImgByName(_storeManager.IntPtrGame, type);
            return point == null;
        }

        public static void PreparingIngredients(ETemplateName type)
        {
            Point point = GetRectangleCenterPoint(type);
            SimulateEventHandler.MouseClick(point);
            Thread.Sleep(150);
        }

        private static Dictionary<ETemplateName, Point> _materialsPointCache = new Dictionary<ETemplateName, Point>();

        public static void ResetData()
        {
            _materialsPointCache = new Dictionary<ETemplateName, Point>();
        }

        public static void MakingCupCake(ETemplateName templateName)
        {
            Log.Info($"【MakingCupCake】开始制作 {templateName.ToString()}Cake  ======>");

            Point oriMaterialPoint = GetRectangleCenterPoint(ETemplateName.CupCake);
            SimulateEventHandler.MouseClick(oriMaterialPoint);

            if (!_materialsPointCache.ContainsKey(templateName))
            {
                var resPoint = MatchTemplateImgByName(_storeManager.IntPtrGame, templateName);
                if (resPoint != null)
                {
                    _materialsPointCache[templateName] = resPoint.Value;
                }
                else
                {
                    throw new Exception($"【MakingCupCake】未找到 {templateName.ToString()}");
                }
            }

            Thread.Sleep(100);
            SimulateEventHandler.MouseClick(_materialsPointCache[templateName]);

            Log.Info($"【MakingCupCake】{templateName.ToString()}Cake 完成制作  <======");
        }

        public static void MakingToast(ETemplateName templateName)
        {
            Log.Info($"【MakingToast】开始制作 {templateName.ToString()}Toast  ======>");

            Point oriMaterialPoint = GetRectangleCenterPoint(ETemplateName.CutToast);
            SimulateEventHandler.MouseClick(oriMaterialPoint);

            if (!_materialsPointCache.ContainsKey(templateName))
            {
                var resPoint = MatchTemplateImgByName(_storeManager.IntPtrGame, templateName);
                if (resPoint != null)
                {
                    _materialsPointCache[templateName] = resPoint.Value;
                }
                else
                {
                    throw new Exception($"【MakingToast】未找到 {templateName.ToString()}");
                }
            }

            Thread.Sleep(100);
            SimulateEventHandler.MouseClick(_materialsPointCache[templateName]);

            Log.Info($"【MakingToast】{templateName.ToString()}Toast 完成制作  <======");
        }

        public static void MakingCroissant(ETemplateName templateName)
        {
            Log.Info($"【MakingCroissant】开始制作 {templateName.ToString()}Croissant  ======>");

            Point oriMaterialPoint = GetRectangleCenterPoint(ETemplateName.CutCroissant);
            SimulateEventHandler.MouseClick(oriMaterialPoint);

            if (!_materialsPointCache.ContainsKey(templateName))
            {
                var resPoint = MatchTemplateImgByName(_storeManager.IntPtrGame, templateName);
                if (resPoint != null)
                {
                    _materialsPointCache[templateName] = resPoint.Value;
                }
                else
                {
                    throw new Exception($"【MakingCroissant】未找到 {templateName.ToString()}");
                }
            }

            Thread.Sleep(100);
            SimulateEventHandler.MouseClick(_materialsPointCache[templateName]);

            Log.Info($"【MakingCroissant】{templateName.ToString()}Croissant 完成制作  <======");
        }

        public static void MakingGlassCupCoffee(ETemplateName templateName)
        {
            Log.Info($"【MakingGlassCupCoffee】开始制作 {templateName.ToString()}Coffee  ======>");

            Point glassCupPoint = GetRectangleCenterPoint(ETemplateName.GlassCup);
            SimulateEventHandler.MouseClick(glassCupPoint);

            Thread.Sleep(100);
            Point oriMaterialPoint = GetRectangleCenterPoint(ETemplateName.OriCoffee);
            SimulateEventHandler.MouseClick(oriMaterialPoint);

            if (!_materialsPointCache.ContainsKey(templateName))
            {
                var resPoint = MatchTemplateImgByName(_storeManager.IntPtrGame, templateName);
                if (resPoint != null)
                {
                    _materialsPointCache[templateName] = resPoint.Value;
                }
                else
                {
                    throw new Exception($"【MakingGlassCupCoffee】未找到 {templateName.ToString()}");
                }
            }

            Thread.Sleep(100);
            SimulateEventHandler.MouseClick(_materialsPointCache[templateName]);

            Log.Info($"【MakingGlassCupCoffee】{templateName.ToString()}Coffee 完成制作  <======");
        }

        public static void MakingTeaCupCoffee(ETemplateName templateName)
        {
            Log.Info($"【MakingTeaCupCoffee】开始制作 {templateName.ToString()}Coffee  ======>");

            Point teaCupPoint = GetRectangleCenterPoint(ETemplateName.TeaCup);
            SimulateEventHandler.MouseClick(teaCupPoint);

            Thread.Sleep(100);
            Point oriMaterialPoint = GetRectangleCenterPoint(ETemplateName.OriCoffee);
            SimulateEventHandler.MouseClick(oriMaterialPoint);

            if (!_materialsPointCache.ContainsKey(templateName))
            {
                var resPoint = MatchTemplateImgByName(_storeManager.IntPtrGame, templateName);
                if (resPoint != null)
                {
                    _materialsPointCache[templateName] = resPoint.Value;
                }
                else
                {
                    throw new Exception($"【MakingTeaCupCoffee】未找到 {templateName.ToString()}");
                }
            }

            Thread.Sleep(100);
            SimulateEventHandler.MouseClick(_materialsPointCache[templateName]);

            Log.Info($"【MakingTeaCupCoffee】{templateName.ToString()}Coffee 完成制作  <======");
        }

        public static ETemplateName GetMaterial(ETemplateName order)
        {
            switch (order)
            {
                case ETemplateName.BeafOnionToast:
                    return ETemplateName.BeafOnion;
                case ETemplateName.TomatoHamToast:
                    return ETemplateName.TomatoHam;
                case ETemplateName.FriedChickenToast:
                    return ETemplateName.FriedChicken;

                case ETemplateName.CheesePomegranateCroissant:
                    return ETemplateName.CheesePomegranate;
                case ETemplateName.TomatoEggCroissant:
                    return ETemplateName.TomatoEgg;
                case ETemplateName.CheeseHamCroissant:
                    return ETemplateName.CheeseHam;

                case ETemplateName.AppleSauceCake:
                    return ETemplateName.AppleSauce;
                case ETemplateName.CheeseSauceCake:
                    return ETemplateName.CheeseSauce;
                case ETemplateName.StrawberryJamCake:
                    return ETemplateName.StrawberryJam;
                case ETemplateName.CandyStickCake:
                    return ETemplateName.CandyStick;
                case ETemplateName.ChocolateChipsCake:
                    return ETemplateName.ChocolateChips;

                case ETemplateName.CheeseMilkCoffee:
                    return ETemplateName.CheeseMilk;
                case ETemplateName.ChocolateCreamCoffee:
                    return ETemplateName.ChocolateCream;
                case ETemplateName.ChocolateJamCoffee:
                    return ETemplateName.ChocolateJam;
                case ETemplateName.StrawberrySugarCoffee:
                    return ETemplateName.StrawberrySugar;
                case ETemplateName.CookieSugarCoffee:
                    return ETemplateName.CookieSugar;
                case ETemplateName.DriedTangerineCoffee:
                    return ETemplateName.DriedTangerine;
                case ETemplateName.StrawberryGuacamoleCoffee:
                    return ETemplateName.StrawberryGuacamole;
            }

            throw new Exception("【GetMaterial】获取订单材料失败");
        }

        public static bool ScrollToTop()
        {
            Log.Info("【ScrollToTop】开始滚动到顶部...");

            ProcessHandler.SetForegroundWindow(_storeManager.IntPtrGame);
            Thread.Sleep(200);

            Point stageListPoint = GetRectangleCenterPoint(ETemplateName.StartSceneStageList);
            SimulateEventHandler.MouseMove(stageListPoint.X, stageListPoint.Y);
            Thread.Sleep(100);

            Point? point = MatchTemplateImgByName(_storeManager.IntPtrGame, ETemplateName.StartSceneFirstStage);
            long startTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (point == null)
            {
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - startTime > 20)
                {
                    Log.Warn("【ScrollToTop】滚动已超过20秒，强制中断！");
                    return false;
                }

                SimulateEventHandler.MouseScroll(2);
                Thread.Sleep(100);

                point = MatchTemplateImgByName(_storeManager.IntPtrGame, ETemplateName.StartSceneFirstStage);
            }

            Log.Info("【ScrollToTop】已滚动到顶部。");
            return true;
        }

        public static async Task<Point?> GetTextLineRectangle(string text, ETemplateName tmplName)
        {
            Point? point = null;

            double scale = 2.0;
            Bitmap image = HandleOcrSourceImage(CaptureWindow(_storeManager.IntPtrGame, tmplName), scale);
            //image.Save($"./Resources/{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.png");
            Rectangle? textRect = await WindowsOcrHelper.FindStringFromImageAsync(image, text, scale);

            if (textRect != null)
            {
                Rectangle tmplRect = GetTemplateRect(tmplName);
                RECT winRect = TemplateController._curWindowRect;
                point = new Point(
                    winRect.Left + tmplRect.X + textRect.Value.X + textRect.Value.Width,
                    winRect.Top + tmplRect.Y + textRect.Value.Y + textRect.Value.Height);
            }

            return point;
        }

        public static async Task<Point?> ScrollDownToStage(string stage = "")
        {
            if (stage == "")
            {
                Log.Info("【ScrollDownToStage】正在检索尚未通关的关卡...");
            }
            else
            {
                Log.Info($"【ScrollDownToStage】正在前往关卡 {stage} ...");
            }

            ProcessHandler.SetForegroundWindow(_storeManager.IntPtrGame);
            Thread.Sleep(200);

            Point stageListPoint = GetRectangleCenterPoint(ETemplateName.StartSceneHalfStageList);
            SimulateEventHandler.MouseMove(stageListPoint.X, stageListPoint.Y);
            Thread.Sleep(100);

            Point? point;

            if (stage == "")
            {
                point = MatchTemplateImgByName(_storeManager.IntPtrGame, ETemplateName.StartSceneDarkStar, null, false);
            }
            else
            {
                point = await GetTextLineRectangle(stage, ETemplateName.StartSceneHalfStageList);
            }

            long startTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            while (point == null)
            {
                if (DateTimeOffset.Now.ToUnixTimeSeconds() - startTime > 20)
                {
                    if (stage != "")
                    {
                        Point secondToLastPoint = GetRectangleCenterPoint(ETemplateName.StartSceneSecondToLastStage);
                        SimulateEventHandler.MouseClick(secondToLastPoint);
                        Thread.Sleep(1000);

                        point = await GetTextLineRectangle(stage, ETemplateName.StartSceneLastStage);
                        Log.Warn("【ScrollDownToStage】检索已超过20秒，强制中断，并最后一次直接检索尾节点。");
                        return point;
                    }
                    Log.Warn("【ScrollDownToStage】检索已超过20秒，强制中断。");
                    return null;
                }

                SimulateEventHandler.MouseScroll(-2);
                Thread.Sleep(100);

                if (stage == "")
                {
                   point = MatchTemplateImgByName(_storeManager.IntPtrGame, ETemplateName.StartSceneDarkStar, null, false);
                }
                else
                {
                    point = await GetTextLineRectangle(stage, ETemplateName.StartSceneHalfStageList);
                }
            }

            Log.Info("【ScrollDownToStage】检索完毕，已找到关卡。");
            return point;
        }

        private static int _searchStageCount = 0;

        public static async Task SelectStageToStart(string stage = "")
        {
            Log.Info("【SelectStageToStart】开始进行关卡筛查...");

            if (!ScrollToTop()) return;

            Point? stagePoint = await ScrollDownToStage(stage);
            if (stagePoint.HasValue)
            {
                _searchStageCount = 0;

                SimulateEventHandler.MouseClick(stagePoint.Value);
                Thread.Sleep(2000);

                await InitializeStageData();

                Log.Info("【SelectStageToStart】前往关卡中...");

                Point startBtnPoint = GetRectangleCenterPoint(ETemplateName.StartSceneStartButton);
                SimulateEventHandler.MouseClick(startBtnPoint);
                Thread.Sleep(2000);
            }
            else
            {
                _searchStageCount++;

                if (stage == "")
                {
                    Log.Error("【SelectStageToStart】无法找到未完美通关的关卡。");
                }
                else
                {
                    Log.Error($"【SelectStageToStart】无法定位到关卡 {stage} 。");
                }

                if (_searchStageCount >= 3)
                {
                    SimulateEventHandler.SendScanCodeKeyPress(SimulateEventHandler.SCAN_F11);

                    string msg = "";
                    switch (_storeManager.CurrentLanguage)
                    {
                        case "繁体中文":
                            msg = "檢索關卡失敗 3 次，任務已暫停。";
                            break;
                        case "English":
                            msg = "Stage search failed 3 times. Task paused.";
                            break;
                        case "日本語":
                            msg = "検索失敗3回。タスクを一時停止しました。";
                            break;
                        default:
                            msg = "连续 3 次关卡检索失败，任务已暂停。";
                            break;
                    }

                    MessageBox.Show(msg, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static async Task<List<ETemplateName?>> GetOrderList()
        {
            int splitCount;
            if (_orderTypeList.Count < 5)
            {
                splitCount = 1;
            }
            else if (_orderTypeList.Count < 10)
            {
                splitCount = 2;
            }
            else
            {
                splitCount = 3;
            }

            List<ETemplateName?> orderList = new List<ETemplateName?>();
            List<Task<ETemplateName?>> tasks = new List<Task<ETemplateName?>>();

            List<List<ETemplateName>> taskRunOrderList = new List<List<ETemplateName>>();

            using (Bitmap matchImg = CaptureWindow(_storeManager.IntPtrGame, ETemplateName.OrderArea))
            {
                // 拆分成多个列表，降低线程开销
                for (int i = 0; i < _orderTypeList.Count; i++)
                {
                    int index = i / splitCount;
                    if (taskRunOrderList.Count <= index)
                    {
                        taskRunOrderList.Add(new List<ETemplateName>());
                    }

                    taskRunOrderList[index].Add(_orderTypeList[i]);

                    if ((i + 1) % splitCount == 0)
                    {
                        Bitmap tmplImage = (Bitmap)matchImg.Clone();
                        tasks.Add(Task.Run(() => CheckOrderListType(taskRunOrderList[index], tmplImage)));
                    }
                }

                if (_orderTypeList.Count % splitCount != 0)
                {
                    Bitmap tmplImage = (Bitmap)matchImg.Clone();
                    tasks.Add(Task.Run(() => CheckOrderListType(taskRunOrderList[taskRunOrderList.Count - 1], tmplImage)));
                }

                while (tasks.Count > 0)
                {
                    Task<ETemplateName?> completedTask = await Task.WhenAny(tasks);

                    // 拿到一个订单直接跳出循环，不需要继续，以免浪费性能。
                    if (completedTask.Result.HasValue)
                    {
                        Log.Info($"【GetOrderList】获取到订单：{completedTask.Result.Value}");
                        orderList.Add(completedTask.Result);
                        break;
                    }

                    tasks.Remove(completedTask);
                }
            }

            return orderList;
        }

        private static readonly List<ETemplateName> NotUsePyramidOrders = new List<ETemplateName>()
        {
            ETemplateName.CookieSugarCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,
            ETemplateName.CandyStickCake,
            ETemplateName.CheeseSauceCake,
            //ETemplateName.CheeseHamCroissant,
            //ETemplateName.CheesePomegranateCroissant,
        };

        private static Task<ETemplateName?> CheckOrderListType(List<ETemplateName> orderList, Bitmap matchImg)
        {
            foreach (var order in orderList)
            {
                bool usePyramid = !NotUsePyramidOrders.Contains(order);

                Point? point = MatchTemplateImgByName(_storeManager.IntPtrGame, order, matchImg, usePyramid);

                if (point != null)
                {
                    return Task.FromResult<ETemplateName?>(order);
                }
            }

            return Task.FromResult<ETemplateName?>(null);
        }
    }
}
