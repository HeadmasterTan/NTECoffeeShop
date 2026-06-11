using NLog;
using NTECoffeeShop.CoffeeShop;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NTECoffeeShop
{
    public partial class Form1 : Form
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static StoreManager _storeManager = StoreManager.GetInstance();

        private static string[] _languagesList = { "简体中文", "繁体中文", "English", "日本語" };
        private static List<string> _stageList;
        private static string _hotKeyText = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void InitStageList()
        {
            _stageList = new List<string>();

            for (int i = 1; i <= 3; i++)
            {
                int startIndex = 1, endIndex = 10;
                if (i == 1) startIndex = 0;

                for (int j = startIndex; j <= endIndex; j++)
                {
                    _stageList.Add($"{i}-{j}");
                }
            }
        }

        private void InitializeData()
        {
            selLanguage.DataSource = _languagesList;
            selLanguage.SelectedIndex = 0;

            InitStageList();
            selectStage.DataSource = _stageList;
            selectStage.SelectedIndex = 1;

            rdBtnRepeat.Checked = true;
            _storeManager.ExecuteType = EExecuteType.Repeat;
            _storeManager.RepeatStage = selectStage.SelectedItem.ToString();

            SetChineseSimplified();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 注册全局热键 F11
            bool hotkeyRegistered = SimulateEventHandler.RegisterHotKey(
                this.Handle,
                SimulateEventHandler.HOTKEY_ID,
                SimulateEventHandler.MOD_NONE,
                SimulateEventHandler.VK_F11);

            if (hotkeyRegistered)
            {
                _hotKeyText = "[F11]\n";
            }

            InitializeData();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312; // 热键消息常量

            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == SimulateEventHandler.HOTKEY_ID)
                {
                    HandleHotKey();
                }
            }

            base.WndProc(ref m);
        }

        private void HandleHotKey()
        {
            if (btnStart.Enabled)
            {
                HandleStart();
            }
            else if (btnPause.Enabled)
            {
                HandlePause();
            }
        }

        private void HandleStart()
        {
            try
            {
                _storeManager.Start();
                if (btnStart.Enabled)
                {
                    btnStart.Enabled = false;
                    btnPause.Enabled = true;

                    rdBtnClear.Enabled = false;
                    rdBtnRepeat.Enabled = false;
                    selectStage.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "【HandleStart】工具启动发生异常");
            }
        }

        private void HandlePause()
        {
            try
            {
                _storeManager.Pause();

                if (btnPause.Enabled)
                {
                    btnStart.Enabled = true;
                    btnPause.Enabled = false;

                    rdBtnClear.Enabled = true;
                    rdBtnRepeat.Enabled = true;
                    selectStage.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "【HandlePause】工具暂停发生异常");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            HandleStart();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            HandlePause();
        }

        private void selLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = selLanguage.SelectedItem.ToString();
            switch (item)
            {
                case "繁体中文":
                    SetChineseTraditional();
                    break;
                case "English":
                    SetEnglish();
                    break;
                case "日本語":
                    SetJapanese();
                    break;
                default:
                    SetChineseSimplified();
                    break;
            }
        }

        private void SetChineseSimplified()
        {
            _storeManager.CurrentLanguage = selLanguage.SelectedItem.ToString();

            this.Text = "安魂曲代理店长";
            rdBtnRepeat.Text = "重复执行关卡";
            rdBtnClear.Text = "自动通关全部关卡";
            btnStart.Text = _hotKeyText + "自动营业";
            btnPause.Text = _hotKeyText + "暂停营业";
        }

        private void SetChineseTraditional()
        {
            _storeManager.CurrentLanguage = selLanguage.SelectedItem.ToString();

            this.Text = "安魂曲代理店長";
            rdBtnRepeat.Text = "重複執行關卡";
            rdBtnClear.Text = "自動通關全部關卡";
            btnStart.Text = _hotKeyText + "自動營業";
            btnPause.Text = _hotKeyText + "暫停營業";
        }

        private void SetEnglish()
        {
            _storeManager.CurrentLanguage = selLanguage.SelectedItem.ToString();

            this.Text = "Acting Manager: Lacrimosa";
            rdBtnRepeat.Text = "Repeat Stage";
            rdBtnClear.Text = "Auto-Clear All Stages";
            btnStart.Text = _hotKeyText + "Auto-Manage";
            btnPause.Text = _hotKeyText + "Pause Management";
        }

        private void SetJapanese()
        {
            _storeManager.CurrentLanguage = selLanguage.SelectedItem.ToString();

            this.Text = "代理店長：レクイエム";
            rdBtnRepeat.Text = "ステージ周回";
            rdBtnClear.Text = "全ステージ自動攻略";
            btnStart.Text = _hotKeyText + "自動営業";
            btnPause.Text = _hotKeyText + "営業一時停止";
        }

        private void rdBtnRepeat_CheckedChanged(object sender, EventArgs e)
        {
            selectStage.Enabled = rdBtnRepeat.Checked;
            if (rdBtnRepeat.Checked)
            {
                _storeManager.ExecuteType = EExecuteType.Repeat;
                _storeManager.RepeatStage = selectStage.SelectedItem.ToString();
            }
        }

        private void rdBtnClear_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnClear.Checked)
            {
                _storeManager.ExecuteType = EExecuteType.AllClear;
            }
        }

        private void selectStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _storeManager.RepeatStage = selectStage.SelectedItem.ToString();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _storeManager.Stop();
        }
    }
}
