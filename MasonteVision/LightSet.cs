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
    public partial class LightSet : Form
    {
        public enum CST_COMMUNICATION_MODE : int
        {
            COMMUNICATION_BY_COM,
            COMMUNICATION_BY_IP,
        }

        private CST_COMMUNICATION_MODE mMode = 0; 
        private int serialPortIndex;
        private String IpAddress;
        private int connectTimeOut = 1;
        public CSTControllerAPI mController = null;
        public LightSet()
        {
            mController = new CSTControllerAPI();
            InitializeComponent();
            textBox_IpAddress.Text = "192.168.1.118";
            textBox_SerialPortIndex.Text = "1";
            textBox_ChannelIndex.Text = "1";
            textBox_Intensity.Text = "255";
        }

        private void radioButton_SerialPort_CheckedChanged(object sender, EventArgs e)
        {
            mMode = CST_COMMUNICATION_MODE.COMMUNICATION_BY_COM;
            textBox_messageShow.Text = "Succeed to set the communication mode is Rs232";
        }

        private void radioButton_IP_CheckedChanged(object sender, EventArgs e)
        {
            mMode = CST_COMMUNICATION_MODE.COMMUNICATION_BY_IP;
            textBox_messageShow.Text = "Succeed to set the communication mode is Enthernet";
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            string serialPortIndexBuff = textBox_SerialPortIndex.Text;
            if ("" == serialPortIndexBuff)
            {
                textBox_messageShow.Text = "Serial name can not be empty";
                return;
            }
            serialPortIndex = Convert.ToInt32(serialPortIndexBuff);
            IpAddress = textBox_IpAddress.Text;
            long iRet = -1;
            if ("Connect" == button_Open.Text)
            {
                switch (mMode)
                {
                    case CST_COMMUNICATION_MODE.COMMUNICATION_BY_COM:
                        {
                            iRet = mController.CreateSerialPort(serialPortIndex);
                            if (10000 != iRet)
                            {
                                textBox_messageShow.Text = "Failed to initialize serial port";
                                return;
                            }
                            else
                            {
                                textBox_messageShow.Text = "Succeed to connect";
                            }
                        }
                        break;
                    case CST_COMMUNICATION_MODE.COMMUNICATION_BY_IP:
                        {
                            if ("" == IpAddress)
                            {
                                textBox_messageShow.Text = "IP can not be empty";
                                return;
                            }

                            iRet = mController.ConnectIP(IpAddress, connectTimeOut);
                            if (10000 != iRet)
                            {
                                textBox_messageShow.Text = "Failed to create Ethernet connection by IP";
                                return;
                            }
                            else
                            {
                                textBox_messageShow.Text = "Succeed to connect";
                            }
                        }
                        break;
                    default:
                        return;
                }
                button_Open.Text = "Disconnect";
                radioButton_SerialPort.Enabled = false;
                textBox_SerialPortIndex.Enabled = false;
                radioButton_IP.Enabled = false;
                textBox_IpAddress.Enabled = false;
            }
            else if ("Disconnect" == button_Open.Text)
            {
                switch (mMode)
                {
                    case CST_COMMUNICATION_MODE.COMMUNICATION_BY_COM:
                        {
                            iRet = mController.ReleaseSerialPort();
                            if (10000 != iRet)
                            {
                                textBox_messageShow.Text = "Failed to release serial port";
                                return;
                            }
                            else
                            {
                                textBox_messageShow.Text = "Succeed to disconnect";
                            }
                        }
                        break;
                    case CST_COMMUNICATION_MODE.COMMUNICATION_BY_IP:
                        {
                            iRet = mController.DestroyIpConnection();
                            if (10000 != iRet)
                            {
                                textBox_messageShow.Text = "Failed to disconnect Ethernet connection by IP";
                                return;
                            }
                            else
                            {
                                textBox_messageShow.Text = "Succeed to disconnect";
                            }
                        }
                        break;
                    default:
                        return;
                }
                button_Open.Text = "Connect";
                radioButton_SerialPort.Enabled = true;
                textBox_SerialPortIndex.Enabled = true;
                radioButton_IP.Enabled = true;
                textBox_IpAddress.Enabled = true;
            }
        }

        private void buttonSetIntensity_Click(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(textBox_ChannelIndex.Text);
            int intensity = Convert.ToInt32(textBox_Intensity.Text);
            if (mController.SetDigitalValue(channel, intensity) == 10000)
            {
                textBox_messageShow.Text = "Succeed to set intensity";
            }
            else
            {
                textBox_messageShow.Text = "Failed to set intensity";
            }
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnConnectCSTL_Click(object sender, EventArgs e)
        {
            mController.CreateSerialPort(4);
        }
    }
}
