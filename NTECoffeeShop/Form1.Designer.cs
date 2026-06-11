namespace NTECoffeeShop
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.selectStage = new System.Windows.Forms.ComboBox();
            this.rdBtnRepeat = new System.Windows.Forms.RadioButton();
            this.rdBtnClear = new System.Windows.Forms.RadioButton();
            this.selLanguage = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(45, 176);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(157, 50);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "自动营业";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(264, 176);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(157, 50);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "暂停营业";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // selectStage
            // 
            this.selectStage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectStage.FormattingEnabled = true;
            this.selectStage.Location = new System.Drawing.Point(180, 64);
            this.selectStage.Name = "selectStage";
            this.selectStage.Size = new System.Drawing.Size(87, 23);
            this.selectStage.TabIndex = 2;
            this.selectStage.SelectedIndexChanged += new System.EventHandler(this.selectStage_SelectedIndexChanged);
            // 
            // rdBtnRepeat
            // 
            this.rdBtnRepeat.AutoSize = true;
            this.rdBtnRepeat.Location = new System.Drawing.Point(45, 68);
            this.rdBtnRepeat.Name = "rdBtnRepeat";
            this.rdBtnRepeat.Size = new System.Drawing.Size(118, 19);
            this.rdBtnRepeat.TabIndex = 4;
            this.rdBtnRepeat.TabStop = true;
            this.rdBtnRepeat.Text = "重复执行关卡";
            this.rdBtnRepeat.UseVisualStyleBackColor = true;
            this.rdBtnRepeat.CheckedChanged += new System.EventHandler(this.rdBtnRepeat_CheckedChanged);
            // 
            // rdBtnClear
            // 
            this.rdBtnClear.AutoSize = true;
            this.rdBtnClear.Location = new System.Drawing.Point(45, 105);
            this.rdBtnClear.Name = "rdBtnClear";
            this.rdBtnClear.Size = new System.Drawing.Size(148, 19);
            this.rdBtnClear.TabIndex = 5;
            this.rdBtnClear.TabStop = true;
            this.rdBtnClear.Text = "自动通关全部关卡";
            this.rdBtnClear.UseVisualStyleBackColor = true;
            this.rdBtnClear.CheckedChanged += new System.EventHandler(this.rdBtnClear_CheckedChanged);
            // 
            // selLanguage
            // 
            this.selLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selLanguage.FormattingEnabled = true;
            this.selLanguage.Location = new System.Drawing.Point(350, 12);
            this.selLanguage.Name = "selLanguage";
            this.selLanguage.Size = new System.Drawing.Size(98, 23);
            this.selLanguage.TabIndex = 6;
            this.selLanguage.SelectedIndexChanged += new System.EventHandler(this.selLanguage_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 259);
            this.Controls.Add(this.selLanguage);
            this.Controls.Add(this.rdBtnClear);
            this.Controls.Add(this.rdBtnRepeat);
            this.Controls.Add(this.selectStage);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "安魂曲店长";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.ComboBox selectStage;
        private System.Windows.Forms.RadioButton rdBtnRepeat;
        private System.Windows.Forms.RadioButton rdBtnClear;
        private System.Windows.Forms.ComboBox selLanguage;
    }
}

