using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAModelEditor.GUI.Forms.ModelImportWizard
{
    public partial class MIWAttributeSourceSelector : Form
    {
        public MIWAttributeSourceSelector(string attributeName)
        {
            InitializeComponent();
            label1.Text = $"Select the source for the attribute {attributeName}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
