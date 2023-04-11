using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AriaLibrary.Objects;
using AriaLibrary.Objects.Nodes;
using Ookii.Dialogs.WinForms;

namespace IAModelEditor.GUI.Forms
{
    public partial class VARIEditorForm : Form
    {
        public ObjectGroup ObjectGroup;
        public VARIEditorForm()
        {
            InitializeComponent();
        }

        private void VARIEditorForm_OnLoad(object sender, EventArgs e)
        {
            foreach (var prim in ((VARI)ObjectGroup.MESH.ChildNodes.First(x => x.Type == "VARI")).PRIMs)
            {
                VARIEditorPrimitiveList.Items.Add(ObjectGroup.MESH.StringBuffer.StringList.Strings[prim.MeshName]);
            }
        }

        private void VARIEditorExportButton_OnClick(object sender, EventArgs e)
        {
            using (VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (ObjectGroup != null)
                    {
                        ObjectGroup.ExportMeshToOBJ(folderBrowser.SelectedPath, VARIEditorPrimitiveList.SelectedIndex);
                    }
                }
            }
        }
    }
}
