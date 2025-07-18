using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MasonteCommInterface;
using System.Net;
using System.Net.Sockets;

namespace MasonteVision
{

    public class MV_SubThread_Flow
    {
        /// <summary>
        /// 子线程
        /// </summary>
        public Thread InitSubThread = null;

        public CS_Server MyServerSocket;
        public CS_Server MyServerSocket2;
        public CS_Server MyServerSocketScan;


        public ClientSocket MyClientSocket;
        public string[] MsgStr;
        public string[] MsgStr2;
        public string[] MsgStrScan;
        public int CalTimes;
        public bool Fixed; 

        /// <summary>
        /// 结构函数
        /// </summary>
        public MV_SubThread_Flow()
        {
            if (MV_Global_Variable.SocketType == "Server")
            {
                if (CreatServer() && Creat3DServer())
                {
                    MV_Global_Variable.CreatSocketSuccess = true;
                }
                else
                {
                    MV_Global_Variable.CreatSocketSuccess = false;
                }
            }
            else if (MV_Global_Variable.SocketType == "Client")
            {
                if (CreatClient())
                {
                    MV_Global_Variable.CreatSocketSuccess = true;
                }
                else
                {
                    MV_Global_Variable.CreatSocketSuccess = false;
                }
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MV_SubThread_Flow()
        {
            if (MV_Global_Variable.SocketType == "Server")
            {
                MyServerSocket.Stop();
                MyServerSocket2.Stop();
                MyServerSocketScan.Stop();
            }
            else if (MV_Global_Variable.SocketType == "Client")
            {
                MyClientSocket.Dispose();
            }
        }

        void CreatThread()
        {
            InitSubThread = new Thread(InitSubFunc)
            {
                IsBackground = true
            };
            InitSubThread.Start();
        }

        void InitSubFunc()
        {

        }

        public bool CreatServer()
        {
            MyServerSocket = new CS_Server();
            MyServerSocket2 = new CS_Server();
            MyServerSocket.Start(IPAddress.Parse(MV_Global_Variable.ServerSocketIP), int.Parse(MV_Global_Variable.ServerSocketPort));
            MyServerSocket2.Start(IPAddress.Parse(MV_Global_Variable.ServerSocketIP), int.Parse(MV_Global_Variable.ServerSocketPort) + 2);
            if (MyServerSocket.MonitorState)
            {
                //MyServerSocket.Message += MyServerSocket_EventIsConnect;
                MyServerSocket.Message += MyServerSocket_EventRecMsgAsync;
            }
            else
            {
                return false;
            }
            if (MyServerSocket2.MonitorState)
            {
                MyServerSocket2.Message += MyServerSocket_EventRecMsgAsync2;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Creat3DServer()
        {
            MyServerSocketScan = new CS_Server();
            MyServerSocketScan.Start(IPAddress.Parse(MV_Global_Variable.ServerSocketIP), int.Parse(MV_Global_Variable.ServerSocketPort) + 1);
            if (MyServerSocketScan.MonitorState)
            {
                //MyServerSocketScan.EventIsConnect += MyServerSocketScan_EventIsConnect;
                MyServerSocketScan.Message += MyServerSocketScan_EventRecMsg;
                return true;
            }
            return false;
        }

        private void MyServerSocketScan_EventIsConnect(object o)
        {
            //throw new NotImplementedException();
            //List<System.Net.Sockets.Socket> sockets = MyServerSocketScan.ClientList;

            //for (int i = 0; i < sockets.Count; i++)
            //{
            //    MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog("当前扫描连接客户端[" + i.ToString() + "]:" + sockets[i].RemoteEndPoint);
            //}

            //if (MyServerSocketScan.ClientCount <= 0)
            //{
            //    MyServerSocketScan.Close();
            //    Creat3DServer();
            //}
        }

        private void MyServerSocketScan_EventRecMsg(object o)
        {
            try
            {
                //throw new NotImplementedException();
                string[] reciveMsg = o.ToString().Split('@');
                MV_Global_Variable.MyFormMain.RecordOperateCor("recive:" + reciveMsg[0]);
                MsgStrScan = reciveMsg[0].Split(' ');
                if (MsgStrScan[0] == "Cam5-1")
                {
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, MV_Global_Variable.MyFormMain.LightValue5);
                    MV_Global_Variable.MyFormMain.Cam5StepStr = MsgStrScan;
                    MV_Global_Variable.MyFormMain.MyBoolRunCommand[2] = true;
                }
                if (MsgStrScan[0] == "Cam3D-1")
                {
                    MV_Global_Variable.MyFormMain.Cam3DStepStr = MsgStrScan;
                    MV_Global_Variable.MyFormMain.MyBoolRunCommand[3] = true;
                }
                if (MsgStrScan[0] == "Cam3D-2")
                {
                    MV_Global_Variable.MyFormMain.Cam3DStepStr = MsgStrScan;
                    MV_Global_Variable.MyFormMain.MyBoolRunCommand[3] = true;
                }
                if (MsgStrScan[0] == "Cam3D-3")
                {
                    MV_Global_Variable.MyFormMain.Cam3DStepStr = MsgStrScan;
                    MV_Global_Variable.MyFormMain.MyBoolRunCommand[3] = true;
                }
                if (MsgStrScan[0] == "Cam3D-4")
                {
                    MV_Global_Variable.MyFormMain.Cam3DStepStr = MsgStrScan;
                    MV_Global_Variable.MyFormMain.MyBoolRunCommand[3] = true;
                }
            }
            catch
            { }
        }

        public bool CreatClient()
        {
            MyClientSocket = new ClientSocket();
            if (MyClientSocket.ConnectServer(IPAddress.Parse(MV_Global_Variable.RemoteServerSocketIP), int.Parse(MV_Global_Variable.RemoteServerSocketPort)))
            {
                MyClientSocket.EventRecMsg += MyClientSocket_EventRecMsg;
                MyClientSocket.SendMessage("connect succeed");
                return true;
            }
            return false;
        }

        public void CloseServerSocket()
        {
            MyServerSocket.Stop();
            MyServerSocket2.Stop();
            MyServerSocketScan.Stop();
        }
        public void CloseClientSocket()
        {
            MyClientSocket.Close();
        }

        private void MyClientSocket_EventRecMsg(object o)
        {
            //throw new NotImplementedException();
        }

        public bool SendMessageToServerAndClose()
        {
            if (MyClientSocket.ConnectServer(IPAddress.Parse(MV_Global_Variable.RemoteServerSocketIP), int.Parse(MV_Global_Variable.RemoteServerSocketPort)))
            {
                return true;
            }
            return false;
        }

        private void MyServerSocket_EventRecMsgAsync(object o)
        {
            try
            {
                //throw new NotImplementedException();         
                string[] reciveMsg = o.ToString().Split('@');
                MV_Global_Variable.MyFormMain.RecordOperateCor("recive:" + reciveMsg[0]);
                MsgStr = reciveMsg[0].Split(' ');

                if (MsgStr[0] == "Cam2-1")
                {
                    MV_Global_Variable.MyFormMain.Cam34StepStr = MsgStr;
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue3) == 10000)
                    //DoLocation:Cam2-1 n X Y Q index  n代表颜色 ->Cam2-1^1^X^Y^Q^Width^Height
                    {
                        try
                        {
                            string cavityplace = MsgStr[5];
                            switch (cavityplace)
                            {
                                case "1":
                                    MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity";
                                    break;
                                case "2":
                                    MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity2";
                                    break;
                                default:
                                    MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity";
                                    break;
                            }
                        }
                        catch
                        {
                            MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity";
                        }
                        string color = MsgStr[1];
                        switch (color)
                        {
                            case "0":
                                //set diff param

                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                            case "1":
                                //set diff param
                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                            case "2":
                                //set diff param

                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                            default:
                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                        }

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }

                }
                if (MsgStr[0] == "Cam1-1")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed = false;
                    MV_Global_Variable.MyFormMain.Cam12StepStr = MsgStr;
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, MV_Global_Variable.MyFormMain.LightValue1) == 10000)
                    {
                        //DoLocation:Cam1-1 X Y Q （X,Y,Q 发A壳示教的机械坐标） （拍电池）->Cam1-1^1^X^Y^Q^Width^Height^AngleOffset
                        MV_Global_Variable.MyFormMain.MyStringRunCommand1 = "Battery";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[0] = true;

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }

                }
                if (MsgStr[0] == "Cam1-12")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed = false;
                    MV_Global_Variable.MyFormMain.Cam12StepStr = MsgStr;
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, MV_Global_Variable.MyFormMain.LightValue1) == 10000)
                    {
                        //DoLocation:Cam1-1 X Y Q （X,Y,Q 发A壳示教的机械坐标） （拍电池）->Cam1-1^1^X^Y^Q^Width^Height^AngleOffset
                        MV_Global_Variable.MyFormMain.MyStringRunCommand1 = "Battery2";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[0] = true;

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }

                }
                if (MsgStr[0] == "Cam1-2")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed = false;
                    MV_Global_Variable.MyFormMain.Cam12StepStr = MsgStr;
                    MV_Global_Variable.MyFormMain.RecordOperateCor("拍Mark");
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel2, MV_Global_Variable.MyFormMain.LightValue2) == 10000
                        && MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel3, MV_Global_Variable.MyFormMain.LightValue3) == 10000)
                    {
                        MV_Global_Variable.MyFormMain.MyStringRunCommand1 = "Mark";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[0] = true;

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }

                }
                if (MsgStr[0] == "Cam3-1")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed = false;
                    MV_Global_Variable.MyFormMain.Cam34StepStr = MsgStr;
                    //DoLocation:Cam3-1 X Y Q T ->Cam3-1^1^X^Y^Q^AngleOffset????
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue4) == 10000)
                    {
                        MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "CavityCheck";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                        CalTimes = int.Parse(MsgStr[4]);

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }
                }
                if (MsgStr[0] == "Cam3-2")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed = false;
                    MV_Global_Variable.MyFormMain.Cam34StepStr = MsgStr;
                    //DoLocation:Cam3-2 X Y Q    ->Cam3-2^1
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue4) == 10000)
                    {
                        MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Film";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }

                }
                if (MsgStr[0] == "Cam3-3")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed = false;
                    MV_Global_Variable.MyFormMain.Cam34StepStr = MsgStr;
                    //DoLocation:Cam3-3 X Y Q    ->CamCam3-3^1^Gap1^Gap2^Gap3^Gap4
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue4) == 10000)
                    {
                        MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Gap";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, 0);
                }
                if (MsgStr[0] == "Cam4-2")
                {
                    MV_Global_Variable.MyFormMain.Cam34StepStr = MsgStr;
                    //DoLocation:Cam4-2 X Y Q    ->Cam4-2^1
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue4) == 10000)
                    {
                        MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "FPC";
                        MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;

                    }
                    else { MyServerSocket.SendAll(MsgStr[0] + "^0"); }
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, 0);
                }

                if (MsgStr[0] == "Cam12Calib")
                {
                    //DoLocation:Cam12Calib n X Y Q  ->Cam12Calib^n^X^Y^Q^1
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel2, MV_Global_Variable.MyFormMain.LightValue2);
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel3, MV_Global_Variable.MyFormMain.LightValue3);
                    MV_Global_Variable.MyFormMain.NPtsStep = int.Parse(MsgStr[1]);
                    MV_Global_Variable.MyFormMain.AxisX = double.Parse(MsgStr[2]);
                    MV_Global_Variable.MyFormMain.AxisY = double.Parse(MsgStr[3]);
                    MV_Global_Variable.MyFormMain.RunCamNPtsCalib("Mark", "Cam12");
                    //}
                    //else { MyClientSocket.SendMessage(MsgStr[0] + "^0"); }
                }
                if (MsgStr[0] == "Cam34Calib")
                {
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue4);

                    //DoLocation:Cam12Calib n X Y Q  ->Cam12Calib^n^X^Y^Q^1
                    MV_Global_Variable.MyFormMain.NPtsStep = int.Parse(MsgStr[1]);
                    MV_Global_Variable.MyFormMain.AxisX = double.Parse(MsgStr[2]);
                    MV_Global_Variable.MyFormMain.AxisY = double.Parse(MsgStr[3]);
                    MV_Global_Variable.MyFormMain.RunCamNPtsCalib("CavityCheck", "Cam34");

                }
                if (MsgStr[0] == "connect" || MsgStr[0] == "Reset")
                {
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, 0);
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel2, 0);
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel3, 0);
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, 0);
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);
                }
            }
            catch
            { MV_Global_Variable.MyFormMain.RecordErrorCor("指令接收出错"); }
        }

        private void MyServerSocket_EventRecMsgAsync2(object o)
        {
            try
            {
                //throw new NotImplementedException();         
                string[] reciveMsg = o.ToString().Split('@');
                MV_Global_Variable.MyFormMain.RecordOperateCor("recive CavityCommand:" + reciveMsg[0]);
                MsgStr2 = reciveMsg[0].Split(' ');
                if (MsgStr2[0] == "Cam2-1")
                {
                    MV_Global_Variable.MyFormMain.RunSucceed2 = false;
                    MV_Global_Variable.MyFormMain.Cam34StepStr = MsgStr2;
                    if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, MV_Global_Variable.MyFormMain.LightValue3) == 10000)
                    //DoLocation:Cam2-1 n X Y Q index model  n代表颜色 ->Cam2-1^1^X^Y^Q^Width^Height
                    {
                        try
                        {
                            if(MsgStr2[6]=="Fix")
                            {
                                Fixed = true;
                            }
                            else
                            { Fixed = false; }
                            string cavityplace = MsgStr2[5];
                            switch (cavityplace)
                            {
                                case "1":
                                    MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity";
                                    break;
                                case "2":
                                    MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity2";
                                    break;
                                default:
                                    MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity";
                                    break;
                            }
                        }
                        catch
                        {
                            MV_Global_Variable.MyFormMain.MyStringRunCommand2 = "Cavity";
                        }
                        string color = MsgStr2[1].Trim();
                        switch (color)
                        {
                            case "0":
                                //set diff param

                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                            case "1":
                                //set diff param
                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                            case "2":
                                //set diff param

                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                            default:
                                MV_Global_Variable.MyFormMain.MyBoolRunCommand[1] = true;
                                break;
                        }

                    }
                    else { MyServerSocket.SendAll(MsgStr2[0] + "^0"); }

                }         
            }
            catch
            { }
        }
        private void MyServerSocket_EventIsConnect(object o)
        {
            //throw new NotImplementedException();
            //List<System.Net.Sockets.Socket> sockets = MyServerSocket.ClientSockets;

            //for (int i = 0; i < sockets.Count; i++)
            //{
            //    MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog("当前组装连接客户端[" + i.ToString() + "]:" + sockets[i].RemoteEndPoint);
            //}

            //if (MyServerSocket.ClientCount <= 0)
            //{
            //    MyServerSocket.Close();
            //    CreatServer();
            //}
        }
    }

    public class CS_Client
    {
        private TcpClient client;
        public delegate void InvokeRecord(string Message);
        public event InvokeRecord Message;
        private byte[] data = new byte[204800];

        /// <summary>  
        /// 链接服务器地址 
        /// </summary>  
        public string IP { get; set; }

        /// <summary>  
        /// 链接服务器端口号 
        /// </summary>  
        public int Port { get; set; }
        /// <summary>  
        /// 链接状态
        /// </summary>  
        private bool _Connected = false;
        public bool Connected
        {
            get
            {
                return _Connected;
            }
            set
            {
                _Connected = value;
            }
        }

        public CS_Client()
        { }
        public void Connect(string ip, int port)
        {
            try
            {
                IP = ip;
                Port = port;
                if (_Connected)
                    DisConnect();
                client = new TcpClient();
                if (!client.ConnectAsync(IP, Port).Wait(300))
                {
                    client.GetStream().Close();
                    client.Close();
                    MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("连接服务器 地址[{0}] 端口号[{1}] 失败 ", IP, Port));
                }
                else
                {
                    _Connected = true;
                    client.GetStream().BeginRead(data, 0, 50000, ReceiveMessage, null);
                }
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("连接服务器 地址[{0}] 端口号[{1}] 失败 ,原因: {2}", IP, Port, ex.ToString()));
            }
        }
        public void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                int bytesRead;
                bytesRead = client.GetStream().EndRead(ar);
                if (bytesRead < 1) return;
                else
                {
                    string Mes = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
                    if (Message != null)
                        Message(Mes);
                }
                client.GetStream().BeginRead(data, 0, 50000, ReceiveMessage, null);
            }
            catch (Exception ex)
            {
                _Connected = false;
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("服务器 地址[{0}] 端口号[{1}] 断开连接 ", IP, Port));
            }
        }
        public void DisConnect()
        {
            try
            {
                if (_Connected)
                {
                    client.GetStream().Close();
                    client.Close();
                    _Connected = false;
                }
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("断开服务器 地址[{0}] 端口号[{1}] 失败 ,原因: {2}", IP, Port, ex.ToString()));
            }
        }
        public bool Send(string Data)
        {
            bool ret = false;
            try
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("发送给服务器 地址[{0}] 端口号[{1}] 》》  {2}", IP, Port, Data));
                NetworkStream ns = client.GetStream();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(Data);
                ns.Write(data, 0, data.Length);
                ns.Flush();
                ret = true;
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("发送给服务器 地址[{0}] 端口号[{1}]失败 ,原因: {2}", IP, Port, ex.ToString()));
            }
            return ret;
        }
    }

    public class CS_Server
    {
        /// <summary>  
        /// 监听状态 
        /// </summary>  
        private bool _MonitorState = false;
        public bool MonitorState
        {
            get
            {
                return _MonitorState;
            }
            set
            {
                _MonitorState = value;
            }
        }
        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore semap = new Semaphore(5, 5000);
        /// <summary>
        /// 客户端队列集合
        /// </summary>
        public List<Clientcs> ClientList = new List<Clientcs>();
        /// <summary>
        /// 服务端
        /// </summary>
        private TcpListener listener = null;
        /// <summary>
        /// 当前IP地址
        /// </summary>
        private IPAddress Ipaddress;
        /// <summary>
        /// 当前监听端口
        /// </summary>
        private int Port;
        /// <summary>
        /// 有客户端上下线响应委托
        /// </summary>
        public delegate void ClientChange(int Num);
        public event ClientChange ChangClientList;
        public delegate void InvokeRecord(string Mes);
        public event InvokeRecord Message;

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start(IPAddress iP, int port)
        {
            try
            {
                if (listener != null)
                {
                    Stop();
                }
                Ipaddress = iP;
                Port = port;
                listener = new TcpListener(Ipaddress, Port);
                listener.Start();
                _MonitorState = true;
                Thread AccTh = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        if (!_MonitorState)
                            return;
                        else
                            GetAcceptTcpClient();
                        Thread.Sleep(50);
                    }
                }));
                AccTh.Start();
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("启动服务器[{0} , {1}] 成功 ", Ipaddress, Port));
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("启动服务器[{0} , {1}] 失败 ,原因: {2} ", Ipaddress, Port, ex.ToString()));
                _MonitorState = false;
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            try
            {
                _MonitorState = false;
                if (listener != null)
                {
                    listener.Stop();
                    listener = null;
                }
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("关闭服务器[{0} , {1}] 失败 ,原因: {2} ", Ipaddress, Port, ex.ToString()));
            }

        }
        /// <summary>
        /// 等待处理新的连接
        /// </summary>
        private void GetAcceptTcpClient()
        {
            try
            {
                semap.WaitOne();
                TcpClient tclient = listener.AcceptTcpClient();
                //维护客户端队列
                Socket socket = tclient.Client;
                NetworkStream stream = new NetworkStream(socket, true); //承载这个Socket
                Clientcs sks = new Clientcs(tclient.Client.RemoteEndPoint as IPEndPoint, tclient, stream);
                //加入客户端集合.
                AddClientList(sks);
                //客户端异步接收
                sks.NTStream.BeginRead(sks.RecBuffer, 0, sks.RecBuffer.Length, new AsyncCallback(EndReader), sks);
                semap.Release();
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("客户端连接服务器 失败 ,原因: {0} ", ex.ToString()));
            }
        }
        /// <summary>
        /// 异步接收发送的信息.
        /// </summary>
        /// <param name="ir"></param>
        private void EndReader(IAsyncResult ir)
        {
            Clientcs sk = ir.AsyncState as Clientcs;
            if (sk != null && listener != null)
            {
                try
                {
                    sk.Offset = sk.NTStream.EndRead(ir);
                    if (sk.Offset > 0)
                    {
                        string Mes = System.Text.Encoding.ASCII.GetString(sk.RecBuffer, 0, sk.Offset);
                        if (Message != null)
                            Message(Mes);
                        sk.NTStream.BeginRead(sk.RecBuffer, 0, sk.RecBuffer.Length, new AsyncCallback(EndReader), sk);
                    }
                    else
                    {
                        ClientList.Remove(sk);
                        MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format(" {0}  断开连接 ", sk.Ip.ToString()));
                    }
                }
                catch
                {
                    lock (this)
                    {
                        //移除异常类,断开连接
                        ClientList.Remove(sk);
                        MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format(" {0}  断开连接 ", sk.Ip.ToString()));
                    }
                }
            }
        }
        /// <summary>
        /// 加入队列.
        /// </summary>
        /// <param name="sk"></param>
        private void AddClientList(Clientcs sk)
        {
            Clientcs sockets = ClientList.Find(o => { return o.Ip == sk.Ip; });
            //如果不存在则添加,否则更新
            if (sockets == null)
            {
                ClientList.Add(sk);
            }
            else
            {
                ClientList.Remove(sockets);
                Thread.Sleep(100);
                ClientList.Add(sk);
            }
            //ChangClientList(ClientList.Count);
            MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format(" {0}  已连接服务器 ", sk.Ip.ToString()));
        }
        /// <summary>
        /// 向某客户端发送信息
        /// </summary>
        /// <param name="ip">客户端IP+端口地址</param>
        /// <param name="SendData">发送的数据包</param>
        public void SendToClient(IPEndPoint ip, string Mes)
        {
            try
            {
                Clientcs sk = ClientList.Find(o => { return o.Ip == ip; });
                if (sk != null && sk.Client.Connected)
                {
                    //获取当前流进行写入.
                    NetworkStream nStream = sk.NTStream;
                    if (nStream.CanWrite)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(Mes);
                        nStream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        //避免流被关闭,重新从对象中获取流
                        nStream = sk.Client.GetStream();
                        if (nStream.CanWrite)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(Mes);
                            nStream.Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            //如果还是无法写入,那么认为客户端中断连接.
                            ClientList.Remove(sk);
                        }
                    }
                    MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("服务器向客户端{0}发送 》》 {1}", ip, Mes));
                }
            }
            catch (Exception ex)
            {
                MV_Global_Variable.MyFormMain.MyUserControlLogList.AddCommLog(string.Format("服务器向客户端{0}发送 》》 {1} 失败！, 原因 : {2}", ip, Mes, ex.ToString()));
            }
        }
        /// <summary>
        /// 向全部客户端发送信息
        /// </summary>
        /// <param name="ip">客户端IP+端口地址</param>
        /// <param name="SendData">发送的数据包</param>
        public void SendAll(string Mes)
        {
            try
            {
                for (int i = 0; i < ClientList.Count; i++)
                {
                    SendToClient(ClientList[i].Ip, Mes);
                }
            }
            catch
            { }
        }

    }
    public class Clientcs
    {
        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public byte[] RecBuffer = new byte[1024];
        /// <summary>
        /// 发送缓冲区
        /// </summary>
        public byte[] SendBuffer = new byte[1024];
        /// <summary>
        /// 异步接收后包的大小
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 创建Sockets对象
        /// </summary>
        /// <param name="ip">Ip地址</param>
        /// <param name="client">TcpClient</param>
        /// <param name="ns">承载客户端Socket的网络流</param>
        public Clientcs(IPEndPoint ip, TcpClient client, NetworkStream ns)
        {
            Ip = ip;
            Client = client;
            NTStream = ns;
        }
        /// <summary>
        /// 当前IP地址,端口号
        /// </summary>
        public IPEndPoint Ip { get; set; }
        /// <summary>
        /// 客户端主通信程序
        /// </summary>
        public TcpClient Client { get; set; }
        /// <summary>
        /// 承载客户端Socket的网络流
        /// </summary>
        public NetworkStream NTStream { get; set; }

    }
}
