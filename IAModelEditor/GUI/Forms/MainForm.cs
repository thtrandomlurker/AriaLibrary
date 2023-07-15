using AriaLibrary.Objects;
using AriaLibrary.Objects.Nodes;
using Ookii.Dialogs.WinForms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using IAModelEditor.GUI.Forms.ModelImportWizard;
using AriaLibrary.Textures;

namespace IAModelEditor.GUI.Forms
{
    public partial class MainForm : Form
    {
        public ObjectGroup? ObjectGroup;
        public string? SourceFilePath;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MenuStripOpen_OnClick(object sender, EventArgs e)
        {
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                ObjectGroup = new ObjectGroup();
                ObjectGroup.LoadPackage(MenuStripOpenFileDialog.FileName);
                SourceFilePath = MenuStripOpenFileDialog.FileName;
                ObjectGroup.SourcePath = SourceFilePath;
                CurrentlyLoadedLabel.Text = $"Currently Loaded: {ObjectGroup.GPR.Heap.Name}";
            }
        }

        private void MenuStripSave_OnClick(object sender, EventArgs e)
        {
            if (ObjectGroup != null)
            {
                if (SourceFilePath != null)
                {
                    ObjectGroup.SavePackage(SourceFilePath);
                }
                else if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ObjectGroup.SavePackage(MenuStripSaveAsFileDialog.FileName);
                }
            }

        }

        private void MenuStripSaveAs_OnClick(object sender, EventArgs e)
        {
            if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (ObjectGroup != null)
                {
                    ObjectGroup.SavePackage(MenuStripSaveAsFileDialog.FileName);
                }
            }
        }
        private void MenuStripExportGPR_OnClick(object sender, EventArgs e)
        {
            if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (ObjectGroup != null)
                {
                    ObjectGroup.GPR.Save(MenuStripSaveAsFileDialog.FileName);
                }
            }
        }

        private void MenuStripEditMESHVariEditor_OnClick(object sender, EventArgs e)
        {
            if (ObjectGroup != null)
            {
                VARIEditorForm variEditor = new VARIEditorForm();
                variEditor.ObjectGroup = ObjectGroup;
                variEditor.Show();
            }
        }

        private void MenuStripExportFBX_OnClick(object sender, EventArgs e)
        {
            if (ObjectGroup != null)
            {
                if (MenuStripExportFBXFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ObjectGroup.ExportModelToFBX(MenuStripExportFBXFileDialog.FileName);
                }
            }
        }

        private void MenuStripReplace_OnClick(object sender, EventArgs e)
        {
            if (ObjectGroup != null)
            {
                if (MenuStripReplaceFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (MIWInit init = new MIWInit(MenuStripReplaceFileDialog.FileName))
                    {
                        init.ShowDialog();
                    }
                }
            }
        }

        private void MenuStripToolsConvertGXT_Click(object sender, EventArgs e)
        {
            // temporarily alter the behavior of MenuStripOpenFileDialog
            MenuStripOpenFileDialog.Multiselect = true;
            MenuStripOpenFileDialog.Filter = "GXT Texture File| *.gxt;*.mxt";
            MenuStripSaveAsFileDialog.FileName = "Select a Directory and press Save";
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in MenuStripOpenFileDialog.FileNames)
                    {
                        GXT.MakeDDSFromGXT(file, $"{Path.GetDirectoryName(MenuStripSaveAsFileDialog.FileName)}\\{Path.GetFileNameWithoutExtension(file)}.dds");
                    }
                }
            }
            MenuStripSaveAsFileDialog.FileName = "";
            MenuStripOpenFileDialog.Multiselect = false;
            MenuStripOpenFileDialog.Filter = "IA / VT Model File| *.mdl";
        }

        private void MenuStripToolsConvertDDS_Click(object sender, EventArgs e)
        {
            // temporarily alter the behavior of MenuStripOpenFileDialog
            MenuStripOpenFileDialog.Multiselect = true;
            MenuStripOpenFileDialog.Filter = "DDS Texture File| *.dds";
            MenuStripSaveAsFileDialog.FileName = "Select a Directory and press Save";
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in MenuStripOpenFileDialog.FileNames)
                    {
                        GXT.MakeGXTFromDDS(file, $"{Path.GetDirectoryName(MenuStripSaveAsFileDialog.FileName)}\\{Path.GetFileNameWithoutExtension(file)}.gxt");
                    }
                }
            }
            MenuStripSaveAsFileDialog.FileName = "";
            MenuStripOpenFileDialog.Multiselect = false;
            MenuStripOpenFileDialog.Filter = "IA / VT Model File| *.mdl";
        }
    }
}