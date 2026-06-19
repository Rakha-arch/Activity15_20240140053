namespace CRUDMahasiswaADO
{
    partial class Dashboard
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartMhs = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbJenis = new System.Windows.Forms.ComboBox();
            this.dtpThn = new System.Windows.Forms.DateTimePicker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnKeFormMhs = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartMhs)).BeginInit();
            this.SuspendLayout();
            // 
            // chartMhs
            // 
            chartArea1.Name = "ChartArea1";
            this.chartMhs.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartMhs.Legends.Add(legend1);
            this.chartMhs.Location = new System.Drawing.Point(37, 137);
            this.chartMhs.Name = "chartMhs";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartMhs.Series.Add(series1);
            this.chartMhs.Size = new System.Drawing.Size(686, 317);
            this.chartMhs.TabIndex = 0;
            this.chartMhs.Text = "chart1";
            this.chartMhs.Click += new System.EventHandler(this.chartMhs_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(296, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "REKAP DATA MAHASISWA";
            // 
            // cmbJenis
            // 
            this.cmbJenis.FormattingEnabled = true;
            this.cmbJenis.Items.AddRange(new object[] {
            "Column",
            "Pie",
            "Line"});
            this.cmbJenis.Location = new System.Drawing.Point(648, 103);
            this.cmbJenis.Name = "cmbJenis";
            this.cmbJenis.Size = new System.Drawing.Size(121, 28);
            this.cmbJenis.TabIndex = 2;
            // 
            // dtpThn
            // 
            this.dtpThn.CustomFormat = "yyyy";
            this.dtpThn.Location = new System.Drawing.Point(131, 101);
            this.dtpThn.Name = "dtpThn";
            this.dtpThn.Size = new System.Drawing.Size(200, 26);
            this.dtpThn.TabIndex = 3;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(353, 102);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(109, 28);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load Data";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click_1);
            // 
            // btnKeFormMhs
            // 
            this.btnKeFormMhs.Location = new System.Drawing.Point(577, 460);
            this.btnKeFormMhs.Name = "btnKeFormMhs";
            this.btnKeFormMhs.Size = new System.Drawing.Size(211, 34);
            this.btnKeFormMhs.TabIndex = 5;
            this.btnKeFormMhs.Text = "Kelola Data Mahasiswa";
            this.btnKeFormMhs.UseVisualStyleBackColor = true;
            this.btnKeFormMhs.Click += new System.EventHandler(this.btnKeFormMhs_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Tahun Masuk";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(498, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 7;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 506);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnKeFormMhs);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dtpThn);
            this.Controls.Add(this.cmbJenis);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chartMhs);
            this.Name = "Dashboard";
            this.Text = "DASHBOARD";
            ((System.ComponentModel.ISupportInitialize)(this.chartMhs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartMhs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbJenis;
        private System.Windows.Forms.DateTimePicker dtpThn;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnKeFormMhs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}