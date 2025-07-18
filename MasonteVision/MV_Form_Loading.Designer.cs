namespace MasonteVision
{
    partial class MV_Form_Loading
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MV_Form_Loading));
            this.progressBar_Loading = new System.Windows.Forms.ProgressBar();
            this.label_Loadingtext = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar_Loading
            // 
            resources.ApplyResources(this.progressBar_Loading, "progressBar_Loading");
            this.progressBar_Loading.Name = "progressBar_Loading";
            // 
            // label_Loadingtext
            // 
            resources.ApplyResources(this.label_Loadingtext, "label_Loadingtext");
            this.label_Loadingtext.Name = "label_Loadingtext";
            // 
            // MV_Form_Loading
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_Loadingtext);
            this.Controls.Add(this.progressBar_Loading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MV_Form_Loading";
            this.Load += new System.EventHandler(this.Loading_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar_Loading;
        private System.Windows.Forms.Label label_Loadingtext;
    }
}