using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasonteVision
{
    public partial class MV_UC_Ethernet : UserControl
    {
        public MV_UC_Ethernet()
        {
            InitializeComponent();
            InitCreatSocket();
        }

        void InitCreatSocket()
        {
            if(MV_Global_Variable.SocketType== "Server")
            {
                radioButton_ServerSocket.Checked = true;
                groupBox_Socket.Enabled = true;
                textBox_IP.Text = MV_Global_Variable.ServerSocketIP;
                textBox_Port.Text = MV_Global_Variable.ServerSocketPort;
            }
            else if(MV_Global_Variable.SocketType == "Client")
            {
                radioButton_ClientSocket.Checked = true;
                groupBox_Socket.Enabled = true;
                textBox_IP.Text = MV_Global_Variable.RemoteServerSocketIP;
                textBox_Port.Text = MV_Global_Variable.RemoteServerSocketPort;
            }
        }

        
    }
}
