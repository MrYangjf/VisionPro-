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
    public partial class MessageBoxEX : Form
    {

        public MessageBoxEX(string caption, string message)
        {
            InitializeComponent();
            SetString(caption, message);
        }

        public void SetString(string caption, string message)
        {
            Caption.Text = caption;
            Tipstext.Text = message;
        }

        private static MessageBoxEX _instance;
        private static DialogResult myresult;
        private bool bMove;
        int windowsMoveDetaX;
        int windowsMoveDetaY;
        int windowsMoveTempX;
        int windowsMoveTempY;

        static void Instance(string caption, string message)
        {
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new MessageBoxEX(caption, message);
            }
            else
            {
                _instance.SetString(caption, message);
            }
        }


        public static DialogResult ShowMessageBox(string caption, string message)
        {
            Instance(caption, message);
            myresult = new DialogResult();
            _instance.ShowDialog();
            return myresult;
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            myresult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myresult = DialogResult.Cancel;
            this.Close();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            bMove = true;
            windowsMoveTempX = Cursor.Position.X;
            windowsMoveTempY = Cursor.Position.Y;
        }

        private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (bMove)
            {
                windowsMoveDetaX = Cursor.Position.X - windowsMoveTempX;
                windowsMoveDetaY = Cursor.Position.Y - windowsMoveTempY;
                windowsMoveTempX = Cursor.Position.X;
                windowsMoveTempY = Cursor.Position.Y;
                this.Location = new Point(this.Location.X + windowsMoveDetaX, this.Location.Y + windowsMoveDetaY);
            }
        }

        private void toolStrip1_MouseUp(object sender, MouseEventArgs e)
        {
            bMove = false;
        }

        private void toolStrip1_MouseLeave(object sender, EventArgs e)
        {
            bMove = false;
        }
    }
}
