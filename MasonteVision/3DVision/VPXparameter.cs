using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro3D;
using Cognex.VisionPro.ToolBlock;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.Display;
using System.Collections;
using Cognex.VisionPro.Caliper;
using System.Runtime.Serialization;
using System.Reflection;

namespace VPPConfiger
{
    [Serializable]
    public class Parameter
    {
        public class UBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return ass.GetType(typeName);
            }
        }

        public CogGraphicInteractiveCollection theMaskRegions = new CogGraphicInteractiveCollection();
        public CogRectangleAffine theMainRegion = new CogRectangleAffine();

        public CogCaliper CaliperParam_1 = new CogCaliper();
        public CogCaliper CaliperParam_2 = new CogCaliper();

        public CogRectangleAffine CaliperRegion_1 = new CogRectangleAffine();
        public CogRectangleAffine CaliperRegion_2 = new CogRectangleAffine();



        public CogFindLine Findline_1 = new CogFindLine();
        public Cog3DRangeImagePlaneEstimator RangeImagePlaneEstimator = new Cog3DRangeImagePlaneEstimator();
        public int GreyThreshold { get; set; }
        public int AreaThreshold { get; set; }

        public bool ReferenceUSE { get; set; }

        public double Referencesink { get; set; }


        public int Count { get; set; }


        public List<CogRectangleAffine> LocateRegion = new List<CogRectangleAffine>();

        public List<double> LocateCliperThreshold = new List<double>();

        public List<int> LocateCliperFilterHalfSize = new List<int>();
        public List<CogGraphicInteractiveCollection> MaskRegions = new List<CogGraphicInteractiveCollection>();
        public List<CogFindLine> FindLine = new List<CogFindLine>();

        public List<double> DisThreshold = new List<double>();
        public List<int> CountThreshold = new List<int>();
        public List<double> FindLineCountRatio = new List<double>();
        public List<CogGraphicInteractiveCollection> SampleRegions = new List<CogGraphicInteractiveCollection>();


        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="Path"></param>
        public void SerializParam(string Path)
        {
            try
            {
                using (FileStream fs = new FileStream(Path, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void DSerializParam(string Path, out Parameter ParameterBuf)
        {
            try
            {
                using (FileStream fs = new FileStream(Path, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Binder = new UBinder();
                    ParameterBuf = bf.Deserialize(fs) as Parameter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ParameterBuf = null;
            }

        }
    }
}
