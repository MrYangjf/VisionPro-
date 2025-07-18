using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigerForm
{
    public partial class GenericToolEditor : Form
    {
        public Cognex.VisionPro.CogToolEditControlBaseV2 theTool;

        public GenericToolEditor()
        {
            InitializeComponent();
        }

        private void GenericToolEditor_Load(object sender, EventArgs e)
        {
            theTool.Parent = this;
            theTool.Dock = DockStyle.Fill;
        }
    }
}
