namespace MasonteVision
{
    partial class MV_UC_Ethernet
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MV_UC_Ethernet));
            this.groupBox_Socket = new System.Windows.Forms.GroupBox();
            this.radioButton_ServerSocket = new System.Windows.Forms.RadioButton();
            this.radioButton_ClientSocket = new System.Windows.Forms.RadioButton();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_Msg = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox_Socket.SuspendLayout();
            this.groupBox_Msg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Socket
            // 
            resources.ApplyResources(this.groupBox_Socket, "groupBox_Socket");
            this.groupBox_Socket.Controls.Add(this.radioButton_ServerSocket);
            this.groupBox_Socket.Controls.Add(this.radioButton_ClientSocket);
            this.groupBox_Socket.Controls.Add(this.textBox_Port);
            this.groupBox_Socket.Controls.Add(this.label2);
            this.groupBox_Socket.Controls.Add(this.textBox_IP);
            this.groupBox_Socket.Controls.Add(this.label1);
            this.groupBox_Socket.Name = "groupBox_Socket";
            this.groupBox_Socket.TabStop = false;
            // 
            // radioButton_ServerSocket
            // 
            resources.ApplyResources(this.radioButton_ServerSocket, "radioButton_ServerSocket");
            this.radioButton_ServerSocket.Name = "radioButton_ServerSocket";
            this.radioButton_ServerSocket.TabStop = true;
            this.radioButton_ServerSocket.UseVisualStyleBackColor = true;
            // 
            // radioButton_ClientSocket
            // 
            resources.ApplyResources(this.radioButton_ClientSocket, "radioButton_ClientSocket");
            this.radioButton_ClientSocket.Name = "radioButton_ClientSocket";
            this.radioButton_ClientSocket.TabStop = true;
            this.radioButton_ClientSocket.UseVisualStyleBackColor = true;
            // 
            // textBox_Port
            // 
            resources.ApplyResources(this.textBox_Port, "textBox_Port");
            this.textBox_Port.Name = "textBox_Port";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBox_IP
            // 
            resources.ApplyResources(this.textBox_IP, "textBox_IP");
            this.textBox_IP.Name = "textBox_IP";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox_Msg
            // 
            resources.ApplyResources(this.groupBox_Msg, "groupBox_Msg");
            this.groupBox_Msg.Controls.Add(this.checkBox3);
            this.groupBox_Msg.Controls.Add(this.richTextBox1);
            this.groupBox_Msg.Name = "groupBox_Msg";
            this.groupBox_Msg.TabStop = false;
            // 
            // checkBox3
            // 
            resources.ApplyResources(this.checkBox3, "checkBox3");
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.groupBox_Socket);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.groupBox_Msg);
            // 
            // MV_UC_Ethernet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MV_UC_Ethernet";
            this.groupBox_Socket.ResumeLayout(false);
            this.groupBox_Socket.PerformLayout();
            this.groupBox_Msg.ResumeLayout(false);
            this.groupBox_Msg.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Socket;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_Msg;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.RadioButton radioButton_ServerSocket;
        private System.Windows.Forms.RadioButton radioButton_ClientSocket;
    }
}
