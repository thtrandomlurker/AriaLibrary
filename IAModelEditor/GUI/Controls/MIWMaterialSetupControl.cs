using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriaLibrary.Objects.Nodes;
using IAModelEditor.GUI.Forms.ModelImportWizard;
using IAModelEditor.ImportHelpers;

namespace IAModelEditor.GUI.Controls
{
    public partial class MIWMaterialSetupControl : UserControl
    {
        new public Form Parent;

        public List<EFFE> MaterialEffects;

        public MIWMaterialSetupControl(Form parent)
        {
            InitializeComponent();
            Parent = parent;
            foreach (var matInfo in ((ModelImportWizard)Parent).MaterialInfos)
            {
                MIWMaterialListbox.Items.Add(matInfo.MaterialName);
            }
            MaterialEffects = new List<EFFE>();
        }

        private void MIWMaterialValidate_Click(object sender, EventArgs e)
        {
            MaterialValidity validityStatus = ((ModelImportWizard)Parent).MaterialInfos[MIWMaterialListbox.SelectedIndex].IsValid(((ModelImportWizard)Parent).WorkingObject.MESH.StringBuffer.StringList);
            MIWMaterialValidationStatus.Text = $"{(validityStatus.Valid ? "Valid. " : "Invalid. ")} {validityStatus.Message}";
        }

        private void MIWMaterialSelectShader_Click(object sender, EventArgs e)
        {
            if (MIWMaterialOpenShaderDialog.ShowDialog() == DialogResult.OK)
            {
                using (MIWShaderSelector shaderSelector = new MIWShaderSelector(MIWMaterialOpenShaderDialog.FileName, this.Parent, this))
                {
                    shaderSelector.ShowDialog();
                }
            }
        }
    }
}
