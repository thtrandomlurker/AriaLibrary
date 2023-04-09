using AriaLibrary.Objects;
using Ookii.Dialogs.WinForms;
namespace IAModelEditor.GUI.Forms
{
    public partial class ModelEditorForm : Form
    {
        public ObjectGroup? ObjectGroup;
        public string? SourceFilePath;
        public ModelEditorForm()
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
    }
}