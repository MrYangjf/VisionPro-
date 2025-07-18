using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasonteVision
{
    public partial class MV_Form_Loading : Form
    {
        public MV_Form_Loading()
        {

            InitializeComponent();
            label_Loadingtext.Text = "";
        }

        #region 功能函数

        public void LoadMessage(string str, int val)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((new EventHandler(delegate
                {
                    Update();

                    label_Loadingtext.Text = str;
                    progressBar_Loading.Value = val;

                    DateTime st = DateTime.Now;
                    DateTime sn;
                    double dd = 0;
                    while (dd < 500)
                    {
                        sn = DateTime.Now;
                        dd = (sn - st).TotalMilliseconds;
                    }
                })));
            }
            else
            {
                Update();

                label_Loadingtext.Text = str;
                progressBar_Loading.Value = val;

                DateTime st = DateTime.Now;
                DateTime sn;
                double dd = 0;
                while (dd < 500)
                {
                    sn = DateTime.Now;
                    dd = (sn - st).TotalMilliseconds;
                }
            }
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            try
            {
                Image img = Image.FromFile(Application.StartupPath + "\\loading.jpg");
                this.BackgroundImage = img;

            }
            catch
            {
                //MessageBox.Show("动态加载图片失败!");
            }
        }

        #endregion 功能函数
    }
}
