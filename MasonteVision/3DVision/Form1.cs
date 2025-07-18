using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


using Cognex.VisionPro;
using Cognex.VisionPro3D;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.PMAlign;

using VPPConfiger;
using System.Collections;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ImageProcessing;

namespace ConfigerForm
{
    public partial class Form2 : Form
    {
        public CogToolBlock theVpp = null;
        public Cognex.VisionPro.ICogImage theImage = null;
        public Parameter MainParameter = new Parameter();

        public double DisplayLower, DisplayUpper;

        public string m_path;
        public string m_vpxPath;
        //public Size resetSize;
        //public Point resetLocation;

        private bool bLoginAdmin;

        CogImageFile theImageFile = new CogImageFile();

        public Form2(Size resetSize, Point resetLocation, double Lower, double Upper)
        {
            InitializeComponent();
            bLoginAdmin = true;
            EnableEditMak(bLoginAdmin);
            this.Size = resetSize;
            this.Location = resetLocation;
            cogRecordDisplay1.ColorMapPredefined = CogDisplayColorMapPredefinedConstants.Grey;
            DisplayLower = Lower;
            DisplayUpper = Upper;
            cogRecordDisplay1.ColorMapLowerRoiLimit = Lower;
            cogRecordDisplay1.ColorMapUpperRoiLimit = Upper;
        }


        public void SetVpxPath(string strVpxPath)
        {

            m_vpxPath = strVpxPath;
        }
        public void SetParam(CogToolBlock checkvpp, Parameter inParameter, string inpath)
        {
            theVpp = checkvpp;
            MainParameter = inParameter;

            //MainParameter.DSerializParam(finder.FileName, out MainParameter);

            if (MainParameter != null)
            {
                Param_Add_Show(MainParameter);
            }


            m_path = inpath;
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog finder = new OpenFileDialog();
            finder.Filter = "算法文件(*.vpp)|*.vpp|所有文件(*.*)|*.*";

            if (finder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //tbVPPName.Text = "加载中...............";
                    theVpp = Cognex.VisionPro.CogSerializer.LoadObjectFromFile(finder.FileName) as CogToolBlock;
                    //tbVPPName.Text = finder.FileName;
                    MessageBox.Show("加载成功！");
                }
                catch (Exception ex)
                {
                    theVpp = null;
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("加载失败！");
                }
            }
        }

        private void btnLoadRange_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileFinder = new OpenFileDialog();
            fileFinder.Filter = "深度图(*.idb)|*.idb|所有文件(*.*)|*.*";
            if (fileFinder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //CogImageF
                CogImageFile theImageFile = new CogImageFile();
                theImageFile.Open(fileFinder.FileName, CogImageFileModeConstants.Read);
                CogImage16Range theRangeImage = theImageFile[0] as CogImage16Range;
                theImageFile.Close();
                if (theRangeImage == null)
                {
                    MessageBox.Show("未能加载文件");
                }
                else
                {
                    if (theVpp != null)
                    {
                        theVpp.Inputs[0].Value = theRangeImage;
                        theVpp.Run();
                        this.cogRecordDisplay1.Image = theImage;
                        this.cogRecordDisplay1.Record = theVpp.CreateLastRunRecord().SubRecords[0];
                        cogRecordDisplay1.Fit(true);

                    }
                    else
                    {

                        MessageBox.Show("工具没有准备好");
                    }


                }
            }

        }

        private void btnComfigPMAlign_Click(object sender, EventArgs e)
        {
            //try
            //{

            //    if (theVpp == null)
            //    {
            //        MessageBox.Show("theVpp不存在！");
            //        return;

            //    }
            //    Cognex.VisionPro.PMAlign.CogPMAlignEditV2 toolEditor = new Cognex.VisionPro.PMAlign.CogPMAlignEditV2();
            //    GenericToolEditor helper = new GenericToolEditor();
            //    helper.theTool = toolEditor;

            //    helper.InitializeComponent();
            //    ICogTool theTool = getToolByTreeName(theVpp, "Locate/CogPMAlignTool-E");
            //    toolEditor.Subject = theTool as Cognex.VisionPro.PMAlign.CogPMAlignTool;

            //    helper.Show();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

        private void btnConfigAreaCheck_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                CogGraphicInteractiveCollection theMaskRegions = GeneralCheck.Inputs["MaskRegions"].Value as CogGraphicInteractiveCollection;
                CogRectangleAffine theMainRegion = GeneralCheck.Inputs["Region"].Value as CogRectangleAffine;

                this.cogRecordDisplay1.Image = GeneralCheck.Inputs["GreyWithCord"].Value as ICogImage;
                this.cogRecordDisplay1.InteractiveGraphics.Clear();
                this.cogRecordDisplay1.InteractiveGraphics.Add(theMainRegion, "Main", false);
                this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                cogRecordDisplay1.Fit(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (theVpp == null)
            //{
            //    MessageBox.Show("theVpp不存在！");
            //    return;

            //}
            //SaveFileDialog finder = new SaveFileDialog();
            //finder.Filter = "算法文件(*.vpp)|*.vpp|所有文件(*.*)|*.*";

            //if (finder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    try
            //    {
            //        Cognex.VisionPro.CogSerializer.SaveObjectToFile(theVpp, finder.FileName);
            //        MessageBox.Show("保存成功！");
            //    }
            //    catch (Exception ex)
            //    {
            //        theVpp = null;
            //        MessageBox.Show(ex.Message);
            //        MessageBox.Show("加载失败！");
            //    }
            //}

            CogSerializer.SaveObjectToFile(theVpp, m_path);
            MessageBox.Show("Saved vpp Successfully!");

        }

        private void btnClearAllMask_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("将要清理所有屏蔽区域，是否继续？", "提醒", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;

            }
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                CogGraphicInteractiveCollection theMaskRegions = GeneralCheck.Inputs["MaskRegions"].Value as CogGraphicInteractiveCollection;

                CogRectangleAffine theMainRegion = GeneralCheck.Inputs["Region"].Value as CogRectangleAffine;
                theMaskRegions.Clear();
                //cogRecordDisplay1.InteractiveGraphics.Remove("Mask");
                //cogRecordDisplay1.InteractiveGraphics.Remove(i);
                this.cogRecordDisplay1.InteractiveGraphics.Clear();
                this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions,
                    "Mask", false
                    );
                this.cogRecordDisplay1.InteractiveGraphics.Add(theMainRegion,
            "Main", false
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void btnClearAllMask_Click(object sender, EventArgs e)
        //{
        //    theMaskRegions.Clear();
        //    //cogRecordDisplay1.InteractiveGraphics.Remove("Mask");
        //    //cogRecordDisplay1.InteractiveGraphics.Remove(i);
        //    this.cogRecordDisplay1.InteractiveGraphics.Clear();
        //    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions,
        //        "Mask", false
        //        );
        //    this.cogRecordDisplay1.InteractiveGraphics.Add(theMainRegion,
        //"Main", false
        //    );
        //}

        private void btnAddRectangle_Click(object sender, EventArgs e)
        {
            try
            {
                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                CogGraphicInteractiveCollection theMaskRegions = GeneralCheck.Inputs["MaskRegions"].Value as CogGraphicInteractiveCollection;

                CogRectangleAffine theMainRegion = GeneralCheck.Inputs["Region"].Value as CogRectangleAffine;

                if (domainUpDown1.Text == "CogRectangleAffine")
                {
                    CogRectangleAffine tmp = new CogRectangleAffine();
                    tmp.Interactive = true;
                    tmp.Color = CogColorConstants.Red;
                    tmp.SelectedSpaceName = "@\\Fixture";
                    tmp.GraphicDOFEnable = CogRectangleAffineDOFConstants.All;
                    theMaskRegions.Add(tmp);
                }
                else if (domainUpDown1.Text == "CogEllipse")
                {
                    CogEllipse tmp = new CogEllipse();
                    tmp.Interactive = true;
                    tmp.Color = CogColorConstants.Red;
                    tmp.SelectedSpaceName = "@\\Fixture";
                    tmp.GraphicDOFEnable = CogEllipseDOFConstants.All;
                    theMaskRegions.Add(tmp);
                }
                else if (domainUpDown1.Text == "CogPolygon")
                {
                    CogPolygon tmp1 = new CogPolygon();
                    tmp1.Interactive = true;
                    tmp1.Color = CogColorConstants.Red;
                    tmp1.SelectedSpaceName = "@\\Fixture";
                    tmp1.AddVertex(0, 0, 0);
                    tmp1.AddVertex(100, 0, 1);
                    tmp1.AddVertex(100, 100, 2);
                    tmp1.AddVertex(0, 100, 3);
                    tmp1.HighlightColor = CogColorConstants.Green;
                    tmp1.HighlightRequest();
                    tmp1.GraphicDOFEnable = CogPolygonDOFConstants.All;
                    theMaskRegions.Add(tmp1);
                }
                this.cogRecordDisplay1.InteractiveGraphics.Clear();
                this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                this.cogRecordDisplay1.InteractiveGraphics.Add(theMainRegion, "Main", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                theBlock.Run();

                CogBlobTool blobTool = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E/CogBlobTool-E") as CogBlobTool;

                CogBlobEditV2 toolEditor = new CogBlobEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;
                helper.InitializeComponent();
                toolEditor.Subject = blobTool;
                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConfigLineCheck_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                CogCaliperTool theCaliperToolT = getToolByTreeName(theBlock, "CaliperT") as CogCaliperTool;
                int iCurCount = (int)theBlock.Inputs[0].Value;
                iCurCount = iCurCount + 1;
                this.tBLineCheckCount.Text = iCurCount.ToString();
                theBlock.Inputs[0].Value = iCurCount;//= iCurCount + 1;
                int idx = iCurCount;
                theBlock.Outputs.Add(new CogToolBlockTerminal("LocateRegion" + idx, new CogRectangleAffine(theCaliperToolT.Region)));
                theBlock.Outputs.Add(new CogToolBlockTerminal("LocateCliperThreshold" + idx, 25.0));
                theBlock.Outputs.Add(new CogToolBlockTerminal("LocateCliperFilterHalfSize" + idx, 8));
                theBlock.Outputs.Add(new CogToolBlockTerminal("MaskRegions" + idx, new CogGraphicInteractiveCollection()));


                CogFindLineTool CogFindLineTool_T = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams/FindLineT") as CogFindLineTool;


                theBlock.Outputs.Add(new CogToolBlockTerminal("FindLine" + idx, new CogFindLine(CogFindLineTool_T.RunParams)));
                theBlock.Outputs.Add(new CogToolBlockTerminal("FindLineCountRatio" + idx, 0.5));
                //theBlock.Outputs.Add(new CogToolBlockTerminal("ClipThreshold" + idx, 25));
                //theBlock.Outputs.Add(new CogToolBlockTerminal("HalfSizeofFliter" + idx, 8));
                //theBlock.Outputs.Add(new CogToolBlockTerminal("ExpectedLineSegment" + idx, new CogLineSegment()));
                theBlock.Outputs.Add(new CogToolBlockTerminal("SampleRegions" + idx, new CogGraphicInteractiveCollection()));

                theBlock.Outputs.Add(new CogToolBlockTerminal("DisThreshold" + idx, 10.0));

                theBlock.Outputs.Add(new CogToolBlockTerminal("CountThreshold" + idx, 4));


                //theBlock.Run();

                //CogBlobTool blobTool = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E/CogBlobTool-E") as CogBlobTool;

                //CogBlobEditV2 toolEditor = new CogBlobEditV2();
                //GenericToolEditor helper = new GenericToolEditor();
                //helper.theTool = toolEditor;
                //helper.InitializeComponent();
                //toolEditor.Subject = blobTool;
                //helper.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnClearLineCheck_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("将要清理所有边线检测项，是否继续？","提醒", MessageBoxButtons.YesNo)!= DialogResult.Yes)
            //{
            //    return;

            //}
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;
            }
            try
            {

                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                int iCurCount = (int)theBlock.Inputs[0].Value;

                if (iCurCount > 0)
                {

                    iCurCount = iCurCount - 1;
                    this.tBLineCheckCount.Text = iCurCount.ToString();
                    theBlock.Inputs[0].Value = iCurCount;//= iCurCount + 1;
                    int idx = iCurCount + 1;

                    theBlock.Outputs.Remove("LocateRegion" + idx);
                    theBlock.Outputs.Remove("LocateCliperThreshold" + idx);
                    theBlock.Outputs.Remove("LocateCliperFilterHalfSize" + idx);
                    theBlock.Outputs.Remove("MaskRegions" + idx);

                    theBlock.Outputs.Remove("FindLine" + idx);
                    theBlock.Outputs.Remove("FindLineCountRatio" + idx);
                    theBlock.Outputs.Remove("SampleRegions" + idx);
                    theBlock.Outputs.Remove("DisThreshold" + idx);
                    theBlock.Outputs.Remove("CountThreshold" + idx);



                }

                //theBlock.Inputs[0].Value = 0;
                //this.tBLineCheckCount.Text = theBlock.Inputs[0].Value.ToString();
                //theBlock.Outputs.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConfigLineLocate_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/CheckSides-E") as CogToolBlock;

                    this.cogRecordDisplay1.Image = GeneralCheck.Inputs["GreyWithCord"].Value as ICogImage;

                    this.cogRecordDisplay1.InteractiveGraphics.Clear();

                    CogToolBlock CheckSidesRunParams = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                    int iCurCount = (int)tBCurIdx.Value;

                    CogRectangleAffine icgi = CheckSidesRunParams.Outputs["LocateRegion" + iCurCount].Value as CogRectangleAffine;
                    //CogCaliper caliper = CheckSidesRunParams.Outputs["LocateCliper" + iCurCount].Value as CogCaliper;
                    icgi.GraphicDOFEnable = CogRectangleAffineDOFConstants.All;
                    icgi.Interactive = true;
                    icgi.XDirectionAdornment = CogRectangleAffineDirectionAdornmentConstants.Arrow;
                    icgi.SelectedSpaceName = "@\\Fixture";
                    //this.cogRecordDisplay1.InteractiveGraphics.Add(icgi, "LineLocate", false);

                    CogCaliperTool CogCaliperTool1 = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams/CaliperT") as CogCaliperTool;


                    Cognex.VisionPro.Caliper.CogCaliperEditV2 toolEditor = new Cognex.VisionPro.Caliper.CogCaliperEditV2();
                    GenericToolEditor helper = new GenericToolEditor();
                    helper.theTool = toolEditor;

                    helper.InitializeComponent();

                    CogCaliperTool1.Region = icgi;
                    CogCaliperTool1.RunParams.FilterHalfSizeInPixels = (int)CheckSidesRunParams.Outputs["LocateCliperFilterHalfSize" + iCurCount.ToString()].Value;
                    CogCaliperTool1.RunParams.ContrastThreshold = (double)CheckSidesRunParams.Outputs["LocateCliperThreshold" + iCurCount.ToString()].Value;

                    //CogCaliperTool1.RunParams = caliper;
                    toolEditor.Subject = CogCaliperTool1 as CogCaliperTool;

                    if (helper.ShowDialog() == DialogResult.None)
                    {
                        ;
                    }

                    CheckSidesRunParams.Outputs["LocateCliperFilterHalfSize" + iCurCount.ToString()].Value = CogCaliperTool1.RunParams.FilterHalfSizeInPixels;
                    CheckSidesRunParams.Outputs["LocateCliperThreshold" + iCurCount.ToString()].Value = CogCaliperTool1.RunParams.ContrastThreshold;
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnConfigLineFinder_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/CheckSides-E") as CogToolBlock;

                    this.cogRecordDisplay1.Image = GeneralCheck.Inputs["GreyWithCord"].Value as ICogImage;

                    this.cogRecordDisplay1.InteractiveGraphics.Clear();

                    CogToolBlock CheckSidesRunParams = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                    int iCurCount = Convert.ToInt32(this.tBCurIdx.Text);

                    CogFindLine theFindLine = CheckSidesRunParams.Outputs["FindLine" + iCurCount].Value as CogFindLine;
                    theFindLine.ExpectedLineSegment.SelectedSpaceName = "@\\Fixture";


                    CogFindLineTool theTool = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams/FindLineT") as CogFindLineTool;


                    Cognex.VisionPro.Caliper.CogFindLineEditV2 toolEditor = new Cognex.VisionPro.Caliper.CogFindLineEditV2();
                    GenericToolEditor helper = new GenericToolEditor();
                    helper.theTool = toolEditor;

                    helper.InitializeComponent();

                    theTool.RunParams = theFindLine;
                    toolEditor.Subject = theTool as CogFindLineTool;

                    if (helper.ShowDialog() == DialogResult.None)
                    {
                        ;
                    }
                    CheckSidesRunParams.Outputs["FindLine" + iCurCount.ToString()].Value = theTool.RunParams;
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butLoadParameter_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            OpenFileDialog finder = new OpenFileDialog();
            finder.Filter = "算法文件(*.vpx)|*.vpx|所有文件(*.*)|*.*";

            if (finder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //Parameter MainParameter = new Parameter();
                    if (MainParameter == null)
                    {
                        Parameter tmpParameter = new Parameter();
                        tmpParameter.DSerializParam(finder.FileName, out MainParameter);
                    }
                    else
                    {
                        MainParameter.DSerializParam(finder.FileName, out MainParameter);
                    }
                    //tbParamName.Text = finder.FileName;
                    Param_Add_Show(MainParameter);
                    // theVpp = Cognex.VisionPro.CogSerializer.LoadObjectFromFile(finder.FileName) as CogToolBlock;
                    MessageBox.Show("参数加载成功！");
                }
                catch (Exception ex)
                {
                    //theVpp = null;
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("参数加载失败！");
                }
            }
        }

        private void butSaveParam_Click(object sender, EventArgs e)
        {

            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }


            if (true)
            {
                try
                {
                    FileSave(m_vpxPath);
                    Parameter _MainParameter = new Parameter();
                    InitParam(_MainParameter);

                    CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                    _MainParameter.theMaskRegions = (CogGraphicInteractiveCollection)GeneralCheck.Inputs["MaskRegions"].Value;
                    _MainParameter.theMainRegion = (CogRectangleAffine)GeneralCheck.Inputs["Region"].Value;

                    CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                    _MainParameter.Count = Convert.ToInt32(theBlock.Inputs[0].Value);
                    _MainParameter.GreyThreshold = Convert.ToInt32(GeneralCheck.Inputs["GreyThreshold"].Value);
                    _MainParameter.AreaThreshold = Convert.ToInt32(GeneralCheck.Inputs["AreaThreshold"].Value);

                    CogToolBlock resetplane = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E") as CogToolBlock;
                    _MainParameter.ReferenceUSE = Convert.ToBoolean(resetplane.Inputs[2].Value);
                    _MainParameter.Referencesink = Convert.ToDouble(resetplane.Inputs[1].Value);

                    CogCaliperTool CaliperTool_1 = getToolByTreeName(theVpp, "Locate/CogCaliperTool1") as CogCaliperTool;
                    _MainParameter.CaliperParam_1 = CaliperTool_1.RunParams;
                    _MainParameter.CaliperRegion_1 = CaliperTool_1.Region;
                    CogCaliperTool CaliperTool_2 = getToolByTreeName(theVpp, "Locate/CogCalipVorizental-E") as CogCaliperTool;
                    _MainParameter.CaliperParam_2 = CaliperTool_2.RunParams;
                    _MainParameter.CaliperRegion_2 = CaliperTool_2.Region;

                    CogFindLineTool FindLineTool = getToolByTreeName(theVpp, "Locate/CogFindRightLine-E") as CogFindLineTool;
                    _MainParameter.Findline_1 = FindLineTool.RunParams;
                    Cog3DRangeImagePlaneEstimatorTool PlaneEstimatorTool = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E/Cog3DRangeImagePlaneEstimatorTool-E") as Cog3DRangeImagePlaneEstimatorTool;
                    _MainParameter.RangeImagePlaneEstimator = PlaneEstimatorTool.RunParams;

                    for (int i = 0; i < theBlock.Outputs.Count; i++)
                    {
                        if ((theBlock.Outputs[i].Name).StartsWith("LocateRegion"))
                        {

                            _MainParameter.LocateRegion.Add(theBlock.Outputs[i].Value as CogRectangleAffine);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("LocateCliperThreshold"))
                        {
                            _MainParameter.LocateCliperThreshold.Add((double)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("LocateCliperFilterHalfSize"))
                        {
                            _MainParameter.LocateCliperFilterHalfSize.Add((int)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("MaskRegions"))
                        {
                            _MainParameter.MaskRegions.Add((CogGraphicInteractiveCollection)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("FindLine") && !(theBlock.Outputs[i].Name).StartsWith("FindLineCountRatio"))
                        {
                            _MainParameter.FindLine.Add((CogFindLine)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("DisThreshold"))
                        {
                            _MainParameter.DisThreshold.Add((double)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("CountThreshold"))
                        {
                            _MainParameter.CountThreshold.Add((int)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("FindLineCountRatio"))
                        {
                            _MainParameter.FindLineCountRatio.Add((double)theBlock.Outputs[i].Value);
                        }
                        if ((theBlock.Outputs[i].Name).StartsWith("SampleRegions"))
                        {
                            _MainParameter.SampleRegions.Add((CogGraphicInteractiveCollection)theBlock.Outputs[i].Value);
                        }
                    }


                    _MainParameter.SerializParam(m_vpxPath);

                    DisplayLower = cogRecordDisplay1.ColorMapLowerRoiLimit;
                    DisplayUpper = cogRecordDisplay1.ColorMapUpperRoiLimit;

                    this.MainParameter = _MainParameter;
                    //Cognex.VisionPro.CogSerializer.SaveObjectToFile(theVpp, finder.FileName);
                    MessageBox.Show("保存参数成功！");
                }
                catch (Exception ex)
                {
                    //theVpp = null;
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("加载参数失败！");
                }
            }
        }
        public void InitParam(Parameter MainParameter)
        {
            MainParameter.LocateRegion.Clear();
            MainParameter.LocateCliperThreshold.Clear();
            MainParameter.LocateCliperFilterHalfSize.Clear();
            MainParameter.MaskRegions.Clear();
            MainParameter.FindLine.Clear();
            MainParameter.DisThreshold.Clear();
            MainParameter.CountThreshold.Clear();

        }
        public void FileSave(string Path)
        {
            if (File.Exists(Path))
            {
                int number = 0;
                for (int i = 0; i < Path.Length; i++)
                {
                    if ((Path.Substring(i, 1) == "\\"))
                    {
                        number = i;
                    }
                }
                string strbuf_1 = Path.Substring(0, number + 1);
                string strbuf_2 = Path.Substring(number + 1, Path.Length - (number + 1));

                string NowDay = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                string NewPath = strbuf_1 + "备份vpx\\" + NowDay;
                if (!Directory.Exists(NewPath))
                {
                    Directory.CreateDirectory(NewPath);
                }
                File.Copy(Path, NewPath + "\\" + strbuf_2, true);


            }
        }
        private void Param_Add_Show(Parameter MainParameter)
        {
            try
            {

                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                GeneralCheck.Inputs["MaskRegions"].Value = MainParameter.theMaskRegions;
                GeneralCheck.Inputs["Region"].Value = MainParameter.theMainRegion;

                GeneralCheck.Inputs["GreyThreshold"].Value = MainParameter.GreyThreshold;
                GeneralCheck.Inputs["AreaThreshold"].Value = MainParameter.AreaThreshold;

                CogToolBlock resetPlane = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E") as CogToolBlock;
                resetPlane.Inputs[2].Value = MainParameter.ReferenceUSE;
                resetPlane.Inputs[1].Value = MainParameter.Referencesink;

                referenceuse.Text = resetPlane.Inputs[2].Value.ToString();
                if (referenceuse.Text == "True")
                {
                    referenceuse.SelectedIndex = 0;
                }
                else
                {
                    referenceuse.SelectedIndex = 1;
                }

                referencesink.Value = (decimal)MainParameter.Referencesink;


                CogCaliperTool CaliperTool_1 = getToolByTreeName(theVpp, "Locate/CogCaliperTool1") as CogCaliperTool;
                CaliperTool_1.RunParams = MainParameter.CaliperParam_1;
                CaliperTool_1.Region = MainParameter.CaliperRegion_1;

                CogCaliperTool CaliperTool_2 = getToolByTreeName(theVpp, "Locate/CogCalipVorizental-E") as CogCaliperTool;
                CaliperTool_2.RunParams = MainParameter.CaliperParam_2;
                CaliperTool_2.Region = MainParameter.CaliperRegion_2;

                CogFindLineTool FindLineTool = getToolByTreeName(theVpp, "Locate/CogFindRightLine-E") as CogFindLineTool;
                FindLineTool.RunParams = MainParameter.Findline_1;
                Cog3DRangeImagePlaneEstimatorTool PlaneEstimatorTool = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E/Cog3DRangeImagePlaneEstimatorTool-E") as Cog3DRangeImagePlaneEstimatorTool;
                PlaneEstimatorTool.RunParams = MainParameter.RangeImagePlaneEstimator;


                numAreaThreshol.Value = (decimal)MainParameter.AreaThreshold;
                numGreyThreshold.Value = (decimal)MainParameter.GreyThreshold;

                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                //CogCaliperTool theCaliperToolT = getToolByTreeName(theBlock, "CaliperT") as CogCaliperTool;
                theBlock.Inputs[0].Value = MainParameter.Count;
                this.tBLineCheckCount.Text = theBlock.Inputs[0].Value.ToString();
                if (MainParameter.Count > 0)
                {
                    num_Scale.Value = (decimal)MainParameter.FindLineCountRatio[0];
                    num_Distance.Value = (decimal)MainParameter.DisThreshold[0];
                    num_Count.Value = (decimal)MainParameter.CountThreshold[0];
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

        private void numGreyThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
            GeneralCheck.Inputs["GreyThreshold"].Value = (int)numGreyThreshold.Value;
        }

        private void numAreaThreshol_ValueChanged(object sender, EventArgs e)
        {
            if (theVpp == null)
            {

                MessageBox.Show("theVpp不存在！");

                return;

            }
            CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
            GeneralCheck.Inputs["AreaThreshold"].Value = (int)numAreaThreshol.Value;
        }

        private void Test_1_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                //theVpp.Inputs[0].Value = theImage;
                theVpp.Run();
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;

                this.cogRecordDisplay1.Image = GeneralCheck.Inputs["GreyWithCord"].Value as ICogImage;
                cogRecordDisplay1.Fit(false);
                GeneralCheck.Run();
                cogRecordDisplay1.Record = GeneralCheck.CreateLastRunRecord().SubRecords[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                CogGraphicInteractiveCollection theMaskRegions = GeneralCheck.Inputs["MaskRegions"].Value as CogGraphicInteractiveCollection;

                CogRectangleAffine theMainRegion = GeneralCheck.Inputs["Region"].Value as CogRectangleAffine;

                int iCount = theMaskRegions.Count;
                int iSelIdx = -1;
                for (int i = 0; i < iCount; i++)
                {
                    if (theMaskRegions[i].Selected)
                    {
                        iSelIdx = i;
                    }
                }
                if (iSelIdx != -1)
                {
                    theMaskRegions.RemoveAt(iSelIdx);
                }
                this.cogRecordDisplay1.InteractiveGraphics.Clear();
                this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                this.cogRecordDisplay1.InteractiveGraphics.Add(theMainRegion, "Main", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 toolEditor = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;

                helper.InitializeComponent();
                ICogTool theTool = theVpp;
                toolEditor.Subject = theTool as Cognex.VisionPro.ToolBlock.CogToolBlock;

                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {


            if (MessageBox.Show("是否需要保存参数和VPP？如需保存，请点\"是\"后手动保存，否则点否后配置窗口关闭。", "提醒", MessageBoxButtons.YesNo) != DialogResult.No)
            {
                e.Cancel = true;

            }
            theImageFile.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            domainUpDown1.SelectedIndex = 0;

        }

        private void Test_2_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {

                //theVpp.Inputs[0].Value = theImage;
                theVpp.Run();

                this.cogRecordDisplay1.Image = theImage;
                this.cogRecordDisplay1.Record = theVpp.CreateLastRunRecord().SubRecords[0];

                cogRecordDisplay1.Fit(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butTestAll_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                //theVpp.Inputs[0].Value = theImage;
                theVpp.Run();

                this.cogRecordDisplay1.Image = theImage;
                this.cogRecordDisplay1.Record = theVpp.CreateLastRunRecord().SubRecords[0];
                cogRecordDisplay1.Fit(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    this.cogRecordDisplay1.Image = theBlock.Inputs["GreyWithCord"].Value as ICogImage;

                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["MaskRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;

                    cogRecordDisplay1.InteractiveGraphics.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                    cogRecordDisplay1.Fit(false);
                }
                else
                {
                    MessageBox.Show("工具不存在！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {

                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["MaskRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;
                    if (domainUpDown1.Text == "CogRectangleAffine")
                    {
                        CogRectangleAffine tmp = new CogRectangleAffine();
                        tmp.Interactive = true;
                        tmp.Color = CogColorConstants.Red;
                        tmp.SelectedSpaceName = "@\\Fixture";
                        tmp.GraphicDOFEnable = CogRectangleAffineDOFConstants.All;
                        theMaskRegions.Add(tmp);
                    }
                    else if (domainUpDown1.Text == "CogEllipse")
                    {
                        CogEllipse tmp = new CogEllipse();
                        tmp.Interactive = true;
                        tmp.Color = CogColorConstants.Red;
                        tmp.SelectedSpaceName = "@\\Fixture";
                        tmp.GraphicDOFEnable = CogEllipseDOFConstants.All;
                        theMaskRegions.Add(tmp);
                    }


                    //tmp.Color = CogColorConstants.Red;

                    this.cogRecordDisplay1.InteractiveGraphics.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    this.cogRecordDisplay1.Image = theBlock.Inputs["GreyWithCord"].Value as ICogImage;

                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["SampleRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;

                    cogRecordDisplay1.InteractiveGraphics.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                    cogRecordDisplay1.Fit(false);
                }
                else
                {
                    MessageBox.Show("工具不存在！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {

                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["SampleRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;
                    if (domainUpDown1.Text == "CogRectangleAffine")
                    {
                        CogRectangleAffine tmp = new CogRectangleAffine();
                        tmp.Interactive = true;
                        tmp.Color = CogColorConstants.Red;
                        tmp.SelectedSpaceName = "@\\Fixture";
                        tmp.GraphicDOFEnable = CogRectangleAffineDOFConstants.All;
                        theMaskRegions.Add(tmp);
                    }
                    else if (domainUpDown1.Text == "CogEllipse")
                    {
                        CogEllipse tmp = new CogEllipse();
                        tmp.Interactive = true;
                        tmp.Color = CogColorConstants.Red;
                        tmp.SelectedSpaceName = "@\\Fixture";
                        tmp.GraphicDOFEnable = CogEllipseDOFConstants.All;
                        theMaskRegions.Add(tmp);
                    }


                    //tmp.Color = CogColorConstants.Red;

                    this.cogRecordDisplay1.InteractiveGraphics.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("将要清理所有屏蔽区域，是否继续？", "提醒", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;

            }

            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {

                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["MaskRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;
                    theMaskRegions.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.Clear();
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("将要清理所有采样区域，是否继续？", "提醒", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;

            }

            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {

                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["SampleRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;
                    theMaskRegions.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.Clear();
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["MaskRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;

                    int iCount = theMaskRegions.Count;
                    int iSelIdx = -1;
                    for (int i = 0; i < iCount; i++)
                    {
                        if (theMaskRegions[i].Selected)
                        {
                            iSelIdx = i;
                        }
                    }
                    if (iSelIdx != -1)
                    {
                        theMaskRegions.RemoveAt(iSelIdx);
                    }
                    this.cogRecordDisplay1.InteractiveGraphics.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;
                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    CogGraphicInteractiveCollection theMaskRegions = theBlock.Outputs["SampleRegions" + tBCurIdx.Value.ToString()].Value as CogGraphicInteractiveCollection;

                    int iCount = theMaskRegions.Count;
                    int iSelIdx = -1;
                    for (int i = 0; i < iCount; i++)
                    {
                        if (theMaskRegions[i].Selected)
                        {
                            iSelIdx = i;
                        }
                    }
                    if (iSelIdx != -1)
                    {
                        theMaskRegions.RemoveAt(iSelIdx);
                    }
                    this.cogRecordDisplay1.InteractiveGraphics.Clear();
                    this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {

                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                Cognex.VisionPro.Caliper.CogCaliperEditV2 toolEditor = new Cognex.VisionPro.Caliper.CogCaliperEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;

                helper.InitializeComponent();
                ICogTool theTool = getToolByTreeName(theVpp, "Locate/CogCaliperTool1");
                toolEditor.Subject = theTool as Cognex.VisionPro.Caliper.CogCaliperTool;

                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {

                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                Cognex.VisionPro.Caliper.CogCaliperEditV2 toolEditor = new Cognex.VisionPro.Caliper.CogCaliperEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;

                helper.InitializeComponent();
                ICogTool theTool = getToolByTreeName(theVpp, "Locate/CogCalipVorizental-E");
                toolEditor.Subject = theTool as Cognex.VisionPro.Caliper.CogCaliperTool;

                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {

            try
            {

                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                Cognex.VisionPro.Caliper.CogFindLineEditV2 toolEditor = new Cognex.VisionPro.Caliper.CogFindLineEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;

                helper.InitializeComponent();
                ICogTool theTool = getToolByTreeName(theVpp, "Locate/CogFindRightLine-E");
                toolEditor.Subject = theTool as Cognex.VisionPro.Caliper.CogFindLineTool;
                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    theBlock.Outputs["FindLineCountRatio" + tBCurIdx.Value.ToString()].Value = (double)num_Scale.Value;


                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tBCurIdx_ValueChanged(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    num_Scale.Value = Convert.ToDecimal(theBlock.Outputs["FindLineCountRatio" + tBCurIdx.Value.ToString()].Value);

                    num_Distance.Value = Convert.ToDecimal(theBlock.Outputs["DisThreshold" + tBCurIdx.Value.ToString()].Value);
                    num_Count.Value = Convert.ToDecimal(theBlock.Outputs["CountThreshold" + tBCurIdx.Value.ToString()].Value);
                }

                else
                {

                    MessageBox.Show("工具不存在！");
                    tBCurIdx.Value = tBCurIdx.Value - 1;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {

                if (theVpp == null)
                {
                    MessageBox.Show("theVpp不存在！");
                    return;

                }
                Cognex.VisionPro3D.Cog3DRangeImagePlaneEstimatorEditV2 toolEditor = new Cognex.VisionPro3D.Cog3DRangeImagePlaneEstimatorEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;

                helper.InitializeComponent();
                ICogTool theTool = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E/Cog3DRangeImagePlaneEstimatorTool-E");
                toolEditor.Subject = theTool as Cognex.VisionPro3D.Cog3DRangeImagePlaneEstimatorTool;

                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {

            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                //theVpp.Inputs[0].Value = theImage;
                theVpp.Run();
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "Locate") as CogToolBlock;


                CogFixtureTool CogFixtureTool2 = getToolByTreeName(GeneralCheck, "CogFixtureTool2") as CogFixtureTool;
                this.cogRecordDisplay1.Image = GeneralCheck.Inputs["Grey"].Value as ICogImage;
                this.cogRecordDisplay1.Record = GeneralCheck.CreateLastRunRecord().SubRecords[0];
                cogRecordDisplay1.Fit(false);
                GeneralCheck.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void num_Distance_ValueChanged(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    theBlock.Outputs["DisThreshold" + tBCurIdx.Value.ToString()].Value = (double)num_Distance.Value;


                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void num_Count_ValueChanged(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckSides-E/CheckSidesRunParams") as CogToolBlock;

                if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                {
                    theBlock.Outputs["CountThreshold" + tBCurIdx.Value.ToString()].Value = (int)num_Count.Value;


                }

                else
                {
                    MessageBox.Show("工具不存在！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void referItem_Changed(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E") as CogToolBlock;

                //if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                //{
                //    theBlock.Outputs["CountThreshold" + tBCurIdx.Value.ToString()].Value = (int)num_Count.Value;
                //}
                //else
                //{
                //    MessageBox.Show("工具不存在！");
                //}
                if (referenceuse.Text == "True")
                {
                    theBlock.Inputs[2].Value = true;
                }
                else
                {
                    theBlock.Inputs[2].Value = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void referencesink_Changed(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "HistogramInfo/ResetPlane-E") as CogToolBlock;

                //if (tBCurIdx.Value < Convert.ToInt32(theBlock.Inputs[0].Value) + 1)
                //{
                //    theBlock.Outputs["CountThreshold" + tBCurIdx.Value.ToString()].Value = (int)num_Count.Value;
                //}
                //else
                //{
                //    MessageBox.Show("工具不存在！");
                //}

                theBlock.Inputs[1].Value = (double)referencesink.Value;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void loginToMask_Click(object sender, EventArgs e)
        {
            //LoginToMask loginstt = new LoginToMask();
            //loginstt.ShowDialog();
            //bLoginAdmin = loginstt.m_bLoginToMask;
            //EnableEditMak(bLoginAdmin);


        }

        public void EnableEditMak(bool bFlag)
        {
            btnAddRectangle.Enabled = bFlag;
            //toolStripButton3D.Enabled = bFlag;
            btnRemoveSelected.Enabled = bFlag;
            btnClearAllMask.Enabled = bFlag;
            btnCheck.Enabled = bFlag;
            AddPoint.Enabled = bFlag;
            DeletePoint.Enabled = bFlag;
            //LeftCheckRegion.Enabled = bFlag;
            //RightCheckRegion.Enabled = bFlag;
            btnConfigAreaCheck.Enabled = bFlag;


        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            if (domainUpDown1.Text == "CogPolygon" && bLoginAdmin)
            {
                AddPoint.Enabled = true;
                DeletePoint.Enabled = true;
            }
            else
            {
                AddPoint.Enabled = false;
                DeletePoint.Enabled = false;
            }
        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                CogGraphicInteractiveCollection theMaskRegions = GeneralCheck.Inputs["MaskRegions"].Value as CogGraphicInteractiveCollection;
                CogRectangleAffine theMainRegion = GeneralCheck.Inputs["Region"].Value as CogRectangleAffine;

                int iCount = theMaskRegions.Count;
                int iSelIdx = -1;
                for (int i = 0; i < iCount; i++)
                {
                    if (theMaskRegions[i].Selected)
                    {
                        iSelIdx = i;
                    }
                }
                if (iSelIdx != -1)
                {
                    if (theMaskRegions[iSelIdx].GetType().Name == "CogPolygon")
                    {

                        CogPolygon temp1 = (CogPolygon)theMaskRegions[iSelIdx];
                        temp1.HighlightRequest();
                        temp1.HighlightColor = CogColorConstants.Green;
                        int nIndex = temp1.HighlightIndex;

                        double x = 0, y = 0;
                        temp1.AreaCenter(out x, out y);

                        temp1.AddVertex(x, y, nIndex);
                        theMaskRegions[iSelIdx] = temp1;
                    }
                    else
                    {
                        MessageBox.Show("请选择一个多边形的点");
                    }
                }
                this.cogRecordDisplay1.InteractiveGraphics.Clear();
                this.cogRecordDisplay1.InteractiveGraphics.AddList(theMaskRegions, "Mask", false);
                this.cogRecordDisplay1.InteractiveGraphics.Add(theMainRegion, "Main", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeletePoint_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock GeneralCheck = getToolByTreeName(theVpp, "CheckAll/GeneralCheck-E") as CogToolBlock;
                CogGraphicInteractiveCollection theMaskRegions = GeneralCheck.Inputs["MaskRegions"].Value as CogGraphicInteractiveCollection;
                CogRectangleAffine theMainRegion = GeneralCheck.Inputs["Region"].Value as CogRectangleAffine;

                int iCount = theMaskRegions.Count;
                int iSelIdx = -1;
                for (int i = 0; i < iCount; i++)
                {
                    if (theMaskRegions[i].Selected)
                    {
                        iSelIdx = i;
                    }
                }
                if (iSelIdx != -1)
                {
                    if (theMaskRegions[iSelIdx].GetType().Name == "CogPolygon")
                    {
                        CogPolygon temp1 = (CogPolygon)theMaskRegions[iSelIdx];
                        int nIndex = temp1.HighlightIndex;
                        temp1.RemoveVertex(nIndex);
                        theMaskRegions[iSelIdx] = temp1;
                    }
                    else
                    {
                        MessageBox.Show("请选择一个多边形的点");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LeftCheckRegion_Click(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckLeft") as CogToolBlock;
                //theBlock.Run();
                CogBlobTool blobTool = getToolByTreeName(theVpp, "CheckAll/CheckLeft/CogBlobTool-E") as CogBlobTool;

                CogBlobEditV2 toolEditor = new CogBlobEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;
                helper.InitializeComponent();
                toolEditor.Subject = blobTool;
                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RightCheckRegion_Click_1(object sender, EventArgs e)
        {
            if (theVpp == null)
            {
                MessageBox.Show("theVpp不存在！");
                return;

            }
            try
            {
                CogToolBlock theBlock = getToolByTreeName(theVpp, "CheckAll/CheckRight") as CogToolBlock;
                //theBlock.Run();
                CogBlobTool blobTool = getToolByTreeName(theVpp, "CheckAll/CheckRight/CogBlobTool-E") as CogBlobTool;

                CogBlobEditV2 toolEditor = new CogBlobEditV2();
                GenericToolEditor helper = new GenericToolEditor();
                helper.theTool = toolEditor;
                helper.InitializeComponent();
                toolEditor.Subject = blobTool;
                helper.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
