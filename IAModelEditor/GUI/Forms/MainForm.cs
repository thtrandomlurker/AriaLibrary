using AriaLibrary.Helpers;
using AriaLibrary.Objects;
using AriaLibrary.Objects.Nodes;
using AriaLibrary.Textures;
using Assimp;
using IAModelEditor.GUI.Forms.ModelImportWizard;
using Ookii.Dialogs.WinForms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Numerics;
using Matrix4x4 = System.Numerics.Matrix4x4;
using System.Security.Permissions;
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
                    ObjectGroup.SavePackage(SourceFilePath + "_");
                    foreach (var file in ObjectGroup.SourcePackage.Files)
                    {
                        file.Close();
                    }
                    ObjectGroup.SourcePackage.BaseStream.Close();
                    File.Delete(SourceFilePath);
                    File.Move(SourceFilePath + "_", SourceFilePath);
                    ObjectGroup.LoadPackage(SourceFilePath);
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
                    foreach (EFFE effe in obj.MESH.ChildNodes.Where(x => x.Type == "EFFE"))
                    {
                        Console.WriteLine($"    Effect[{effe.EffectID}]: {obj.MESH.StringBuffer.StringList.Strings[effe.EffectType]}");
                        Console.WriteLine($"    {obj.MESH.StringBuffer.StringList.Strings[effe.EffectName]}: {obj.MESH.StringBuffer.StringList.Strings[effe.EffectFileName]}");
                    }
                }
            }
            MenuStripOpenFileDialog.Multiselect = false;
        }

        private void viewShaderPackageInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // temporarily alter the behavior of MenuStripOpenFileDialog
            MenuStripOpenFileDialog.Filter = "Shader Package File|*.csp";
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (MIWShaderInfo shaderInfoForm = new MIWShaderInfo(MenuStripOpenFileDialog.FileName))
                {
                    shaderInfoForm.ShowDialog();
                }
            }
            MenuStripOpenFileDialog.Filter = "IA / VT Model File| *.mdl";
        }

        private void clearNODEInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (NODE node in ObjectGroup.NODT.ChildNodes)
            {
                node.NodeParent = -1;
                node.NodeChild = -1;
            }
        }

        private void calculateStringHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (StringHashCalculatorForm c = new StringHashCalculatorForm())
            {
                c.ShowDialog();
            }
        }

        private void MenuStripReplace_Click(object sender, EventArgs e)
        {
            if (MenuStripReplaceFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (ModelReplaceWizard replaceDlg = new ModelReplaceWizard(MenuStripReplaceFileDialog.FileName, ref ObjectGroup))
                {
                    replaceDlg.ShowDialog();
                }
            }
        }

        private void calculateBindPoseStringHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (StringBindPoseHashCalculatorForm c = new StringBindPoseHashCalculatorForm())
            {
                c.ShowDialog();
            }
        }

        private void dumpBindPoseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // temporarily alter the behavior of MenuStripOpenFileDialog
            MenuStripOpenFileDialog.Filter = "Bind Pose Info|*.60se";
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream bpStream = File.OpenRead(MenuStripOpenFileDialog.FileName);
                BindPose bp = new BindPose();
                using (BinaryReader reader = new BinaryReader(bpStream))
                {
                    bp.Read(reader);
                    foreach (var entry in bp.BoneHierarchy)
                    {
                        Console.WriteLine($"Bone: {bp.BoneNames[entry.ID]} {((entry.ParentID & 0x8000) != 0 ? ($"--> {bp.BoneNames[entry.ParentID & 0x7FFF]}") : "")}");
                        uint bHashT = StringHelper.GetBindPoseStringHash(bp.BoneNames[entry.ID]);
                        if (bp.BoneHashes.Contains(bHashT))
                        {
                            Console.WriteLine($"Hash {bHashT} exists in table");
                        }
                        else
                        {
                            Console.WriteLine($"Hash {bHashT} not found");
                        }
                    }

                    foreach (var entry in bp.BoneOrderList)
                    {
                        BoneRelation rel = bp.BoneHierarchy[entry];
                        Console.WriteLine($"Skin Bone: {entry} --> {bp.BoneNames[rel.ID]}");
                    }
                }
                // removes duplicate entries from hierarchy.
                List<BoneRelation> hier = new List<BoneRelation>();
                for (int i = 0; i < bp.BoneHierarchy.Length; i++)
                {
                    for (int j = 0; j < bp.BoneOrderList.Length; j++)
                    {

                    }
                    BoneRelation b = new BoneRelation()
                    {
                        ID = bp.BoneHierarchy[i].ID,
                        ParentID = bp.BoneHierarchy[i].ParentID
                    };
                    if (!hier.Any(x => x.ID == b.ID && x.ParentID == b.ParentID))
                    {
                        hier.Add(b);
                    }
                    if (hier.Any(x => x.ID == b.ID && x.ParentID == b.ParentID))
                    {
                        for (int j = 0; j < bp.BoneOrderList.Length; j++)
                        {
                            if (bp.BoneOrderList[j] == i)
                            {
                                bp.BoneOrderList[j] = (short)hier.FindIndex(x => x.ID == b.ID && x.ParentID == b.ParentID);
                            }
                        }
                    }
                }
                bp.BoneHierarchy = hier.ToArray();
                using (BinaryWriter writer = new BinaryWriter(File.Create(Path.Combine(Path.GetDirectoryName(MenuStripOpenFileDialog.FileName), "test.60se"))))
                {
                    bp.Write(writer);
                }
            }
            MenuStripOpenFileDialog.Filter = "IA / VT Model File| *.mdl";
        }

        private void sEFromBRNTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // temporarily alter the behavior of MenuStripOpenFileDialog
            MenuStripOpenFileDialog.Filter = "BRNT|*.BRNT";
            if (MenuStripOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream brntStream = File.OpenRead(MenuStripOpenFileDialog.FileName);
                BRNT brnt = new BRNT();
                BindPose bp = new BindPose();
                using (BinaryReader reader = new BinaryReader(brntStream))
                {
                    brnt.Read(reader);
                    bp.BoneHierarchy = new BoneRelation[brnt.Bones.Count];
                    bp.BoneBindInfos = new BindInfo[brnt.Bones.Count];
                    bp.BoneOrderList = new short[brnt.Bones.Count];
                    bp.BoneHashes = new uint[brnt.Bones.Count];
                    for (int i = 0; i < brnt.Bones.Count; i++)
                    {
                        AriaLibrary.Objects.Bone? bone = brnt.Bones[i];

                        short boneId = bone.BoneID;
                        short parentId = bone.BoneParent;

                        BoneRelation b = new BoneRelation()
                        {
                            ID = boneId,
                            ParentID = parentId == -1 ? (short)0x7FFF : (short)(parentId | 0x8000)
                        };

                        if (brnt.Bones[i].BoneName == "LUpperarm")
                        {
                            brnt.Bones[i].Scale *= 2.0f;
                        }

                        BindInfo bind = new BindInfo()
                        {
                            Rotation = MathHelper.EulerAnglesToQuaternion(bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z),
                            Translation = new Vector4(bone.Translation.X, bone.Translation.Y, bone.Translation.Z, 1.0f),
                            Scale = new Vector4(bone.Scale.X, bone.Scale.Y, bone.Scale.Z, 1.0f)
                        };

                        uint boneHash = StringHelper.GetBindPoseStringHash(bone.BoneName);

                        bp.BoneHierarchy[i] = b;
                        bp.BoneBindInfos[i] = bind;
                        bp.BoneOrderList[i] = (short)i;
                        bp.BoneHashes[i] = boneHash;
                        bp.BoneNames.Add(bone.BoneName);
                    }
                }
                using (BinaryWriter writer = new BinaryWriter(File.Create(Path.Combine(Path.GetDirectoryName(MenuStripOpenFileDialog.FileName), "TEST.60SE"))))
                {
                    bp.Write(writer);
                }
            }
            MenuStripOpenFileDialog.Filter = "IA / VT Model File| *.mdl";
        }
    }
}