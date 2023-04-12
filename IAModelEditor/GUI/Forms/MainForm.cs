using AriaLibrary.Objects;
using AriaLibrary.Objects.Nodes;
using Ookii.Dialogs.WinForms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

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

        private void MenuStripExportOBJ_OnClick(object sender, EventArgs e)
        {
            using (VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (ObjectGroup != null)
                    {
                        ObjectGroup.ExportModelAsModifiedOBJ(folderBrowser.SelectedPath);
                    }
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
    }
}