using NLog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static NTECoffeeShop.CoffeeShop.ImageHandler;
using static NTECoffeeShop.CoffeeShop.ManagerScene;
using static NTECoffeeShop.CoffeeShop.TemplateController;

namespace NTECoffeeShop.CoffeeShop
{
    enum EExecuteType
    {
        Repeat,
        AllClear,
    }

    enum EManagerStatus
    {
        Idle, // 无状态
        InOperation, // 经营中
        Settlement, // 结算
    }

    internal class StoreManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private const string GAME_PROCESS_NAME = "HTGame"; // 异环的程序名
        private const int INTERVAL_MS = 1000 / 60; // 默认帧率
        private const int REST_TIME_MS = 185; // 休息时间，单位毫秒

        private Process _prcGame; // 游戏进程对象
        private IntPtr _intPtrGame; // 游戏窗口句柄

        private TaskController _tasks;
        private bool _isTaskRunning;
        private string _curRunningTaskName;

        private static StoreManager _uniqueInstance;
        private static readonly object _lock = new object();

        private static EManagerStatus _managerStatus = EManagerStatus.Idle;
        private static EExecuteType _executeType;
        private static string _repeatStage;
        private static string _curLanguage;

        public IntPtr IntPtrGame => _intPtrGame;

        public EExecuteType ExecuteType
        {
            get => _executeType;
            set => _executeType = value;
        }

        public string RepeatStage
        {
            get => _repeatStage;
            set => _repeatStage = value;
        }

        public string CurrentLanguage
        {
            get => _curLanguage;
            set => _curLanguage = value;
        }

        private StoreManager() {}

        public static StoreManager GetInstance()
        {
            // 一重检查：避免不必要的加锁开销
            if (_uniqueInstance == null)
            {
                lock (_lock)
                {
                    // 二重检查：确保多线程并发时只创建一次
                    if (_uniqueInstance == null)
                    {
                        _uniqueInstance = new StoreManager();
                    }
                }
            }

            return _uniqueInstance;
        }

        private void InitializeData()
        {
            ProcessHandler.SetForegroundWindow(_intPtrGame);
            Thread.Sleep(200);

            GetPureClientRect(_intPtrGame, out RECT windowRect);
            if (windowRect != TemplateController._curWindowRect)
            {
                if (TemplateController._curWindowRect.Right != 0)
                {
                    Log.Info("【InitializeData】窗口尺寸发生变化，重新初始化数据");
                }

                InitRatio(windowRect);
                InitializeImages(_intPtrGame);
            }
        }

        public void Start()
        {
            if (_tasks == null)
            {
                Log.Info("【Start】=========================================================");
                Log.Info("【Start】工具启动，开始初始化数据...");

                _prcGame = ProcessHandler.GetProcess(GAME_PROCESS_NAME);

                if (_prcGame == null)
                {
                    throw new Exception("游戏未运行");
                }

                // 初始化一些数据。
                _intPtrGame = _prcGame.MainWindowHandle;
                InitializeData();

                // 游戏进程和句柄均已记录，开始执行店长任务
                _tasks = new TaskController();
                _curRunningTaskName = "ManagerLoop";
                _tasks.StartTask("ManagerLoop", ManagerLoop);
                _isTaskRunning = true;

                Log.Info("【Start】数据初始化完毕");
                Log.Info("【Start】=========================================================");
            }
            else if (!_isTaskRunning)
            {
                InitializeData();

                _tasks.ResumeTasks(_curRunningTaskName);
                _isTaskRunning = true;
            }
        }

        public void Pause()
        {
            if (_tasks != null && _isTaskRunning)
            {
                _tasks.PauseTask(_curRunningTaskName);
                _isTaskRunning = false;
            }
        }

        public void Stop()
        {
            _tasks?.StopAllTasks();
        }

        private void ShowErrorMessage(string message)
        {
            string text = $"{message}\n请关闭工具后重新打开";
            string caption = "不温馨提示";

            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool HandleEndGameScene()
        {
            Point? endGameLightStarPoint =
                MatchTemplateImgByName(_intPtrGame, ETemplateName.GameCompleteStar, null, false);
            Point? endGameDarkStarPoint =
                MatchTemplateImgByName(_intPtrGame, ETemplateName.GameFailedStar, null, false);
            if (endGameLightStarPoint.HasValue || endGameDarkStarPoint.HasValue)
            {
                if (endGameLightStarPoint.HasValue)
                {
                    Point? energyEmptyPoint = MatchTemplateImgByName(_intPtrGame, ETemplateName.GameResultEnergyEmpty);
                    if (energyEmptyPoint.HasValue)
                    {
                        SimulateEventHandler.SendScanCodeKeyPress(SimulateEventHandler.SCAN_F11);
                        Log.Warn("【HandleEndGameScene】都市活力已用完 ============================================");

                        string msg = "";
                        switch (_curLanguage)
                        {
                            case "繁体中文":
                                msg = "都市活力耗盡，安魂曲下班了。";
                                break;
                            case "English":
                                msg = "Energy depleted. Lacrimosa is off-duty.";
                                break;
                            case "日本語":
                                msg = "都市活力切れ。レクイエムは退勤しました。";
                                break;
                            default:
                                msg = "都市活力已用完，安魂曲下班了。";
                                break;
                        }

                        MessageBox.Show( msg, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return true;
                    }

                    Log.Info("【ManagerLoop】营业达标，3 秒后领取收益。");
                    Thread.Sleep(3000);

                    Point endSceneBtnPoint = GetRectangleCenterPoint(ETemplateName.EndSceneConfirmButton);
                    SimulateEventHandler.MouseClick(endSceneBtnPoint);

                    Log.Info("【ManagerLoop】收益领取完毕，休息 3 秒。");
                    Thread.Sleep(3000);
                }
                else
                {
                    Log.Info("【ManagerLoop】营业不达标！3 秒后退回页面。");
                    Thread.Sleep(3000);

                    Point endSceneBackPoint = GetRectangleCenterPoint(ETemplateName.EndSceneBackButton);
                    SimulateEventHandler.MouseClick(endSceneBackPoint);

                    Log.Info("【ManagerLoop】休息 3 秒。");
                    Thread.Sleep(3000);
                }

                return true;
            }

            return false;
        }

        private bool CheckToPreparing(ETemplateName type)
        {
            Point? emptyPoint = MatchTemplateImgByName(_intPtrGame, type);

            switch (type)
            {
                case ETemplateName.EmptyCoffee:
                    Point? coffeeMachinePoint = MatchTemplateImgByName(_intPtrGame, ETemplateName.CoffeeMachine);
                    return emptyPoint.HasValue && coffeeMachinePoint.HasValue;
                case ETemplateName.EmptyCupCake:
                    return emptyPoint.HasValue;
                case ETemplateName.EmptyCroissant:
                case ETemplateName.EmptyToast:
                    Point? cuttingBoardPoint = MatchTemplateImgByName(_intPtrGame, ETemplateName.CuttingBoard);
                    return emptyPoint.HasValue && cuttingBoardPoint.HasValue;
            }

            return false;
        }

        private async Task ManagerLoop(ConcurrentDictionary<string, TaskCompletionSource<bool>> tcsDict, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (tcsDict.TryGetValue("ManagerLoop", out var tcs))
                    {
                        // 如果tcs.Task未完成，代码会在此处异步挂起，不阻塞线程池线程
                        await tcs.Task;
                    }

                    try
                    {
                        if (IsStartTipsVisible())
                        {
                            Log.Info("【ManagerLoop】正在倒计时...");
                            ManagerScene.ResetData();
                            Thread.Sleep(500);
                            _managerStatus = EManagerStatus.InOperation;
                            continue;
                        }

                        // 经营中时不判断，以免影响效率
                        if (_managerStatus != EManagerStatus.InOperation)
                        {
                            if (HandleEndGameScene()) continue;
                        }

                        Point? clearFlagPoint = 
                            MatchTemplateImgByName(_intPtrGame, ETemplateName.GameClearFlag, null, false);
                        if (clearFlagPoint.HasValue)
                        {
                            SimulateEventHandler.SendScanCodeKeyPress(SimulateEventHandler.SCAN_ESCAPE);
                            _managerStatus = EManagerStatus.Settlement;
                            Thread.Sleep(4000);
                            continue;
                        }

                        // 优先肘击团三郎。
                        if (IsTuanSanLang())
                        {
                            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            var hammerRect = GetRectangleCenterPoint(ETemplateName.Hammer);
                            SimulateEventHandler.MouseClick(hammerRect);
                            Log.Info($"【ManagerLoop】肘击团三郎耗时: {DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime} ms");

                            Thread.Sleep((int)(REST_TIME_MS * 2));
                            continue;
                        }

                        // 如果有订单则进入制作流程。
                        if (GetProgressBarPoint().HasValue)
                        {
                            var orderList = await GetOrderList();

                            foreach (var order in orderList)
                            {
                                if (order == null) continue;
                                ETemplateName material = GetMaterial(order.Value);

                                switch (order)
                                {
                                    case ETemplateName.CheeseMilkCoffee:
                                    case ETemplateName.DriedTangerineCoffee:
                                    case ETemplateName.StrawberrySugarCoffee:
                                        if (!CheckIsEmpty(ETemplateName.OriCoffee))
                                        {
                                            MakingTeaCupCoffee(material);
                                        }
                                        else
                                        {
                                            Log.Warn("【ManagerLoop】正在制作 OriCoffee");
                                        }
                                        break;

                                    case ETemplateName.ChocolateCreamCoffee:
                                    case ETemplateName.ChocolateJamCoffee:
                                    case ETemplateName.CookieSugarCoffee:
                                    case ETemplateName.StrawberryGuacamoleCoffee:
                                        if (!CheckIsEmpty(ETemplateName.OriCoffee))
                                        {
                                            MakingGlassCupCoffee(material);
                                        }
                                        else
                                        {
                                            Log.Warn("【ManagerLoop】正在制作 OriCoffee");
                                        }
                                        break;

                                    case ETemplateName.AppleSauceCake:
                                    case ETemplateName.CandyStickCake:
                                    case ETemplateName.CheeseSauceCake:
                                    case ETemplateName.ChocolateChipsCake:
                                    case ETemplateName.StrawberryJamCake:
                                        if (!CheckIsEmpty(ETemplateName.CupCake))
                                        {
                                            MakingCupCake(material);
                                        }
                                        else
                                        {
                                            Log.Warn("【ManagerLoop】正在制作 CupCake");
                                        }
                                        break;

                                    case ETemplateName.BeafOnionToast:
                                    case ETemplateName.FriedChickenToast:
                                    case ETemplateName.TomatoHamToast:
                                        if (!CheckIsEmpty(ETemplateName.CutToast))
                                        {
                                            MakingToast(material);
                                        }
                                        else
                                        {
                                            Log.Warn("【ManagerLoop】正在制作 CutToast");
                                        }
                                        break;

                                    case ETemplateName.CheeseHamCroissant:
                                    case ETemplateName.CheesePomegranateCroissant:
                                    case ETemplateName.TomatoEggCroissant:
                                        if (!CheckIsEmpty(ETemplateName.CutCroissant))
                                        {
                                            MakingCroissant(material);
                                        }
                                        else
                                        {
                                            Log.Warn("【ManagerLoop】正在制作 CutCroissant");
                                        }
                                        break;
                                }
                            }
                        }

                        // 没有咖啡。
                        if (CheckToPreparing(ETemplateName.EmptyCoffee))
                        {
                            PreparingIngredients(ETemplateName.CoffeeMachine);
                            continue;
                        }

                        // 没有蛋糕。
                        if (CheckToPreparing(ETemplateName.EmptyCupCake))
                        {
                            PreparingIngredients(ETemplateName.PipingBag);
                            continue;
                        }

                        // 没有可颂切片。
                        if (CheckToPreparing(ETemplateName.EmptyCroissant))
                        {
                            PreparingIngredients(ETemplateName.OriginCroissant);
                            continue;
                        }

                        // 没有吐司切片。
                        if (CheckToPreparing(ETemplateName.EmptyToast))
                        {
                            PreparingIngredients(ETemplateName.OriginToast);
                            continue;
                        }

                        // 检测到需要按F进入【店长特供】玩法
                        // 因为选项有两个，要做拦截得做多语言，暂时放开不检测。
                        //Point? enterFPoint = MatchTemplateImgByName(_intPtrGame, ETemplateName.EnterFKey);
                        //if (enterFPoint != null)
                        //{
                        //    _managerStatus = EManagerStatus.Idle;
                        //    SimulateEventHandler.SendScanCodeKeyPress(SimulateEventHandler.SCAN_F);
                        //    Thread.Sleep(2000);
                        //    continue;
                        //}

                        // 检测到正在【店长特供】首页
                        Point? openPoint = MatchTemplateImgByName(_intPtrGame, ETemplateName.StartSceneOpen);
                        if (openPoint != null)
                        {
                            _managerStatus = EManagerStatus.Idle;
                            switch (_executeType)
                            {
                                case EExecuteType.AllClear:
                                    // 执行查找未完美通关的关卡并开始
                                    await SelectStageToStart();
                                    break;
                                case EExecuteType.Repeat:
                                    // 执行重复通关模式
                                    await SelectStageToStart(_repeatStage);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "【ManagerLoop】执行过程中发生异常");
                        _managerStatus = EManagerStatus.Idle;
                    }

                    Thread.Sleep(INTERVAL_MS);
                }
            }
            catch (OperationCanceledException)
            {
                Log.Warn("任务 [ManagerLoop] 收到取消信号");
            }
            catch (Exception e)
            {
                Log.Error(e, "任务 [ManagerLoop] 发生错误");
                ShowErrorMessage(e.Message);
            }
            finally
            {
                Log.Info("任务 [ManagerLoop] 已停止");
            }
        }
    }
}
