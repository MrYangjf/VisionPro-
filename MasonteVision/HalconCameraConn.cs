using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace MasonteVision
{
    public delegate void OneFrameCallBackEventHandler(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser);
    public delegate void ErrorCallBackEventHandler(uint nMsgType, IntPtr pUser);
    public delegate void RunContinueCallBackEventHandler(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo);

    public class HikCameraConn
    {


        /// <summary>
        /// 声明图像回调委托
        /// </summary>
        private MyCamera.cbOutputExdelegate SDKImageCallBack;
        /// <summary>
        /// 声明相机报错回调委托
        /// </summary>
        private MyCamera.cbExceptiondelegate SDKErrorCallBack;
        /// <summary>
        /// 每帧图像获取结束后触发事件
        /// </summary>
        public event OneFrameCallBackEventHandler OneFrameCallBack;
        /// <summary>
        /// 相机报错后触发事件
        /// </summary>
        public event ErrorCallBackEventHandler ErrorCallBack;
        /// <summary>
        /// 持续取像时每次取像后触发
        /// </summary>
        public event RunContinueCallBackEventHandler RunContinueCallBack;
        /// <summary>
        /// 是否注册了取像回调函数
        /// </summary>
        public bool IsEnableImageCallBack = false;

        /// <summary>
        /// 相机总数量
        /// </summary>
        public int CameraCount = 0;

        public bool IniOk;

        #region 图像数据转化变量
        byte[] m_pDataForRed = new byte[20 * 1024 * 1024];
        byte[] m_pDataForGreen = new byte[20 * 1024 * 1024];
        byte[] m_pDataForBlue = new byte[20 * 1024 * 1024];
        uint g_nPayloadSize = 0;
        #endregion

        /// <summary>
        /// USB接口设备信息
        /// </summary>
        public List<string> USBDevices = new List<string>();
        /// <summary>
        /// 网口接口设备信息
        /// </summary>
        public List<string> GigeDevices = new List<string>();
        /// <summary>
        /// 所有设备清单
        /// </summary>
        MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        /// <summary>
        /// 相机取出的对象信息
        /// </summary>
        MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
        /// <summary>
        /// .NET相机类（绑定后通过此类获取相机信息和进行相机操作）
        /// </summary>
        public MyCamera m_MyCamera = new MyCamera();
        /// <summary>
        /// 循环取像标志位
        /// </summary>
        bool m_bGrabbing = false;
        /// <summary>
        /// 取像线程
        /// </summary>
        Thread m_hReceiveThread = null;
        /// <summary>
        /// 取像锁（用于单次执行取像函数时，该函数不会被其他地方调用）
        /// </summary>
        private static Object BufForDriverLock = new Object();
        /// <summary>
        /// 实例化时绑定带序列号的相机
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="IsCallImageBack">是否使用回调</param>
        public HikCameraConn(string SN, bool IsCallImageBack)
        {
            if (!IsCallImageBack)
            {
                DeviceListAcq();
               IniOk= BindingCamera(SN);
            }
            else
            {
                DeviceListAcq();
               IniOk= BindingCamera(SN);
                RegisterImageCallBack();
            }
        }
        /// <summary>
        /// 实例化时指定顺序绑定相机
        /// </summary>
        /// <param name="Index"></param>
        /// /// <param name="IsCallImageBack">是否使用回调</param>
        public HikCameraConn(int Index, bool IsCallImageBack)
        {
            if (!IsCallImageBack)
            {
                DeviceListAcq();
                BindingCamera(Index);
            }
            else
            {
                DeviceListAcq();
                BindingCamera(Index);
                RegisterImageCallBack();
            }
        }
        /// <summary>
        /// 实例化时不绑定相机
        /// </summary>
        public HikCameraConn()
        {
            DeviceListAcq();
        }
        /// <summary>
        /// 绑定指定SN的相机
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public bool BindingCamera(string SN)
        {
            int index = GetIndexBySN(SN);
            return InitDevice(index);
        }
        /// <summary>
        /// 绑定指定序列的相机
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public bool BindingCamera(int Index)
        {
            return InitDevice(Index);
        }

        ~HikCameraConn()
        {
            DisposeDevices();
        }

        /// <summary>
        /// 错误信息日提示
        /// </summary>
        /// <param name="csMessage">错误信息</param>
        /// <param name="nErrorNum">错误信息码</param>
        private void ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " 错误或非法句柄 "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " 不支持该功能 "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " 存储已满 "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " 功能命令行出错 "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " 修改参数失败 "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " 应用源错误 "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " 无数据反馈 "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " 运行环境改变 "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " 版本不匹配 "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " 容量不足 "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " 未知错误 "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " 通用错误 "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " 条件不可用 "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " 设备不可用或被占用 "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " 设备被占用或者网口连接失败 "; break;
                case MyCamera.MV_E_NETER: errorMsg += " 网口错误 "; break;
            }

            //MessageBox.Show(errorMsg, "提示");
        }
        /// <summary>
        /// 判断像素格式是否为黑白
        /// </summary>
        /// <param name="enType"></param>
        /// <returns></returns>
        private bool IsMonoPixelFormat(MyCamera.MvGvspPixelType enType)
        {
            switch (enType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 判断像素格式是否为彩色
        /// </summary>
        /// <param name="enType"></param>
        /// <returns></returns>
        private bool IsColorPixelFormat(MyCamera.MvGvspPixelType enType)
        {
            switch (enType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGBA8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGRA8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 图片格式转换为Mono8
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pInData"></param>
        /// <param name="pOutData"></param>
        /// <param name="nHeight"></param>
        /// <param name="nWidth"></param>
        /// <param name="nPixelType"></param>
        /// <returns></returns>
        public Int32 ConvertToMono8(object obj, IntPtr pInData, IntPtr pOutData, ushort nHeight, ushort nWidth, MyCamera.MvGvspPixelType nPixelType)
        {
            if (IntPtr.Zero == pInData || IntPtr.Zero == pOutData)
            {
                return MyCamera.MV_E_PARAMETER;
            }

            int nRet = MyCamera.MV_OK;
            MyCamera device = obj as MyCamera;
            MyCamera.MV_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

            stPixelConvertParam.pSrcData = pInData;//源数据
            if (IntPtr.Zero == stPixelConvertParam.pSrcData)
            {
                return -1;
            }

            stPixelConvertParam.nWidth = nWidth;//图像宽度
            stPixelConvertParam.nHeight = nHeight;//图像高度
            stPixelConvertParam.enSrcPixelType = nPixelType;//源数据的格式
            stPixelConvertParam.nSrcDataLen = (uint)(nWidth * nHeight * ((((uint)nPixelType) >> 16) & 0x00ff) >> 3);

            stPixelConvertParam.nDstBufferSize = (uint)(nWidth * nHeight * ((((uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed) >> 16) & 0x00ff) >> 3);
            stPixelConvertParam.pDstBuffer = pOutData;//转换后的数据
            stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            stPixelConvertParam.nDstBufferSize = (uint)(nWidth * nHeight * 3);

            nRet = device.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
            if (MyCamera.MV_OK != nRet)
            {
                return -1;
            }

            return nRet;
        }
        /// <summary>
        /// 图片格式转换为Mono8
        /// </summary>
        /// <param name="pInData"></param>
        /// <param name="pOutData"></param>
        /// <param name="nHeight"></param>
        /// <param name="nWidth"></param>
        /// <param name="nPixelType"></param>
        /// <returns></returns>
        public Int32 ConvertToMono8(IntPtr pInData, IntPtr pOutData, ushort nHeight, ushort nWidth, MyCamera.MvGvspPixelType nPixelType)
        {
            if (IntPtr.Zero == pInData || IntPtr.Zero == pOutData)
            {
                return MyCamera.MV_E_PARAMETER;
            }

            int nRet = MyCamera.MV_OK;
            MyCamera device = m_MyCamera;
            MyCamera.MV_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

            stPixelConvertParam.pSrcData = pInData;//源数据
            if (IntPtr.Zero == stPixelConvertParam.pSrcData)
            {
                return -1;
            }

            stPixelConvertParam.nWidth = nWidth;//图像宽度
            stPixelConvertParam.nHeight = nHeight;//图像高度
            stPixelConvertParam.enSrcPixelType = nPixelType;//源数据的格式
            stPixelConvertParam.nSrcDataLen = (uint)(nWidth * nHeight * ((((uint)nPixelType) >> 16) & 0x00ff) >> 3);

            stPixelConvertParam.nDstBufferSize = (uint)(nWidth * nHeight * ((((uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed) >> 16) & 0x00ff) >> 3);
            stPixelConvertParam.pDstBuffer = pOutData;//转换后的数据
            stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            stPixelConvertParam.nDstBufferSize = (uint)(nWidth * nHeight * 3);

            nRet = device.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
            if (MyCamera.MV_OK != nRet)
            {
                return -1;
            }

            return nRet;
        }
        /// <summary>
        /// 图片转化为RGB图片
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pSrc"></param>
        /// <param name="nHeight"></param>
        /// <param name="nWidth"></param>
        /// <param name="nPixelType"></param>
        /// <param name="pDst"></param>
        /// <returns></returns>
        public Int32 ConvertToRGB(object obj, IntPtr pSrc, ushort nHeight, ushort nWidth, MyCamera.MvGvspPixelType nPixelType, IntPtr pDst)
        {
            if (IntPtr.Zero == pSrc || IntPtr.Zero == pDst)
            {
                return MyCamera.MV_E_PARAMETER;
            }

            int nRet = MyCamera.MV_OK;
            MyCamera device = obj as MyCamera;
            MyCamera.MV_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

            stPixelConvertParam.pSrcData = pSrc;//源数据
            if (IntPtr.Zero == stPixelConvertParam.pSrcData)
            {
                return -1;
            }

            stPixelConvertParam.nWidth = nWidth;//图像宽度
            stPixelConvertParam.nHeight = nHeight;//图像高度
            stPixelConvertParam.enSrcPixelType = nPixelType;//源数据的格式
            stPixelConvertParam.nSrcDataLen = (uint)(nWidth * nHeight * ((((uint)nPixelType) >> 16) & 0x00ff) >> 3);

            stPixelConvertParam.nDstBufferSize = (uint)(nWidth * nHeight * ((((uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed) >> 16) & 0x00ff) >> 3);
            stPixelConvertParam.pDstBuffer = pDst;//转换后的数据
            stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            stPixelConvertParam.nDstBufferSize = (uint)nWidth * nHeight * 3;

            nRet = device.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
            if (MyCamera.MV_OK != nRet)
            {
                return -1;
            }

            return MyCamera.MV_OK;
        }
        /// <summary>
        /// 图片转化为RGB图片
        /// </summary>
        /// <param name="pSrc"></param>
        /// <param name="nHeight"></param>
        /// <param name="nWidth"></param>
        /// <param name="nPixelType"></param>
        /// <param name="pDst"></param>
        /// <returns></returns>
        public Int32 ConvertToRGB(IntPtr pSrc, ushort nHeight, ushort nWidth, MyCamera.MvGvspPixelType nPixelType, IntPtr pDst)
        {
            if (IntPtr.Zero == pSrc || IntPtr.Zero == pDst)
            {
                return MyCamera.MV_E_PARAMETER;
            }

            int nRet = MyCamera.MV_OK;
            MyCamera device = m_MyCamera;
            MyCamera.MV_PIXEL_CONVERT_PARAM stPixelConvertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

            stPixelConvertParam.pSrcData = pSrc;//源数据
            if (IntPtr.Zero == stPixelConvertParam.pSrcData)
            {
                return -1;
            }

            stPixelConvertParam.nWidth = nWidth;//图像宽度
            stPixelConvertParam.nHeight = nHeight;//图像高度
            stPixelConvertParam.enSrcPixelType = nPixelType;//源数据的格式
            stPixelConvertParam.nSrcDataLen = (uint)(nWidth * nHeight * ((((uint)nPixelType) >> 16) & 0x00ff) >> 3);

            stPixelConvertParam.nDstBufferSize = (uint)(nWidth * nHeight * ((((uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed) >> 16) & 0x00ff) >> 3);
            stPixelConvertParam.pDstBuffer = pDst;//转换后的数据
            stPixelConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            stPixelConvertParam.nDstBufferSize = (uint)nWidth * nHeight * 3;

            nRet = device.MV_CC_ConvertPixelType_NET(ref stPixelConvertParam);//格式转换
            if (MyCamera.MV_OK != nRet)
            {
                return -1;
            }

            return MyCamera.MV_OK;
        }
        /// <summary>
        /// 通过SN号获取相机Index
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public int GetIndexBySN(string SN)
        {
            int index = -1;
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                    if (SN == gigeInfo.chSerialNumber)
                    {
                        index = i;
                    }

                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (SN == usbInfo.chSerialNumber)
                    {
                        index = i;
                    }
                }
            }
            return index;
        }
        /// <summary>
        /// 获取设备清单
        /// </summary>
        private void DeviceListAcq()
        {
            // ch:创建设备列表 | en:Create Device List
            System.GC.Collect();
            m_stDeviceList.nDeviceNum = 0;
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_stDeviceList);
            if (0 != nRet)
            {
                ShowErrorMsg("查询设备是失败!", 0);
                return;
            }
            CameraCount = (int)m_stDeviceList.nDeviceNum;
            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        GigeDevices.Add("GEV: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        GigeDevices.Add("GEV: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        USBDevices.Add("U3V: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        USBDevices.Add("U3V: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }
            
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public bool InitDevice(int Index)
        {
           
            if (m_stDeviceList.nDeviceNum == 0)
            {
                ShowErrorMsg("无设备可用", 0);
                return false;
            }
            if (Index < m_stDeviceList.nDeviceNum)
            {
                MyCamera.MV_CC_DEVICE_INFO device =
                   (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[Index],
                                                                 typeof(MyCamera.MV_CC_DEVICE_INFO));

                if (null == m_MyCamera)
                {
                    m_MyCamera = new MyCamera();
                    if (null == m_MyCamera)
                    {
                        return false;
                    }
                }

                int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }

                nRet = m_MyCamera.MV_CC_OpenDevice_NET();

                if (MyCamera.MV_OK != nRet)
                {
                    m_MyCamera.MV_CC_DestroyDevice_NET();
                    ShowErrorMsg("设备打开失败!", nRet);
                    return false;
                }

                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        if (nRet != MyCamera.MV_OK)
                        {
                            ShowErrorMsg("数据包大小设置失败!", nRet);
                        }
                    }
                    else
                    {
                        ShowErrorMsg("数据包大小获取失败!", nPacketSize);
                    }
                }

                // ch:获取包大小 || en: Get Payload Size
                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                nRet = m_MyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("Get PayloadSize Fail");
                    return false;
                }
                g_nPayloadSize = stParam.nCurValue;

                // ch:获取高 || en: Get Height
                nRet = m_MyCamera.MV_CC_GetIntValue_NET("Height", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("Get Height Fail");
                    return false;
                }
                uint nHeight = stParam.nCurValue;

                // ch:获取宽 || en: Get Width
                nRet = m_MyCamera.MV_CC_GetIntValue_NET("Width", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("Get Width Fail");
                    return false;
                }
                uint nWidth = stParam.nCurValue;

                m_pDataForRed = new byte[nWidth * nHeight];
                m_pDataForGreen = new byte[nWidth * nHeight];
                m_pDataForBlue = new byte[nWidth * nHeight];



                // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                

                RegisterErrorCallBack();

                return true;
            }
            else
            { return false; }
        }
    
        /// <summary>
        /// 取像
        /// </summary>
        public bool Grab()
        {
            m_bGrabbing = true;
            if (!IsEnableImageCallBack)
            {
                if (m_hReceiveThread == null)
                {
                    m_hReceiveThread = new Thread(ReceiveThreadProcess);
                    m_hReceiveThread.IsBackground = true;
                    m_hReceiveThread.Start(m_MyCamera);
                }
            }

            // ch:开始采集 | en:Start Grabbing
            int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_bGrabbing = false;
               
                ShowErrorMsg("取像失败!", nRet);
                return m_bGrabbing;
            }
            return true;
        }
        /// <summary>
        /// 设置为触发模式
        /// </summary>
        public void SetSoftTriMode()
        {
            ChagetoTriggerMode(true);
            ChoseTriggerSouce(7);
            Grab();

        }
        /// <summary>
        /// 停止取像
        /// </summary>
        public void Stop()
        {
            // ch:标志位设为false | en:Set flag bit false
            m_bGrabbing = false;
            if (!IsEnableImageCallBack)
            { m_hReceiveThread = null; }
            // ch:停止采集 | en:Stop Grabbing
            int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("停止取像失败!", nRet);
            }
        }

        void RegisterErrorCallBack()
        {
            SDKErrorCallBack = new MyCamera.cbExceptiondelegate(OnErrorCallBack);
            int nRet = m_MyCamera.MV_CC_RegisterExceptionCallBack_NET(SDKErrorCallBack, IntPtr.Zero);
            if (MyCamera.MV_OK != nRet)
            {
                //注册连接失败函数错误
                return;
            }
            GC.KeepAlive(SDKErrorCallBack);
        }

        void OnErrorCallBack(uint nMsgType, IntPtr pUser)
        {
            if (ErrorCallBack != null)
            { ErrorCallBack(nMsgType, pUser); }
        }
        /// <summary>
        /// 注册图像回调
        /// </summary>
        public void RegisterImageCallBack()
        {
            SDKImageCallBack = new MyCamera.cbOutputExdelegate(OnSDKImageCallBack);

            int nRet = m_MyCamera.MV_CC_RegisterImageCallBackEx_NET(SDKImageCallBack, IntPtr.Zero);
            if (MyCamera.MV_OK != nRet)
            {
                //注册回调函数失败
                IsEnableImageCallBack = false;
                return;
            }
            IsEnableImageCallBack = true;
        }
        void OnSDKImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            IntPtr pImageBuffer = Marshal.AllocHGlobal((int)g_nPayloadSize * 3);

            IntPtr RedPtr = IntPtr.Zero;
            IntPtr GreenPtr = IntPtr.Zero;
            IntPtr BluePtr = IntPtr.Zero;
            IntPtr pTemp = IntPtr.Zero;
            

            if (IsColorPixelFormat(pFrameInfo.enPixelType))
            {
                if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
                {
                    pTemp = pData;
                }
                else
                {
                    int nRet = ConvertToRGB(pData, pFrameInfo.nHeight, pFrameInfo.nWidth, pFrameInfo.enPixelType, pImageBuffer);
                    if (MyCamera.MV_OK != nRet)
                    {
                        return;
                    }
                    pTemp = pImageBuffer;
                }

                unsafe
                {
                    byte* pBufForSaveImage = (byte*)pTemp;

                    UInt32 nSupWidth = (pFrameInfo.nWidth + (UInt32)3) & 0xfffffffc;

                    for (int nRow = 0; nRow < pFrameInfo.nHeight; nRow++)
                    {
                        for (int col = 0; col < pFrameInfo.nWidth; col++)
                        {
                            m_pDataForRed[nRow * nSupWidth + col] = pBufForSaveImage[nRow * pFrameInfo.nWidth * 3 + (3 * col)];
                            m_pDataForGreen[nRow * nSupWidth + col] = pBufForSaveImage[nRow * pFrameInfo.nWidth * 3 + (3 * col + 1)];
                            m_pDataForBlue[nRow * nSupWidth + col] = pBufForSaveImage[nRow * pFrameInfo.nWidth * 3 + (3 * col + 2)];
                        }
                    }
                }

                RedPtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pDataForRed, 0);
                GreenPtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pDataForGreen, 0);
                BluePtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pDataForBlue, 0);


            }
            else if (IsMonoPixelFormat(pFrameInfo.enPixelType))
            {
                if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    pTemp = pData;
                }
                else
                {
                    int nRet = ConvertToMono8(pData, pImageBuffer, pFrameInfo.nHeight, pFrameInfo.nWidth, pFrameInfo.enPixelType);
                    if (MyCamera.MV_OK != nRet)
                    {
                        return;
                    }
                    pTemp = pImageBuffer;
                }
                
            }
            OneFrameCallBack?.Invoke(pData, ref pFrameInfo, pUser);
            pData = IntPtr.Zero;
        }

        /// <summary>
        /// 释放相机资源
        /// </summary>
        public void DisposeDevices()
        {
            // ch:取流标志位清零 | en:Reset flow flag bit
            try
            {
                if (m_bGrabbing == true)
                {
                    m_bGrabbing = false;
                    m_hReceiveThread = null;
                }


                // ch:关闭设备 | en:Close Device
                m_MyCamera.MV_CC_CloseDevice_NET();
                m_MyCamera.MV_CC_DestroyDevice_NET();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        /// <summary>
        /// 关闭触发模式
        /// </summary>
        public void ChagetoContinuesMode()
        {
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
        }
        /// <summary>
        /// 选择触发模式
        /// </summary>
        /// <param name="isTrigger"></param>
        public void ChagetoTriggerMode(bool isTrigger)
        {
            // ch:打开触发模式 | en:Open Trigger Mode
            if (isTrigger)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
            }
            else
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
            }

            // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
            //           1 - Line1;
            //           2 - Line2;
            //           3 - Line3;
            //           4 - Counter;
            //           7 - Software;




        }
        /// <summary>
        /// 选择触发源
        /// </summary>
        /// <param name="index"></param>
        public void ChoseTriggerSouce(int index)
        {
            if (m_bGrabbing)
            {
                m_bGrabbing = false;
            }

            // ch:触发源选择:0 - Line0; | en:Trigger source select:0 - Line0;
            //           1 - Line1;
            //           2 - Line2;
            //           3 - Line3;
            //           4 - Counter;
            //           7 - Software;
            if (index == 7)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
            }

            if (index == 0)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
            }

            if (index == 1)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE1);
            }

            if (index == 2)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE2);
            }

            if (index == 3)
            {
                m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE3);
            }
        }
        /// <summary>
        /// 发送软触发命令
        /// </summary>
        public void SoftTriggerOnce()
        {
            int nRet = m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                ShowErrorMsg("软触发失败!", nRet);
            }
        }
        /// <summary>
        /// 取像线程功能函数
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveThreadProcess(object obj)
        {
            int nRet = MyCamera.MV_OK;
            MyCamera device = obj as MyCamera;

            IntPtr pData = Marshal.AllocHGlobal((int)g_nPayloadSize * 3);
            if (pData == IntPtr.Zero)
            {
                return;
            }
            IntPtr pImageBuffer = Marshal.AllocHGlobal((int)g_nPayloadSize * 3);
            if (pImageBuffer == IntPtr.Zero)
            {
                return;
            }

            uint nDataSize = g_nPayloadSize * 3;

            IntPtr RedPtr = IntPtr.Zero;
            IntPtr GreenPtr = IntPtr.Zero;
            IntPtr BluePtr = IntPtr.Zero;
            IntPtr pTemp = IntPtr.Zero;

            while (m_bGrabbing)
            {
                nRet = device.MV_CC_GetOneFrameTimeout_NET(pData, nDataSize, ref pFrameInfo, 1000);
                if (MyCamera.MV_OK == nRet)
                {
                    if (IsColorPixelFormat(pFrameInfo.enPixelType))
                    {
                        if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
                        {
                            pTemp = pData;
                        }
                        else
                        {
                            nRet = ConvertToRGB(obj, pData, pFrameInfo.nHeight, pFrameInfo.nWidth, pFrameInfo.enPixelType, pImageBuffer);
                            if (MyCamera.MV_OK != nRet)
                            {
                                return;
                            }
                            pTemp = pImageBuffer;
                        }

                        unsafe
                        {
                            byte* pBufForSaveImage = (byte*)pTemp;

                            UInt32 nSupWidth = (pFrameInfo.nWidth + (UInt32)3) & 0xfffffffc;

                            for (int nRow = 0; nRow < pFrameInfo.nHeight; nRow++)
                            {
                                for (int col = 0; col < pFrameInfo.nWidth; col++)
                                {
                                    m_pDataForRed[nRow * nSupWidth + col] = pBufForSaveImage[nRow * pFrameInfo.nWidth * 3 + (3 * col)];
                                    m_pDataForGreen[nRow * nSupWidth + col] = pBufForSaveImage[nRow * pFrameInfo.nWidth * 3 + (3 * col + 1)];
                                    m_pDataForBlue[nRow * nSupWidth + col] = pBufForSaveImage[nRow * pFrameInfo.nWidth * 3 + (3 * col + 2)];
                                }
                            }
                        }

                        RedPtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pDataForRed, 0);
                        GreenPtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pDataForGreen, 0);
                        BluePtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pDataForBlue, 0);

                       
                    }
                    else if (IsMonoPixelFormat(pFrameInfo.enPixelType))
                    {
                        if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                        {
                            pTemp = pData;
                        }
                        else
                        {
                            nRet = ConvertToMono8(device, pData, pImageBuffer, pFrameInfo.nHeight, pFrameInfo.nWidth, pFrameInfo.enPixelType);
                            if (MyCamera.MV_OK != nRet)
                            {
                                return;
                            }
                            pTemp = pImageBuffer;
                        }
                       
                    }
                    else
                    {
                        continue;
                    }
                    if (RunContinueCallBack != null)
                    {
                        RunContinueCallBack?.Invoke(pData, ref pFrameInfo);

                    }

                }
                else
                {
                    continue;
                }
            }
            if (pData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pData);
            }
            if (pImageBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pImageBuffer);
            }
            return;

        }
        /// <summary>
        /// 获取相机参数
        /// </summary>
        /// <param name="str_ParamName"></param>
        /// <returns></returns>
        public string GetCameraParam(string str_ParamName)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET(str_ParamName, ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                return stParam.fCurValue.ToString();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="str_ParamName"></param>
        /// <param name="str_Value"></param>
        public void SetCameraParam(string str_ParamName, string str_Value)
        {
            try
            {
                int nRet = m_MyCamera.MV_CC_SetFloatValue_NET(str_ParamName, float.Parse(str_Value));
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Set  Fail!", nRet);
                }
            }
            catch
            { }
        }
        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="str_ParamName"></param>
        /// <param name="Enum_Value"></param>
        public void SetCameraParam(string str_ParamName, int Enum_Value)
        {
            int nRet = m_MyCamera.MV_CC_SetEnumValue_NET(str_ParamName, (uint)Enum_Value);
            if (nRet != MyCamera.MV_OK)
            {
                ShowErrorMsg("设置失败!", nRet);
            }
        }
        /// <summary>
        /// 获取图像格式是否超出预期
        /// </summary>
        /// <param name="enPixelFormat"></param>
        /// <returns></returns>
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
