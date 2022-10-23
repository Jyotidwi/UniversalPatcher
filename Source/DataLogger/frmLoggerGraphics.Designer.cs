﻿
namespace UniversalPatcher
{
    partial class frmLoggerGraphics
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLoggerGraphics));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chkShowPoints = new System.Windows.Forms.CheckBox();
            this.groupLiveSeconds = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numShowMax = new System.Windows.Forms.NumericUpDown();
            this.numDisplayInterval = new System.Windows.Forms.NumericUpDown();
            this.labelShowMax = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.dataGridValues = new System.Windows.Forms.DataGridView();
            this.txtLogSeparator = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDisplayData = new System.Windows.Forms.Timer(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadLastLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupLiveSeconds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numShowMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDisplayInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridValues)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Area3DStyle.Enable3D = true;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.Name = "ChartArea1";
            chartArea1.ShadowColor = System.Drawing.Color.White;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 2;
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(947, 573);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chkShowPoints);
            this.splitContainer1.Panel1.Controls.Add(this.groupLiveSeconds);
            this.splitContainer1.Panel1.Controls.Add(this.btnApply);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridValues);
            this.splitContainer1.Panel1.Controls.Add(this.txtLogSeparator);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chart1);
            this.splitContainer1.Size = new System.Drawing.Size(1199, 573);
            this.splitContainer1.SplitterDistance = 248;
            this.splitContainer1.TabIndex = 2;
            // 
            // chkShowPoints
            // 
            this.chkShowPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowPoints.AutoSize = true;
            this.chkShowPoints.Location = new System.Drawing.Point(12, 454);
            this.chkShowPoints.Name = "chkShowPoints";
            this.chkShowPoints.Size = new System.Drawing.Size(84, 17);
            this.chkShowPoints.TabIndex = 11;
            this.chkShowPoints.Text = "Show points";
            this.chkShowPoints.UseVisualStyleBackColor = true;
            // 
            // groupLiveSeconds
            // 
            this.groupLiveSeconds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLiveSeconds.Controls.Add(this.label2);
            this.groupLiveSeconds.Controls.Add(this.numShowMax);
            this.groupLiveSeconds.Controls.Add(this.numDisplayInterval);
            this.groupLiveSeconds.Controls.Add(this.labelShowMax);
            this.groupLiveSeconds.Location = new System.Drawing.Point(3, 504);
            this.groupLiveSeconds.Name = "groupLiveSeconds";
            this.groupLiveSeconds.Size = new System.Drawing.Size(243, 66);
            this.groupLiveSeconds.TabIndex = 8;
            this.groupLiveSeconds.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Display interval seconds";
            // 
            // numShowMax
            // 
            this.numShowMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numShowMax.Location = new System.Drawing.Point(188, 13);
            this.numShowMax.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numShowMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numShowMax.Name = "numShowMax";
            this.numShowMax.Size = new System.Drawing.Size(40, 20);
            this.numShowMax.TabIndex = 5;
            this.numShowMax.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numDisplayInterval
            // 
            this.numDisplayInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numDisplayInterval.Location = new System.Drawing.Point(188, 40);
            this.numDisplayInterval.Name = "numDisplayInterval";
            this.numDisplayInterval.Size = new System.Drawing.Size(39, 20);
            this.numDisplayInterval.TabIndex = 7;
            this.numDisplayInterval.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numDisplayInterval.ValueChanged += new System.EventHandler(this.numDisplayInterval_ValueChanged);
            // 
            // labelShowMax
            // 
            this.labelShowMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelShowMax.AutoSize = true;
            this.labelShowMax.Location = new System.Drawing.Point(6, 15);
            this.labelShowMax.Name = "labelShowMax";
            this.labelShowMax.Size = new System.Drawing.Size(102, 13);
            this.labelShowMax.TabIndex = 4;
            this.labelShowMax.Text = "Show max seconds:";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Location = new System.Drawing.Point(12, 477);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnShowData_Click);
            // 
            // dataGridValues
            // 
            this.dataGridValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridValues.Location = new System.Drawing.Point(3, 3);
            this.dataGridValues.Name = "dataGridValues";
            this.dataGridValues.Size = new System.Drawing.Size(242, 442);
            this.dataGridValues.TabIndex = 2;
            // 
            // txtLogSeparator
            // 
            this.txtLogSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogSeparator.Location = new System.Drawing.Point(192, 451);
            this.txtLogSeparator.Name = "txtLogSeparator";
            this.txtLogSeparator.Size = new System.Drawing.Size(39, 20);
            this.txtLogSeparator.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 454);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Log separator:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1199, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadLogfileToolStripMenuItem,
            this.loadLastLogfileToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadProfileToolStripMenuItem,
            this.saveProfileToolStripMenuItem,
            this.saveProfileAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadLogfileToolStripMenuItem
            // 
            this.loadLogfileToolStripMenuItem.Name = "loadLogfileToolStripMenuItem";
            this.loadLogfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadLogfileToolStripMenuItem.Text = "Load logfile";
            this.loadLogfileToolStripMenuItem.Click += new System.EventHandler(this.loadLogfileToolStripMenuItem_Click);
            // 
            // loadProfileToolStripMenuItem
            // 
            this.loadProfileToolStripMenuItem.Name = "loadProfileToolStripMenuItem";
            this.loadProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadProfileToolStripMenuItem.Text = "Load profile";
            this.loadProfileToolStripMenuItem.Click += new System.EventHandler(this.loadProfileToolStripMenuItem_Click);
            // 
            // saveProfileToolStripMenuItem
            // 
            this.saveProfileToolStripMenuItem.Name = "saveProfileToolStripMenuItem";
            this.saveProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProfileToolStripMenuItem.Text = "Save profile";
            this.saveProfileToolStripMenuItem.Click += new System.EventHandler(this.saveProfileToolStripMenuItem_Click);
            // 
            // saveProfileAsToolStripMenuItem
            // 
            this.saveProfileAsToolStripMenuItem.Name = "saveProfileAsToolStripMenuItem";
            this.saveProfileAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProfileAsToolStripMenuItem.Text = "Save profile as...";
            this.saveProfileAsToolStripMenuItem.Click += new System.EventHandler(this.saveProfileAsToolStripMenuItem_Click);
            // 
            // timerDisplayData
            // 
            this.timerDisplayData.Interval = 3000;
            this.timerDisplayData.Tick += new System.EventHandler(this.timerDisplayData_Tick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // loadLastLogfileToolStripMenuItem
            // 
            this.loadLastLogfileToolStripMenuItem.Name = "loadLastLogfileToolStripMenuItem";
            this.loadLastLogfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadLastLogfileToolStripMenuItem.Text = "Load last logfile";
            this.loadLastLogfileToolStripMenuItem.Click += new System.EventHandler(this.loadLastLogfileToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtResult);
            this.splitContainer2.Size = new System.Drawing.Size(1199, 665);
            this.splitContainer2.SplitterDistance = 573;
            this.splitContainer2.TabIndex = 4;
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(1199, 88);
            this.txtResult.TabIndex = 0;
            this.txtResult.Text = "";
            // 
            // frmLoggerGraphics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 689);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmLoggerGraphics";
            this.Text = "frmLoggerGraphics";
            this.Load += new System.EventHandler(this.frmLoggerGraphics_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupLiveSeconds.ResumeLayout(false);
            this.groupLiveSeconds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numShowMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDisplayInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridValues)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLogfileToolStripMenuItem;
        private System.Windows.Forms.TextBox txtLogSeparator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridValues;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.NumericUpDown numShowMax;
        private System.Windows.Forms.Label labelShowMax;
        private System.Windows.Forms.Timer timerDisplayData;
        private System.Windows.Forms.NumericUpDown numDisplayInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem loadProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileAsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupLiveSeconds;
        private System.Windows.Forms.CheckBox chkShowPoints;
        private System.Windows.Forms.ToolStripMenuItem loadLastLogfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox txtResult;
    }
}