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
                LoadedPlatformLabel.Text = $"Platform: {ObjectGroup.GPR.Platform}";
                MenuStripSave.Enabled = true;
                MenuStripSaveAs.Enabled = true;
                MenuStripExport.Enabled = true;
                MenuStripExportGPR.Enabled = true;
                MenuStripExportFBX.Enabled = true;
                MenuStripExportFBXBasic.Enabled = true;
                MenuStripReplace.Enabled = true;
                Console.WriteLine("Dummy");
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

        private void MenuStripCreate_Click(object sender, EventArgs e)
        {
            if (MenuStripReplaceFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (ModelImportWizard.ModelImportWizard init = new ModelImportWizard.ModelImportWizard(MenuStripReplaceFileDialog.FileName))
                {
                    init.ShowDialog();
                }
            }
        }

        private void clearCSTSsForScienceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                ObjectGroup obj = new ObjectGroup();
                obj.LoadPackage(MenuStripOpenFileDialog.FileName);

                foreach (var node in obj.MESH.ChildNodes)
                {
                    if (node is CSTS cstsNode)
                    {
                        cstsNode.ConstantValues.Clear();
                    }
                }

                if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    obj.SavePackage(MenuStripSaveAsFileDialog.FileName);
                }
            }
        }

        private void unswizzleDDSInPlacToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuStripOpenFileDialog.Multiselect = true;
            MenuStripOpenFileDialog.Filter = "DDS Texture File| *.dds;";
            MenuStripSaveAsFileDialog.FileName = "Select a Directory and press Save";
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (MenuStripSaveAsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in MenuStripOpenFileDialog.FileNames)
                    {
                        GXT.UnswizzleFromDDS(file, $"{Path.GetDirectoryName(MenuStripSaveAsFileDialog.FileName)}\\{Path.GetFileNameWithoutExtension(file)}_unswizzled.dds");
                    }
                }
            }
            MenuStripSaveAsFileDialog.FileName = "";
            MenuStripOpenFileDialog.Multiselect = false;
            MenuStripOpenFileDialog.Filter = "IA / VT Model File| *.mdl";
        }

        private void MenuStripExportFBXBasic_Click(object sender, EventArgs e)
        {
            if (ObjectGroup != null)
            {
                if (MenuStripExportFBXFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ObjectGroup.ExportModelToBasicFBX(MenuStripExportFBXFileDialog.FileName);
                }
            }
        }

        private void sanityCzechToolStripMenuItem_Click(object sender, EventArgs e)
        {// temporarily alter the behavior of MenuStripOpenFileDialog
            MenuStripOpenFileDialog.Multiselect = true;
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in MenuStripOpenFileDialog.FileNames)
                {
                    ObjectGroup obj = new ObjectGroup();
                    obj.LoadPackage(file);

                    int countTRSP = obj.MESH.ChildNodes.Count(x => x.Type == "TRSP");
                    int countEFFE = obj.MESH.ChildNodes.Count(x => x.Type == "EFFE");
                    int countMATE = obj.MESH.ChildNodes.Count(x => x.Type == "MATE");

                    VARI vari = obj.MESH.ChildNodes.First(x => x.Type == "VARI") as VARI;
                    int countPRIM = vari.PRIMs.Count;

                    Console.WriteLine($"File: {Path.GetFileName(file)}");
                    Console.WriteLine($"    TRSP: {countTRSP}");
                    Console.WriteLine($"    EFFE: {countEFFE}");
                    Console.WriteLine($"    MATE: {countMATE}");
                    Console.WriteLine($"    PRIM: {countPRIM}");

                }
            }
            MenuStripOpenFileDialog.Multiselect = false;
        }
    }
}