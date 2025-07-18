

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro3D;
using System.Runtime.InteropServices;

namespace MasonteVision
{
    public partial class MV_UC_ParametersSetting : UserControl
    {

        [DllImport("kernel32")]
        public static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        private List<Label> _messageLabel = new List<Label>();
        private List<TextBox> _MessageNumeric = new List<TextBox>();


        public MV_UC_ParametersSetting()
        {
            InitializeComponent();
            ReadCurrentVariable();
            AddProjectList();
        }

        void AddProjectList()
        {
            comboBox3.Items.Clear();
            for (int i = 0; i < MV_Global_Variable.myProjectList.Count; i++)
            {
                comboBox3.Items.Add(MV_Global_Variable.myProjectList[i]);
                if (MV_Global_Variable.myProjectList[i] == MV_Global_Variable.ProjectName) comboBox3.SelectedIndex = i;
            }
        }

        public void ReadCurrentVariable()
        {
            ImageHeight.Text = MV_Global_Variable.MyFormMain.ImageHeight;

            Cam1Expouse.Text = MV_Global_Variable.CameraBackExposure1.ToString();
            Cam2Expouse.Text = MV_Global_Variable.CameraBackExposure2.ToString();


            MarkExpouse1.Text = MV_Global_Variable.CameraHandMarkExposure1.ToString();
            MarkExpouse2.Text = MV_Global_Variable.CameraHandMarkExposure2.ToString();


            CavityExpouse3.Text = MV_Global_Variable.CameraCavityExposure3.ToString();
            CavityExpouse4.Text = MV_Global_Variable.CameraCavityExposure4.ToString();


            CalExpouse3.Text = MV_Global_Variable.CameraCalOffsetExposure3.ToString();
            CalExpouse4.Text = MV_Global_Variable.CameraCalOffsetExposure4.ToString();

            LightValue1.Text = MV_Global_Variable.MyFormMain.LightValue1.ToString();
            LightValue2.Text = MV_Global_Variable.MyFormMain.LightValue2.ToString();
            LightValue3.Text = MV_Global_Variable.MyFormMain.LightValue3.ToString();
            LightValue4.Text = MV_Global_Variable.MyFormMain.LightValue4.ToString();
            LightValue5.Text = MV_Global_Variable.MyFormMain.LightValue5.ToString();

            Channel1.Text = MV_Global_Variable.MyFormMain.LightChannel1.ToString();
            Channel2.Text = MV_Global_Variable.MyFormMain.LightChannel2.ToString();
            Channel3.Text = MV_Global_Variable.MyFormMain.LightChannel3.ToString();
            Channel4.Text = MV_Global_Variable.MyFormMain.LightChannel4.ToString();
            Channel5.Text = MV_Global_Variable.MyFormMain.LightChannel5.ToString();

            AddOffsetA1.Text = MV_Global_Variable.AddOffsetA1.ToString();
            AddOffsetX1.Text = MV_Global_Variable.AddOffsetX1.ToString();
            AddOffsetY1.Text = MV_Global_Variable.AddOffsetY1.ToString();

            AddOffsetA2.Text = MV_Global_Variable.AddOffsetA2.ToString();
            AddOffsetX2.Text = MV_Global_Variable.AddOffsetX2.ToString();
            AddOffsetY2.Text = MV_Global_Variable.AddOffsetY2.ToString();

            HeightValueSetting.Text = MV_Global_Variable.HeightValueSetting.ToString();

            VariableShow_SystemTimeout.Text = MV_Global_Variable.GlobalLoadingTimeout.ToString();

            VariableShow_OKImageStorePath.Text = MV_Global_Variable.ImageStorePath;
            VariableShow_ImageStoreDays.Text = MV_Global_Variable.ImageStoreDays;

            checkBox1.Checked = MV_Global_Variable.Save3DImage;
            checkBox2.Checked = MV_Global_Variable.Save3DOKImage;
            checkBox3.Checked = MV_Global_Variable.Save3DNGImage;
            checkBox4.Checked = MV_Global_Variable.SaveOriImage;
            teMark1OffsetX.Text = MV_Global_Variable.BatteryMarkOffsetX1.ToString();
            teMark1OffsetY.Text = MV_Global_Variable.BatteryMarkOffsetY1.ToString();
            teMark1OffsetA.Text = MV_Global_Variable.BatteryMarkOffsetA1.ToString();

            teMark2OffsetX.Text = MV_Global_Variable.BatteryMarkOffsetX2.ToString();
            teMark2OffsetY.Text = MV_Global_Variable.BatteryMarkOffsetY2.ToString();
            teMark2OffsetA.Text = MV_Global_Variable.BatteryMarkOffsetA2.ToString();

            comboBox1.SelectedIndex = MV_Global_Variable.Battery1BarcodeCam;
            comboBox2.SelectedIndex = MV_Global_Variable.Battery2BarcodeCam;

            tebLightValue6.Text = MV_Global_Variable.LightValue6.ToString();
            SmartRayExpTime.Text = MV_Global_Variable.SmartRayExpTime.ToString();
            SmartRayLaserBrightness.Text = MV_Global_Variable.SmartRayLaserBrightness.ToString();
            SmartRayLaserThresold.Text = MV_Global_Variable.SmartRayLaserThreshold.ToString();
        }

        private void ChangeCurrentVariable()
        {
            MV_Global_Variable.MyFormMain.ImageHeight = ImageHeight.Text;

            MV_Global_Variable.GlobalLoadingTimeout = int.Parse(VariableShow_SystemTimeout.Text);

            MV_Global_Variable.MyFormMain.LightChannel1 = int.Parse(Channel1.Text);
            MV_Global_Variable.MyFormMain.LightChannel2 = int.Parse(Channel2.Text);
            MV_Global_Variable.MyFormMain.LightChannel3 = int.Parse(Channel3.Text);
            MV_Global_Variable.MyFormMain.LightChannel4 = int.Parse(Channel4.Text);
            MV_Global_Variable.MyFormMain.LightChannel5 = int.Parse(Channel5.Text);

            MV_Global_Variable.MyFormMain.LightValue1 = int.Parse(LightValue1.Text);
            MV_Global_Variable.MyFormMain.LightValue2 = int.Parse(LightValue2.Text);
            MV_Global_Variable.MyFormMain.LightValue3 = int.Parse(LightValue3.Text);
            MV_Global_Variable.MyFormMain.LightValue4 = int.Parse(LightValue4.Text);
            MV_Global_Variable.MyFormMain.LightValue5 = int.Parse(LightValue5.Text);

            MV_Global_Variable.CameraBackExposure1 = Cam1Expouse.Text;
            MV_Global_Variable.CameraBackExposure2 = Cam2Expouse.Text;


            MV_Global_Variable.CameraHandMarkExposure1 = MarkExpouse1.Text;
            MV_Global_Variable.CameraHandMarkExposure2 = MarkExpouse2.Text;



            MV_Global_Variable.CameraCavityExposure3 = CavityExpouse3.Text;
            MV_Global_Variable.CameraCavityExposure4 = CavityExpouse4.Text;


            MV_Global_Variable.CameraCalOffsetExposure3 = CalExpouse3.Text;
            MV_Global_Variable.CameraCalOffsetExposure4 = CalExpouse4.Text;

            MV_Global_Variable.AddOffsetA1 = AddOffsetA1.Text;
            MV_Global_Variable.AddOffsetX1 = AddOffsetX1.Text;
            MV_Global_Variable.AddOffsetY1 = AddOffsetY1.Text;

            MV_Global_Variable.AddOffsetA2 = AddOffsetA2.Text;
            MV_Global_Variable.AddOffsetX2 = AddOffsetX2.Text;
            MV_Global_Variable.AddOffsetY2 = AddOffsetY2.Text;

            MV_Global_Variable.HeightValueSetting = HeightValueSetting.Text;

            MV_Global_Variable.ImageStorePath = VariableShow_OKImageStorePath.Text;
            MV_Global_Variable.ImageStoreDays = VariableShow_ImageStoreDays.Text;

            MV_Global_Variable.Save3DImage = checkBox1.Checked;
            MV_Global_Variable.Save3DOKImage = checkBox2.Checked;
            MV_Global_Variable.Save3DNGImage = checkBox3.Checked;
            MV_Global_Variable.SaveOriImage = checkBox4.Checked;
            MV_Global_Variable.BatteryMarkOffsetX1 = teMark1OffsetX.Text;
            MV_Global_Variable.BatteryMarkOffsetY1 = teMark1OffsetY.Text;
            MV_Global_Variable.BatteryMarkOffsetA1 = teMark1OffsetA.Text;

            MV_Global_Variable.BatteryMarkOffsetX2 = teMark2OffsetX.Text;
            MV_Global_Variable.BatteryMarkOffsetY2 = teMark2OffsetY.Text;
            MV_Global_Variable.BatteryMarkOffsetA2 = teMark2OffsetA.Text;

            MV_Global_Variable.Battery1BarcodeCam = comboBox1.SelectedIndex;
            MV_Global_Variable.Battery2BarcodeCam = comboBox2.SelectedIndex;
            int.TryParse(tebLightValue6.Text, out MV_Global_Variable.LightValue6);
            int.TryParse(SmartRayExpTime.Text, out MV_Global_Variable.SmartRayExpTime);
            int.TryParse(SmartRayLaserBrightness.Text, out MV_Global_Variable.SmartRayLaserBrightness);
            int.TryParse(SmartRayLaserThresold.Text, out MV_Global_Variable.SmartRayLaserThreshold);
            MV_Global_Variable.MyFormMain.CstLightSet.mController.SetDigitalValue(6, MV_Global_Variable.LightValue6);
        }

        private void toolStripButton_ReadCurrent_Click(object sender, EventArgs e)
        {
            MV_Global_Variable.MyFormMain.RecordParamCor("读取运行参数");
            ReadCurrentVariable();

        }

        private void toolStripButton_ApplyToSystem_Click(object sender, EventArgs e)
        {
            MV_Global_Variable.MyFormMain.RecordParamCor("修改运行参数");
            ChangeCurrentVariable();
        }

        private void toolStripButton_SaveToLocal_Click(object sender, EventArgs e)
        {
            MV_Global_Variable.MyFormMain.RecordParamCor("保存运行参数");
            MV_Global_Variable.MyFormMain.SaveVariable();
            MV_Global_Variable.MyFormMain.SaveThreadLock = true;
        }

        private void btnCalBat_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam12;
            //cogToolBlockEditV21.Subject.Inputs["Cam1"].Value = MV_Global_Variable.MyFormMain.cogCalibrationImgCam1;
            //cogToolBlockEditV21.Subject.Inputs["Cam2"].Value = MV_Global_Variable.MyFormMain.cogCalibrationImgCam2;
            //cogToolBlockEditV21.Subject.Run();

        }

        private void btnCalHand_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam1QRCode;
        }

        private void btnCalA_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam34;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam34Check;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam5Code;
        }

        private void btnCalib_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogNPointCalibrationTool;
        }

        private void btnCam1_Click(object sender, EventArgs e)
        {
            cogAcqFifoEditV21.Subject = MV_Global_Variable.MyFormMain.cogAcqCam1;
        }

        private void btnCam2_Click(object sender, EventArgs e)
        {
            cogAcqFifoEditV21.Subject = MV_Global_Variable.MyFormMain.cogAcqCam2;
        }

        private void btnCam3_Click(object sender, EventArgs e)
        {
            cogAcqFifoEditV21.Subject = MV_Global_Variable.MyFormMain.cogAcqCam3;
        }

        private void btnCam4_Click(object sender, EventArgs e)
        {
            cogAcqFifoEditV21.Subject = MV_Global_Variable.MyFormMain.cogAcqCam4;
        }



        private void Button5_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam34Cal;
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam34Cavity2;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            cogCalibNPointToNPointEditV21.Subject = MV_Global_Variable.MyFormMain.cogCalibNPointToNPointToolPosition[1];
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            cogCalibNPointToNPointEditV21.Subject = MV_Global_Variable.MyFormMain.cogCalibNPointToNPointToolPosition[3];
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            cogCalibCheckerboardEditV22.Subject = MV_Global_Variable.MyFormMain.cogCalibCheckerboardToolCam1;
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            cogCalibCheckerboardEditV22.Subject = MV_Global_Variable.MyFormMain.cogCalibCheckerboardToolCam2;
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            cogCalibCheckerboardEditV22.Subject = MV_Global_Variable.MyFormMain.cogCalibCheckerboardToolCam3;
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            cogCalibCheckerboardEditV22.Subject = MV_Global_Variable.MyFormMain.cogCalibCheckerboardToolCam4;
        }

        private void btnChangeImagePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog MyfolderBrowserDialog = new FolderBrowserDialog();
            DialogResult dialogResult = MyfolderBrowserDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                VariableShow_OKImageStorePath.Text = MyfolderBrowserDialog.SelectedPath;
            }
        }

        private void Cam5Expouse_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCalBat2_Click(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlockCam12BigBattery;
        }

        private void LoadProject_Click(object sender, EventArgs e)
        {
            MV_Global_Variable.MyFormMain.LoadProjectValue(comboBox3.SelectedItem.ToString());
            ReadCurrentVariable();
        }

        private void AddProject_Click(object sender, EventArgs e)
        {
            string NewProject = tebNewProjectName.Text;
            MV_Global_Variable.MyFormMain.AddProject(NewProject);
            MV_Global_Variable.MyFormMain.ReadProjectList();
            AddProjectList();
        }

        private void btnScan1Check1_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam1;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam1, MV_Global_Variable.MyFormMain.MainParameter1, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam1.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D1.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter1 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan2Check1_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam2;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam2, MV_Global_Variable.MyFormMain.MainParameter2, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam2.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D2.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter2 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan1Check2_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam12;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam12, MV_Global_Variable.MyFormMain.MainParameter12, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam12.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D12.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter12 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan2Check2_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam22;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam22, MV_Global_Variable.MyFormMain.MainParameter22, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam22.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D22.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter22 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan3Check1_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam3;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam3, MV_Global_Variable.MyFormMain.MainParameter3, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam3.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D3.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter3 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan4Check1_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam4;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam4, MV_Global_Variable.MyFormMain.MainParameter4, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam4.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D4.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter4 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan3Check2_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam32;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam32, MV_Global_Variable.MyFormMain.MainParameter32, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam32.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D32.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter32 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        private void btnScan4Check2_Click(object sender, EventArgs e)
        {
            Point stPoint = new Point(50, 50);
            cogToolBlockEditV22.Subject = MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam42;
            ConfigerForm.Form2 ConfigerVPP = new ConfigerForm.Form2(panel3Dscan.Size, stPoint, MV_Global_Variable.MyFormMain.ConfigerDisplayLower, MV_Global_Variable.MyFormMain.ConfigerDisplayUpper);
            ConfigerVPP.SetParam(MV_Global_Variable.MyFormMain.cogRunToolBlock3DCam42, MV_Global_Variable.MyFormMain.MainParameter42, MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "cogRunToolBlock3DCam42.vpp");
            ConfigerVPP.SetVpxPath(MV_Global_Variable.MyFormMain.RunToolBlcokPath2 + "3D42.vpx");
            ConfigerVPP.ShowDialog();
            MV_Global_Variable.MyFormMain.MainParameter42 = ConfigerVPP.MainParameter;
            MV_Global_Variable.MyFormMain.ConfigerDisplayLower = ConfigerVPP.DisplayLower;
            MV_Global_Variable.MyFormMain.ConfigerDisplayUpper = ConfigerVPP.DisplayUpper;
        }

        public void EraseSection(string sectionName)
        {
            WritePrivateProfileString(sectionName, null, null, Application.StartupPath + "\\INI\\Debug.ini");
        }

        private void btnDeleteProject_Click(object sender, EventArgs e)
        {
            DialogResult re = MessageBoxEX.ShowMessageBox("删除工艺提示", string.Format("请确认是否删除配置：{0}", comboBox3.SelectedItem.ToString()));
            if (re == DialogResult.OK)
            {
                EraseSection(comboBox3.SelectedItem.ToString());
                MV_Global_Variable.MyFormMain.ReadProjectList();
                AddProjectList();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
