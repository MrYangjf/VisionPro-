using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasonteDataProcess.FileProcess;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Cognex.VisionPro;
using Cognex.VisionPro3D;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ToolBlock;
using SmartRay.VisionTools;
using Smartray;
using Smartray.Sample;
using SmartRay;
using VPPConfiger;
using ConfigerForm;
using Cognex.VisionPro.Caliper;

namespace MasonteVision
{
    public partial class MV_Form_Main : Form
    {
        #region 运行变量

        /// <summary>
        /// 结构：表示一个点
        /// </summary>
        public struct MyPoint
        {
            //横、纵坐标
            public double x, y;
            //构造函数
            public MyPoint(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            //该点到指定点pTarget的距离
            public double DistanceTo(MyPoint p)
            {
                return Math.Sqrt((p.x - x) * (p.x - x) + (p.y - y) * (p.y - y));
            }
            //重写ToString方法
            public override string ToString()
            {
                return string.Concat("Point (",
                 this.x.ToString("#0.000"), ',',
                 this.y.ToString("#0.000"), ')');
            }
        }
        //HalconModelAndFlow[] myhalconflow;
        public static bool PLCConnectioned = false;
        private bool boolProgramRunning = false;
        public int today = DateTime.Today.Day;
        public bool[] MyBoolRunCommand = new bool[4];
        public string[] Cam12StepStr;
        public string[] Cam34StepStr;
        public string[] Cam5StepStr;
        public string[] Cam3DStepStr;
        public bool EnableCavity2;
        public double Scan3DCount = 0;
        public string MyStringRunCommand;
        public string MyStringRunCommand1;
        public string MyStringRunCommand2;
        public string MyStringRunCommand3D;
        public string TodayCsvPath;
        public string todayImagePath;
        public int NPtsStep = 0;
        public double AxisX;
        public double AxisY;
        public MyPoint[] TestGapPoint = new MyPoint[8];
        public double[] Gap = new double[8];
        public bool LoadVppSucceed;
        public bool Load3DSucceed;
        public bool JustShowCam5;
        public bool Cam5Lock;
        public bool Cam5ActionOver;
        public bool SaveVppOver = true;
        public bool SaveThreadLock;
        public double[] DisplayLower = new double[8];
        public double[] DisplayUpper = new double[8];

        public double ConfigerDisplayLower, ConfigerDisplayUpper;

        bool BatteryGImgSaveOK, CavityGImgSaveOK;
        Bitmap SaveBitmapRunBattery1cam1, SaveBitmapRunBattery1cam2, SaveBitmapRunCavity1cam1, SaveBitmapRunCavity1cam2;
        Bitmap SaveBitmapRunBattery2cam1, SaveBitmapRunBattery2cam2, SaveBitmapRunCavity2cam1, SaveBitmapRunCavity2cam2;

        public bool IsUsingDebug;

        #region vp变量
        //窗口工具
        //public CogRecordDisplay cogRecordDisplayCam1;
        //public CogRecordDisplay cogRecordDisplayCam2;
        //public CogRecordDisplay cogRecordDisplayCam3;
        //public CogRecordDisplay cogRecordDisplayCam4;
        //public CogRecordDisplay cogRecordDisplayCam5;
        //public CogRecordDisplay cogRecordDisplayCam6;

        //取像工具
        public CogAcqFifoTool cogAcqCam1;
        public CogAcqFifoTool cogAcqCam2;
        public CogAcqFifoTool cogAcqCam3;
        public CogAcqFifoTool cogAcqCam4;
        public CogAcqFifoTool cogAcqCam5;

        //加载图像工具
        public CogImageFileTool cogImgCam1 = new CogImageFileTool();
        public CogImageFileTool cogImgCam2 = new CogImageFileTool();
        public CogImageFileTool cogImgCam3 = new CogImageFileTool();
        public CogImageFileTool cogImgCam4 = new CogImageFileTool();
        public CogImageFileTool cogImgCam5 = new CogImageFileTool();
        public CogImageFileTool cogImgCam6 = new CogImageFileTool();

        public CogImage8Grey cogCalibrationImgCam1;
        public CogImage8Grey cogCalibrationImgCam2;
        public CogImage8Grey cogCalibrationImgCam3;
        public CogImage8Grey cogCalibrationImgCam4;
        public CogImage8Grey cogCalibrationImgCam5;

        //运行TB工具
        public CogToolBlock cogRunToolBlockCam5Code;//读码工具
        public CogToolBlock cogRunToolBlockCam12;//1、2相机拍照工具
        public CogToolBlock cogRunToolBlockCam12BigBattery;
        public CogToolBlock cogRunToolBlockCam34;//3、4相机拍照工具
        public CogToolBlock cogRunToolBlockCam34Cavity2;//3、4相机拍照工具
        public CogToolBlock cogRunToolBlockCam1QRCode;//12相机正面读码&Mark
        public CogToolBlock cogRunToolBlockCam34Check;//检测矫正
        public CogToolBlock cogRunToolBlockCam34Cal;
        public CogToolBlock cogRunToolBlockCam34CheckFilm;//复检Gap
        public CogToolBlock cogRunToolBlockCam34CheckFPC;//复检FPC
        public CogToolBlock cogRunToolBlockCam34CheckGap;//复检Gap
        public CogToolBlock cogNPointCalibrationTool;

        //运行3D工具
        public CogToolBlock cogRunToolBlock3DCam1;//
        public CogToolBlock cogRunToolBlock3DCam2;//
        public CogToolBlock cogRunToolBlock3DCam3;//
        public CogToolBlock cogRunToolBlock3DCam4;//

        public CogToolBlock cogRunToolBlock3DCam12;//
        public CogToolBlock cogRunToolBlock3DCam22;//
        public CogToolBlock cogRunToolBlock3DCam32;//
        public CogToolBlock cogRunToolBlock3DCam42;//

        //标定
        public CogCalibCheckerboardTool cogCalibCheckerboardToolCam1;
        public CogCalibCheckerboardTool cogCalibCheckerboardToolCam2;
        public CogCalibCheckerboardTool cogCalibCheckerboardToolCam3;
        public CogCalibCheckerboardTool cogCalibCheckerboardToolCam4;

        public CogCalibNPointToNPointTool[] cogCalibNPointToNPointToolPosition = new CogCalibNPointToNPointTool[9];

        //定义图像变量
        public CogImage8Grey ImageCam1 = new CogImage8Grey();
        public CogImage8Grey ImageCam2 = new CogImage8Grey();
        public CogImage8Grey ImageCam3 = new CogImage8Grey();
        public CogImage8Grey ImageCam4 = new CogImage8Grey();
        public CogImage8Grey ImageCam5 = new CogImage8Grey();
        public ICogImage CognexRangeImage;


        //定义cogLabel变量
        public CogGraphicLabel cogLabelCam1 = new CogGraphicLabel();
        public CogGraphicLabel cogLabelCam2 = new CogGraphicLabel();
        public CogGraphicLabel cogLabelCam3 = new CogGraphicLabel();
        public CogGraphicLabel cogLabelCam4 = new CogGraphicLabel();
        public CogGraphicLabel cogLabelCam5 = new CogGraphicLabel();
        public CogGraphicLabel cogLabelCam6 = new CogGraphicLabel();

        private Parameter MainParameter = new Parameter();
        public Parameter MainParameter1 = new Parameter();
        public Parameter MainParameter2 = new Parameter();
        public Parameter MainParameter3 = new Parameter();
        public Parameter MainParameter4 = new Parameter();
        public Parameter MainParameter12 = new Parameter();
        public Parameter MainParameter22 = new Parameter();
        public Parameter MainParameter32 = new Parameter();
        public Parameter MainParameter42 = new Parameter();

        #endregion

        #region Cal变量

        public int LightValue1, LightValue2, LightValue3, LightValue4, LightValue5;
        public int LightChannel1, LightChannel2, LightChannel3, LightChannel4, LightChannel5;

        public bool DatumModel;

        public double BatteryTLX, BatteryTLY, BatteryTRX, BatteryTRY, BatteryBRX, BatteryBRY, BatteryBLX, BatteryBLY;
        public double BatteryHandMarkX1, BatteryHandMarkY1, BatteryHandMarkX2, BatteryHandMarkY2;
        public double CavityTLX, CavityTLY, CavityTRX, CavityTRY, CavityBRX, CavityBRY, CavityBLX, CavityBLY;
        public double Cavity2TLX, Cavity2TLY, Cavity2TRX, Cavity2TRY, Cavity2BRX, Cavity2BRY, Cavity2BLX, Cavity2BLY;

        public double AxisMoveX, AxisMoveY, AxisMoveA;

        public string BatteryQRCode, CavityQRCode;

        public double BatteryWidth, BatteryHeight;
        public double CavityWidth, CavityHeight;
        public string CavityBarcode;

        public double BatteryWidth2, BatteryHeight2;
        public double CavityWidth2, CavityHeight2;

        public bool RunSucceed2;
        public bool RunSucceed;
        public bool ScanSucceed;
        public bool RunReadCodeSucceed;

        DateTime CodeStartTime, CodeEndTime;
        TimeSpan ReadTimeSpan;

        #endregion

        #region SamrtRay变量
        public DateTime m3DstartTime, m3DendTime;
        public bool StartTick;
        bool isRun = false;
        public bool StartRun;
        SensorManager sensorManager;
        Sensor sensor0;
        string Camera3DDirc;
        ConvertSmartRayImage convertSmartRayImage = new ConvertSmartRayImage();
        public string ImageHeight = "2000";

        AutoResetEvent mAutoResetEvent = new AutoResetEvent(true);
        #endregion

        #region 路径

        public string AcqPath = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\AcqPath\\";
        public string CalibPath = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\Calib\\";
        public string RunToolBlcokPath = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\RunToolBlock\\2D\\";
        public string RunToolBlcokPath2 = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\RunToolBlock\\3D\\";
        #endregion

        #endregion

        #region 相机窗口和相机变量

        //static string[] SpinnakerCameraSN = new string[1] { "17289066" };
        //SpinnakerCamera mySpinnakerCamera;

        /// <summary>
        /// 显示窗口，如使用Halcon等  可自选
        /// </summary>
        //public PictureBox[] ShowBox;
        //public HalconCameraConn[] MyCamera;
        //public Halcon3DCamera My3DCamera;

        HikCameraConn myHikCameraConn;

        #endregion

        #region 子线程对象

        /// <summary>
        /// 载入线程
        /// </summary>
        private Thread _LoadingThread = null;
        /// <summary>
        /// VPP载入线程
        /// </summary>
        private Thread _LoadingVppThread = null;
        /// <summary>
        /// 3d载入线程
        /// </summary>
        private Thread _Loading3DThread = null;

        public Thread InitCameraThread1 = null;
        public Thread InitCameraThread2 = null;
        public Thread InitCameraThread3 = null;
        public Thread InitCameraThread4 = null;
        public Thread InitCameraThread3d = null;

        public Thread SaveThread = null;


        public MV_SubThread_Flow MySubThreadFlow = null;

        #endregion

        #region 窗口对象&控件对象

        public MV_Form_Login MyFormLogin = null;

        public LightSet CstLightSet = null;

        public MV_UC_DataTable MyUserControlDataTable = null;

        public MV_UC_LOGList MyUserControlLogList = null;

        public MV_Form_Loading MyFormLoading = null;

        public MV_UC_ParametersSetting MyUserControlParamSetting = null;


        //public MV_UC_Ethernet MyUserControlEthernetConnection = null;

        #endregion

        #region  文件类

        /// <summary>
        /// LOG文件类
        /// </summary>
        public LOGFile MyOperateLogFile;

        /// <summary>
        /// INI文件类
        /// </summary>
        public INIFile MySystemINIFile;

        /// <summary>
        /// 用于生成EXCEL文件
        /// </summary>
        //public ExcelFile MyExcelFile;

        public CSVFile MyCsvFile;


        #endregion

        #region 移动窗口的变量

        private bool _boolMoveAllown = false;

        private int _windowsMoveDetaX;

        private int _windowsMoveDetaY;

        private int _windowsMoveTempX;

        private int _windowsMoveTempY;

        #endregion

        #region 窗口大小变量

        private int _windowsSizeX;

        private int _windowsSizeY;

        #endregion

        #region 功能函数

        void SetRecordDispaly()
        {
            cogRecordDisplayCam6.ColorMapLowerRoiLimit = DisplayLower[0];
            cogRecordDisplayCam6.ColorMapUpperRoiLimit = DisplayUpper[0];
            cogRecordDisplayCam7.ColorMapLowerRoiLimit = DisplayLower[1];
            cogRecordDisplayCam7.ColorMapUpperRoiLimit = DisplayUpper[1];
            cogRecordDisplayCam8.ColorMapLowerRoiLimit = DisplayLower[2];
            cogRecordDisplayCam8.ColorMapUpperRoiLimit = DisplayUpper[2];
            cogRecordDisplayCam9.ColorMapLowerRoiLimit = DisplayLower[3];
            cogRecordDisplayCam9.ColorMapUpperRoiLimit = DisplayUpper[3];
            cogRecordDisplayCam10.ColorMapLowerRoiLimit = DisplayLower[4];
            cogRecordDisplayCam10.ColorMapUpperRoiLimit = DisplayUpper[4];
            cogRecordDisplayCam11.ColorMapLowerRoiLimit = DisplayLower[5];
            cogRecordDisplayCam11.ColorMapUpperRoiLimit = DisplayUpper[5];
            cogRecordDisplayCam12.ColorMapLowerRoiLimit = DisplayLower[6];
            cogRecordDisplayCam12.ColorMapUpperRoiLimit = DisplayUpper[6];
            cogRecordDisplayCam13.ColorMapLowerRoiLimit = DisplayLower[7];
            cogRecordDisplayCam13.ColorMapUpperRoiLimit = DisplayUpper[7];

        }

        void SaveRecordDispaly()
        {
            DisplayLower[0] = cogRecordDisplayCam6.ColorMapLowerRoiLimit;
            DisplayUpper[0] = cogRecordDisplayCam6.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower0", DisplayLower[0].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper0", DisplayUpper[0].ToString("0.0000"));

            DisplayLower[1] = cogRecordDisplayCam7.ColorMapLowerRoiLimit;
            DisplayUpper[1] = cogRecordDisplayCam7.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower1", DisplayLower[1].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper1", DisplayUpper[1].ToString("0.0000"));

            DisplayLower[2] = cogRecordDisplayCam8.ColorMapLowerRoiLimit;
            DisplayUpper[2] = cogRecordDisplayCam8.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower2", DisplayLower[2].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper2", DisplayUpper[2].ToString("0.0000"));

            DisplayLower[3] = cogRecordDisplayCam9.ColorMapLowerRoiLimit;
            DisplayUpper[3] = cogRecordDisplayCam9.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower3", DisplayLower[3].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper3", DisplayUpper[3].ToString("0.0000"));

            DisplayLower[4] = cogRecordDisplayCam10.ColorMapLowerRoiLimit;
            DisplayUpper[4] = cogRecordDisplayCam10.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower4", DisplayLower[4].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper4", DisplayUpper[4].ToString("0.0000"));

            DisplayLower[5] = cogRecordDisplayCam11.ColorMapLowerRoiLimit;
            DisplayUpper[5] = cogRecordDisplayCam11.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower5", DisplayLower[5].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper5", DisplayUpper[5].ToString("0.0000"));

            DisplayLower[6] = cogRecordDisplayCam12.ColorMapLowerRoiLimit;
            DisplayUpper[6] = cogRecordDisplayCam12.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower6", DisplayLower[6].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper6", DisplayUpper[6].ToString("0.0000"));

            DisplayLower[7] = cogRecordDisplayCam13.ColorMapLowerRoiLimit;
            DisplayUpper[7] = cogRecordDisplayCam13.ColorMapUpperRoiLimit;
            MySystemINIFile.WriteValue("系统变量", "DisplayLower7", DisplayLower[7].ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "DisplayUpper7", DisplayUpper[7].ToString("0.0000"));


            MySystemINIFile.WriteValue("系统变量", "ConfigerDisplayLower", ConfigerDisplayLower.ToString("0.0000"));
            MySystemINIFile.WriteValue("系统变量", "ConfigerDisplayUpper", ConfigerDisplayUpper.ToString("0.0000"));
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void ExitApp()
        {
            SaveRecordDispaly();

            //记录关闭日志
            OperateLog("程序关闭！");
            //关闭自建线程

            myHikCameraConn.DisposeDevices();
            //关闭窗口和消息
            this.Close();
            Application.Exit();
            //强制关闭所有环境
            Environment.Exit(1);

        }

        /// <summary>
        /// 事件
        /// </summary>
        void CreatDelegateEvent()
        {
        }

        /// <summary>
        /// 创建载入线程
        /// </summary>
        void CreatLoadThread()
        {
            _LoadingThread = new Thread(LoadingFunc);
            _LoadingThread.IsBackground = true;
            _LoadingThread.Start();
        }

        void CreatLoadVppThread()
        {
            _LoadingVppThread = new Thread(LoadVisionProVpp);
            _LoadingVppThread.IsBackground = true;
            _LoadingVppThread.Start();
        }

        void CreatLoad3DThread()
        {
            _Loading3DThread = new Thread(InitSmartRayCamera);
            _Loading3DThread.IsBackground = true;
            _Loading3DThread.Start();
        }

        /// <summary>
        /// 创建子流程
        /// </summary>
        void CreatSubFlow()
        {
            MySubThreadFlow = new MV_SubThread_Flow();
        }

        /// <summary>
        /// 全局变量的初始化赋值
        /// </summary>
        private void InitGlobalVariable()
        {
            MV_Global_Variable.GlobalCurrentUser = "未登录";
            MV_Global_Variable.GlobalCurrentLevel = "null";

            MV_Global_Variable.FinishIniVariable = true;
        }

        /// <summary>
        /// 写入系统变量默认值
        /// </summary>
        private void WriteDefultSyetemVar()
        {
            MySystemINIFile.WriteValue("系统变量", "载入超时时间", "20000");
            MySystemINIFile.WriteValue("系统变量", "相机数量", "1");
            MySystemINIFile.WriteValue("系统变量", "相机窗口列数", "1");
            MySystemINIFile.WriteValue("系统变量", "相机窗口行数", "1");
            MySystemINIFile.WriteValue("系统变量", "OK图像路径", "D:\\Image\\");
            MySystemINIFile.WriteValue("系统变量", "NG图像路径", "D:\\Image\\NG\\");
            MySystemINIFile.WriteValue("系统变量", "图像存储天数", "30");
            MySystemINIFile.WriteValue("系统变量", "ResultCSVPath", "D:\\ResultRecord\\");

            MySystemINIFile.WriteValue("系统变量", "Socket类型", "Server");
            MySystemINIFile.WriteValue("系统变量", "作服务器IP", "127.0.0.1");
            MySystemINIFile.WriteValue("系统变量", "作服务器Port", "3000");
            MySystemINIFile.WriteValue("系统变量", "连接远端服务器IP", "192.168.0.100");
            MySystemINIFile.WriteValue("系统变量", "连接远端服务器Port", "3000");
            MySystemINIFile.WriteValue("系统变量", "灰点相机启用", "false");

            //MySystemINIFile.WriteValue("系统变量", "相机1序列号", "");
            //MySystemINIFile.WriteValue("系统变量", "相机2序列号", "");
            //MySystemINIFile.WriteValue("系统变量", "相机3序列号", "");
            //MySystemINIFile.WriteValue("系统变量", "相机4序列号", "");
            //MySystemINIFile.WriteValue("系统变量", "相机5序列号", "");
            //MySystemINIFile.WriteValue("系统变量", "相机6序列号", "");

            MySystemINIFile.WriteValue("系统变量", "相机1曝光时间", "5");
            MySystemINIFile.WriteValue("系统变量", "相机2曝光时间", "5");
            MySystemINIFile.WriteValue("系统变量", "相机3曝光时间", "15");
            MySystemINIFile.WriteValue("系统变量", "相机4曝光时间", "15");
            MySystemINIFile.WriteValue("系统变量", "相机5曝光时间", "5");
            //MySystemINIFile.WriteValue("系统变量", "相机6曝光时间", "5");

            //MySystemINIFile.WriteValue("系统变量", "相机1触发模式", "");
            //MySystemINIFile.WriteValue("系统变量", "相机2触发模式", "");
            //MySystemINIFile.WriteValue("系统变量", "相机3触发模式", "");
            //MySystemINIFile.WriteValue("系统变量", "相机4触发模式", "");
            //MySystemINIFile.WriteValue("系统变量", "相机5触发模式", "");
            //MySystemINIFile.WriteValue("系统变量", "相机6触发模式", "");

            MySystemINIFile.WriteValue("系统变量", "LightValue1", "100");
            MySystemINIFile.WriteValue("系统变量", "LightValue2", "100");
            MySystemINIFile.WriteValue("系统变量", "LightValue3", "100");
            MySystemINIFile.WriteValue("系统变量", "LightValue4", "100");
            MySystemINIFile.WriteValue("系统变量", "LightValue5", "100");

            MySystemINIFile.WriteValue("系统变量", "LightChannel1", "1");
            MySystemINIFile.WriteValue("系统变量", "LightChannel2", "2");
            MySystemINIFile.WriteValue("系统变量", "LightChannel3", "3");
            MySystemINIFile.WriteValue("系统变量", "LightChannel4", "4");
            MySystemINIFile.WriteValue("系统变量", "LightChannel5", "5");


            MySystemINIFile.WriteValue("系统变量", "AddOffsetX1", 0);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetY1", 0);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetA1", 0);

            MySystemINIFile.WriteValue("系统变量", "AddOffsetX2", 0);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetY2", 0);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetA2", 0);

            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetX1", 0);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetY1", 0);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetA1", 0);

            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetX2", 0);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetY2", 0);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetA2", 0);

            MySystemINIFile.WriteValue("系统变量", "ImageHeight", 2000);
            MySystemINIFile.WriteValue("系统变量", "IsUsingBigBatteryVpp", "false");

            MySystemINIFile.WriteValue("系统变量", "HeightValueSetting", "2000");

            MySystemINIFile.WriteValue("系统变量", "Battery1BarcodeCam", 0);
            MySystemINIFile.WriteValue("系统变量", "Battery2BarcodeCam", 0);
        }

        public void ReadProjectList()
        {
            MV_Global_Variable.myProjectList = MySystemINIFile.ReadSections();
            MV_Global_Variable.myProjectList.Remove("系统变量");
        }

        public void AddProject(string projectName)
        {
            MV_Global_Variable.ProjectName = projectName;

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1曝光时间", MV_Global_Variable.CameraBackExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2曝光时间", MV_Global_Variable.CameraBackExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3曝光时间", MV_Global_Variable.CameraBackExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4曝光时间", MV_Global_Variable.CameraBackExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1Cavity曝光时间", MV_Global_Variable.CameraCavityExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2Cavity曝光时间", MV_Global_Variable.CameraCavityExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3Cavity曝光时间", MV_Global_Variable.CameraCavityExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4Cavity曝光时间", MV_Global_Variable.CameraCavityExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机曝光时间", MV_Global_Variable.SmartRayExpTime);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光阈值", MV_Global_Variable.SmartRayLaserThreshold);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光强度", MV_Global_Variable.SmartRayLaserBrightness);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "LightValue6", MV_Global_Variable.LightValue6);
        }

        public void LoadProjectValue(string ProjectName)
        {
            MV_Global_Variable.ProjectName = ProjectName;

            MV_Global_Variable.CameraBackExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1曝光时间");
            MV_Global_Variable.CameraBackExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2曝光时间");
            MV_Global_Variable.CameraBackExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3曝光时间");
            MV_Global_Variable.CameraBackExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4曝光时间");

            MV_Global_Variable.CameraHandMarkExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1HandMark曝光时间");
            MV_Global_Variable.CameraHandMarkExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2HandMark曝光时间");
            MV_Global_Variable.CameraHandMarkExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3HandMark曝光时间");
            MV_Global_Variable.CameraHandMarkExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4HandMark曝光时间");

            MV_Global_Variable.CameraCavityExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1Cavity曝光时间");
            MV_Global_Variable.CameraCavityExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2Cavity曝光时间");
            MV_Global_Variable.CameraCavityExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3Cavity曝光时间");
            MV_Global_Variable.CameraCavityExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4Cavity曝光时间");

            MV_Global_Variable.CameraCalOffsetExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1CalOffset曝光时间");
            MV_Global_Variable.CameraCalOffsetExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2CalOffset曝光时间");
            MV_Global_Variable.CameraCalOffsetExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3CalOffset曝光时间");
            MV_Global_Variable.CameraCalOffsetExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4CalOffset曝光时间");


            MV_Global_Variable.LightValue6 = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "LightValue6");
            MV_Global_Variable.SmartRayExpTime = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "3D相机曝光时间");
            MV_Global_Variable.SmartRayLaserThreshold = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "3D相机激光阈值");
            MV_Global_Variable.SmartRayLaserBrightness = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "3D相机激光强度");
            CstLightSet.mController.SetDigitalValue(6, MV_Global_Variable.LightValue6);
        }

        /// <summary>
        /// 系统变量初始化（系统关键变量，读于ini文件,优先于所有步骤）
        /// </summary>
        private void InitSytemVariable()
        {
            //判断是否存在该INI文件
            if (!MySystemINIFile.IsExisting())
            {
                //不存在立即创建并写入默认变量
                MySystemINIFile.CreateIni();
                WriteDefultSyetemVar();
            }


            ImageHeight = MySystemINIFile.GetStringValue("系统变量", "ImageHeight");

            LightValue1 = MySystemINIFile.GetIntValue("系统变量", "LightValue1");
            LightValue2 = MySystemINIFile.GetIntValue("系统变量", "LightValue2");
            LightValue3 = MySystemINIFile.GetIntValue("系统变量", "LightValue3");
            LightValue4 = MySystemINIFile.GetIntValue("系统变量", "LightValue4");
            LightValue5 = MySystemINIFile.GetIntValue("系统变量", "LightValue5");

            LightChannel1 = MySystemINIFile.GetIntValue("系统变量", "LightChannel1");
            LightChannel2 = MySystemINIFile.GetIntValue("系统变量", "LightChannel2");
            LightChannel3 = MySystemINIFile.GetIntValue("系统变量", "LightChannel3");
            LightChannel4 = MySystemINIFile.GetIntValue("系统变量", "LightChannel4");
            LightChannel5 = MySystemINIFile.GetIntValue("系统变量", "LightChannel5");

            DisplayLower[0] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower0");
            DisplayLower[1] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower1");
            DisplayLower[2] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower2");
            DisplayLower[3] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower3");
            DisplayLower[4] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower4");
            DisplayLower[5] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower5");
            DisplayLower[6] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower6");
            DisplayLower[7] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayLower7");

            DisplayUpper[0] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper0");
            DisplayUpper[1] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper1");
            DisplayUpper[2] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper2");
            DisplayUpper[3] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper3");
            DisplayUpper[4] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper4");
            DisplayUpper[5] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper5");
            DisplayUpper[6] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper6");
            DisplayUpper[7] = MySystemINIFile.GetDoubleValue("系统变量", "DisplayUpper7");

            ConfigerDisplayLower = MySystemINIFile.GetDoubleValue("系统变量", "ConfigerDisplayLower");
            ConfigerDisplayUpper = MySystemINIFile.GetDoubleValue("系统变量", "ConfigerDisplayUpper");

            //将数值读取至系统中
            MV_Global_Variable.GlobalLoadingTimeout = MySystemINIFile.GetIntValue("系统变量", "载入超时时间");
            MV_Global_Variable.GlobalCameraCount = MySystemINIFile.GetIntValue("系统变量", "相机数量");
            MV_Global_Variable.GlobalCameraColums = MySystemINIFile.GetIntValue("系统变量", "相机窗口列数");
            MV_Global_Variable.GlobalCameraRows = MySystemINIFile.GetIntValue("系统变量", "相机窗口行数");
            MV_Global_Variable.VisionMasterSolutionPath = MySystemINIFile.GetStringValue("系统变量", "sol文件路径");
            MV_Global_Variable.VisionMasterEXEPath = MySystemINIFile.GetStringValue("系统变量", "VMexe路径");
            MV_Global_Variable.VisionMasterSolutionName = MySystemINIFile.GetStringValue("系统变量", "sol文件名");
            MV_Global_Variable.ImageStorePath = MySystemINIFile.GetStringValue("系统变量", "OK图像路径");
            MV_Global_Variable.ImageStoreDays = MySystemINIFile.GetStringValue("系统变量", "图像存储天数");
            MV_Global_Variable.NGImageStorePath = MySystemINIFile.GetStringValue("系统变量", "NG图像路径");
            MV_Global_Variable.SocketType = MySystemINIFile.GetStringValue("系统变量", "Socket类型");
            MV_Global_Variable.ServerSocketIP = MySystemINIFile.GetStringValue("系统变量", "作服务器IP");
            MV_Global_Variable.ServerSocketPort = MySystemINIFile.GetStringValue("系统变量", "作服务器Port");
            MV_Global_Variable.RemoteServerSocketIP = MySystemINIFile.GetStringValue("系统变量", "连接远端服务器IP");
            MV_Global_Variable.RemoteServerSocketPort = MySystemINIFile.GetStringValue("系统变量", "连接远端服务器Port");
            MV_Global_Variable.ResultCSVPath = MySystemINIFile.GetStringValue("系统变量", "ResultCSVPath");

            MV_Global_Variable.ProjectName = MySystemINIFile.GetStringValue("系统变量", "ProjectName");

            MV_Global_Variable.CameraSerialNumber1 = MySystemINIFile.GetStringValue("系统变量", "相机1序列号");
            MV_Global_Variable.CameraSerialNumber2 = MySystemINIFile.GetStringValue("系统变量", "相机2序列号");
            MV_Global_Variable.CameraSerialNumber3 = MySystemINIFile.GetStringValue("系统变量", "相机3序列号");
            MV_Global_Variable.CameraSerialNumber4 = MySystemINIFile.GetStringValue("系统变量", "相机4序列号");
            MV_Global_Variable.CameraSerialNumber5 = MySystemINIFile.GetStringValue("系统变量", "相机5序列号");
            MV_Global_Variable.CameraSerialNumber6 = MySystemINIFile.GetStringValue("系统变量", "相机6序列号");

            MV_Global_Variable.CameraBackExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1曝光时间");
            MV_Global_Variable.CameraBackExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2曝光时间");
            MV_Global_Variable.CameraBackExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3曝光时间");
            MV_Global_Variable.CameraBackExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4曝光时间");

            MV_Global_Variable.CameraHandMarkExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1HandMark曝光时间");
            MV_Global_Variable.CameraHandMarkExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2HandMark曝光时间");
            MV_Global_Variable.CameraHandMarkExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3HandMark曝光时间");
            MV_Global_Variable.CameraHandMarkExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4HandMark曝光时间");

            MV_Global_Variable.CameraCavityExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1Cavity曝光时间");
            MV_Global_Variable.CameraCavityExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2Cavity曝光时间");
            MV_Global_Variable.CameraCavityExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3Cavity曝光时间");
            MV_Global_Variable.CameraCavityExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4Cavity曝光时间");

            MV_Global_Variable.CameraCalOffsetExposure1 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机1CalOffset曝光时间");
            MV_Global_Variable.CameraCalOffsetExposure2 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机2CalOffset曝光时间");
            MV_Global_Variable.CameraCalOffsetExposure3 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机3CalOffset曝光时间");
            MV_Global_Variable.CameraCalOffsetExposure4 = MySystemINIFile.GetStringValue(MV_Global_Variable.ProjectName, "相机4CalOffset曝光时间");

            MV_Global_Variable.SmartRayExpTime = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "3D相机曝光时间");
            MV_Global_Variable.SmartRayLaserThreshold = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "3D相机激光阈值");
            MV_Global_Variable.SmartRayLaserBrightness = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "3D相机激光强度");
            MV_Global_Variable.LightValue6 = MySystemINIFile.GetIntValue(MV_Global_Variable.ProjectName, "LightValue6");

            MV_Global_Variable.TriggerModel1 = MySystemINIFile.GetStringValue("系统变量", "相机1触发模式");
            MV_Global_Variable.TriggerModel2 = MySystemINIFile.GetStringValue("系统变量", "相机2触发模式");
            MV_Global_Variable.TriggerModel3 = MySystemINIFile.GetStringValue("系统变量", "相机3触发模式");
            MV_Global_Variable.TriggerModel4 = MySystemINIFile.GetStringValue("系统变量", "相机4触发模式");
            MV_Global_Variable.TriggerModel5 = MySystemINIFile.GetStringValue("系统变量", "相机5触发模式");
            MV_Global_Variable.TriggerModel6 = MySystemINIFile.GetStringValue("系统变量", "相机6触发模式");

            MV_Global_Variable.Battery1BarcodeCam = MySystemINIFile.GetIntValue("系统变量", "Battery1BarcodeCam");
            MV_Global_Variable.Battery2BarcodeCam = MySystemINIFile.GetIntValue("系统变量", "Battery2BarcodeCam");

            MV_Global_Variable.AddOffsetX1 = MySystemINIFile.GetStringValue("系统变量", "AddOffsetX1");
            MV_Global_Variable.AddOffsetY1 = MySystemINIFile.GetStringValue("系统变量", "AddOffsetY1");
            MV_Global_Variable.AddOffsetA1 = MySystemINIFile.GetStringValue("系统变量", "AddOffsetA1");

            MV_Global_Variable.AddOffsetX2 = MySystemINIFile.GetStringValue("系统变量", "AddOffsetX2");
            MV_Global_Variable.AddOffsetY2 = MySystemINIFile.GetStringValue("系统变量", "AddOffsetY2");
            MV_Global_Variable.AddOffsetA2 = MySystemINIFile.GetStringValue("系统变量", "AddOffsetA2");

            MV_Global_Variable.BatteryMarkOffsetA1 = MySystemINIFile.GetStringValue("系统变量", "BatteryMarkOffsetA1");
            MV_Global_Variable.BatteryMarkOffsetX1 = MySystemINIFile.GetStringValue("系统变量", "BatteryMarkOffsetX1");
            MV_Global_Variable.BatteryMarkOffsetY1 = MySystemINIFile.GetStringValue("系统变量", "BatteryMarkOffsetY1");

            MV_Global_Variable.BatteryMarkOffsetA2 = MySystemINIFile.GetStringValue("系统变量", "BatteryMarkOffsetA2");
            MV_Global_Variable.BatteryMarkOffsetX2 = MySystemINIFile.GetStringValue("系统变量", "BatteryMarkOffsetX2");
            MV_Global_Variable.BatteryMarkOffsetY2 = MySystemINIFile.GetStringValue("系统变量", "BatteryMarkOffsetY2");

            MV_Global_Variable.Save3DImage = MySystemINIFile.GetBoolValue("系统变量", "Save3DImage");
            MV_Global_Variable.Save3DOKImage = MySystemINIFile.GetBoolValue("系统变量", "Save3DOKImage");
            MV_Global_Variable.Save3DNGImage = MySystemINIFile.GetBoolValue("系统变量", "Save3DNGImage");
            MV_Global_Variable.SaveOriImage = MySystemINIFile.GetBoolValue("系统变量", "SaveOriImage");
            MV_Global_Variable.IsUsingBigBatteryVpp = MySystemINIFile.GetBoolValue("系统变量", "IsUsingBigBatteryVpp");

            MV_Global_Variable.HeightValueSetting = MySystemINIFile.GetStringValue("系统变量", "HeightValueSetting");

            ReadProjectList();

            MV_Global_Variable.CameraSerialNumbers = new string[6] {
                MV_Global_Variable.CameraSerialNumber1, MV_Global_Variable.CameraSerialNumber2, MV_Global_Variable.CameraSerialNumber3,
                MV_Global_Variable.CameraSerialNumber4, MV_Global_Variable.CameraSerialNumber5, MV_Global_Variable.CameraSerialNumber6
            };
            MV_Global_Variable.CameraExposureArray = new string[6] {
                MV_Global_Variable.CameraExposure1, MV_Global_Variable.CameraExposure2, MV_Global_Variable.CameraExposure3,
                MV_Global_Variable.CameraExposure4, MV_Global_Variable.CameraExposure5, MV_Global_Variable.CameraExposure6
            };
            MV_Global_Variable.TriggerModelArray = new string[6] {
                MV_Global_Variable.TriggerModel1, MV_Global_Variable.TriggerModel2, MV_Global_Variable.TriggerModel3,
                MV_Global_Variable.TriggerModel4, MV_Global_Variable.TriggerModel5, MV_Global_Variable.TriggerModel6
            };
            CheckSystemPath();
        }

        /// <summary>
        /// 检测系统文件夹是否存在
        /// </summary>
        public void CheckSystemPath()
        {

            todayImagePath = MV_Global_Variable.ImageStorePath + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!Directory.Exists(todayImagePath))
            {
                //不存在立即创建
                for (int i = 1; i < 7; i++)
                {
                    Directory.CreateDirectory(todayImagePath + "cam" + i.ToString());
                    if (i == 6)
                    {
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D1\\OK\\");
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D2\\OK\\");
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D3\\OK\\");
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D4\\OK\\");

                        Directory.CreateDirectory(todayImagePath + "cam6\\3D1\\NG\\");
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D2\\NG\\");
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D3\\NG\\");
                        Directory.CreateDirectory(todayImagePath + "cam6\\3D4\\NG\\");
                    }
                }
                for (int i = 1; i < 7; i++)
                {
                    Directory.CreateDirectory(todayImagePath + "AnalysisCam" + i.ToString());
                }

            }
            string[] Folders = Directory.GetDirectories(MV_Global_Variable.ImageStorePath);
            for (int i = 0; i < Folders.Length; i++)
            {
                DirectoryInfo Folderinfo = new DirectoryInfo(Folders[i]);
                DateTime CreatTime = Folderinfo.CreationTime;
                if (DateTime.Compare(CreatTime, DateTime.Today.AddDays(-int.Parse(MV_Global_Variable.ImageStoreDays))) < 0)
                {
                    Folderinfo.Delete(true);
                }
            }


            AcqPath = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\AcqPath\\";
            CalibPath = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\Calib\\";
            RunToolBlcokPath = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\RunToolBlock\\2D\\";
            RunToolBlcokPath2 = "E:\\" + MV_Global_Variable.VisionProSolutionPath + "\\RunToolBlock\\3D\\";


            if (!Directory.Exists(AcqPath))
            {
                try
                {
                    Directory.CreateDirectory(AcqPath);
                }
                catch
                { }
            }
            if (!Directory.Exists(CalibPath))
            {
                try
                {
                    Directory.CreateDirectory(CalibPath);
                }
                catch
                { }
            }
            if (!Directory.Exists(RunToolBlcokPath))
            {
                try
                {
                    Directory.CreateDirectory(RunToolBlcokPath);
                }
                catch
                { }
            }
            if (!Directory.Exists(RunToolBlcokPath2))
            {
                try
                {
                    Directory.CreateDirectory(RunToolBlcokPath2);
                }
                catch
                { }
            }


        }

        /// <summary>
        /// 保存系统变量
        /// </summary>
        public void SaveVariable()
        {
            MySystemINIFile.WriteValue("系统变量", "载入超时时间", MV_Global_Variable.GlobalLoadingTimeout);
            MySystemINIFile.WriteValue("系统变量", "相机数量", MV_Global_Variable.GlobalCameraCount);
            MySystemINIFile.WriteValue("系统变量", "相机窗口列数", MV_Global_Variable.GlobalCameraColums);
            MySystemINIFile.WriteValue("系统变量", "相机窗口行数", MV_Global_Variable.GlobalCameraRows);
            MySystemINIFile.WriteValue("系统变量", "sol文件路径", MV_Global_Variable.VisionMasterSolutionPath);
            MySystemINIFile.WriteValue("系统变量", "sol文件名", MV_Global_Variable.VisionMasterSolutionName);
            MySystemINIFile.WriteValue("系统变量", "VMexe路径", MV_Global_Variable.VisionMasterEXEPath);
            MySystemINIFile.WriteValue("系统变量", "OK图像路径", MV_Global_Variable.ImageStorePath);
            MySystemINIFile.WriteValue("系统变量", "NG图像路径", MV_Global_Variable.NGImageStorePath);
            MySystemINIFile.WriteValue("系统变量", "ResultCSVPath", MV_Global_Variable.ResultCSVPath);
            MySystemINIFile.WriteValue("系统变量", "图像存储天数", MV_Global_Variable.ImageStoreDays);
            MySystemINIFile.WriteValue("系统变量", "ImageHeight", ImageHeight);

            MySystemINIFile.WriteValue("系统变量", "ProjectName", MV_Global_Variable.ProjectName);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1曝光时间", MV_Global_Variable.CameraBackExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2曝光时间", MV_Global_Variable.CameraBackExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3曝光时间", MV_Global_Variable.CameraBackExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4曝光时间", MV_Global_Variable.CameraBackExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4HandMark曝光时间", MV_Global_Variable.CameraHandMarkExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1Cavity曝光时间", MV_Global_Variable.CameraCavityExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2Cavity曝光时间", MV_Global_Variable.CameraCavityExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3Cavity曝光时间", MV_Global_Variable.CameraCavityExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4Cavity曝光时间", MV_Global_Variable.CameraCavityExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机1CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure1);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机2CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure2);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机3CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure3);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "相机4CalOffset曝光时间", MV_Global_Variable.CameraCalOffsetExposure4);

            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "LightValue6", MV_Global_Variable.LightValue6);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机曝光时间", MV_Global_Variable.SmartRayExpTime);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光阈值", MV_Global_Variable.SmartRayLaserThreshold);
            MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光强度", MV_Global_Variable.SmartRayLaserBrightness);

            MySystemINIFile.WriteValue("系统变量", "LightChannel1", LightChannel1);
            MySystemINIFile.WriteValue("系统变量", "LightChannel2", LightChannel2);
            MySystemINIFile.WriteValue("系统变量", "LightChannel3", LightChannel3);
            MySystemINIFile.WriteValue("系统变量", "LightChannel4", LightChannel4);
            MySystemINIFile.WriteValue("系统变量", "LightChannel5", LightChannel5);


            MySystemINIFile.WriteValue("系统变量", "LightValue1", LightValue1);
            MySystemINIFile.WriteValue("系统变量", "LightValue2", LightValue2);
            MySystemINIFile.WriteValue("系统变量", "LightValue3", LightValue3);
            MySystemINIFile.WriteValue("系统变量", "LightValue4", LightValue4);
            MySystemINIFile.WriteValue("系统变量", "LightValue5", LightValue5);

            MySystemINIFile.WriteValue("系统变量", "AddOffsetX1", MV_Global_Variable.AddOffsetX1);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetY1", MV_Global_Variable.AddOffsetY1);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetA1", MV_Global_Variable.AddOffsetA1);

            MySystemINIFile.WriteValue("系统变量", "AddOffsetX2", MV_Global_Variable.AddOffsetX2);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetY2", MV_Global_Variable.AddOffsetY2);
            MySystemINIFile.WriteValue("系统变量", "AddOffsetA2", MV_Global_Variable.AddOffsetA2);

            MySystemINIFile.WriteValue("系统变量", "HeightValueSetting", MV_Global_Variable.HeightValueSetting);
            MySystemINIFile.WriteValue("系统变量", "Save3DImage", MV_Global_Variable.Save3DImage);
            MySystemINIFile.WriteValue("系统变量", "Save3DNGImage", MV_Global_Variable.Save3DNGImage);
            MySystemINIFile.WriteValue("系统变量", "Save3DOKImage", MV_Global_Variable.Save3DOKImage);
            MySystemINIFile.WriteValue("系统变量", "SaveOriImage", MV_Global_Variable.SaveOriImage);
            MySystemINIFile.WriteValue("系统变量", "IsUsingBigBatteryVpp", MV_Global_Variable.IsUsingBigBatteryVpp);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetX1", MV_Global_Variable.BatteryMarkOffsetX1);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetY1", MV_Global_Variable.BatteryMarkOffsetY1);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetA1", MV_Global_Variable.BatteryMarkOffsetA1);

            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetX2", MV_Global_Variable.BatteryMarkOffsetX2);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetY2", MV_Global_Variable.BatteryMarkOffsetY2);
            MySystemINIFile.WriteValue("系统变量", "BatteryMarkOffsetA2", MV_Global_Variable.BatteryMarkOffsetA2);

            MySystemINIFile.WriteValue("系统变量", "Battery1BarcodeCam", MV_Global_Variable.Battery1BarcodeCam);
            MySystemINIFile.WriteValue("系统变量", "Battery2BarcodeCam", MV_Global_Variable.Battery2BarcodeCam);

            SaveRecordDispaly();
        }

        /// <summary>                 
        /// 对主界面进行布置           
        /// </summary>
        private void FixParentWindows()
        {
            MV_Global_Variable.MyFormMain = this;

            Size = Screen.PrimaryScreen.Bounds.Size;
            _windowsSizeX = Size.Width;
            _windowsSizeY = Size.Height;

            MV_Global_Variable.FinishParentForm = true;
        }

        /// <summary>
        /// 落位父窗口原始位置
        /// </summary>
        private void LocationParentWindows()
        {
            this.Location = new Point(0, 0);
        }

        /// <summary>
        /// 创建需要使用的子窗口
        /// </summary>
        private void CreatWindows()
        {
            MyFormLogin = new MV_Form_Login();

            CstLightSet = new LightSet();
            CstLightSet.mController.CreateSerialPort(4);

            MV_Global_Variable.FinishFormCreate = true;
        }

        /// <summary>
        /// 创建需要使用的子控件
        /// </summary>
        private void CreatUserControls()
        {
            MyUserControlLogList = new MV_UC_LOGList();
            MyUserControlLogList.Parent = Container_Workspace_WorkMes.Panel2;
            MyUserControlLogList.Dock = DockStyle.Fill;
            MyUserControlLogList.Show();

            MyUserControlDataTable = new MV_UC_DataTable();
            MyUserControlDataTable.Parent = Container_Workspace_MainUI.Panel1;
            MyUserControlDataTable.Dock = DockStyle.Fill;

            MyUserControlParamSetting = new MV_UC_ParametersSetting();
            MyUserControlParamSetting.Parent = Container_Workspace_MainUI.Panel1;
            MyUserControlParamSetting.Dock = DockStyle.Fill;


            //MyUserControlEthernetConnection = new MV_UC_Ethernet();
            //MyUserControlEthernetConnection.Parent = Container_Workspace_MainUI.Panel1;
            //MyUserControlEthernetConnection.Dock = DockStyle.Fill;

            if (MV_Global_Variable.EnableSpinnakerCamera)
            {
                //mySpinnakerCamera = new SpinnakerCamera(MV_Global_Variable.SpinnakerCameraSerialNumber);
            }

            MV_Global_Variable.FinishControlCreate = true;
        }

        /// <summary>
        /// 创建需要使用的文件类
        /// </summary>
        private void CreatFileClass()
        {
            MyOperateLogFile = new LOGFile();
            MySystemINIFile = new INIFile(Application.StartupPath + "\\INI\\Debug.ini");
            //MyExcelFile = new ExcelFile();
            MyCsvFile = new CSVFile();
        }

        /// <summary>
        /// 相机图像显示控件
        /// </summary>
        private void CameraShowControls()
        {
            try
            {
                //myHikCameraConn = new HikCameraConn(MV_Global_Variable.CameraSerialNumber5, true);
                //myHikCameraConn.SetSoftTriMode();
                //myHikCameraConn.OneFrameCallBack += MyHikCameraConn_OneFrameCallBack;

                myHikCameraConn = new HikCameraConn(MV_Global_Variable.CameraSerialNumber5, false);
                myHikCameraConn.ChagetoContinuesMode();
                myHikCameraConn.RunContinueCallBack += MyHikCameraConn_RunContinueCallBack;

                cogAcqCam1 = (CogAcqFifoTool)CogSerializer.LoadObjectFromFile(AcqPath + "cogAcqCam1.vpp");
                cogAcqCam2 = (CogAcqFifoTool)CogSerializer.LoadObjectFromFile(AcqPath + "cogAcqCam2.vpp");
                cogAcqCam3 = (CogAcqFifoTool)CogSerializer.LoadObjectFromFile(AcqPath + "cogAcqCam3.vpp");
                cogAcqCam4 = (CogAcqFifoTool)CogSerializer.LoadObjectFromFile(AcqPath + "cogAcqCam4.vpp");
                //cogAcqCam5 = (CogAcqFifoTool)CogSerializer.LoadObjectFromFile(AcqPath + "cogAcqCam5.vpp");

                cogRecordDisplayCam1.AutoFit = true;
                cogRecordDisplayCam2.AutoFit = true;
                cogRecordDisplayCam3.AutoFit = true;
                cogRecordDisplayCam4.AutoFit = true;
                cogRecordDisplayCam5.AutoFit = true;
                cogRecordDisplayCam6.AutoFit = true;
                cogRecordDisplayCam7.AutoFit = true;
                cogRecordDisplayCam8.AutoFit = true;
                cogRecordDisplayCam9.AutoFit = true;
                cogRecordDisplayCam10.AutoFit = true;
                cogRecordDisplayCam11.AutoFit = true;
                cogRecordDisplayCam12.AutoFit = true;
                cogRecordDisplayCam13.AutoFit = true;

                cogRecordDisplayCam6.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam7.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam8.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam9.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam10.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam11.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam12.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;
                cogRecordDisplayCam13.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Grey;

                SetRecordDispaly();

                if (!myHikCameraConn.IniOk)
                {
                    MV_Global_Variable.FinishCameraCreate = false;
                    return;
                }
                MV_Global_Variable.FinishCameraCreate = true;

            }
            catch (Exception ex)
            {
                MV_Global_Variable.FinishCameraCreate = false;
            }

        }

        private void MyHikCameraConn_RunContinueCallBack(IntPtr pData, ref MvCamCtrl.NET.MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            //throw new NotImplementedException();
            if (!Cam5ActionOver && !Cam5Lock)
            {
                Cam5Lock = true;
                CodeEndTime = DateTime.Now;
                ReadTimeSpan = CodeEndTime - CodeStartTime;
                string SendMessage;
                try
                {
                    if (ReadTimeSpan.TotalSeconds > 3)
                    {
                        myHikCameraConn.Stop();
                        Cam5ActionOver = true;
                        RunReadCodeSucceed = false;
                        SendMessage = "Cam5-1^0";
                        MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                        CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);
                        RecordOperateCor("SendMessage:" + SendMessage);
                        Cam5Lock = false;
                    }
                    if (pFrameInfo.enPixelType == MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        CogImage8Root cogImage8Root = new CogImage8Root();
                        cogImage8Root.Initialize((Int32)pFrameInfo.nWidth, (Int32)pFrameInfo.nHeight, pData, (Int32)pFrameInfo.nWidth, null);

                        CogImage8Grey cogImage8Grey = new CogImage8Grey();
                        cogImage8Grey.SetRoot(cogImage8Root);
                        ImageCam5 = cogImage8Grey;
                        cogRecordDisplayCam5.Image = cogImage8Grey.ScaleImage((int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight);
                        cogRunToolBlockCam5Code.Inputs["Cam5"].Value = cogImage8Grey.ScaleImage((int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight);
                        cogRunToolBlockCam5Code.Run();

                        if (cogRunToolBlockCam5Code.RunStatus.Result == CogToolResultConstants.Accept)
                        {
                            myHikCameraConn.Stop();
                            Cam5ActionOver = true;
                            CavityQRCode = cogRunToolBlockCam5Code.Outputs["CodeString"].Value.ToString();
                            RunReadCodeSucceed = true;
                            SendMessage = "Cam5-1^1^" + CavityQRCode;
                            cogRecordDisplayCam5.Record = cogRunToolBlockCam5Code.CreateLastRunRecord().SubRecords[0];
                            MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                            MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);

                            RecordOperateCor("SendMessage:" + SendMessage);

                            SaveQRImage(todayImagePath + "cam5\\", ImageCam5);

                            Cam5ActionOver = true;
                        }
                        else
                        {
                            CavityQRCode = "Fail";
                            SendMessage = "Cam5-1^0";
                            RunReadCodeSucceed = false;

                        }
                        Cam5Lock = false;
                    }

                }
                catch
                {
                    SendMessage = "Cam5-1^0";
                    MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);
                    RecordOperateCor("SendMessage:" + SendMessage);
                    CavityQRCode = "NA";
                    RunReadCodeSucceed = false;
                    Cam5ActionOver = true;
                    Cam5Lock = false;
                }
            }
        }

        private void MyHikCameraConn_OneFrameCallBack(IntPtr pData, ref MvCamCtrl.NET.MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            //throw new NotImplementedException();
            CodeEndTime = DateTime.Now;
            ReadTimeSpan = CodeEndTime - CodeStartTime;
            string SendMessage;
            try
            {
                if (ReadTimeSpan.TotalSeconds > 3)
                {
                    RunReadCodeSucceed = false;
                    SendMessage = "Cam5-1^0";
                    MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                    CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);

                    RecordOperateCor("SendMessage:" + SendMessage);
                    return;
                }
                if (pFrameInfo.enPixelType == MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    CogImage8Root cogImage8Root = new CogImage8Root();
                    cogImage8Root.Initialize((Int32)pFrameInfo.nWidth, (Int32)pFrameInfo.nHeight, pData, (Int32)pFrameInfo.nWidth, null);

                    CogImage8Grey cogImage8Grey = new CogImage8Grey();
                    cogImage8Grey.SetRoot(cogImage8Root);
                    ImageCam5 = cogImage8Grey;
                    cogRecordDisplayCam5.Image = cogImage8Grey.ScaleImage((int)pFrameInfo.nWidth, (int)pFrameInfo.nHeight);
                    System.GC.Collect();
                    cogRunToolBlockCam5Code.Inputs["Cam5"].Value = ImageCam5;
                    cogRunToolBlockCam5Code.Run();

                    if (cogRunToolBlockCam5Code.RunStatus.Result == CogToolResultConstants.Accept)
                    {

                        CavityQRCode = cogRunToolBlockCam5Code.Outputs["CodeString"].Value.ToString();
                        RunReadCodeSucceed = true;
                        SendMessage = "Cam5-1^1^" + CavityQRCode;
                        cogRecordDisplayCam5.Record = cogRunToolBlockCam5Code.CreateLastRunRecord().SubRecords[0];
                        MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                        MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);
                        RecordOperateCor("SendMessage:" + SendMessage);
                        SaveQRImage(todayImagePath + "cam5\\", ImageCam5);
                    }
                    else
                    {
                        myHikCameraConn.SoftTriggerOnce();
                        CavityQRCode = "Fail";
                        SendMessage = "Cam5-1^0";
                        RunReadCodeSucceed = false;
                    }
                    cogImage8Root.Dispose();
                    cogImage8Grey.Dispose();
                }

            }
            catch
            {
                SendMessage = "Cam5-1^0";
                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);
                RecordOperateCor("SendMessage:" + SendMessage);
                CavityQRCode = "NA";
                RunReadCodeSucceed = false;

            }

        }


        /// <summary>
        /// 允许运行的控件
        /// </summary>
        private void EnableControls()
        {
            CstLightSet.mController.SetDigitalValue(6, MV_Global_Variable.LightValue6);
            UIstatusTimer.Enabled = true;
        }

        /// <summary>
        /// 显示初始化结果
        /// </summary>
        private void ShowInitMes()
        {
            if (MV_Global_Variable.FinishParentForm &&
                MV_Global_Variable.FinishFormCreate && MV_Global_Variable.FinishControlCreate &&
                MV_Global_Variable.FinishIniVariable && MV_Global_Variable.FinishCameraCreate &&
                Load3DSucceed && LoadVppSucceed)
            {
                MyUserControlLogList.AddRunLog("初始化成功！");
                OperateLog("初始化成功！");
            }
            else
            {
                if (!MV_Global_Variable.FinishCameraCreate)
                {
                    MyUserControlLogList.AddAlarmLog("相机加载失败！");
                    ErrorLog("相机加载失败");
                }
                if (!LoadVppSucceed)
                {
                    MyUserControlLogList.AddAlarmLog("Vpp加载失败！");
                    ErrorLog("Vpp加载失败");
                }
                if (!Load3DSucceed)
                {
                    MyUserControlLogList.AddAlarmLog("3D加载失败！");
                    ErrorLog("3D加载失败");
                }
                MyUserControlLogList.AddAlarmLog("初始化失败！");
            }
            BringToFront();
        }

        /// <summary>
        /// 定义日志字符串格式
        /// </summary>
        /// <param name="mes">日志信息</param>
        /// <returns></returns>
        private string FormateForLog(string action, string msg)
        {
            return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,") + DateTime.Now.Millisecond + "]_" + action + "_" + msg;
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="mes">日志信息</param>
        public void OperateLog(string msg)
        {
            try
            {
                MyOperateLogFile.SaveLog(FormateForLog("Operation", msg), "D:\\VisionLog\\");
            }
            catch
            { }
        }

        /// <summary>
        /// 记录报警日志
        /// </summary>
        /// <param name="mes"></param>
        public void ErrorLog(string msg)
        {
            try
            {
                MyOperateLogFile.SaveLog(FormateForLog("Error", msg));
            }
            catch
            { }
        }

        public void RecordParamCor(string msg)
        {
            if (boolProgramRunning)
            {
                MyUserControlLogList.AddParaModi(msg);
                OperateLog(msg);
            }
        }

        public void RecordErrorCor(string msg)
        {
            MyUserControlLogList.AddAlarmLog(msg);
            ErrorLog(msg);
        }

        public void RecordOperateCor(string msg)
        {
            if (boolProgramRunning)
            {
                MyUserControlLogList.AddRunLog(msg);
                OperateLog(msg);
            }
        }

        /// <summary>
        /// 载入函数
        /// </summary>
        void LoadingFunc()
        {
            MV_Global_Variable.GlobalLoadingStart = DateTime.Now;

            int step = 0;

            MyFormLoading = new MV_Form_Loading();
            MyFormLoading.Show();
            //MyFormLoading.BringToFront();
            MyFormLoading.LoadMessage("程序启动", step);
            step = 10;

            while (!MV_Global_Variable.FinishParentForm ||
                !MV_Global_Variable.FinishFormCreate || !MV_Global_Variable.FinishControlCreate ||
                !MV_Global_Variable.FinishIniVariable || !MV_Global_Variable.FinishCameraCreate || !LoadVppSucceed || !Load3DSucceed)
            {
                MV_Global_Variable.GlobalLoadingEnd = DateTime.Now;
                if ((MV_Global_Variable.GlobalLoadingEnd - MV_Global_Variable.GlobalLoadingStart).TotalSeconds < MV_Global_Variable.GlobalLoadingTimeout)
                {
                    switch (step)
                    {
                        case 10:
                            if (MV_Global_Variable.FinishParentForm)
                            {
                                MyFormLoading.LoadMessage("主窗体初始化完成", step);

                                step = 20;
                            }
                            break;
                        case 20:
                            if (MV_Global_Variable.FinishFormCreate)
                            {
                                MyFormLoading.LoadMessage("子窗体初始化完成", step);
                                step = 30;
                            }
                            break;
                        case 30:
                            if (MV_Global_Variable.FinishControlCreate)
                            {
                                MyFormLoading.LoadMessage("子控件初始化完成", step);
                                step = 50;
                            }
                            break;
                        case 40:
                            if (MV_Global_Variable.FinishCameraCreate)
                            {
                                MyFormLoading.LoadMessage("相机初始化完成", step);
                                step = 60;
                            }
                            else
                            {
                                MyFormLoading.LoadMessage("等待相机初始化完成...等待时间：" + (MV_Global_Variable.GlobalLoadingEnd - MV_Global_Variable.GlobalLoadingStart).ToString("ss"), step);
                            }
                            break;
                        case 50:
                            if (MV_Global_Variable.FinishIniVariable)
                            {
                                MyFormLoading.LoadMessage("变量初始化完成", step);
                                step = 40;
                            }
                            break;
                        case 60:
                            if (LoadVppSucceed)
                            {
                                MyFormLoading.LoadMessage("VPP加载成功", step);
                                step = 100;
                            }
                            break;
                        case 100:
                            MyFormLoading.LoadMessage("程序载入完毕！", step);
                            break;
                    }

                }
                else
                {
                    break;
                }
            }

            MyFormLoading.Close();
            MyFormLoading.Dispose();

        }

        /// <summary>
        /// 等待载入完成
        /// </summary>
        void WaitLoadingFinish()
        {
            while (_LoadingThread.ThreadState != ThreadState.Stopped)
            {
            }

        }

        /// <summary>
        /// 权限管理
        /// </summary>
        /// <param name="level"></param>
        private void Authorization(string level)
        {
            switch (level)
            {
                case "1":
                    AdministratorRights();
                    break;
                case "2":
                    EnginnerRights();
                    break;
                case "3":
                    OperatorRights();
                    break;
                default:
                    OperatorRights();
                    break;
            }
        }

        /// <summary>
        /// 管理员权限
        /// </summary>
        void AdministratorRights()
        {

        }

        /// <summary>
        /// 工程师权限
        /// </summary>
        void EnginnerRights()
        {

        }

        /// <summary>
        /// 操作员/默认权限
        /// </summary>
        void OperatorRights()
        {

        }

        void SaveImage(string path, CogImage16Range imageData, bool isSavePic)
        {
            Task task = new Task(() =>
            {
                try
                {
                    if (MV_Global_Variable.Save3DImage && isSavePic)
                    {
                        string SaveName = DateTime.Now.ToString("HHmmss");
                        string SavePath = path + SaveName + ".idb";
                        CogImageFile ImageFile = new CogImageFile();
                        ImageFile.Open(SavePath, CogImageFileModeConstants.Write);
                        ImageFile.Append(imageData);
                        ImageFile.Close();
                    }
                }
                catch
                { }
            });
            task.Start();
        }

        void SaveImage(string path, CogImage8Grey imageData)
        {
            Task task = new Task(() =>
             {
                 try
                 {
                     if (MV_Global_Variable.SaveOriImage)
                     {
                         string SaveName = DateTime.Now.ToString("HHmmss");
                         string SavePath = path + SaveName + ".bmp";
                         CogImageFileTool mycogImgCam = new CogImageFileTool();
                         mycogImgCam.InputImage = imageData;
                         mycogImgCam.Operator.Open(SavePath, CogImageFileModeConstants.Write);
                         mycogImgCam.Run();
                     }
                 }
                 catch
                 { }
             });
            task.Start();
        }

        void SaveNGImage(string path, CogImage8Grey imageData)
        {
            Task task = new Task(() =>
            {
                try
                {
                    string SaveName = "NG_File_" + DateTime.Now.ToString("HHmmss");
                    string SavePath = path + SaveName + ".bmp";
                    CogImageFileTool mycogImgCam = new CogImageFileTool();
                    mycogImgCam.InputImage = imageData;
                    mycogImgCam.Operator.Open(SavePath, CogImageFileModeConstants.Write);
                    mycogImgCam.Run();

                }
                catch
                { }
            });
            task.Start();
        }


        void SaveGImage(string path, string Barcode, Bitmap imageData)
        {
            Task task = new Task(() =>
            {
                try
                {
                    string SaveName = Barcode;
                    string SavePath = path + SaveName + ".jpg";
                    imageData.Save(SavePath);
                }
                catch
                { }
            });
            task.Start();
        }

        void SaveQRImage(string path, CogImage8Grey imageData)
        {
            try
            {
                string SaveName = DateTime.Now.ToString("HHmmss") + DateTime.Now.Millisecond.ToString();
                string SavePath = path + SaveName + ".bmp";
                cogImgCam5.InputImage = imageData;
                cogImgCam5.Operator.Open(SavePath, CogImageFileModeConstants.Write);
                cogImgCam5.Run();
            }
            catch
            { }
        }

        private void ShowLabelInfo(CogRecordDisplay cogRecordDisplay, CogGraphicLabel lbl, double x, double y, string info, CogColorConstants color)
        {
            lbl.Color = color;
            lbl.Font = new Font(FontFamily.GenericMonospace, 18);
            lbl.Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
            lbl.SelectedSpaceName = "#";
            lbl.SetXYText(x, y, info);
            cogRecordDisplay.StaticGraphics.Add(lbl, "str1");
        }

        void LoadVisionProVpp()
        {
            try
            {

                cogNPointCalibrationTool = (CogToolBlock)CogSerializer.LoadObjectFromFile(CalibPath + "cogNPtsMatchCam.vpp");
                cogRunToolBlockCam5Code = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlcokCam5QRCode.vpp");
                cogRunToolBlockCam12 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam12.vpp");
                cogRunToolBlockCam12BigBattery = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam12BigBattery.vpp");
                cogRunToolBlockCam34 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34Cavity1.vpp");
                cogRunToolBlockCam34Cavity2 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34Cavity2.vpp");
                cogRunToolBlockCam1QRCode = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam1QRCode.vpp");
                cogRunToolBlockCam34Check = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34Check.vpp");
                cogRunToolBlockCam34Cal = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34Cal.vpp");
                cogRunToolBlockCam34CheckFPC = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34CheckFPC.vpp");
                cogRunToolBlockCam34CheckGap = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34CheckGap.vpp");
                cogRunToolBlockCam34CheckFilm = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath + "cogRunToolBlockCam34CheckFilm.vpp");


                for (int i = 1; i < 5; i++)
                {
                    cogCalibNPointToNPointToolPosition[i] = new CogCalibNPointToNPointTool();
                    cogCalibNPointToNPointToolPosition[i] = (CogCalibNPointToNPointTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibNPToolCam" + i + ".vpp");
                }

                cogCalibCheckerboardToolCam1 = (CogCalibCheckerboardTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibCheckboardToolCam1.vpp");
                cogCalibCheckerboardToolCam2 = (CogCalibCheckerboardTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibCheckboardToolCam2.vpp");
                cogCalibCheckerboardToolCam3 = (CogCalibCheckerboardTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibCheckboardToolCam3.vpp");
                cogCalibCheckerboardToolCam4 = (CogCalibCheckerboardTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibCheckboardToolCam4.vpp");

                cogRunToolBlock3DCam1 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam1.vpp");
                cogRunToolBlock3DCam2 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam2.vpp");
                cogRunToolBlock3DCam3 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam3.vpp");
                cogRunToolBlock3DCam4 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam4.vpp");

                cogRunToolBlock3DCam12 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam12.vpp");
                cogRunToolBlock3DCam22 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam22.vpp");
                cogRunToolBlock3DCam32 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam32.vpp");
                cogRunToolBlock3DCam42 = (CogToolBlock)CogSerializer.LoadObjectFromFile(RunToolBlcokPath2 + "cogRunToolBlock3DCam42.vpp");

                Load3DVppParameters();


                LoadVppSucceed = true;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
                LoadVppSucceed = false;
            }

        }

        void Load3DVppParameters()
        {
            string STRpath1 = RunToolBlcokPath2 + "3D1.vpx";
            if (File.Exists(STRpath1))
            {
                MainParameter.DSerializParam(STRpath1, out MainParameter1);
                Param_Add(cogRunToolBlock3DCam1, MainParameter1);
            }

            string STRpath2 = RunToolBlcokPath2 + "3D2.vpx";
            if (File.Exists(STRpath2))
            {
                MainParameter.DSerializParam(STRpath2, out MainParameter2);
                Param_Add(cogRunToolBlock3DCam2, MainParameter2);
            }

            string STRpath3 = RunToolBlcokPath2 + "3D3.vpx";
            if (File.Exists(STRpath3))
            {
                MainParameter.DSerializParam(STRpath3, out MainParameter3);
                Param_Add(cogRunToolBlock3DCam3, MainParameter3);
            }

            string STRpath4 = RunToolBlcokPath2 + "3D4.vpx";
            if (File.Exists(STRpath4))
            {
                MainParameter.DSerializParam(STRpath4, out MainParameter4);
                Param_Add(cogRunToolBlock3DCam4, MainParameter4);
            }

            STRpath1 = RunToolBlcokPath2 + "3D12.vpx";
            if (File.Exists(STRpath1))
            {
                MainParameter.DSerializParam(STRpath1, out MainParameter12);
                Param_Add(cogRunToolBlock3DCam12, MainParameter12);
            }

            STRpath2 = RunToolBlcokPath2 + "3D22.vpx";
            if (File.Exists(STRpath2))
            {
                MainParameter.DSerializParam(STRpath2, out MainParameter22);
                Param_Add(cogRunToolBlock3DCam22, MainParameter22);
            }

            STRpath3 = RunToolBlcokPath2 + "3D32.vpx";
            if (File.Exists(STRpath3))
            {
                MainParameter.DSerializParam(STRpath3, out MainParameter32);
                Param_Add(cogRunToolBlock3DCam32, MainParameter32);
            }

            STRpath4 = RunToolBlcokPath2 + "3D42.vpx";
            if (File.Exists(STRpath4))
            {
                MainParameter.DSerializParam(STRpath4, out MainParameter42);
                Param_Add(cogRunToolBlock3DCam42, MainParameter42);
            }
        }

        ICogTool getToolByTreeName(CogToolBlock theRootToolBlock, string treeName)
        {

            string[] branchs = treeName.Split('/');
            CogToolBlock tmp = theRootToolBlock;
            for (int i = 0; i < branchs.Length - 1; i++)
            {
                tmp = tmp.Tools[branchs[i]] as CogToolBlock;

            }
            try
            {
                ICogTool theTool = tmp.Tools[branchs[branchs.Length - 1]];
                return theTool;
            }
            catch
            {

                return null; ;
            }

        }

        private void Param_Add(CogToolBlock blockvpp, Parameter MainParameter)
        {
            try
            {

                CogToolBlock GeneralCheck = getToolByTreeName(blockvpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                GeneralCheck.Inputs["MaskRegions"].Value = MainParameter.theMaskRegions;
                GeneralCheck.Inputs["Region"].Value = MainParameter.theMainRegion;
                GeneralCheck.Inputs["GreyThreshold"].Value = MainParameter.GreyThreshold;
                GeneralCheck.Inputs["AreaThreshold"].Value = MainParameter.AreaThreshold;

                CogToolBlock resetPlane = getToolByTreeName(blockvpp, "HistogramInfo/ResetPlane-E") as CogToolBlock;
                resetPlane.Inputs[2].Value = MainParameter.ReferenceUSE;
                resetPlane.Inputs[1].Value = MainParameter.Referencesink;

                //referenceuse.Text = resetPlane.Inputs[2].Value.ToString();
                //referencesink.Value = (decimal)MainParameter.Referencesink;


                CogCaliperTool CaliperTool_1 = getToolByTreeName(blockvpp, "Locate/CogCaliperTool1") as CogCaliperTool;
                CaliperTool_1.RunParams = MainParameter.CaliperParam_1;
                CaliperTool_1.Region = MainParameter.CaliperRegion_1;

                CogCaliperTool CaliperTool_2 = getToolByTreeName(blockvpp, "Locate/CogCalipVorizental-E") as CogCaliperTool;
                CaliperTool_2.RunParams = MainParameter.CaliperParam_2;
                CaliperTool_2.Region = MainParameter.CaliperRegion_2;


                CogFindLineTool FindLineTool = getToolByTreeName(blockvpp, "Locate/CogFindRightLine-E") as CogFindLineTool;
                FindLineTool.RunParams = MainParameter.Findline_1;
                Cog3DRangeImagePlaneEstimatorTool PlaneEstimatorTool = getToolByTreeName(blockvpp, "HistogramInfo/ResetPlane-E/Cog3DRangeImagePlaneEstimatorTool-E") as Cog3DRangeImagePlaneEstimatorTool;
                PlaneEstimatorTool.RunParams = MainParameter.RangeImagePlaneEstimator;


                //numAreaThreshol.Value = (decimal)MainParameter.AreaThreshold;
                //numGreyThreshold.Value = (decimal)MainParameter.GreyThreshold;

                CogToolBlock theBlock = getToolByTreeName(blockvpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                //CogCaliperTool theCaliperToolT = getToolByTreeName(theBlock, "CaliperT") as CogCaliperTool;
                theBlock.Inputs[0].Value = MainParameter.Count;
                //this.tBLineCheckCount.Text = theBlock.Inputs[0].Value.ToString();
                if (MainParameter.Count > 0)
                {
                    //num_Scale.Value = (decimal)MainParameter.FindLineCountRatio[0];
                    //num_Distance.Value = (decimal)MainParameter.DisThreshold[0];
                    //num_Count.Value = (decimal)MainParameter.CountThreshold[0];
                }

                theBlock.Outputs.Clear();


                for (int i = 0; i < MainParameter.Count; i++)
                {

                    int idx = i + 1;

                    theBlock.Outputs.Add(new CogToolBlockTerminal("LocateRegion" + idx, MainParameter.LocateRegion[i]));


                    theBlock.Outputs.Add(new CogToolBlockTerminal("LocateCliperThreshold" + idx, MainParameter.LocateCliperThreshold[i]));

                    theBlock.Outputs.Add(new CogToolBlockTerminal("LocateCliperFilterHalfSize" + idx, MainParameter.LocateCliperFilterHalfSize[i]));


                    theBlock.Outputs.Add(new CogToolBlockTerminal("MaskRegions" + idx, MainParameter.MaskRegions[i]));


                    //CogFindLineTool CogFindLineTool_T = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams/FindLineT") as CogFindLineTool;


                    theBlock.Outputs.Add(new CogToolBlockTerminal("FindLine" + idx, MainParameter.FindLine[i]));

                    theBlock.Outputs.Add(new CogToolBlockTerminal("FindLineCountRatio" + idx, MainParameter.FindLineCountRatio[i]));

                    theBlock.Outputs.Add(new CogToolBlockTerminal("SampleRegions" + idx, MainParameter.SampleRegions[i]));


                    theBlock.Outputs.Add(new CogToolBlockTerminal("DisThreshold" + idx, MainParameter.DisThreshold[i]));


                    theBlock.Outputs.Add(new CogToolBlockTerminal("CountThreshold" + idx, MainParameter.CountThreshold[i]));

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void SaveVisionProVpp()
        {
            while (true)
            {
                if (SaveThreadLock && SaveVppOver)
                {
                    SaveVppOver = false;
                    SaveThreadLock = false;
                    try
                    {
                        CogSerializer.SaveObjectToFile(cogNPointCalibrationTool, CalibPath + "cogNPtsMatchCam.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam5Code, RunToolBlcokPath + "cogRunToolBlcokCam5QRCode.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam12, RunToolBlcokPath + "cogRunToolBlockCam12.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam12BigBattery, RunToolBlcokPath + "cogRunToolBlockCam12BigBattery.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34, RunToolBlcokPath + "cogRunToolBlockCam34Cavity1.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34Cavity2, RunToolBlcokPath + "cogRunToolBlockCam34Cavity2.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam1QRCode, RunToolBlcokPath + "cogRunToolBlockCam1QRCode.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34Check, RunToolBlcokPath + "cogRunToolBlockCam34Check.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34Cal, RunToolBlcokPath + "cogRunToolBlockCam34Cal.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34CheckFPC, RunToolBlcokPath + "cogRunToolBlockCam34CheckFPC.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34CheckGap, RunToolBlcokPath + "cogRunToolBlockCam34CheckGap.vpp");
                        CogSerializer.SaveObjectToFile(cogRunToolBlockCam34CheckFilm, RunToolBlcokPath + "cogRunToolBlockCam34CheckFilm.vpp");

                        CogSerializer.SaveObjectToFile(cogAcqCam1, AcqPath + "cogAcqCam1.vpp");
                        CogSerializer.SaveObjectToFile(cogAcqCam2, AcqPath + "cogAcqCam2.vpp");
                        CogSerializer.SaveObjectToFile(cogAcqCam3, AcqPath + "cogAcqCam3.vpp");
                        CogSerializer.SaveObjectToFile(cogAcqCam4, AcqPath + "cogAcqCam4.vpp");

                        CogSerializer.SaveObjectToFile(cogCalibCheckerboardToolCam1, CalibPath + "cogCalibCheckboardToolCam1.vpp");
                        CogSerializer.SaveObjectToFile(cogCalibCheckerboardToolCam2, CalibPath + "cogCalibCheckboardToolCam2.vpp");
                        CogSerializer.SaveObjectToFile(cogCalibCheckerboardToolCam3, CalibPath + "cogCalibCheckboardToolCam3.vpp");
                        CogSerializer.SaveObjectToFile(cogCalibCheckerboardToolCam4, CalibPath + "cogCalibCheckboardToolCam4.vpp");

                        RecordParamCor("保存VPP结束");
                        SaveVppOver = true;
                    }
                    catch
                    {
                        RecordParamCor("保存VPP结束");
                        SaveVppOver = true;
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        void CreatThread()
        {
            InitCameraThread1 = new Thread(InitSubFunc1)
            {
                IsBackground = true
            };
            InitCameraThread1.Start();

            InitCameraThread2 = new Thread(InitSubFunc2)
            {
                IsBackground = true
            };
            InitCameraThread2.Start();

            //InitCameraThread3 = new Thread(InitSubFunc3)
            //{
            //    IsBackground = true
            //};
            //InitCameraThread3.Start();

            InitCameraThread4 = new Thread(InitSubFunc4)
            {
                IsBackground = true
            };
            InitCameraThread4.Start();

            //InitCameraThread3d = new Thread(AcqusitionFromSensor)
            //{
            //    IsBackground = true
            //};
            //InitCameraThread3d.Start();

            SaveThread = new Thread(SaveVisionProVpp)
            {
                IsBackground = true
            };
            SaveThread.Start();
        }

        void InitSubFunc1()
        {
            while (true)
            {
                if (boolProgramRunning)
                {
                    if (MyBoolRunCommand[0])
                    {
                        try
                        {
                            MyBoolRunCommand[0] = false;

                            DateTime St, Et;
                            St = DateTime.Now;

                            RunCamBatteryCavityStep(MyStringRunCommand1, "Cam12");

                            Et = DateTime.Now;

                            if ((Et - St).TotalMilliseconds < 4200)
                            {

                                string SendMessage;
                                if (MyStringRunCommand1 == "Battery")
                                {
                                    if (RunSucceed)
                                    {

                                        SendMessage = Cam12StepStr[0] + "^1^" + Cam12StepStr[1] + "^" + Cam12StepStr[2] + "^" + Cam12StepStr[3] + "^" +
                                       BatteryWidth + "^" + BatteryHeight + "^" + BatteryWidth2 + "^" + BatteryHeight2;
                                    }
                                    else
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                   if( MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, 0)!=10000)
                                    {
                                        ErrorLog("光源关闭失败！");
                                    }
                                }
                                if (MyStringRunCommand1 == "Battery2")
                                {
                                    if (RunSucceed)
                                    {

                                        SendMessage = Cam12StepStr[0] + "^1^" + Cam12StepStr[1] + "^" + Cam12StepStr[2] + "^" + Cam12StepStr[3] + "^" +
                                       BatteryWidth + "^" + BatteryHeight + "^" + BatteryWidth2 + "^" + BatteryHeight2;
                                    }
                                    else
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                   if( MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, 0)!=10000)
                                    {
                                        ErrorLog("光源关闭失败！");
                                    }
                                }
                                if (MyStringRunCommand1 == "Mark")
                                {
                                    //MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, 0);
                                    if (RunSucceed)
                                    {
                                        SendMessage = Cam12StepStr[0] + "^1^" + BatteryQRCode;

                                    }
                                    else
                                    {
                                        SendMessage = Cam12StepStr[0] + "^0";
                                    }
                                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel2, 0);
                                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel3, 0);
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                }
                            }
                        }
                        catch
                        {
                            if (MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel1, 0) != 10000)
                            {
                                ErrorLog("光源关闭失败！");
                            }
                        }

                    }
                }
                else { Thread.Sleep(500); }
            }
        }

        void InitSubFunc2()
        {
            while (true)
            {
                if (boolProgramRunning)
                {
                    if (MyBoolRunCommand[1])
                    {
                        try
                        {
                            MyBoolRunCommand[1] = false;

                            DateTime St, Et;
                            St = DateTime.Now;

                            RunCamBatteryCavityStep(MyStringRunCommand2, "Cam34");

                            Et = DateTime.Now;

                            if ((Et - St).TotalMilliseconds < 4200)
                            {
                                string SendMessage;
                                string SendGap;
                                if (MyStringRunCommand2 == "Cavity")
                                {
                                    if (RunSucceed2)
                                    {
                                        SendMessage = Cam34StepStr[0] + "^1 ^" + Cam34StepStr[2] + "^" + Cam34StepStr[3] + "^" + Cam34StepStr[4] + "^" +
                                       CavityWidth + "^" + CavityHeight + "^" + CavityWidth2 + "^" + CavityHeight2 + "^1^" + CavityBarcode;
                                    }
                                    else
                                    {
                                        SendMessage = Cam34StepStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket2.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, 0);
                                }
                                if (MyStringRunCommand2 == "Cavity2")
                                {
                                    if (RunSucceed2)
                                    {
                                        SendMessage = Cam34StepStr[0] + "^1 ^" + Cam34StepStr[2] + "^" + Cam34StepStr[3] + "^" + Cam34StepStr[4] + "^" +
                                       CavityWidth + "^" + CavityHeight + "^" + CavityWidth2 + "^" + CavityHeight2 + "^2^" + CavityBarcode;
                                    }
                                    else
                                    {
                                        SendMessage = Cam34StepStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket2.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, 0);
                                }
                                if (MyStringRunCommand2 == "CavityCheck")
                                {
                                    if (RunSucceed)
                                    {
                                        SendMessage = Cam34StepStr[0] + "^1^" + Cam34StepStr[1] + "^" + Cam34StepStr[2] + "^" + Cam34StepStr[3] + "^" +
                                       AxisMoveA.ToString("0.00") + "^" + AxisMoveX.ToString("0.00") + "^" + AxisMoveY.ToString("0.00");
                                        SendGap = "Cam3-3^1^" + Gap[0].ToString("0.000") + "^" + Gap[1].ToString("0.000") + "^" + Gap[2].ToString("0.000") + "^" + Gap[3].ToString("0.000") + "^"
                                            + Gap[4].ToString("0.000") + "^" + Gap[5].ToString("0.000") + "^" + Gap[6].ToString("0.000") + "^" + Gap[7].ToString("0.000");
                                    }
                                    else
                                    {
                                        SendMessage = Cam34StepStr[0] + "^0";
                                        SendGap = "Cam3-3^0^0";
                                    }
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                    Thread.Sleep(200);
                                    MySubThreadFlow.MyServerSocket.SendAll(SendGap);
                                    RecordOperateCor("SendMessage:" + SendGap);
                                    MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel4, 0);
                                }
                                if (MyStringRunCommand2 == "Film")
                                {
                                    if (MV_Global_Variable.MyFormMain.RunSucceed)
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^1";
                                    }
                                    else
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                }
                                if (MyStringRunCommand2 == "Gap")
                                {
                                    if (MV_Global_Variable.MyFormMain.RunSucceed)
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^1";
                                    }
                                    else
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                }
                                if (MyStringRunCommand2 == "FPC")
                                {
                                    if (MV_Global_Variable.MyFormMain.RunSucceed)
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^1";
                                    }
                                    else
                                    {
                                        SendMessage = MySubThreadFlow.MsgStr[0] + "^0";
                                    }
                                    MySubThreadFlow.MyServerSocket.SendAll(SendMessage);
                                    RecordOperateCor("SendMessage:" + SendMessage);
                                }
                            }
                        }
                        catch
                        {
                            //string SendMessage = Cam34StepStr[0] + "^0";
                            //MySubThreadFlow.MyServerSocket.SendMessageToAll(SendMessage);
                            //RecordOperateCor("SendMessage:" + SendMessage);
                        }
                    }
                }
                else { Thread.Sleep(500); }
            }
        }

        void InitSubFunc3()
        {
            while (true)
            {
                if (boolProgramRunning)
                {


                }
                else { Thread.Sleep(500); }
            }
        }

        void InitSubFunc4()
        {
            while (true)
            {
                if (boolProgramRunning)
                {

                    if (MyBoolRunCommand[3])
                    {
                        MyBoolRunCommand[3] = false;
                        m3DstartTime = DateTime.Now;
                        try
                        {

                            StartTick = true;
                            Capture3DCameraAsync(Cam3DStepStr[0], true);

                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        m3DendTime = DateTime.Now;
                        if (StartTick && (m3DendTime - m3DstartTime).TotalSeconds > 300)
                        {
                            StartRun = false;
                            //Capture3DCameraAsync(Camera3DDirc, false);
                            sensor0.StopAcquisition();
                            StartTick = false;
                        }
                    }
                    if (MyBoolRunCommand[2])
                    {
                        MyBoolRunCommand[2] = false;
                        RunCamBatteryCavityStep("QRcode", "Cam5");
                    }

                    if (isRun)
                    {
                        isRun = false;
                        if (StartRun)
                        {
                            int micTime;
                            Api.GetExposureTime(sensor0._sensorObject, 0, out micTime);
                            if (micTime != MV_Global_Variable.SmartRayExpTime)
                                Api.SetExposureTime(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayExpTime);

                            int LaserThreshold;
                            Api.Get3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, out LaserThreshold);
                            if (LaserThreshold != MV_Global_Variable.SmartRayLaserThreshold)
                                Api.Set3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayLaserThreshold);

                            int LaserBrightness;
                            Api.GetLaserBrightness(sensor0._sensorObject, out LaserBrightness);
                            if (LaserBrightness != MV_Global_Variable.SmartRayLaserBrightness)
                                Api.SetLaserBrightness(sensor0._sensorObject, MV_Global_Variable.SmartRayLaserBrightness);

                            sensor0.ClearImageData();
                            Api.SetPacketSize(sensor0._sensorObject, 0);
                            sensor0.StartAcquisition();

                            MySubThreadFlow.MyServerSocketScan.SendAll(Cam3DStepStr[0] + "^GO");
                            RecordOperateCor("SendMessage:" + Cam3DStepStr[0] + "^GO");
                            //sensor0.WaitForImage(1);

                        }
                        else
                        {
                            sensor0.StopAcquisition();
                        }

                    }

                    Thread.Sleep(100);
                }
                else { Thread.Sleep(100); }
            }
        }

        void ChangeExpTime(string product)
        {
            switch (product)
            {
                case "Battery":
                    if (cogAcqCam1.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraBackExposure1))
                        cogAcqCam1.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraBackExposure1);

                    if (cogAcqCam2.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraBackExposure2))
                        cogAcqCam2.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraBackExposure2);


                    break;
                case "Battery2":
                    if (cogAcqCam1.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraBackExposure1))
                        cogAcqCam1.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraBackExposure1);

                    if (cogAcqCam2.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraBackExposure2))
                        cogAcqCam2.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraBackExposure2);


                    break;
                case "Mark":
                    if (cogAcqCam1.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraHandMarkExposure1))
                        cogAcqCam1.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraHandMarkExposure1);

                    if (cogAcqCam2.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraHandMarkExposure2))
                        cogAcqCam2.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraHandMarkExposure2);


                    break;
                case "Cavity":

                    if (cogAcqCam3.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraCavityExposure3))
                        cogAcqCam3.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraCavityExposure3);

                    if (cogAcqCam4.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraCavityExposure4))
                        cogAcqCam4.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraCavityExposure4);
                    break;
                case "Cavity2":


                    if (cogAcqCam3.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraCavityExposure3))
                        cogAcqCam3.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraCavityExposure3);

                    if (cogAcqCam4.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraCavityExposure4))
                        cogAcqCam4.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraCavityExposure4);
                    break;
                case "CavityCheck":


                    if (cogAcqCam3.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraCalOffsetExposure3))
                        cogAcqCam3.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraCalOffsetExposure3);

                    if (cogAcqCam4.Operator.OwnedExposureParams.Exposure != double.Parse(MV_Global_Variable.CameraCalOffsetExposure4))
                        cogAcqCam4.Operator.OwnedExposureParams.Exposure = double.Parse(MV_Global_Variable.CameraCalOffsetExposure4);
                    break;
            }

        }

        void AcqImage(string Product, string BussyCam)
        {
            ChangeExpTime(Product);
            switch (BussyCam)
            {
                case "Cam12":
                    #region 清空显示
                    //cogRecordDisplayCam1.InteractiveGraphics.Clear();
                    //cogRecordDisplayCam1.StaticGraphics.Clear();
                    //cogRecordDisplayCam1.Record = null;
                    //cogRecordDisplayCam2.InteractiveGraphics.Clear();
                    //cogRecordDisplayCam2.StaticGraphics.Clear();
                    //cogRecordDisplayCam2.Record = null;

                    #endregion

                    cogAcqCam1.Run();
                    ImageCam1 = (CogImage8Grey)cogAcqCam1.OutputImage;

                    cogAcqCam2.Run();
                    ImageCam2 = (CogImage8Grey)cogAcqCam2.OutputImage;

                    break;
                case "Cam1":
                    #region 清空显示
                    cogRecordDisplayCam1.InteractiveGraphics.Clear();
                    cogRecordDisplayCam1.StaticGraphics.Clear();
                    cogRecordDisplayCam1.Record = null;

                    #endregion

                    cogAcqCam1.Run();
                    ImageCam1 = (CogImage8Grey)cogAcqCam1.OutputImage;

                    break;
                case "Cam2":
                    #region 清空显示
                    cogRecordDisplayCam2.InteractiveGraphics.Clear();
                    cogRecordDisplayCam2.StaticGraphics.Clear();
                    cogRecordDisplayCam2.Record = null;

                    #endregion

                    cogAcqCam2.Run();
                    ImageCam2 = (CogImage8Grey)cogAcqCam2.OutputImage;
                    break;
                case "Cam34":

                    #region 清空显示
                    //cogRecordDisplayCam3.InteractiveGraphics.Clear();
                    //cogRecordDisplayCam3.StaticGraphics.Clear();
                    //cogRecordDisplayCam3.Record = null;
                    //cogRecordDisplayCam4.InteractiveGraphics.Clear();
                    //cogRecordDisplayCam4.StaticGraphics.Clear();
                    //cogRecordDisplayCam4.Record = null;
                    #endregion

                    cogAcqCam3.Run();
                    ImageCam3 = (CogImage8Grey)cogAcqCam3.OutputImage;
                    cogAcqCam4.Run();
                    ImageCam4 = (CogImage8Grey)cogAcqCam4.OutputImage;

                    break;
                case "Cam3":

                    #region 清空显示
                    cogRecordDisplayCam3.InteractiveGraphics.Clear();
                    cogRecordDisplayCam3.StaticGraphics.Clear();
                    cogRecordDisplayCam3.Record = null;

                    #endregion

                    cogAcqCam3.Run();
                    ImageCam3 = (CogImage8Grey)cogAcqCam3.OutputImage;

                    break;
                case "Cam4":

                    #region 清空显示

                    cogRecordDisplayCam4.InteractiveGraphics.Clear();
                    cogRecordDisplayCam4.StaticGraphics.Clear();
                    cogRecordDisplayCam4.Record = null;
                    #endregion

                    cogAcqCam4.Run();
                    ImageCam4 = (CogImage8Grey)cogAcqCam4.OutputImage;

                    break;
                case "Cam5":

                    #region 清空显示

                    cogRecordDisplayCam5.InteractiveGraphics.Clear();
                    cogRecordDisplayCam5.StaticGraphics.Clear();
                    cogRecordDisplayCam5.Record = null;
                    #endregion

                    // ch: 触发命令 || en: Trigger command

                    if (myHikCameraConn.Grab())
                    {
                        Cam5ActionOver = false;
                        CodeStartTime = DateTime.Now;
                    }
                    else
                    {
                        ErrorLog("读码相机开启失败");
                        string SendMessage = "Cam5-1^0";
                        MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                        MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(MV_Global_Variable.MyFormMain.LightChannel5, 0);
                        RecordOperateCor("SendMessage:" + SendMessage);
                        CavityQRCode = "NA";
                        RunReadCodeSucceed = false;
                        Cam5ActionOver = true;
                        Cam5Lock = false;
                    }
                    break;
            }
        }

        void RunCalibToolBlock(string BussyCam)
        {
            switch (BussyCam)
            {
                case "Cam12":
                    cogCalibCheckerboardToolCam1.InputImage = ImageCam1;
                    cogCalibCheckerboardToolCam1.Run();
                    cogCalibCheckerboardToolCam2.InputImage = ImageCam2;
                    cogCalibCheckerboardToolCam2.Run();

                    cogCalibNPointToNPointToolPosition[1].InputImage = cogCalibCheckerboardToolCam1.OutputImage;
                    cogCalibNPointToNPointToolPosition[1].Run();
                    cogCalibrationImgCam1 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[1].OutputImage;

                    cogCalibNPointToNPointToolPosition[1].InputImage = cogCalibCheckerboardToolCam2.OutputImage;
                    cogCalibNPointToNPointToolPosition[1].Run();
                    cogCalibrationImgCam2 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[1].OutputImage;

                    break;
                case "Cam1":
                    cogCalibCheckerboardToolCam1.InputImage = ImageCam1;
                    cogCalibCheckerboardToolCam1.Run();
                    cogCalibNPointToNPointToolPosition[1].InputImage = cogCalibCheckerboardToolCam1.OutputImage;
                    cogCalibNPointToNPointToolPosition[1].Run();
                    cogCalibrationImgCam1 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[1].OutputImage;

                    break;
                case "Cam2":
                    cogCalibCheckerboardToolCam2.InputImage = ImageCam2;
                    cogCalibCheckerboardToolCam2.Run();
                    cogCalibNPointToNPointToolPosition[1].InputImage = cogCalibCheckerboardToolCam2.OutputImage;
                    cogCalibNPointToNPointToolPosition[1].Run();
                    cogCalibrationImgCam2 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[1].OutputImage;
                    break;
                case "Cam34":
                    cogCalibCheckerboardToolCam3.InputImage = ImageCam3;
                    cogCalibCheckerboardToolCam3.Run();
                    cogCalibCheckerboardToolCam4.InputImage = ImageCam4;
                    cogCalibCheckerboardToolCam4.Run();

                    cogCalibNPointToNPointToolPosition[3].InputImage = cogCalibCheckerboardToolCam3.OutputImage;
                    cogCalibNPointToNPointToolPosition[3].Run();
                    cogCalibrationImgCam3 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[3].OutputImage;

                    cogCalibNPointToNPointToolPosition[3].InputImage = cogCalibCheckerboardToolCam4.OutputImage;
                    cogCalibNPointToNPointToolPosition[3].Run();
                    cogCalibrationImgCam4 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[3].OutputImage;

                    break;
                case "Cam3":
                    cogCalibCheckerboardToolCam3.InputImage = ImageCam3;
                    cogCalibCheckerboardToolCam3.Run();
                    cogCalibNPointToNPointToolPosition[3].InputImage = cogCalibCheckerboardToolCam3.OutputImage;
                    cogCalibNPointToNPointToolPosition[3].Run();
                    cogCalibrationImgCam3 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[3].OutputImage;
                    break;
                case "Cam4":
                    cogCalibCheckerboardToolCam4.InputImage = ImageCam4;
                    cogCalibCheckerboardToolCam4.Run();
                    cogCalibNPointToNPointToolPosition[3].InputImage = cogCalibCheckerboardToolCam4.OutputImage;
                    cogCalibNPointToNPointToolPosition[3].Run();
                    cogCalibrationImgCam4 = (CogImage8Grey)cogCalibNPointToNPointToolPosition[3].OutputImage;
                    break;

            }
        }

        void RunOnlyCalibToolBlock(string BussyCam)
        {
            switch (BussyCam)
            {
                case "Cam12":

                    cogAcqCam1.Run();
                    ImageCam1 = (CogImage8Grey)cogAcqCam1.OutputImage;
                    cogAcqCam2.Run();
                    ImageCam2 = (CogImage8Grey)cogAcqCam2.OutputImage;

                    cogCalibCheckerboardToolCam1.InputImage = ImageCam1;
                    cogCalibCheckerboardToolCam1.Calibration.CalibrationImage = ImageCam1;

                    cogCalibCheckerboardToolCam2.InputImage = ImageCam2;
                    cogCalibCheckerboardToolCam2.Calibration.CalibrationImage = ImageCam2;


                    break;
                case "Cam1":
                    cogAcqCam1.Run();
                    ImageCam1 = (CogImage8Grey)cogAcqCam1.OutputImage;
                    cogCalibCheckerboardToolCam1.InputImage = ImageCam1;
                    cogCalibCheckerboardToolCam1.Run();
                    break;
                case "Cam2":
                    cogAcqCam2.Run();
                    ImageCam2 = (CogImage8Grey)cogAcqCam2.OutputImage;
                    cogCalibCheckerboardToolCam2.InputImage = ImageCam2;
                    cogCalibCheckerboardToolCam2.Run();

                    break;
                case "Cam34":
                    cogAcqCam3.Run();
                    ImageCam3 = (CogImage8Grey)cogAcqCam3.OutputImage;
                    cogAcqCam4.Run();
                    ImageCam4 = (CogImage8Grey)cogAcqCam4.OutputImage;
                    cogCalibCheckerboardToolCam3.InputImage = ImageCam3;
                    cogCalibCheckerboardToolCam3.Calibration.CalibrationImage = ImageCam3;

                    cogCalibCheckerboardToolCam4.InputImage = ImageCam4;
                    cogCalibCheckerboardToolCam4.Calibration.CalibrationImage = ImageCam4;


                    break;
                case "Cam3":
                    cogAcqCam3.Run();
                    ImageCam3 = (CogImage8Grey)cogAcqCam3.OutputImage;

                    cogCalibCheckerboardToolCam3.InputImage = ImageCam3;
                    cogCalibCheckerboardToolCam3.Run();

                    break;
                case "Cam4":
                    cogAcqCam4.Run();
                    ImageCam4 = (CogImage8Grey)cogAcqCam4.OutputImage;
                    cogCalibCheckerboardToolCam4.InputImage = ImageCam4;
                    cogCalibCheckerboardToolCam4.Run();

                    break;

            }
        }

        void RunOnlyNptsToolBlock(string BussyCam)
        {
            switch (BussyCam)
            {
                case "Cam12":

                    cogAcqCam1.Run();
                    ImageCam1 = (CogImage8Grey)cogAcqCam1.OutputImage;
                    cogAcqCam2.Run();
                    ImageCam2 = (CogImage8Grey)cogAcqCam2.OutputImage;

                    cogCalibCheckerboardToolCam1.InputImage = ImageCam1;
                    cogCalibCheckerboardToolCam1.Run();

                    cogCalibCheckerboardToolCam2.InputImage = ImageCam2;
                    cogCalibCheckerboardToolCam2.Run();


                    break;
                case "Cam1":
                    cogAcqCam1.Run();
                    ImageCam1 = (CogImage8Grey)cogAcqCam1.OutputImage;
                    cogCalibCheckerboardToolCam1.InputImage = ImageCam1;
                    cogCalibCheckerboardToolCam1.Run();
                    break;
                case "Cam2":
                    cogAcqCam2.Run();
                    ImageCam2 = (CogImage8Grey)cogAcqCam2.OutputImage;
                    cogCalibCheckerboardToolCam2.InputImage = ImageCam2;
                    cogCalibCheckerboardToolCam2.Run();

                    break;
                case "Cam34":
                    cogAcqCam3.Run();
                    ImageCam3 = (CogImage8Grey)cogAcqCam3.OutputImage;
                    cogAcqCam4.Run();
                    ImageCam4 = (CogImage8Grey)cogAcqCam4.OutputImage;
                    cogCalibCheckerboardToolCam3.InputImage = ImageCam3;
                    cogCalibCheckerboardToolCam3.Run();

                    cogCalibCheckerboardToolCam4.InputImage = ImageCam4;
                    cogCalibCheckerboardToolCam4.Run();


                    break;
                case "Cam3":
                    cogAcqCam3.Run();
                    ImageCam3 = (CogImage8Grey)cogAcqCam3.OutputImage;

                    cogCalibCheckerboardToolCam3.InputImage = ImageCam3;
                    cogCalibCheckerboardToolCam3.Run();

                    break;
                case "Cam4":
                    cogAcqCam4.Run();
                    ImageCam4 = (CogImage8Grey)cogAcqCam4.OutputImage;
                    cogCalibCheckerboardToolCam4.InputImage = ImageCam4;
                    cogCalibCheckerboardToolCam4.Run();

                    break;

            }
        }

        void RunNPointCalibrationToolBlock(string BussyCam)
        {
            switch (BussyCam)
            {
                case "Cam12":
                    cogNPointCalibrationTool.Inputs["Img"].Value = cogCalibCheckerboardToolCam1.OutputImage;
                    cogNPointCalibrationTool.Run();
                    if (cogNPointCalibrationTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        cogRecordDisplayCam1.Image = cogCalibCheckerboardToolCam1.OutputImage;
                        cogRecordDisplayCam1.Record = cogNPointCalibrationTool.CreateLastRunRecord().SubRecords[0];

                        //把像素值和机器人的坐标 赋值给标定工具
                        cogCalibNPointToNPointToolPosition[1].InputImage = cogCalibCheckerboardToolCam1.OutputImage;

                        //输入相机像素值
                        cogCalibNPointToNPointToolPosition[1].Calibration.SetUncalibratedPointX(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["X"].Value)/*相机拍照的值*/);
                        cogCalibNPointToNPointToolPosition[1].Calibration.SetUncalibratedPointY(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["Y"].Value)/*相机拍照的值*/);

                        //输入机器人的值
                        cogCalibNPointToNPointToolPosition[1].Calibration.SetRawCalibratedPointX(NPtsStep, AxisX);
                        cogCalibNPointToNPointToolPosition[1].Calibration.SetRawCalibratedPointY(NPtsStep, AxisY);
                        if (NPtsStep == 8)
                        {
                            cogCalibNPointToNPointToolPosition[1].CalibrationImage = cogCalibCheckerboardToolCam1.OutputImage;
                            cogCalibNPointToNPointToolPosition[1].Calibration.Calibrate();
                            CogSerializer.SaveObjectToFile(cogCalibNPointToNPointToolPosition[1], CalibPath + "cogCalibNPToolCam1.vpp");
                            cogCalibNPointToNPointToolPosition[1] = (CogCalibNPointToNPointTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibNPToolCam1.vpp");
                        }
                        string message = "Cam12Calib^1";
                        MySubThreadFlow.MyServerSocket.SendAll(message);
                    }
                    else
                    {
                        string message = "Cam12Calib^0";
                        MySubThreadFlow.MyServerSocket.SendAll(message);
                    }
                    break;
                case "Cam2":
                    cogRecordDisplayCam2.Image = cogCalibCheckerboardToolCam2.OutputImage;
                    cogNPointCalibrationTool.Inputs["Img"].Value = cogCalibCheckerboardToolCam2.OutputImage;
                    cogNPointCalibrationTool.Run();
                    if (cogNPointCalibrationTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        cogRecordDisplayCam2.Record = cogNPointCalibrationTool.CreateLastRunRecord();
                        //把像素值和机器人的坐标 赋值给标定工具
                        cogCalibNPointToNPointToolPosition[2].InputImage = cogCalibCheckerboardToolCam2.OutputImage;

                        //输入相机像素值
                        cogCalibNPointToNPointToolPosition[2].Calibration.SetUncalibratedPointX(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["X"].Value)/*相机拍照的值*/);
                        cogCalibNPointToNPointToolPosition[2].Calibration.SetUncalibratedPointY(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["Y"].Value)/*相机拍照的值*/);

                        //输入机器人的值
                        cogCalibNPointToNPointToolPosition[2].Calibration.SetRawCalibratedPointX(NPtsStep, AxisX);
                        cogCalibNPointToNPointToolPosition[2].Calibration.SetRawCalibratedPointY(NPtsStep, AxisY);
                    }
                    break;
                case "Cam34":
                    cogRecordDisplayCam3.Image = cogCalibCheckerboardToolCam3.OutputImage;
                    cogNPointCalibrationTool.Inputs["Img"].Value = cogCalibCheckerboardToolCam3.OutputImage;
                    cogNPointCalibrationTool.Run();
                    if (cogNPointCalibrationTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        cogRecordDisplayCam3.Record = cogNPointCalibrationTool.CreateLastRunRecord().SubRecords[0];
                        cogRecordDisplayCam3.Image = cogCalibCheckerboardToolCam3.OutputImage;
                        //把像素值和机器人的坐标 赋值给标定工具
                        cogCalibNPointToNPointToolPosition[3].InputImage = cogCalibCheckerboardToolCam3.OutputImage;

                        //输入相机像素值
                        cogCalibNPointToNPointToolPosition[3].Calibration.SetUncalibratedPointX(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["X"].Value)/*相机拍照的值*/);
                        cogCalibNPointToNPointToolPosition[3].Calibration.SetUncalibratedPointY(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["Y"].Value)/*相机拍照的值*/);

                        //输入机器人的值
                        cogCalibNPointToNPointToolPosition[3].Calibration.SetRawCalibratedPointX(NPtsStep, AxisX);
                        cogCalibNPointToNPointToolPosition[3].Calibration.SetRawCalibratedPointY(NPtsStep, AxisY);

                        if (NPtsStep == 8)
                        {
                            cogCalibNPointToNPointToolPosition[3].CalibrationImage = cogCalibCheckerboardToolCam1.OutputImage;
                            cogCalibNPointToNPointToolPosition[3].Calibration.Calibrate();
                            CogSerializer.SaveObjectToFile(cogCalibNPointToNPointToolPosition[3], CalibPath + "cogCalibNPToolCam3.vpp");
                            cogCalibNPointToNPointToolPosition[3] = (CogCalibNPointToNPointTool)CogSerializer.LoadObjectFromFile(CalibPath + "cogCalibNPToolCam3.vpp");
                        }
                        string message = "Cam34Calib^1";
                        MySubThreadFlow.MyServerSocket.SendAll(message);
                    }
                    else
                    {
                        string message = "Cam34Calib^0";
                        MySubThreadFlow.MyServerSocket.SendAll(message);
                    }
                    break;
                case "Cam4":
                    cogRecordDisplayCam4.Image = cogCalibCheckerboardToolCam4.OutputImage;
                    cogNPointCalibrationTool.Inputs["Img"].Value = cogCalibCheckerboardToolCam4.OutputImage;
                    cogNPointCalibrationTool.Run();
                    if (cogNPointCalibrationTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        cogRecordDisplayCam4.Record = cogNPointCalibrationTool.CreateLastRunRecord();
                        //把像素值和机器人的坐标 赋值给标定工具
                        cogCalibNPointToNPointToolPosition[4].InputImage = cogCalibCheckerboardToolCam4.OutputImage;

                        //输入相机像素值
                        cogCalibNPointToNPointToolPosition[4].Calibration.SetUncalibratedPointX(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["X"].Value)/*相机拍照的值*/);
                        cogCalibNPointToNPointToolPosition[4].Calibration.SetUncalibratedPointY(NPtsStep, Convert.ToDouble(cogNPointCalibrationTool.Outputs["Y"].Value)/*相机拍照的值*/);

                        //输入机器人的值
                        cogCalibNPointToNPointToolPosition[4].Calibration.SetRawCalibratedPointX(NPtsStep, AxisX);
                        cogCalibNPointToNPointToolPosition[4].Calibration.SetRawCalibratedPointY(NPtsStep, AxisY);
                    }
                    break;

            }
        }

        /// <summary>
        /// 拍照分析步骤
        /// </summary>
        /// <param name="Product">产品</param>
        /// <param name="BussyCam">相机</param>
        void RunPositionToolBlock(string Product, string BussyCam)
        {
            switch (Product)
            {
                case "Battery":
                    EnableCavity2 = false;
                    switch (BussyCam)
                    {
                        case "Cam12":
                            cogRunToolBlockCam12.Inputs["Cam1"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam12.Inputs["Cam2"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam1":
                            cogRunToolBlockCam12.Inputs["Cam1"].Value = cogCalibrationImgCam1;
                            //cogRunToolBlockCam12.Inputs["cam2Img"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam2":
                            //cogRunToolBlockCam12.Inputs["cam1Img"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam12.Inputs["Cam2"].Value = cogCalibrationImgCam2;
                            break;
                    }
                    cogRecordDisplayCam1.InteractiveGraphics.Clear();
                    cogRecordDisplayCam1.StaticGraphics.Clear();
                    cogRecordDisplayCam1.Record = null;
                    cogRecordDisplayCam2.InteractiveGraphics.Clear();
                    cogRecordDisplayCam2.StaticGraphics.Clear();
                    cogRecordDisplayCam2.Record = null;
                    cogRunToolBlockCam12.Run();
                    cogRecordDisplayCam1.Image = cogCalibrationImgCam1;
                    cogRecordDisplayCam2.Image = cogCalibrationImgCam2;

                    if (cogRunToolBlockCam12.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed = true;
                        try
                        {
                            cogRecordDisplayCam1.Record = cogRunToolBlockCam12.CreateLastRunRecord().SubRecords[0];
                            cogRecordDisplayCam2.Record = cogRunToolBlockCam12.CreateLastRunRecord().SubRecords[2];

                            BatteryTLX = (double)cogRunToolBlockCam12.Outputs["BatteryTLX"].Value - 330;
                            BatteryTLY = (double)cogRunToolBlockCam12.Outputs["BatteryTLY"].Value + 595;
                            BatteryTRX = (double)cogRunToolBlockCam12.Outputs["BatteryTRX"].Value - 330;
                            BatteryTRY = (double)cogRunToolBlockCam12.Outputs["BatteryTRY"].Value + 595;
                            BatteryBLX = (double)cogRunToolBlockCam12.Outputs["BatteryBLX"].Value - 330;
                            BatteryBLY = (double)cogRunToolBlockCam12.Outputs["BatteryBLY"].Value + 595;
                            BatteryBRX = (double)cogRunToolBlockCam12.Outputs["BatteryBRX"].Value - 330;
                            BatteryBRY = (double)cogRunToolBlockCam12.Outputs["BatteryBRY"].Value + 595;


                            BatteryWidth = (double)cogRunToolBlockCam12.Outputs["BatteryTopDistance"].Value;
                            BatteryHeight = (double)cogRunToolBlockCam12.Outputs["BatteryLeftDistance"].Value;
                            BatteryWidth2 = (double)cogRunToolBlockCam12.Outputs["BatteryBottomDistance"].Value;
                            BatteryHeight2 = (double)cogRunToolBlockCam12.Outputs["BatteryRightDistance"].Value;

                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 10, "BatteryWidth:" + BatteryWidth.ToString("0.00"), CogColorConstants.Green);
                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 200, "BatteryHeight:" + BatteryHeight.ToString("0.00"), CogColorConstants.Green);
                        }
                        catch
                        { RunSucceed = false; }
                    }
                    else
                    {
                        SaveNGImage(todayImagePath + "cam1\\Battery1", cogCalibrationImgCam1);
                        SaveNGImage(todayImagePath + "cam2\\Battery1", cogCalibrationImgCam2);
                        RunSucceed = false;
                    }
                    try
                    {
                        SaveBitmapRunBattery1cam1 = (Bitmap)cogRecordDisplayCam1.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        SaveBitmapRunBattery1cam2 = (Bitmap)cogRecordDisplayCam2.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        BatteryGImgSaveOK = true;
                    }
                    catch
                    {
                        BatteryGImgSaveOK = false;
                    }
                    SaveImage(todayImagePath + "cam1\\", cogCalibrationImgCam1);
                    SaveImage(todayImagePath + "cam2\\", cogCalibrationImgCam2);
                    break;
                case "Battery2":
                    EnableCavity2 = true;
                    switch (BussyCam)
                    {
                        case "Cam12":
                            cogRunToolBlockCam12BigBattery.Inputs["Cam1"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam12BigBattery.Inputs["Cam2"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam1":
                            cogRunToolBlockCam12BigBattery.Inputs["Cam1"].Value = cogCalibrationImgCam1;
                            //cogRunToolBlockCam12.Inputs["cam2Img"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam2":
                            //cogRunToolBlockCam12.Inputs["cam1Img"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam12BigBattery.Inputs["Cam2"].Value = cogCalibrationImgCam2;
                            break;
                    }
                    cogRecordDisplayCam1.InteractiveGraphics.Clear();
                    cogRecordDisplayCam1.StaticGraphics.Clear();
                    cogRecordDisplayCam1.Record = null;
                    cogRecordDisplayCam2.InteractiveGraphics.Clear();
                    cogRecordDisplayCam2.StaticGraphics.Clear();
                    cogRecordDisplayCam2.Record = null;
                    cogRunToolBlockCam12BigBattery.Run();
                    cogRecordDisplayCam1.Image = cogCalibrationImgCam1;
                    cogRecordDisplayCam2.Image = cogCalibrationImgCam2;

                    if (cogRunToolBlockCam12BigBattery.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed = true;
                        try
                        {
                            cogRecordDisplayCam1.Record = cogRunToolBlockCam12BigBattery.CreateLastRunRecord().SubRecords[0];
                            cogRecordDisplayCam2.Record = cogRunToolBlockCam12BigBattery.CreateLastRunRecord().SubRecords[2];

                            BatteryTLX = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryTLX"].Value - 330;
                            BatteryTLY = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryTLY"].Value + 595;
                            BatteryTRX = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryTRX"].Value - 330;
                            BatteryTRY = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryTRY"].Value + 595;
                            BatteryBLX = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryBLX"].Value - 330;
                            BatteryBLY = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryBLY"].Value + 595;
                            BatteryBRX = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryBRX"].Value - 330;
                            BatteryBRY = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryBRY"].Value + 595;


                            BatteryWidth = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryTopDistance"].Value;
                            BatteryHeight = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryLeftDistance"].Value;
                            BatteryWidth2 = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryBottomDistance"].Value;
                            BatteryHeight2 = (double)cogRunToolBlockCam12BigBattery.Outputs["BatteryRightDistance"].Value;


                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 10, "BatteryWidth:" + BatteryWidth.ToString("0.00"), CogColorConstants.Green);
                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 200, "BatteryHeight:" + BatteryHeight.ToString("0.00"), CogColorConstants.Green);
                        }
                        catch
                        { RunSucceed = false; }
                    }
                    else
                    {
                        SaveNGImage(todayImagePath + "cam1\\Battery2", cogCalibrationImgCam1);
                        SaveNGImage(todayImagePath + "cam2\\Battery2", cogCalibrationImgCam2);
                        RunSucceed = false;
                    }
                    try
                    {
                        SaveBitmapRunBattery1cam1 = (Bitmap)cogRecordDisplayCam1.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        SaveBitmapRunBattery1cam2 = (Bitmap)cogRecordDisplayCam2.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        BatteryGImgSaveOK = true;
                    }
                    catch
                    {
                        BatteryGImgSaveOK = false;
                    }
                    SaveImage(todayImagePath + "cam1\\", cogCalibrationImgCam1);
                    SaveImage(todayImagePath + "cam2\\", cogCalibrationImgCam2);

                    break;
                case "Mark":
                    switch (BussyCam)
                    {
                        case "Cam12":
                            cogRunToolBlockCam1QRCode.Inputs["Cam1"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam1QRCode.Inputs["Cam2"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam1":
                            cogRunToolBlockCam1QRCode.Inputs["Cam1"].Value = cogCalibrationImgCam1;
                            //cogRunToolBlockCam12.Inputs["cam2Img"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam2":
                            //cogRunToolBlockCam12.Inputs["cam1Img"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam1QRCode.Inputs["Cam2"].Value = cogCalibrationImgCam2;
                            break;
                    }
                    cogRecordDisplayCam1.InteractiveGraphics.Clear();
                    cogRecordDisplayCam1.StaticGraphics.Clear();
                    cogRecordDisplayCam1.Record = null;
                    cogRecordDisplayCam2.InteractiveGraphics.Clear();
                    cogRecordDisplayCam2.StaticGraphics.Clear();
                    cogRecordDisplayCam2.Record = null;
                    //cogRunToolBlockCam1QRCode.Inputs["LineUp"].Value = cogRunToolBlockCam12.Outputs["BatteryLineUp"].Value;
                    cogRunToolBlockCam1QRCode.Run();
                    cogRecordDisplayCam1.Image = cogCalibrationImgCam1;
                    cogRecordDisplayCam2.Image = cogCalibrationImgCam2;
                    SaveImage(todayImagePath + "cam1\\", cogCalibrationImgCam1);
                    SaveImage(todayImagePath + "cam2\\", cogCalibrationImgCam2);
                    if (((double)cogRunToolBlockCam1QRCode.Outputs["Find1Count"].Value == 1
                                && (double)cogRunToolBlockCam1QRCode.Outputs["Find2Count"].Value == 1) &&
                                ((double)cogRunToolBlockCam1QRCode.Outputs["QRCount"].Value >= 1 || (double)cogRunToolBlockCam1QRCode.Outputs["QRCount2"].Value >= 1))
                    {
                        RunSucceed = true;
                        cogRecordDisplayCam1.Record = cogRunToolBlockCam1QRCode.CreateLastRunRecord().SubRecords[0];
                        // cogRecordDisplayCam1.Record = cogRunToolBlockCam1QRCode.CreateLastRunRecord().SubRecords[1];
                        if (Cam12StepStr != null)
                        {
                            if (Cam12StepStr[1] == "1")
                            {
                                if (MV_Global_Variable.Battery1BarcodeCam == 0)
                                { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString"].Value.ToString(); }
                                else if (MV_Global_Variable.Battery1BarcodeCam == 1)
                                { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString2"].Value.ToString(); }
                            }
                            if (Cam12StepStr[1] == "2")
                            {
                                if (MV_Global_Variable.Battery2BarcodeCam == 0)
                                { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString"].Value.ToString(); }
                                else if (MV_Global_Variable.Battery2BarcodeCam == 1)
                                { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString2"].Value.ToString(); }
                            }
                        }
                        else
                        {
                            BatteryQRCode = "Error Step";
                        }
                        BatteryHandMarkX1 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkX1"].Value - 330;
                        BatteryHandMarkY1 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkY1"].Value + 595;
                        BatteryHandMarkX2 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkX2"].Value - 330;
                        BatteryHandMarkY2 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkY2"].Value + 595;
                        ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1, "OK,BarCode:" + BatteryQRCode, CogColorConstants.Green);
                        ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 400, "Mark X1:" + BatteryHandMarkX1.ToString("0.00"), CogColorConstants.Green);
                        ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 800, "Mark Y1:" + BatteryHandMarkY1.ToString("0.00"), CogColorConstants.Green);
                        ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1200, "Mark X2:" + BatteryHandMarkX2.ToString("0.00"), CogColorConstants.Green);
                        ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1600, "Mark Y2:" + BatteryHandMarkY2.ToString("0.00"), CogColorConstants.Green);

                    }
                    else
                    {
                        try
                        {
                            if ((double)cogRunToolBlockCam1QRCode.Outputs["QRCount"].Value >= 1 || (double)cogRunToolBlockCam1QRCode.Outputs["QRCount2"].Value >= 1)
                            {
                                if (Cam12StepStr[1] == "1")
                                {
                                    if (MV_Global_Variable.Battery1BarcodeCam == 0)
                                    { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString"].Value.ToString(); }
                                    else if (MV_Global_Variable.Battery1BarcodeCam == 1)
                                    { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString2"].Value.ToString(); }
                                }
                                if (Cam12StepStr[1] == "2")
                                {
                                    if (MV_Global_Variable.Battery2BarcodeCam == 0)
                                    { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString"].Value.ToString(); }
                                    else if (MV_Global_Variable.Battery2BarcodeCam == 1)
                                    { BatteryQRCode = cogRunToolBlockCam1QRCode.Outputs["QRCodeString2"].Value.ToString(); }
                                }
                            }
                            else
                            {
                                BatteryQRCode = "NA";
                            }
                            if ((double)cogRunToolBlockCam1QRCode.Outputs["Find1Count"].Value == 1
                                && (double)cogRunToolBlockCam1QRCode.Outputs["Find2Count"].Value == 1)
                            {
                                BatteryHandMarkX1 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkX1"].Value - 330;
                                BatteryHandMarkY1 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkY1"].Value + 595;
                                BatteryHandMarkX2 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkX2"].Value - 330;
                                BatteryHandMarkY2 = (double)cogRunToolBlockCam1QRCode.Outputs["HandMarkY2"].Value + 595;

                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 400, "Mark X1:" + BatteryHandMarkX1.ToString("0.00"), CogColorConstants.Green);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 800, "Mark Y1:" + BatteryHandMarkY1.ToString("0.00"), CogColorConstants.Green);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1200, "Mark X2:" + BatteryHandMarkX2.ToString("0.00"), CogColorConstants.Green);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1600, "Mark Y2:" + BatteryHandMarkY2.ToString("0.00"), CogColorConstants.Green);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1, "NG,BarCode:" + BatteryQRCode, CogColorConstants.Red);

                                RunSucceed = true;
                            }
                            else
                            {
                                SaveNGImage(todayImagePath + "cam1\\Mark", cogCalibrationImgCam1);
                                SaveNGImage(todayImagePath + "cam2\\Mark", cogCalibrationImgCam2);

                                BatteryHandMarkX1 = 0;
                                BatteryHandMarkY1 = 0;
                                BatteryHandMarkX2 = 0;
                                BatteryHandMarkY2 = 0;
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 400, "Mark X1:" + BatteryHandMarkX1.ToString("0.00"), CogColorConstants.Red);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 800, "Mark Y1:" + BatteryHandMarkY1.ToString("0.00"), CogColorConstants.Red);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1200, "Mark X2:" + BatteryHandMarkX2.ToString("0.00"), CogColorConstants.Red);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1600, "Mark Y2:" + BatteryHandMarkY2.ToString("0.00"), CogColorConstants.Red);
                                ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1, "NG,BarCode:" + BatteryQRCode, CogColorConstants.Red);
                                RunSucceed = false;
                            }
                        }
                        catch
                        {
                            BatteryHandMarkX1 = 0;
                            BatteryHandMarkY1 = 0;
                            BatteryHandMarkX2 = 0;
                            BatteryHandMarkY2 = 0;
                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 400, "Mark X1:" + BatteryHandMarkX1.ToString("0.00"), CogColorConstants.Red);
                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 800, "Mark Y1:" + BatteryHandMarkY1.ToString("0.00"), CogColorConstants.Red);
                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1200, "Mark X2:" + BatteryHandMarkX2.ToString("0.00"), CogColorConstants.Red);
                            ShowLabelInfo(cogRecordDisplayCam1, cogLabelCam1, 10, 1600, "Mark Y2:" + BatteryHandMarkY2.ToString("0.00"), CogColorConstants.Red);
                            ShowLabelInfo(cogRecordDisplayCam2, cogLabelCam1, 10, 1, "NG,BarCode:" + BatteryQRCode, CogColorConstants.Red);
                            RunSucceed = false;
                        }
                    }
                    if (BatteryGImgSaveOK)
                    {
                        SaveGImage(todayImagePath + "AnalysisCam1\\", BatteryQRCode, SaveBitmapRunBattery1cam1);
                        SaveGImage(todayImagePath + "AnalysisCam2\\", BatteryQRCode, SaveBitmapRunBattery1cam2);
                    }
                    break;
                case "Cavity":
                    ;
                    switch (BussyCam)
                    {
                        case "Cam34":
                            cogRunToolBlockCam34.Inputs["Cam3"].Value = cogCalibrationImgCam3;
                            cogRunToolBlockCam34.Inputs["Cam4"].Value = cogCalibrationImgCam4;

                            break;
                        case "Cam3":
                            cogRunToolBlockCam34.Inputs["Cam3"].Value = cogCalibrationImgCam3;
                            //cogRunToolBlockCam12.Inputs["cam2Img"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam4":
                            //cogRunToolBlockCam12.Inputs["cam1Img"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam34.Inputs["Cam4"].Value = cogCalibrationImgCam4;
                            break;
                    }
                    cogRecordDisplayCam3.InteractiveGraphics.Clear();
                    cogRecordDisplayCam3.StaticGraphics.Clear();
                    cogRecordDisplayCam3.Record = null;
                    cogRecordDisplayCam4.InteractiveGraphics.Clear();
                    cogRecordDisplayCam4.StaticGraphics.Clear();
                    cogRecordDisplayCam4.Record = null;
                    cogRunToolBlockCam34.Run();
                    cogRecordDisplayCam3.Image = cogCalibrationImgCam3;
                    cogRecordDisplayCam4.Image = cogCalibrationImgCam4;
                    SaveImage(todayImagePath + "cam3\\", cogCalibrationImgCam3);
                    SaveImage(todayImagePath + "cam4\\", cogCalibrationImgCam4);
                    if (cogRunToolBlockCam34.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed2 = true;
                        try
                        {

                            CavityTLX = (double)cogRunToolBlockCam34.Outputs["CavityTLX"].Value;
                            CavityTLY = (double)cogRunToolBlockCam34.Outputs["CavityTLY"].Value;
                            CavityTRX = (double)cogRunToolBlockCam34.Outputs["CavityTRX"].Value;
                            CavityTRY = (double)cogRunToolBlockCam34.Outputs["CavityTRY"].Value;
                            CavityBLX = (double)cogRunToolBlockCam34.Outputs["CavityBLX"].Value;
                            CavityBLY = (double)cogRunToolBlockCam34.Outputs["CavityBLY"].Value;
                            CavityBRX = (double)cogRunToolBlockCam34.Outputs["CavityBRX"].Value;
                            CavityBRY = (double)cogRunToolBlockCam34.Outputs["CavityBRY"].Value;

                            CavityWidth = (double)cogRunToolBlockCam34.Outputs["TopDistance"].Value;
                            CavityHeight = (double)cogRunToolBlockCam34.Outputs["LeftDistance"].Value;
                            CavityWidth2 = (double)cogRunToolBlockCam34.Outputs["BottomDistance"].Value;
                            CavityHeight2 = (double)cogRunToolBlockCam34.Outputs["RightDistance"].Value;
                            CavityBarcode = (string)cogRunToolBlockCam34.Outputs["CavityBarcode"].Value;

                            cogRecordDisplayCam3.Record = cogRunToolBlockCam34.CreateLastRunRecord().SubRecords[0];
                            cogRecordDisplayCam4.Record = cogRunToolBlockCam34.CreateLastRunRecord().SubRecords[2];
                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1, "CavityWidth:" + CavityWidth.ToString("0.00"), CogColorConstants.Green);
                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 400, "CavityHeight:" + CavityHeight.ToString("0.00"), CogColorConstants.Green);

                        }
                        catch { }

                    }
                    else
                    {
                        SaveNGImage(todayImagePath + "cam3\\Cavity1", cogCalibrationImgCam3);
                        SaveNGImage(todayImagePath + "cam4\\Cavity1", cogCalibrationImgCam4);

                        RunSucceed2 = false;
                        try
                        {
                            CavityTLX = (double)cogRunToolBlockCam34.Outputs["CavityTLX"].Value;
                            CavityTLY = (double)cogRunToolBlockCam34.Outputs["CavityTLY"].Value;
                            CavityTRX = (double)cogRunToolBlockCam34.Outputs["CavityTRX"].Value;
                            CavityTRY = (double)cogRunToolBlockCam34.Outputs["CavityTRY"].Value;
                            CavityBLX = (double)cogRunToolBlockCam34.Outputs["CavityBLX"].Value;
                            CavityBLY = (double)cogRunToolBlockCam34.Outputs["CavityBLY"].Value;
                            CavityBRX = (double)cogRunToolBlockCam34.Outputs["CavityBRX"].Value;
                            CavityBRY = (double)cogRunToolBlockCam34.Outputs["CavityBRY"].Value;

                            CavityWidth = (double)cogRunToolBlockCam34.Outputs["TopDistance"].Value;
                            CavityHeight = (double)cogRunToolBlockCam34.Outputs["LeftDistance"].Value;
                            CavityWidth2 = (double)cogRunToolBlockCam34.Outputs["BottomDistance"].Value;
                            CavityHeight2 = (double)cogRunToolBlockCam34.Outputs["RightDistance"].Value;
                            CavityBarcode = "NA";

                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1, "CavityWidth:" + CavityWidth.ToString("0.00"), CogColorConstants.Red);
                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 400, "CavityHeight:" + CavityHeight.ToString("0.00"), CogColorConstants.Red);
                        }
                        catch
                        {

                        }
                    }
                    try
                    {
                        SaveBitmapRunCavity1cam1 = (Bitmap)cogRecordDisplayCam3.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        SaveBitmapRunCavity1cam2 = (Bitmap)cogRecordDisplayCam4.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        SaveGImage(todayImagePath + "AnalysisCam3\\", string.Format("cavity1_{0}", BatteryQRCode), SaveBitmapRunCavity1cam1);
                        SaveGImage(todayImagePath + "AnalysisCam4\\", string.Format("cavity1_{0}", BatteryQRCode), SaveBitmapRunCavity1cam2);
                        CavityGImgSaveOK = true;
                    }
                    catch
                    {
                        CavityGImgSaveOK = false;
                    }

                    break;
                case "Cavity2":
                    switch (BussyCam)
                    {
                        case "Cam34":
                            cogRunToolBlockCam34Cavity2.Inputs["Cam3"].Value = cogCalibrationImgCam3;
                            cogRunToolBlockCam34Cavity2.Inputs["Cam4"].Value = cogCalibrationImgCam4;
                            break;
                        case "Cam3":
                            cogRunToolBlockCam34Cavity2.Inputs["Cam3"].Value = cogCalibrationImgCam3;
                            //cogRunToolBlockCam12.Inputs["cam2Img"].Value = cogCalibrationImgCam2;
                            break;
                        case "Cam4":
                            //cogRunToolBlockCam12.Inputs["cam1Img"].Value = cogCalibrationImgCam1;
                            cogRunToolBlockCam34Cavity2.Inputs["Cam4"].Value = cogCalibrationImgCam4;
                            break;
                    }
                    cogRecordDisplayCam3.InteractiveGraphics.Clear();
                    cogRecordDisplayCam3.StaticGraphics.Clear();
                    cogRecordDisplayCam3.Record = null;
                    cogRecordDisplayCam4.InteractiveGraphics.Clear();
                    cogRecordDisplayCam4.StaticGraphics.Clear();
                    cogRecordDisplayCam4.Record = null;
                    cogRunToolBlockCam34Cavity2.Run();
                    cogRecordDisplayCam3.Image = cogCalibrationImgCam3;
                    cogRecordDisplayCam4.Image = cogCalibrationImgCam4;


                    if (cogRunToolBlockCam34Cavity2.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed2 = true;
                        try
                        {
                            cogRecordDisplayCam3.Record = cogRunToolBlockCam34Cavity2.CreateLastRunRecord().SubRecords[0];
                            cogRecordDisplayCam4.Record = cogRunToolBlockCam34Cavity2.CreateLastRunRecord().SubRecords[2];
                            Cavity2TLX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTLX"].Value;
                            Cavity2TLY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTLY"].Value;
                            Cavity2TRX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTRX"].Value;
                            Cavity2TRY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTRY"].Value;
                            Cavity2BLX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBLX"].Value;
                            Cavity2BLY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBLY"].Value;
                            Cavity2BRX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBRX"].Value;
                            Cavity2BRY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBRY"].Value;

                            CavityWidth = (double)cogRunToolBlockCam34Cavity2.Outputs["TopDistance"].Value;
                            CavityHeight = (double)cogRunToolBlockCam34Cavity2.Outputs["LeftDistance"].Value;
                            CavityWidth2 = (double)cogRunToolBlockCam34Cavity2.Outputs["BottomDistance"].Value;
                            CavityHeight2 = (double)cogRunToolBlockCam34Cavity2.Outputs["RightDistance"].Value;

                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1, "CavityWidth:" + CavityWidth.ToString("0.00"), CogColorConstants.Green);
                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 400, "CavityHeight:" + CavityHeight.ToString("0.00"), CogColorConstants.Green);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        SaveNGImage(todayImagePath + "cam3\\Cavity2", cogCalibrationImgCam3);
                        SaveNGImage(todayImagePath + "cam4\\Cavity2", cogCalibrationImgCam4);
                        RunSucceed2 = false;
                        try
                        {
                            Cavity2TLX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTLX"].Value;
                            Cavity2TLY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTLY"].Value;
                            Cavity2TRX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTRX"].Value;
                            Cavity2TRY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityTRY"].Value;
                            Cavity2BLX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBLX"].Value;
                            Cavity2BLY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBLY"].Value;
                            Cavity2BRX = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBRX"].Value;
                            Cavity2BRY = (double)cogRunToolBlockCam34Cavity2.Outputs["CavityBRY"].Value;

                            CavityWidth = (double)cogRunToolBlockCam34Cavity2.Outputs["TopDistance"].Value;
                            CavityHeight = (double)cogRunToolBlockCam34Cavity2.Outputs["LeftDistance"].Value;
                            CavityWidth2 = (double)cogRunToolBlockCam34Cavity2.Outputs["BottomDistance"].Value;
                            CavityHeight2 = (double)cogRunToolBlockCam34Cavity2.Outputs["RightDistance"].Value;

                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1, "CavityWidth:" + CavityWidth.ToString("0.00"), CogColorConstants.Red);
                            ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 400, "CavityHeight:" + CavityHeight.ToString("0.00"), CogColorConstants.Red);
                        }
                        catch
                        {

                        }
                    }
                    try
                    {
                        SaveImage(todayImagePath + "cam3\\", cogCalibrationImgCam3);
                        SaveImage(todayImagePath + "cam4\\", cogCalibrationImgCam4);
                        SaveBitmapRunCavity2cam1 = (Bitmap)cogRecordDisplayCam3.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        SaveBitmapRunCavity2cam2 = (Bitmap)cogRecordDisplayCam4.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                        SaveGImage(todayImagePath + "AnalysisCam3\\", string.Format("cavity2_{0}", BatteryQRCode), SaveBitmapRunCavity2cam1);
                        SaveGImage(todayImagePath + "AnalysisCam4\\", string.Format("cavity2_{0}", BatteryQRCode), SaveBitmapRunCavity2cam2);
                    }
                    catch { }
                    break;
                case "CavityCheck":

                    cogRunToolBlockCam34Check.Inputs["Cam3"].Value = cogCalibrationImgCam3;
                    cogRunToolBlockCam34Check.Inputs["Cam4"].Value = cogCalibrationImgCam4;
                    cogRunToolBlockCam34Cal.Inputs["Cam3"].Value = cogCalibrationImgCam3;

                    cogRunToolBlockCam34Check.Inputs["InputHandMarkX1"].Value = BatteryHandMarkX1;
                    cogRunToolBlockCam34Check.Inputs["InputHandMarkX2"].Value = BatteryHandMarkX2;
                    cogRunToolBlockCam34Check.Inputs["InputHandMarkY1"].Value = BatteryHandMarkY1;
                    cogRunToolBlockCam34Check.Inputs["InputHandMarkY2"].Value = BatteryHandMarkY2;

                    cogRunToolBlockCam34Check.Run();

                    if (cogRunToolBlockCam34Check.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        try
                        {

                            MyPoint BatteryTL, BatteryTR, BatteryBL, BatteryBR;
                            MyPoint CavityTL, CavityTR, CavityBL, CavityBR;

                            BatteryTL = new MyPoint(BatteryTLX, BatteryTLY);
                            BatteryTR = new MyPoint(BatteryTRX, BatteryTRY);
                            BatteryBL = new MyPoint(BatteryBLX, BatteryBLY);
                            BatteryBR = new MyPoint(BatteryBRX, BatteryBRY);

                            if (!EnableCavity2)
                            {
                                AxisMoveX = (double)cogRunToolBlockCam34Check.Outputs["DiffX"].Value
                                    + double.Parse(MV_Global_Variable.BatteryMarkOffsetX1);
                                AxisMoveY = (double)cogRunToolBlockCam34Check.Outputs["DiffY"].Value
                                    + double.Parse(MV_Global_Variable.BatteryMarkOffsetY1);
                                AxisMoveA = -(double)cogRunToolBlockCam34Check.Outputs["DiffA"].Value
                                    + double.Parse(MV_Global_Variable.BatteryMarkOffsetA1);


                                CavityTL = new MyPoint(CavityTLX, CavityTLY);
                                CavityTR = new MyPoint(CavityTRX, CavityTRY);
                                CavityBL = new MyPoint(CavityBLX, CavityBLY);
                                CavityBR = new MyPoint(CavityBRX, CavityBRY);

                                cogRunToolBlockCam34Cal.Inputs["CavityTLX"].Value = CavityTLX;
                                cogRunToolBlockCam34Cal.Inputs["CavityTLY"].Value = CavityTLY;
                                cogRunToolBlockCam34Cal.Inputs["CavityTRX"].Value = CavityTRX;
                                cogRunToolBlockCam34Cal.Inputs["CavityTRY"].Value = CavityTRY;
                                cogRunToolBlockCam34Cal.Inputs["CavityBLX"].Value = CavityBLX;
                                cogRunToolBlockCam34Cal.Inputs["CavityBLY"].Value = CavityBLY;
                                cogRunToolBlockCam34Cal.Inputs["CavityBRX"].Value = CavityBRX;
                                cogRunToolBlockCam34Cal.Inputs["CavityBRY"].Value = CavityBRY;
                            }
                            else
                            {
                                AxisMoveX = (double)cogRunToolBlockCam34Check.Outputs["DiffX"].Value
                                    + double.Parse(MV_Global_Variable.BatteryMarkOffsetX2);
                                AxisMoveY = (double)cogRunToolBlockCam34Check.Outputs["DiffY"].Value
                                    + double.Parse(MV_Global_Variable.BatteryMarkOffsetY2);
                                AxisMoveA = -(double)cogRunToolBlockCam34Check.Outputs["DiffA"].Value
                                    + double.Parse(MV_Global_Variable.BatteryMarkOffsetA2);

                                CavityTL = new MyPoint(Cavity2TLX, Cavity2TLY);
                                CavityTR = new MyPoint(Cavity2TRX, Cavity2TRY);
                                CavityBL = new MyPoint(Cavity2BLX, Cavity2BLY);
                                CavityBR = new MyPoint(Cavity2BRX, Cavity2BRY);

                                cogRunToolBlockCam34Cal.Inputs["CavityTLX"].Value = Cavity2TLX;
                                cogRunToolBlockCam34Cal.Inputs["CavityTLY"].Value = Cavity2TLY;
                                cogRunToolBlockCam34Cal.Inputs["CavityTRX"].Value = Cavity2TRX;
                                cogRunToolBlockCam34Cal.Inputs["CavityTRY"].Value = Cavity2TRY;
                                cogRunToolBlockCam34Cal.Inputs["CavityBLX"].Value = Cavity2BLX;
                                cogRunToolBlockCam34Cal.Inputs["CavityBLY"].Value = Cavity2BLY;
                                cogRunToolBlockCam34Cal.Inputs["CavityBRX"].Value = Cavity2BRX;
                                cogRunToolBlockCam34Cal.Inputs["CavityBRY"].Value = Cavity2BRY;
                            }

                            BatteryTL = PointNew(BatteryTL, -AxisMoveX, -AxisMoveY, AxisMoveA);
                            BatteryTR = PointNew(BatteryTR, -AxisMoveX, -AxisMoveY, AxisMoveA);
                            BatteryBL = PointNew(BatteryBL, -AxisMoveX, -AxisMoveY, AxisMoveA);
                            BatteryBR = PointNew(BatteryBR, -AxisMoveX, -AxisMoveY, AxisMoveA);

                            TestGapPoint[0] = new MyPoint((BatteryTL.x + ((BatteryTR.x - BatteryTL.x) / 3)), (BatteryTL.y + ((BatteryTR.y - BatteryTL.y) / 3)));
                            TestGapPoint[1] = new MyPoint((BatteryTL.x + ((BatteryTR.x - BatteryTL.x) * 2 / 3)), (BatteryTL.y + ((BatteryTR.y - BatteryTL.y) * 2 / 3)));
                            TestGapPoint[2] = new MyPoint((BatteryTL.x + ((BatteryBL.x - BatteryTL.x) / 3)), (BatteryTL.y + ((BatteryBL.y - BatteryTL.y) / 3)));
                            TestGapPoint[3] = new MyPoint((BatteryTL.x + ((BatteryBL.x - BatteryTL.x) * 2 / 3)), (BatteryTL.y + ((BatteryBL.y - BatteryTL.y) * 2 / 3)));
                            TestGapPoint[4] = new MyPoint((BatteryTR.x + ((BatteryBR.x - BatteryTR.x) / 3)), (BatteryTR.y + ((BatteryBR.y - BatteryTR.y) / 3)));
                            TestGapPoint[5] = new MyPoint((BatteryTR.x + ((BatteryBR.x - BatteryTR.x) * 2 / 3)), (BatteryTR.y + ((BatteryBR.y - BatteryTR.y) * 2 / 3)));
                            TestGapPoint[6] = new MyPoint((BatteryBL.x + ((BatteryBR.x - BatteryBL.x) / 3)), (BatteryBL.y + ((BatteryBR.y - BatteryBL.y) / 3)));
                            TestGapPoint[7] = new MyPoint((BatteryBL.x + ((BatteryBR.x - BatteryBL.x) * 2 / 3)), (BatteryBL.y + ((BatteryBR.y - BatteryBL.y) * 2 / 3)));

                            if (!EnableCavity2)
                            {
                                Gap[0] = pointToLine(CavityTL, CavityTR, TestGapPoint[0]);
                                Gap[1] = pointToLine(CavityTL, CavityTR, TestGapPoint[1]);
                                Gap[2] = pointToLine(CavityTL, CavityBL, TestGapPoint[2]);
                                Gap[3] = pointToLine(CavityTL, CavityBL, TestGapPoint[3]);
                                Gap[4] = pointToLine(CavityTR, CavityBR, TestGapPoint[4]);
                                Gap[5] = pointToLine(CavityTR, CavityBR, TestGapPoint[5]);
                                Gap[6] = pointToLine(CavityBL, CavityBR, TestGapPoint[6]);
                                Gap[7] = pointToLine(CavityBL, CavityBR, TestGapPoint[7]);
                            }
                            else
                            {
                                Gap[0] = pointToLine(CavityTL, CavityTR, TestGapPoint[0]);
                                Gap[1] = pointToLine(CavityTL, CavityTR, TestGapPoint[1]);
                                Gap[2] = pointToLine(CavityTL, CavityBL, TestGapPoint[2]);
                                Gap[3] = pointToLine(CavityTL, CavityBL, TestGapPoint[3]);
                                Gap[4] = pointToLine(CavityTR, CavityBR, TestGapPoint[4]);
                                Gap[5] = pointToLine(CavityTR, CavityBR, TestGapPoint[5]);
                                Gap[6] = pointToLine(CavityBL, CavityBR, TestGapPoint[6]);
                                Gap[7] = pointToLine(CavityBL, CavityBR, TestGapPoint[7]);
                            }
                            RecordOperateCor("Gap:" + Gap[0].ToString("0.000") + ";" + Gap[1].ToString("0.000") + ";" + Gap[2].ToString("0.000") + ";" + Gap[3].ToString("0.000") + ";"
                                + Gap[4].ToString("0.000") + ";" + Gap[5].ToString("0.000") + ";" + Gap[6].ToString("0.000") + ";" + Gap[7].ToString("0.000"));

                            cogRunToolBlockCam34Cal.Inputs["BatteryTLX"].Value = BatteryTL.x;
                            cogRunToolBlockCam34Cal.Inputs["BatteryTLY"].Value = BatteryTL.y;
                            cogRunToolBlockCam34Cal.Inputs["BatteryTRX"].Value = BatteryTR.x;
                            cogRunToolBlockCam34Cal.Inputs["BatteryTRY"].Value = BatteryTR.y;
                            cogRunToolBlockCam34Cal.Inputs["BatteryBLX"].Value = BatteryBL.x;
                            cogRunToolBlockCam34Cal.Inputs["BatteryBLY"].Value = BatteryBL.y;
                            cogRunToolBlockCam34Cal.Inputs["BatteryBRX"].Value = BatteryBR.x;
                            cogRunToolBlockCam34Cal.Inputs["BatteryBRY"].Value = BatteryBR.y;

                            cogRunToolBlockCam34Cal.Run();
                            if (cogRunToolBlockCam34Cal.RunStatus.Result == CogToolResultConstants.Accept)
                            {
                                if (MySubThreadFlow.CalTimes == 0)
                                {
                                    cogRecordDisplayCam3.InteractiveGraphics.Clear();
                                    cogRecordDisplayCam3.StaticGraphics.Clear();
                                    cogRecordDisplayCam3.Record = null;
                                    cogRecordDisplayCam4.InteractiveGraphics.Clear();
                                    cogRecordDisplayCam4.StaticGraphics.Clear();
                                    cogRecordDisplayCam4.Record = null;
                                    cogRecordDisplayCam3.Image = cogCalibrationImgCam3;
                                    cogRecordDisplayCam4.Image = cogCalibrationImgCam4;
                                    cogRecordDisplayCam3.Record = cogRunToolBlockCam34Cal.CreateLastRunRecord().SubRecords[0];
                                    SaveImage(todayImagePath + "cam3\\", cogCalibrationImgCam3);
                                    SaveImage(todayImagePath + "cam4\\", cogCalibrationImgCam4);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1, "Gap1:" + Gap[0].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 400, "Gap2:" + Gap[1].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 800, "Gap3:" + Gap[2].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1200, "Gap4:" + Gap[3].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 1600, "Gap5:" + Gap[4].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 2000, "Gap6:" + Gap[5].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 2400, "Gap7:" + Gap[6].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam3, cogLabelCam3, 10, 2800, "Gap8:" + Gap[7].ToString("0.00") + ";", CogColorConstants.Green);
                                }
                                else
                                {
                                    cogRecordDisplayCam4.InteractiveGraphics.Clear();
                                    cogRecordDisplayCam4.StaticGraphics.Clear();
                                    cogRecordDisplayCam4.Record = null;
                                    cogRecordDisplayCam4.Image = cogCalibrationImgCam3;
                                    cogRecordDisplayCam4.Record = cogRunToolBlockCam34Cal.CreateLastRunRecord().SubRecords[0];
                                    SaveImage(todayImagePath + "cam3\\", cogCalibrationImgCam3);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 0, "Gap1:" + Gap[0].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 400, "Gap2:" + Gap[1].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 800, "Gap3:" + Gap[2].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 1200, "Gap4:" + Gap[3].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 1600, "Gap5:" + Gap[4].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 2000, "Gap6:" + Gap[5].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 2400, "Gap7:" + Gap[6].ToString("0.00") + ";", CogColorConstants.Green);
                                    ShowLabelInfo(cogRecordDisplayCam4, cogLabelCam3, 10, 2800, "Gap8:" + Gap[7].ToString("0.00") + ";", CogColorConstants.Green);

                                }

                                if (!EnableCavity2)
                                {
                                    AxisMoveX = (double)cogRunToolBlockCam34Cal.Outputs["DiffX"].Value + double.Parse(MV_Global_Variable.AddOffsetX1);
                                    AxisMoveY = (double)cogRunToolBlockCam34Cal.Outputs["DiffY"].Value + double.Parse(MV_Global_Variable.AddOffsetY1);
                                    AxisMoveA = (((double)cogRunToolBlockCam34Cal.Outputs["DiffA"].Value / Math.PI) * 180) + double.Parse(MV_Global_Variable.AddOffsetA1);

                                }
                                else
                                {
                                    AxisMoveX = (double)cogRunToolBlockCam34Cal.Outputs["DiffX"].Value + double.Parse(MV_Global_Variable.AddOffsetX2);
                                    AxisMoveY = (double)cogRunToolBlockCam34Cal.Outputs["DiffY"].Value + double.Parse(MV_Global_Variable.AddOffsetY2);
                                    AxisMoveA = (((double)cogRunToolBlockCam34Cal.Outputs["DiffA"].Value / Math.PI) * 180) + double.Parse(MV_Global_Variable.AddOffsetA2);

                                }
                                RunSucceed = true;
                            }
                            else
                            {
                                RunSucceed = false;
                            }

                        }
                        catch
                        {

                            RunSucceed = false;
                        }

                    }
                    else
                    {

                        RunSucceed = false;
                    }
                    break;
                case "QRCode":
                    cogRunToolBlockCam5Code.Inputs["Cam5"].Value = ImageCam5;
                    cogRunToolBlockCam5Code.Run();
                    try
                    {
                        CavityQRCode = cogRunToolBlockCam5Code.Outputs["CodeString"].Value.ToString();

                        try
                        {
                            cogRecordDisplayCam5.Record = cogRunToolBlockCam5Code.CreateLastRunRecord().SubRecords[0];

                            ShowLabelInfo(cogRecordDisplayCam5, cogLabelCam5, 10, 1, "Barcode:" + CavityQRCode, CogColorConstants.Green);
                        }
                        catch
                        {

                        }
                        //RunSucceed = true;
                        break;
                    }
                    catch
                    {
                        CavityQRCode = "NA";
                        ShowLabelInfo(cogRecordDisplayCam5, cogLabelCam5, 10, 1, "Barcode:" + CavityQRCode, CogColorConstants.Red);
                        //RunSucceed = false;
                    }
                    break;
            }
        }

        void RunCalibCheckBorad(string product, string BussyCam)
        {
            AcqImage(product, BussyCam);
            RunOnlyCalibToolBlock(BussyCam);
            try
            {
                switch (BussyCam)
                {
                    case "Cam12":

                        cogCalibCheckerboardToolCam1.Calibration.Calibrate();
                        cogCalibCheckerboardToolCam2.Calibration.Calibrate();
                        break;
                    case "Cam34":
                        cogCalibCheckerboardToolCam3.Calibration.Calibrate();
                        cogCalibCheckerboardToolCam4.Calibration.Calibrate();
                        break;
                }
            }
            catch
            { }
        }


        /// <summary>
        /// 计算点P(x,y)与X轴正方向的夹角
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <returns>夹角弧度</returns>
        private static double radPOX(double x, double y)
        {
            //P在(0,0)的情况
            if (x == 0 && y == 0) return 0;
            //P在四个坐标轴上的情况：x正、x负、y正、y负
            if (y == 0 && x > 0) return 0;
            if (y == 0 && x < 0) return Math.PI;
            if (x == 0 && y > 0) return Math.PI / 2;
            if (x == 0 && y < 0) return Math.PI / 2 * 3;
            //点在第一、二、三、四象限时的情况
            if (x > 0 && y > 0) return Math.Atan(y / x);
            if (x < 0 && y > 0) return Math.PI - Math.Atan(y / -x);
            if (x < 0 && y < 0) return Math.PI + Math.Atan(-y / -x);
            if (x > 0 && y < 0) return Math.PI * 2 - Math.Atan(-y / x);
            return 0;
        }

        private MyPoint PointRotate(MyPoint p1, double RadAngle)
        {
            MyPoint tmp = new MyPoint();
            Point center = new Point(0, 0);
            double angleHude = RadAngle;/*角度变成弧度*/
            double x1 = (p1.x - center.X) * Math.Cos(angleHude) + (p1.y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.x - center.X) * Math.Sin(angleHude) + (p1.y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.x = x1;
            tmp.y = y1;
            return tmp;
        }

        private MyPoint PointNew(MyPoint p1, double OffsetX, double OffsetY, double OffsetRadA)
        {
            MyPoint tmp = new MyPoint();
            tmp = PointRotate(p1, OffsetRadA);
            tmp.x = tmp.x - OffsetX;
            tmp.y = tmp.y - OffsetY;
            return tmp;
        }

        private double lineSpace(double x1, double y1, double x2, double y2)
        {
            double lineLength = 0;
            lineLength = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            return lineLength;

        }

        private double pointToLine(MyPoint p1, MyPoint p2, MyPoint p0)
        {
            double space = 0;
            double a, b, c;
            a = lineSpace(p1.x, p1.y, p2.x, p2.y);// 线段的长度    
            b = lineSpace(p1.x, p1.y, p0.x, p0.y);// (x1,y1)到点的距离    
            c = lineSpace(p2.x, p2.y, p0.x, p0.y);// (x2,y2)到点的距离    
            if (c <= 0.000001 || b <= 0.000001)
            {
                space = 0;
                return space;
            }
            if (a <= 0.000001)
            {
                space = b;
                return space;
            }
            if (c * c >= a * a + b * b)
            {
                space = b;
                return space;
            }
            if (b * b >= a * a + c * c)
            {
                space = c;
                return space;
            }
            double p = (a + b + c) / 2;// 半周长    
            double s = Math.Sqrt(p * (p - a) * (p - b) * (p - c));// 海伦公式求面积    
            space = 2 * s / a;// 返回点到线的距离（利用三角形面积公式求高）    
            return space;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel1, LightValue1);
            CstLightSet.mController.SetDigitalValue(LightChannel2, 0);
            CstLightSet.mController.SetDigitalValue(LightChannel3, 0);

            RunCamBatteryCavityStep("Battery", "Cam12");

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel1, 0);
            CstLightSet.mController.SetDigitalValue(LightChannel2, LightValue2);
            CstLightSet.mController.SetDigitalValue(LightChannel3, LightValue3);
            RunCamBatteryCavityStep("Mark", "Cam12");
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel4, LightValue4);
            RunCamBatteryCavityStep("Cavity", "Cam34");

        }

        private void Button6_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel4, LightValue4);
            RunCamBatteryCavityStep("CavityCheck", "Cam34");
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel5, LightValue5);
            RunCamBatteryCavityStep("QRcode", "Cam5");
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (!StartRun)
            {
                Camera3DDirc = "Cam3D-1";
                Capture3DCameraAsync(Camera3DDirc, true);
            }
        }
        private void Button9_Click(object sender, EventArgs e)
        {
            if (!StartRun)
            {
                Camera3DDirc = "Cam3D-2";
                Capture3DCameraAsync(Camera3DDirc, true);
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (!StartRun)
            {
                Camera3DDirc = "Cam3D-3";
                Capture3DCameraAsync(Camera3DDirc, true);
            }
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            if (!StartRun)
            {
                Camera3DDirc = "Cam3D-4";
                Capture3DCameraAsync(Camera3DDirc, true);
            }
        }

        /// <summary>
        /// 返回点P围绕点A旋转弧度rad后的坐标
        /// </summary>
        /// <param name="P">待旋转点坐标</param>
        /// <param name="A">旋转中心坐标</param>
        /// <param name="rad">旋转弧度</param>
        /// <param name="isClockwise">true:顺时针/false:逆时针</param>
        /// <returns>旋转后坐标</returns>
        private static MyPoint RotatePoint(MyPoint P, MyPoint A,
         double rad, bool isClockwise = true)
        {
            //点Temp1
            MyPoint Temp1 = new MyPoint(P.x - A.x, P.y - A.y);
            //点Temp1到原点的长度
            double lenO2Temp1 = Temp1.DistanceTo(new MyPoint(0, 0));
            //∠T1OX弧度
            double angT1OX = radPOX(Temp1.x, Temp1.y);
            //∠T2OX弧度（T2为T1以O为圆心旋转弧度rad）
            double angT2OX = angT1OX - (isClockwise ? 1 : -1) * rad;
            //点Temp2
            MyPoint Temp2 = new MyPoint(
             lenO2Temp1 * Math.Cos(angT2OX),
             lenO2Temp1 * Math.Sin(angT2OX));
            //点Q
            return new MyPoint(Temp2.x + A.x, Temp2.y + A.y);
        }

        public void RunCheckToolBlock(string product, string BussyCam, string Model)
        {
            switch (Model)
            {
                case "Film":
                    AcqImage(product, BussyCam);
                    cogRunToolBlockCam34CheckFilm.Inputs["cam3Img"].Value = ImageCam3;
                    cogRunToolBlockCam34CheckFilm.Inputs["cam4Img"].Value = ImageCam4;
                    cogRunToolBlockCam34CheckFilm.Run();
                    if (cogRunToolBlockCam34CheckFilm.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed = true;
                    }
                    else
                    {
                        RunSucceed = false;
                    }
                    break;
                case "Gap":
                    AcqImage(product, BussyCam);
                    RunCalibToolBlock(BussyCam);
                    cogRunToolBlockCam34CheckGap.Inputs["cam3Img"].Value = cogCalibrationImgCam3;
                    cogRunToolBlockCam34CheckGap.Inputs["cam3Img"].Value = cogCalibrationImgCam4;
                    cogRunToolBlockCam34CheckGap.Run();
                    if (cogRunToolBlockCam34CheckGap.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed = true;
                    }
                    else
                    {
                        RunSucceed = false;
                    }
                    break;
                case "FPC":
                    AcqImage(product, BussyCam);
                    cogRunToolBlockCam34CheckFPC.Inputs["cam3Img"].Value = ImageCam3;
                    cogRunToolBlockCam34CheckFPC.Inputs["cam4Img"].Value = ImageCam4;
                    cogRunToolBlockCam34CheckFPC.Run();
                    if (cogRunToolBlockCam34CheckFPC.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        RunSucceed = true;
                    }
                    else
                    {
                        RunSucceed = false;
                    }
                    break;

            }
        }

        void ReStart3DCamera()
        {
            isRun = false;
            StartRun = false;
            StartTick = false;
            sensor0.StopAcquisition();
            sensor0.Disconnect();
            this.Enabled = false;
            sensor0.AcqusitionCompletedEvent -= new Sensor.delegateAcqusitionCompleted(OnCompletedAcqEvent); //new Sensor.AcqusitionCompleted(OnCompletedAcqEvent);
            try
            {
                sensor0.Connect(10000);
                //Load Calibration File form Sensor0
                sensor0.LoadCalibrationDataFromSensor();
                string path = "E:\\3DFile\\20210530.par";

                //Loading Parameter set for Sensor0
                Api.LoadParameterSetFromFile(sensor0._sensorObject, path);

                // define image acquiring Type
                Api.SetImageAcquisitionType(sensor0._sensorObject, Api.ImageAcquisitionType.ZMapIntensityLaserLineThickness);

                //Setting Acquisition Mode as Continue Mode
                Api.SetAcquisitionMode(sensor0._sensorObject, Api.AcquisitionMode.RepeatSnapshot);

                //Register Acquisition One image Complete Event
                sensor0.AcqusitionCompletedEvent += new Sensor.delegateAcqusitionCompleted(OnCompletedAcqEvent); //new Sensor.AcqusitionCompleted(OnCompletedAcqEvent);


                Api.SetReadyForAcquisitionStatus(sensor0._sensorObject, (int)Api.DigitalOutput.Channel2, 1);
                //Setting for Profiles Number
                //Api.SetNumberOfProfilesToCapture(sensor0._sensorObject, 200);
                //Setting PackSize
                //Api.SetPacketSize(sensor0._sensorObject, 50);
                Api.SetNumberOfProfilesToCapture(sensor0._sensorObject, uint.Parse(ImageHeight));
                Api.SetPacketSize(sensor0._sensorObject, 0);
                //Api.SetPacketTimeOut(sensor0._sensorObject, 0);

                if (MV_Global_Variable.SmartRayExpTime != 0)
                {
                    Api.SetExposureTime(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayExpTime);
                }
                else
                {
                    Api.GetExposureTime(sensor0._sensorObject, 0, out MV_Global_Variable.SmartRayExpTime);
                    MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机曝光时间", MV_Global_Variable.SmartRayExpTime);
                }

                if (MV_Global_Variable.SmartRayLaserThreshold != 0)
                {
                    Api.Set3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayLaserThreshold);
                }
                else
                {
                    Api.Get3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, out MV_Global_Variable.SmartRayLaserThreshold);
                    MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光阈值", MV_Global_Variable.SmartRayLaserThreshold);
                }

                if (MV_Global_Variable.SmartRayLaserBrightness != 0)
                {
                    Api.SetLaserBrightness(sensor0._sensorObject, MV_Global_Variable.SmartRayLaserBrightness);
                }
                else
                {
                    Api.GetLaserBrightness(sensor0._sensorObject, out MV_Global_Variable.SmartRayLaserBrightness);
                    MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光强度", MV_Global_Variable.SmartRayLaserBrightness);
                }

                sensor0.SendParameterSet();
            }
            catch { }
            this.Enabled = true;
        }
        void InitSmartRayCamera()
        {
            try
            {
                sensorManager = new SensorManager();
                sensor0 = sensorManager.CreateSensor("Sensor0");
                sensor0.Connect(10000);
                //Load Calibration File form Sensor0
                sensor0.LoadCalibrationDataFromSensor();
                string path = "E:\\3DFile\\20210530.par";

                //Loading Parameter set for Sensor0
                Api.LoadParameterSetFromFile(sensor0._sensorObject, path);

                // define image acquiring Type
                Api.SetImageAcquisitionType(sensor0._sensorObject, Api.ImageAcquisitionType.ZMapIntensityLaserLineThickness);

                //Setting Acquisition Mode as Continue Mode
                Api.SetAcquisitionMode(sensor0._sensorObject, Api.AcquisitionMode.RepeatSnapshot);

                //Register Acquisition One image Complete Event
                sensor0.AcqusitionCompletedEvent += new Sensor.delegateAcqusitionCompleted(OnCompletedAcqEvent); //new Sensor.AcqusitionCompleted(OnCompletedAcqEvent);


                Api.SetReadyForAcquisitionStatus(sensor0._sensorObject, (int)Api.DigitalOutput.Channel2, 1);
                //Setting for Profiles Number
                //Api.SetNumberOfProfilesToCapture(sensor0._sensorObject, 200);
                //Setting PackSize
                //Api.SetPacketSize(sensor0._sensorObject, 50);
                Api.SetNumberOfProfilesToCapture(sensor0._sensorObject, uint.Parse(ImageHeight));
                Api.SetPacketSize(sensor0._sensorObject, 0);
                //Api.SetPacketTimeOut(sensor0._sensorObject, 0);

                if (MV_Global_Variable.SmartRayExpTime != 0)
                {
                    Api.SetExposureTime(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayExpTime);
                }
                else
                {
                    Api.GetExposureTime(sensor0._sensorObject, 0, out MV_Global_Variable.SmartRayExpTime);
                    MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机曝光时间", MV_Global_Variable.SmartRayExpTime);
                }

                if (MV_Global_Variable.SmartRayLaserThreshold != 0)
                {
                    Api.Set3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayLaserThreshold);
                }
                else
                {
                    Api.Get3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, out MV_Global_Variable.SmartRayLaserThreshold);
                    MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光阈值", MV_Global_Variable.SmartRayLaserThreshold);
                }

                if (MV_Global_Variable.SmartRayLaserBrightness != 0)
                {
                    Api.SetLaserBrightness(sensor0._sensorObject, MV_Global_Variable.SmartRayLaserBrightness);
                }
                else
                {
                    Api.GetLaserBrightness(sensor0._sensorObject, out MV_Global_Variable.SmartRayLaserBrightness);
                    MySystemINIFile.WriteValue(MV_Global_Variable.ProjectName, "3D相机激光强度", MV_Global_Variable.SmartRayLaserBrightness);
                }

                sensor0.SendParameterSet();

                Load3DSucceed = true;
            }
            catch
            {
                Load3DSucceed = false;
            }
        }

        private void Btn_CalibCam12_Click(object sender, EventArgs e)
        {
            RunCalibCheckBorad("Battery", "Cam12");
        }

        private void Btn_CalibCam34_Click(object sender, EventArgs e)
        {
            RunCalibCheckBorad("Battery", "Cam34");
        }

        private void btnLoad3dPic_Click(object sender, EventArgs e)
        {

        }

        private void cogRecordDisplayCam7_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam7.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam7.Parent = CamShowMAXPanel;
                cogRecordDisplayCam7.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void cogRecordDisplayCam8_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam8.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam8.Parent = CamShowMAXPanel;
                cogRecordDisplayCam8.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void cogRecordDisplayCam9_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam9.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam9.Parent = CamShowMAXPanel;
                cogRecordDisplayCam9.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void CogRecordDisplayCam1_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam1.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam1.Parent = CamShowMAXPanel;
                cogRecordDisplayCam1.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void cogRecordDisplayCam10_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam10.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam10.Parent = CamShowMAXPanel;
                cogRecordDisplayCam10.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void cogRecordDisplayCam11_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam11.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam11.Parent = CamShowMAXPanel;
                cogRecordDisplayCam11.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void cogRecordDisplayCam12_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam12.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam12.Parent = CamShowMAXPanel;
                cogRecordDisplayCam12.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel1, LightValue1);
            CstLightSet.mController.SetDigitalValue(LightChannel2, 0);
            CstLightSet.mController.SetDigitalValue(LightChannel3, 0);
            RunCamBatteryCavityStep("Battery2", "Cam12");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ReStart3DCamera();
        }

        private void cogRecordDisplayCam13_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam13.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam13.Parent = CamShowMAXPanel;
                cogRecordDisplayCam13.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void CogRecordDisplayCam2_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam2.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam2.Parent = CamShowMAXPanel;
                cogRecordDisplayCam2.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void CogRecordDisplayCam3_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam3.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam3.Parent = CamShowMAXPanel;
                cogRecordDisplayCam3.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void CogRecordDisplayCam4_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam4.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam4.Parent = CamShowMAXPanel;
                cogRecordDisplayCam4.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void CogRecordDisplayCam5_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam5.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam5.Parent = CamShowMAXPanel;
                cogRecordDisplayCam5.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void CogRecordDisplayCam6_DoubleClick(object sender, EventArgs e)
        {
            if (cogRecordDisplayCam6.Parent == tableLayoutPanel_Camera)
            {
                cogRecordDisplayCam6.Parent = CamShowMAXPanel;
                cogRecordDisplayCam6.BringToFront();
            }
            else
            {
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam1, 0, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam2, 1, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam3, 2, 0);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam4, 0, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam5, 1, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam6, 2, 1);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam7, 0, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam8, 1, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam9, 2, 2);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam10, 0, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam11, 1, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam12, 2, 3);
                tableLayoutPanel_Camera.Controls.Add(cogRecordDisplayCam13, 0, 4);
            }
        }

        private void BatteryDatumCam12PIC_Click(object sender, EventArgs e)
        {
            if (ImageCam1 == null || ImageCam2 == null)
            {
                MessageBox.Show("未正确加载图片！");
                return;
            }
            RunCamBatteryCavityStepPic("Battery", "Cam12");
        }

        private void BatteryDatumCam12PIC2_Click(object sender, EventArgs e)
        {
            RunCamBatteryCavityStepPic("Battery2", "Cam12");
        }

        private void CavityDatumCam34PIC2_Click(object sender, EventArgs e)
        {
            RunCamBatteryCavityStepPic("Cavity2", "Cam34");
        }

        private void CavityDatumCam34PIC_Click(object sender, EventArgs e)
        {
            RunCamBatteryCavityStepPic("Cavity", "Cam34");
        }

        private void BarcodeCam5PIC_Click(object sender, EventArgs e)
        {
            RunCamBatteryCavityStepPic("QRCode", "Cam5");
        }

        private void btnLoadCam1Pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog myopenpic = new OpenFileDialog();
            DialogResult OpenRe = myopenpic.ShowDialog();
            if (OpenRe == DialogResult.OK)
            {
                string Picpath = myopenpic.FileName;
                cogImgCam1.Operator.Open(Picpath, CogImageFileModeConstants.Read);
                cogImgCam1.Run();
                ImageCam1 = cogImgCam1.OutputImage as CogImage8Grey;
            }
        }

        private void btnLoadCam2Pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog myopenpic = new OpenFileDialog();
            DialogResult OpenRe = myopenpic.ShowDialog();
            if (OpenRe == DialogResult.OK)
            {
                string Picpath = myopenpic.FileName;
                cogImgCam1.Operator.Open(Picpath, CogImageFileModeConstants.Read);
                cogImgCam1.Run();
                ImageCam2 = cogImgCam1.OutputImage as CogImage8Grey;
            }
        }

        private void btnLoadCam3Pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog myopenpic = new OpenFileDialog();
            DialogResult OpenRe = myopenpic.ShowDialog();
            if (OpenRe == DialogResult.OK)
            {
                string Picpath = myopenpic.FileName;
                cogImgCam1.Operator.Open(Picpath, CogImageFileModeConstants.Read);
                cogImgCam1.Run();
                ImageCam3 = cogImgCam1.OutputImage as CogImage8Grey;
            }
        }

        private void btnLoadCam4Pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog myopenpic = new OpenFileDialog();
            DialogResult OpenRe = myopenpic.ShowDialog();
            if (OpenRe == DialogResult.OK)
            {
                string Picpath = myopenpic.FileName;
                cogImgCam1.Operator.Open(Picpath, CogImageFileModeConstants.Read);
                cogImgCam1.Run();
                ImageCam4 = cogImgCam1.OutputImage as CogImage8Grey;
            }
        }

        private void btnLoadCam5Pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog myopenpic = new OpenFileDialog();
            DialogResult OpenRe = myopenpic.ShowDialog();
            if (OpenRe == DialogResult.OK)
            {
                string Picpath = myopenpic.FileName;
                cogImgCam1.Operator.Open(Picpath, CogImageFileModeConstants.Read);
                cogImgCam1.Run();
                ImageCam5 = cogImgCam1.OutputImage as CogImage8Grey;
            }
        }

        private void HandMarkCam12PIC_Click(object sender, EventArgs e)
        {
            RunCamBatteryCavityStepPic("Mark", "Cam12");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            CstLightSet.mController.SetDigitalValue(LightChannel4, LightValue4);
            RunCamBatteryCavityStep("Cavity2", "Cam34");
        }

        private void GapCalCam34PIC_Click(object sender, EventArgs e)
        {
            RunCamBatteryCavityStepPic("CavityCheck", "Cam34");
        }

        public void Capture3DCameraAsync(string Dirc, bool Enable)
        {
            Camera3DDirc = Dirc;
            isRun = true;
            StartRun = Enable;
        }

        private void AcqusitionFromSensor()
        {
            while (true)
            {
                try
                {
                    if (isRun)
                    {
                        isRun = false;
                        if (StartRun)
                        {
                            int micTime;
                            Api.GetExposureTime(sensor0._sensorObject, 0, out micTime);
                            if (micTime != MV_Global_Variable.SmartRayExpTime)
                                Api.SetExposureTime(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayExpTime);

                            int LaserThreshold;
                            Api.Get3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, out LaserThreshold);
                            if (LaserThreshold != MV_Global_Variable.SmartRayLaserThreshold)
                                Api.Set3DLaserLineBrightnessThreshold(sensor0._sensorObject, 0, MV_Global_Variable.SmartRayLaserThreshold);

                            int LaserBrightness;
                            Api.GetLaserBrightness(sensor0._sensorObject, out LaserBrightness);
                            if (LaserBrightness != MV_Global_Variable.SmartRayLaserBrightness)
                                Api.SetLaserBrightness(sensor0._sensorObject, MV_Global_Variable.SmartRayLaserBrightness);

                            sensor0.StartAcquisition();
                            //sensor0.WaitForImage(1);

                        }
                        else
                        {
                            sensor0.StopAcquisition();
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ce)
                {
                    sensor0.StopAcquisition();
                }

            }

        }

        private static void LogRegionsInfo(CogToolBlock Tb)
        {
            CogToolBlock checkAll = Tb.Tools["CheckAll"] as CogToolBlock;
            if (checkAll == null)
            {

            }
            else
            {
                List<CogPolygon> allRegions = checkAll.Outputs["AllRegions"].Value as List<CogPolygon>;
                if (Tb.Outputs.Contains("Heights"))
                {
                    List<double> allHeights = Tb.Outputs["Heights"].Value as List<double>;

                    if (allHeights != null)
                    {
                        if (allRegions != null)
                        {
                            for (int i = 0; i < allRegions.Count; i++)
                            {
                                CogRectangle enclos = allRegions[i].EnclosingRectangle(CogCopyShapeConstants.GeometryOnly);
                            }
                        }
                    }
                    else
                    {

                        for (int i = 0; i < allRegions.Count; i++)
                        {
                            CogRectangle enclos = allRegions[i].EnclosingRectangle(CogCopyShapeConstants.GeometryOnly);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < allRegions.Count; i++)
                    {
                        CogRectangle enclos = allRegions[i].EnclosingRectangle(CogCopyShapeConstants.GeometryOnly);
                    }
                }
            }
        }

        void OnCompletedAcqEvent(object sender, EventArgs e)
        {
            //HiPerfTimer timer = new HiPerfTimer();
            try
            {
                RecordOperateCor("3DScan Complited!");
                StartTick = false;
                string SendMessage = "";
                SensorImageData imageDatas = sensor0.GetLastImageData();
                ushort[] dataArray = imageDatas.ZMapImage.GetImage();
                Capture3DCameraAsync(Camera3DDirc, false);
                Scan3DCount = 0;
                if (dataArray != null)
                {
                    CognexRangeImage = convertSmartRayImage.Convert2VisionProImage(dataArray, imageDatas.Width, dataArray.Length / imageDatas.Width, 0.056, 0.056, 0.0085);//最后的三个参数为具体的相机的分辨率
                    CogImage16Range VisionPro3Dimage = CognexRangeImage as CogImage16Range;
                    switch (Camera3DDirc)
                    {
                        case "Cam3D-1":
                            try
                            {

                                cogRecordDisplayCam6.InteractiveGraphics.Clear();
                                cogRecordDisplayCam6.StaticGraphics.Clear();
                                cogRecordDisplayCam6.Record = null;
                                cogRecordDisplayCam7.InteractiveGraphics.Clear();
                                cogRecordDisplayCam7.StaticGraphics.Clear();
                                cogRecordDisplayCam7.Record = null;

                                cogRecordDisplayCam6.Image = VisionPro3Dimage;
                                cogRecordDisplayCam7.Image = VisionPro3Dimage;

                                cogRunToolBlock3DCam1.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam1.Run();

                                cogRunToolBlock3DCam12.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam12.Run();

                                double SendResult1 = -1, SendResult2 = -1;

                                if (cogRunToolBlock3DCam1.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result = (bool)cogRunToolBlock3DCam1.Outputs[2].Value;
                                    cogRecordDisplayCam6.Record = cogRunToolBlock3DCam1.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam6.Fit(true);
                                    if (Result) SendResult1 = 1;

                                }
                                else
                                {
                                    cogRecordDisplayCam6.Record = cogRunToolBlock3DCam1.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam6.Fit(true);
                                }


                                if (cogRunToolBlock3DCam12.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result2 = (bool)cogRunToolBlock3DCam12.Outputs[2].Value;
                                    cogRecordDisplayCam7.Record = cogRunToolBlock3DCam12.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam7.Fit(true);
                                    if (Result2) SendResult2 = 1;
                                }
                                else
                                {

                                    cogRecordDisplayCam7.Record = cogRunToolBlock3DCam12.CreateLastRunRecord().SubRecords[0];

                                    cogRecordDisplayCam7.Fit(true);
                                }

                                if (SendResult1 == 1 && SendResult2 == 1)
                                {
                                    SaveImage(todayImagePath + "cam6\\3D1\\OK\\", VisionPro3Dimage, MV_Global_Variable.Save3DOKImage);
                                }
                                else
                                {
                                    SaveImage(todayImagePath + "cam6\\3D1\\NG\\", VisionPro3Dimage, MV_Global_Variable.Save3DNGImage);
                                }

                                string StrSendResult = string.Format("^SF^1^{0}^{1}", SendResult1, SendResult2);
                                SendMessage = Cam3DStepStr[0] + StrSendResult;
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);

                            }
                            catch
                            {
                                SendMessage = Cam3DStepStr[0] + "^SF^0^" + "-1^-1";
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);
                            }

                            break;
                        case "Cam3D-2":
                            try
                            {

                                cogRecordDisplayCam8.InteractiveGraphics.Clear();
                                cogRecordDisplayCam8.StaticGraphics.Clear();
                                cogRecordDisplayCam8.Record = null;
                                cogRecordDisplayCam9.InteractiveGraphics.Clear();
                                cogRecordDisplayCam9.StaticGraphics.Clear();
                                cogRecordDisplayCam9.Record = null;

                                cogRecordDisplayCam8.Image = VisionPro3Dimage;
                                cogRecordDisplayCam9.Image = VisionPro3Dimage;

                                cogRunToolBlock3DCam2.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam2.Run();
                                cogRunToolBlock3DCam22.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam22.Run();

                                double SendResult1 = -1, SendResult2 = -1;

                                if (cogRunToolBlock3DCam2.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result = (bool)cogRunToolBlock3DCam2.Outputs[2].Value;
                                    cogRecordDisplayCam8.Record = cogRunToolBlock3DCam2.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam8.Fit(true);
                                    if (Result) SendResult1 = 1;

                                }
                                else
                                {
                                    cogRecordDisplayCam8.Record = cogRunToolBlock3DCam2.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam8.Fit(true);
                                }


                                if (cogRunToolBlock3DCam22.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result2 = (bool)cogRunToolBlock3DCam22.Outputs[2].Value;
                                    cogRecordDisplayCam9.Record = cogRunToolBlock3DCam22.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam9.Fit(true);
                                    if (Result2) SendResult2 = 1;
                                }
                                else
                                {

                                    cogRecordDisplayCam9.Record = cogRunToolBlock3DCam22.CreateLastRunRecord().SubRecords[0];

                                    cogRecordDisplayCam9.Fit(true);
                                }

                                if (SendResult1 == 1 && SendResult2 == 1)
                                {
                                    SaveImage(todayImagePath + "cam6\\3D2\\OK\\", VisionPro3Dimage, MV_Global_Variable.Save3DOKImage);
                                }
                                else
                                {
                                    SaveImage(todayImagePath + "cam6\\3D2\\NG\\", VisionPro3Dimage, MV_Global_Variable.Save3DNGImage);
                                }

                                string StrSendResult = string.Format("^SF^1^{0}^{1}", SendResult1, SendResult2);
                                SendMessage = Cam3DStepStr[0] + StrSendResult;
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);

                            }
                            catch
                            {
                                SendMessage = Cam3DStepStr[0] + "^SF^0^" + "-1^-1";
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);
                            }

                            break;
                        case "Cam3D-3":
                            try
                            {
                                //SaveImage(todayImagePath + "cam6\\3D3", VisionPro3Dimage);
                                cogRecordDisplayCam10.InteractiveGraphics.Clear();
                                cogRecordDisplayCam10.StaticGraphics.Clear();
                                cogRecordDisplayCam10.Record = null;
                                cogRecordDisplayCam11.InteractiveGraphics.Clear();
                                cogRecordDisplayCam11.StaticGraphics.Clear();
                                cogRecordDisplayCam11.Record = null;

                                cogRecordDisplayCam10.Image = VisionPro3Dimage;
                                cogRecordDisplayCam11.Image = VisionPro3Dimage;

                                cogRunToolBlock3DCam3.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam3.Run();

                                cogRunToolBlock3DCam32.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam32.Run();

                                double SendResult1 = -1, SendResult2 = -1;

                                if (cogRunToolBlock3DCam3.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result = (bool)cogRunToolBlock3DCam3.Outputs[2].Value;
                                    cogRecordDisplayCam10.Record = cogRunToolBlock3DCam3.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam10.Fit(true);
                                    if (Result) SendResult1 = 1;

                                }
                                else
                                {
                                    cogRecordDisplayCam10.Record = cogRunToolBlock3DCam3.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam10.Fit(true);
                                }


                                if (cogRunToolBlock3DCam32.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result2 = (bool)cogRunToolBlock3DCam32.Outputs[2].Value;
                                    cogRecordDisplayCam11.Record = cogRunToolBlock3DCam32.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam11.Fit(true);
                                    if (Result2) SendResult2 = 1;
                                }
                                else
                                {

                                    cogRecordDisplayCam11.Record = cogRunToolBlock3DCam32.CreateLastRunRecord().SubRecords[0];

                                    cogRecordDisplayCam11.Fit(true);
                                }

                                if (SendResult1 == 1 && SendResult2 == 1)
                                {
                                    SaveImage(todayImagePath + "cam6\\3D3\\OK\\", VisionPro3Dimage, MV_Global_Variable.Save3DOKImage);
                                }
                                else
                                {
                                    SaveImage(todayImagePath + "cam6\\3D3\\NG\\", VisionPro3Dimage, MV_Global_Variable.Save3DNGImage);
                                }

                                string StrSendResult = string.Format("^SF^1^{0}^{1}", SendResult1, SendResult2);
                                SendMessage = Cam3DStepStr[0] + StrSendResult;
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);

                            }
                            catch
                            {
                                SendMessage = Cam3DStepStr[0] + "^SF^0^" + "-1^-1";
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);
                            }


                            break;
                        case "Cam3D-4":
                            try
                            {

                                cogRecordDisplayCam12.InteractiveGraphics.Clear();
                                cogRecordDisplayCam12.StaticGraphics.Clear();
                                cogRecordDisplayCam12.Record = null;
                                cogRecordDisplayCam13.InteractiveGraphics.Clear();
                                cogRecordDisplayCam13.StaticGraphics.Clear();
                                cogRecordDisplayCam13.Record = null;

                                cogRecordDisplayCam12.Image = VisionPro3Dimage;
                                cogRecordDisplayCam13.Image = VisionPro3Dimage;

                                cogRunToolBlock3DCam4.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam4.Run();
                                cogRunToolBlock3DCam42.Inputs[0].Value = VisionPro3Dimage;
                                cogRunToolBlock3DCam42.Run();
                                double SendResult1 = -1, SendResult2 = -1;

                                if (cogRunToolBlock3DCam4.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result = (bool)cogRunToolBlock3DCam4.Outputs[2].Value;
                                    cogRecordDisplayCam12.Record = cogRunToolBlock3DCam4.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam12.Fit(true);
                                    if (Result) SendResult1 = 1;

                                }
                                else
                                {
                                    cogRecordDisplayCam12.Record = cogRunToolBlock3DCam4.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam12.Fit(true);
                                }


                                if (cogRunToolBlock3DCam42.RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    bool Result2 = (bool)cogRunToolBlock3DCam42.Outputs[2].Value;
                                    cogRecordDisplayCam13.Record = cogRunToolBlock3DCam42.CreateLastRunRecord().SubRecords[0];
                                    cogRecordDisplayCam13.Fit(true);
                                    if (Result2) SendResult2 = 1;
                                }
                                else
                                {

                                    cogRecordDisplayCam13.Record = cogRunToolBlock3DCam42.CreateLastRunRecord().SubRecords[0];

                                    cogRecordDisplayCam13.Fit(true);
                                }

                                if (SendResult1 == 1 && SendResult2 == 1)
                                {
                                    SaveImage(todayImagePath + "cam6\\3D4\\OK\\", VisionPro3Dimage, MV_Global_Variable.Save3DOKImage);
                                }
                                else
                                {
                                    SaveImage(todayImagePath + "cam6\\3D4\\NG\\", VisionPro3Dimage, MV_Global_Variable.Save3DNGImage);
                                }

                                string StrSendResult = string.Format("^SF^1^{0}^{1}", SendResult1, SendResult2);
                                SendMessage = Cam3DStepStr[0] + StrSendResult;
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);

                            }
                            catch
                            {
                                SendMessage = Cam3DStepStr[0] + "^SF^0^" + "-1^-1";
                                MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                                RecordOperateCor(SendMessage);
                            }

                            break;
                    }


                }
                else
                {
                    SendMessage = Cam3DStepStr[0] + "^SF^0^";
                    MySubThreadFlow.MyServerSocketScan.SendAll(SendMessage);
                    RecordOperateCor(SendMessage);
                }

                //release buffer
                sensor0.ClearImageData();
                GC.Collect();
                StartRun = false;
            }
            catch
            {
                Capture3DCameraAsync(Camera3DDirc, false);
                StartRun = false;
            }

        }

        public void RunCamBatteryCavityStep(string Product, string BussyCam)
        {
            if (Product != "Cavity2" || MySubThreadFlow.Fixed)
            {
                AcqImage(Product, BussyCam);
            }
            RunCalibToolBlock(BussyCam);
            RunPositionToolBlock(Product, BussyCam);
        }

        public void RunCamBatteryCavityStepPic(string Product, string BussyCam)
        {
            RunCalibToolBlock(BussyCam);
            RunPositionToolBlock(Product, BussyCam);
        }

        public void RunCamNPtsCalib(string product, string BussyCam)
        {
            AcqImage(product, BussyCam);
            RunOnlyNptsToolBlock(BussyCam);
            RunNPointCalibrationToolBlock(BussyCam);
        }

        #endregion

        public MV_Form_Main()
        {
            InitializeComponent();


            //创建文件类
            CreatFileClass();
            //载入
            CreatLoadThread();
            //读取系统关键变量
            InitSytemVariable();
            //主窗体
            FixParentWindows();
            //子窗体
            CreatWindows();
            //子控件
            CreatUserControls();
            //全局变量
            InitGlobalVariable();
            //加载相机显示控件
            CreatLoad3DThread();
            CreatLoadVppThread();
            CameraShowControls();
            //注册所有委托
            CreatDelegateEvent();
            CreatSubFlow();
            CreatThread();
            //默认启动或关闭一些控件
            EnableControls();
            //程序启动完成日志
            OperateLog("程序开启！");
            //等待所有加载项载入完成
            WaitLoadingFinish();
            ShowInitMes();
            BringToFront();
        }

        private void UIstatusTimer_Tick(object sender, EventArgs e)
        {
            bottomlabel_time.Text = DateTime.Now.ToString("yyyy/MM/dd  HH:mm:ss");
            bottomlabel_UserMes.Text = "当前用户：" + MV_Global_Variable.GlobalCurrentUser;
            Authorization(MV_Global_Variable.GlobalCurrentLevel);
            if (boolProgramRunning)
            {
                if (!MyBoolRunCommand[0] && !MyBoolRunCommand[1])
                {
                    MyStringRunCommand = "Waitting...";
                }
            }
            else
            {
                MyStringRunCommand = "Puse";
            }
            bottomlabel_RunMes.Text = MyStringRunCommand;
            if (today != DateTime.Today.Day)
            {
                CheckSystemPath();
            }
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                NPtsStep = int.Parse(textBox1.Text);
                AxisX = double.Parse(textBox2.Text);
                AxisY = double.Parse(textBox3.Text);
                RunCamNPtsCalib("Mark", "Cam12");
            }
            catch
            { }
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                NPtsStep = int.Parse(textBox1.Text);
                AxisX = double.Parse(textBox2.Text);
                AxisY = double.Parse(textBox3.Text);
                RunCamNPtsCalib("CavityCheck", "Cam34");
            }
            catch
            { }
        }

        private void MainButton_Exit_Click(object sender, EventArgs e)
        {
            if (MV_Global_Variable.Language == "en-US")
            {
                DialogResult closedr = MessageBoxEX.ShowMessageBox("Attention!", "Is Exit?");
                if (closedr == DialogResult.OK)
                {
                    ExitApp();
                }
            }
            else
            {
                DialogResult closedr = MessageBoxEX.ShowMessageBox("注意！", "确认退出？");
                if (closedr == DialogResult.OK)
                {
                    ExitApp();
                }
            }

        }

        private void TOPtoolStrip_MouseDown(object sender, MouseEventArgs e)
        {
            //_boolMoveAllown = true;
            //_windowsMoveTempX = Cursor.Position.X;
            //_windowsMoveTempY = Cursor.Position.Y;
        }

        private void TOPtoolStrip_MouseUp(object sender, MouseEventArgs e)
        {
            //_boolMoveAllown = false;
        }

        private void TOPtoolStrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (_boolMoveAllown)
            {
                _windowsMoveDetaX = Cursor.Position.X - _windowsMoveTempX;
                _windowsMoveDetaY = Cursor.Position.Y - _windowsMoveTempY;
                _windowsMoveTempX = Cursor.Position.X;
                _windowsMoveTempY = Cursor.Position.Y;
                this.Location = new Point(this.Location.X + _windowsMoveDetaX, this.Location.Y + _windowsMoveDetaY);
            }
        }

        private void MainButton_Login_IN_Click(object sender, EventArgs e)
        {
            MyFormLogin.Show();
            MyFormLogin.TopMost = true;
        }

        private void MainButton_Login_COR_Click(object sender, EventArgs e)
        {

        }

        private void MainButton_Login_OUT_Click(object sender, EventArgs e)
        {
            MV_Global_Variable.MyFormMain.MyFormLogin.Logout();
        }

        private void MainButton_Minisize_Click(object sender, EventArgs e)
        {
            this.Hide();
            MasonteVision_Icon.Visible = true;
            //MasonteVision_Icon.ShowBalloonTip(100);
        }

        private void MasonteVision_Icon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.BringToFront();
                MasonteVision_Icon.Visible = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                MasonteVision_Icon.ContextMenuStrip = contextMenuStrip_Icon;
            }
        }

        private void TOPtoolStrip_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LocationParentWindows();
        }

        private void MainBotton_SearchData_Click(object sender, EventArgs e)
        {
            MyUserControlDataTable.Show();
            MyUserControlDataTable.BringToFront();
        }

        private void MainButton_BackMain_Click(object sender, EventArgs e)
        {
            CamShowMAXPanel.Show();
            CamShowMAXPanel.BringToFront();
        }

        private void MainButton_Run_Click(object sender, EventArgs e)
        {
            if (TopToolStrip.InvokeRequired)
            {

                TopToolStrip.Invoke((new EventHandler(delegate
                {
                    if (!boolProgramRunning)
                    {//程序启动
                        boolProgramRunning = true;
                        MainButton_Run.Image = Properties.Resources.暂停;
                        MainButton_Run.BackColor = Color.Yellow;
                        if (MV_Global_Variable.Language == "en-US")
                        {
                            MainButton_Run.Text = "Pause";
                        }
                        else
                        {
                            MainButton_Run.Text = "暂停程序";
                        }
                        OperateLog("程序运行");
                    }
                    else
                    {//程序暂停
                        boolProgramRunning = false;
                        MainButton_Run.Image = Properties.Resources.启动;
                        MainButton_Run.BackColor = SystemColors.Control;
                        if (MV_Global_Variable.Language == "en-US")
                        {
                            MainButton_Run.Text = "Run Program";
                        }
                        else
                        {
                            MainButton_Run.Text = "程序启动";
                        }
                        OperateLog("程序暂停");
                    }
                })));

            }
            else
            {
                if (!boolProgramRunning)
                {//程序启动
                    boolProgramRunning = true;
                    MainButton_Run.Image = Properties.Resources.暂停;
                    MainButton_Run.BackColor = Color.Yellow;
                    if (MV_Global_Variable.Language == "en-US")
                    {
                        MainButton_Run.Text = "Pause";
                    }
                    else
                    {
                        MainButton_Run.Text = "暂停程序";
                    }
                    OperateLog("程序运行");
                }
                else
                {//程序暂停
                    boolProgramRunning = false;
                    MainButton_Run.Image = Properties.Resources.启动;
                    MainButton_Run.BackColor = SystemColors.Control;
                    if (MV_Global_Variable.Language == "en-US")
                    {
                        MainButton_Run.Text = "Runing";
                    }
                    else
                    {
                        MainButton_Run.Text = "程序启动";
                    }
                    OperateLog("程序暂停");
                }
            }
        }

        private void MainButton_Settings_Click(object sender, EventArgs e)
        {
            MyUserControlParamSetting.Show();
            MyUserControlParamSetting.BringToFront();
        }

        private void MainButton_EthernetConnection_Click(object sender, EventArgs e)
        {
            //MyUserControlEthernetConnection.Show();
            //MyUserControlEthernetConnection.BringToFront();
        }

        private void MV_Form_Main_Shown(object sender, EventArgs e)
        {
            this.Enabled = true;
            MainButton_Run_Click(sender, e);
        }

        private void MainButton_Comm_Click(object sender, EventArgs e)
        {

            CstLightSet.Show();
            CstLightSet.TopMost = true;
        }

        private void enUSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBoxEX.ShowMessageBox("注意！", "语言设置 : English(US) ?");
            if (dr == DialogResult.OK)
            { Program.LanguageFile.WriteValue("System", "Language", "en-US"); }
        }

        private void 简体中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBoxEX.ShowMessageBox("Attention!", "Set Language : 中文（简体） ?");
            if (dr == DialogResult.OK)
            { Program.LanguageFile.WriteValue("System", "Language", ""); }
        }


    }
}
