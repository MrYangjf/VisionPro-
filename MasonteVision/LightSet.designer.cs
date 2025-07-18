namespace MasonteVision
{
    partial class LightSet
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_ChannelIndex = new System.Windows.Forms.TextBox();
            this.textBox_Intensity = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonSetIntensity = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Open = new System.Windows.Forms.Button();
            this.textBox_IpAddress = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton_IP = new System.Windows.Forms.RadioButton();
            this.textBox_SerialPortIndex = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButton_SerialPort = new System.Windows.Forms.RadioButton();
            this.textBox_messageShow = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.btnConnectCSTL = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnConnectCSTL);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textBox_ChannelIndex);
            this.groupBox2.Controls.Add(this.textBox_Intensity);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.buttonSetIntensity);
            this.groupBox2.Location = new System.Drawing.Point(12, 35);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 161);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "光源操作";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "亮度设置：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(46, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "通道设置：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(233, 69);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 21;
            this.label9.Text = "(范围:[0-255])";
            // 
            // textBox_ChannelIndex
            // 
            this.textBox_ChannelIndex.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_ChannelIndex.Location = new System.Drawing.Point(113, 25);
            this.textBox_ChannelIndex.MaxLength = 2;
            this.textBox_ChannelIndex.Name = "textBox_ChannelIndex";
            this.textBox_ChannelIndex.Size = new System.Drawing.Size(114, 21);
            this.textBox_ChannelIndex.TabIndex = 15;
            this.textBox_ChannelIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_Intensity
            // 
            this.textBox_Intensity.Location = new System.Drawing.Point(113, 64);
            this.textBox_Intensity.MaxLength = 3;
            this.textBox_Intensity.Name = "textBox_Intensity";
            this.textBox_Intensity.Size = new System.Drawing.Size(114, 21);
            this.textBox_Intensity.TabIndex = 20;
            this.textBox_Intensity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(233, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "(范围:[0-8])";
            // 
            // buttonSetIntensity
            // 
            this.buttonSetIntensity.Location = new System.Drawing.Point(252, 110);
            this.buttonSetIntensity.Name = "buttonSetIntensity";
            this.buttonSetIntensity.Size = new System.Drawing.Size(111, 23);
            this.buttonSetIntensity.TabIndex = 19;
            this.buttonSetIntensity.Text = "设定";
            this.buttonSetIntensity.UseVisualStyleBackColor = true;
            this.buttonSetIntensity.Click += new System.EventHandler(this.buttonSetIntensity_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_Open);
            this.groupBox1.Controls.Add(this.textBox_IpAddress);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.radioButton_IP);
            this.groupBox1.Controls.Add(this.textBox_SerialPortIndex);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButton_SerialPort);
            this.groupBox1.Location = new System.Drawing.Point(12, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(27, 27);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication Mode Select";
            this.groupBox1.Visible = false;
            // 
            // button_Open
            // 
            this.button_Open.Location = new System.Drawing.Point(20, 155);
            this.button_Open.Name = "button_Open";
            this.button_Open.Size = new System.Drawing.Size(93, 23);
            this.button_Open.TabIndex = 25;
            this.button_Open.Text = "Connect";
            this.button_Open.UseVisualStyleBackColor = true;
            this.button_Open.Click += new System.EventHandler(this.button_Open_Click);
            // 
            // textBox_IpAddress
            // 
            this.textBox_IpAddress.Location = new System.Drawing.Point(113, 119);
            this.textBox_IpAddress.MaxLength = 16;
            this.textBox_IpAddress.Name = "textBox_IpAddress";
            this.textBox_IpAddress.Size = new System.Drawing.Size(125, 21);
            this.textBox_IpAddress.TabIndex = 23;
            this.textBox_IpAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "IP Address:";
            // 
            // radioButton_IP
            // 
            this.radioButton_IP.AutoSize = true;
            this.radioButton_IP.Location = new System.Drawing.Point(16, 93);
            this.radioButton_IP.Name = "radioButton_IP";
            this.radioButton_IP.Size = new System.Drawing.Size(185, 16);
            this.radioButton_IP.TabIndex = 21;
            this.radioButton_IP.TabStop = true;
            this.radioButton_IP.Text = "Ethernet Communiction By IP";
            this.radioButton_IP.UseVisualStyleBackColor = true;
            this.radioButton_IP.CheckedChanged += new System.EventHandler(this.radioButton_IP_CheckedChanged);
            // 
            // textBox_SerialPortIndex
            // 
            this.textBox_SerialPortIndex.Location = new System.Drawing.Point(138, 47);
            this.textBox_SerialPortIndex.MaxLength = 5;
            this.textBox_SerialPortIndex.Name = "textBox_SerialPortIndex";
            this.textBox_SerialPortIndex.Size = new System.Drawing.Size(100, 21);
            this.textBox_SerialPortIndex.TabIndex = 15;
            this.textBox_SerialPortIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "SerialportIndex:";
            // 
            // radioButton_SerialPort
            // 
            this.radioButton_SerialPort.AutoSize = true;
            this.radioButton_SerialPort.Location = new System.Drawing.Point(16, 25);
            this.radioButton_SerialPort.Name = "radioButton_SerialPort";
            this.radioButton_SerialPort.Size = new System.Drawing.Size(167, 16);
            this.radioButton_SerialPort.TabIndex = 13;
            this.radioButton_SerialPort.TabStop = true;
            this.radioButton_SerialPort.Text = "SerialPort Communication";
            this.radioButton_SerialPort.UseVisualStyleBackColor = true;
            this.radioButton_SerialPort.CheckedChanged += new System.EventHandler(this.radioButton_SerialPort_CheckedChanged);
            // 
            // textBox_messageShow
            // 
            this.textBox_messageShow.Location = new System.Drawing.Point(12, 202);
            this.textBox_messageShow.Name = "textBox_messageShow";
            this.textBox_messageShow.ReadOnly = true;
            this.textBox_messageShow.Size = new System.Drawing.Size(376, 21);
            this.textBox_messageShow.TabIndex = 25;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(397, 25);
            this.toolStrip1.TabIndex = 26;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::MasonteVision.Properties.Resources.退出2;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.ToolStripButton1_Click);
            // 
            // btnConnectCSTL
            // 
            this.btnConnectCSTL.Location = new System.Drawing.Point(16, 112);
            this.btnConnectCSTL.Name = "btnConnectCSTL";
            this.btnConnectCSTL.Size = new System.Drawing.Size(111, 23);
            this.btnConnectCSTL.TabIndex = 23;
            this.btnConnectCSTL.Text = "重连";
            this.btnConnectCSTL.UseVisualStyleBackColor = true;
            this.btnConnectCSTL.Click += new System.EventHandler(this.btnConnectCSTL_Click);
            // 
            // LightSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 238);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBox_messageShow);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LightSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_ChannelIndex;
        private System.Windows.Forms.TextBox textBox_Intensity;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonSetIntensity;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Open;
        private System.Windows.Forms.TextBox textBox_IpAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioButton_IP;
        private System.Windows.Forms.TextBox textBox_SerialPortIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton_SerialPort;
        private System.Windows.Forms.TextBox textBox_messageShow;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnectCSTL;
    }
}

