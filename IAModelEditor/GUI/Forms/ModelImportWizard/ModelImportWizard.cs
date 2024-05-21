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
using AriaLibrary.Objects.GraphicsProgram;
using IAModelEditor.GUI.Controls;
using IAModelEditor.ImportHelpers;
using Assimp;
using Assimp.Configs;
using System.Numerics;
using AriaLibrary.Helpers;
using Matrix4x4 = System.Numerics.Matrix4x4;
using System.Security.Cryptography.X509Certificates;

namespace IAModelEditor.GUI.Forms.ModelImportWizard
{
    public partial class ModelImportWizard : Form
    {
        string? mSourceFilePath;
        public List<MaterialInfo> MaterialInfos;
        public ObjectGroup WorkingObject;
        public ModelImportWizard(string path)
        {
            InitializeComponent();
            mSourceFilePath = path;
            // set the title bar
            Text = $"Model Import Wizard: {Path.GetFileName(mSourceFilePath)}";
            MaterialInfos = new List<MaterialInfo>();
        }

        private void MIWInitButtonNext_Click(object sender, EventArgs e)
        {
            // Stage 1
            if (MIWActiveStageControl is MIWInitControl)
            {
                WorkingObject = new ObjectGroup();

                WorkingObject.MESH.Remark = new REM("Model created using AriaLibrary v0.1");

                WorkingObject.MESH.StringBuffer.StringList.Strings.Add(((MIWInitControl)MIWActiveStageControl).MIWInitModelName.Text);
                WorkingObject.MESH.StringBuffer.StringList.Strings.Add(((MIWInitControl)MIWActiveStageControl).MIWInitDSNAMode.Text);

                WorkingObject.MESH.ChildNodes.Add(new DSNA() { Object = 0, Mode = 1 });  // hardcoded because always same

                AssimpContext context = new AssimpContext();

                // (credits to skyth) borrow this from MikuMikuLibrary https://github.com/blueskythlikesclouds/MikuMikuLibrary/blob/master/MikuMikuLibrary/Objects/Processing/Assimp/AssimpSceneHelper.cs
                context.SetConfig(new FBXPreservePivotsConfig(false));
                context.SetConfig(new MaxBoneCountConfig(64));
                context.SetConfig(new MeshTriangleLimitConfig(524288));
                context.SetConfig(new MeshVertexLimitConfig(32768));
                context.SetConfig(new VertexBoneWeightLimitConfig(4));
                context.SetConfig(new VertexCacheSizeConfig(63));

                Scene scene = context.ImportFile(mSourceFilePath,
                PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate |
                PostProcessSteps.SplitLargeMeshes | PostProcessSteps.LimitBoneWeights |
                PostProcessSteps.ImproveCacheLocality | PostProcessSteps.SortByPrimitiveType |
                PostProcessSteps.SplitByBoneCount | PostProcessSteps.FlipUVs);

                List<string> skinBoneList = new List<string>();

                // check if scene contains bones
                bool hasBone = false;
                foreach (var mesh in scene.Meshes)
                {
                    if (mesh.BoneCount > 0)
                    {
                        hasBone = true;
                        foreach (var bone in mesh.Bones)
                        {
                            skinBoneList.Add(bone.Name);
                        }
                    }
                }

                // create the BRNT if so
                if (hasBone)
                {
                    WorkingObject.BRNT = new BRNT();

                    void AddBones(Node aiNode)
                    {
                        Bone bone = new Bone();
                        bone.BoneName = aiNode.Name;
                        Matrix4x4 boneMat = aiNode.Transform.ToNumerics();
                        Matrix4x4.Decompose(boneMat, out Vector3 scl, out System.Numerics.Quaternion rot, out Vector3 loc);
                        bone.Translation = new Vector3(-loc.X, -loc.Y, -loc.Z);
                        Vector3 eulerRot = MathHelper.QuaternionToEulerAngles(rot.W, rot.X, rot.Y, rot.Z);
                        eulerRot *= (float)(180 / Math.PI);  // conv all at once
                        bone.Rotation = eulerRot;
                        bone.Scale = scl;
                        bone.BoneID = (short)WorkingObject.BRNT.Bones.Count;
                        bone.SkinID = (short)skinBoneList.FindIndex(x => x == aiNode.Name);
                        WorkingObject.BRNT.Bones.Add(bone);

                        if (aiNode.Parent != null)
                        {
                            bone.BoneParent = WorkingObject.BRNT.Bones.First(x => x.BoneName == aiNode.Name).BoneID;
                        }

                        foreach (var child in aiNode.Children)
                        {
                            AddBones(child);
                        }
                    }
                    AddBones(scene.RootNode.Children[0]);
                }

                Console.WriteLine("BRNT PREPASS FINISHED");

                // start with the TRSPs (Base on Materials, *NOT* on meshes, it's TRANSPARENCY settings. per mat, not mesh.)
                // seemingly no reference from anything though... based on index?
                for (int i = 0; i < scene.MaterialCount; i++)
                {
                    WorkingObject.MESH.ChildNodes.Add(new TRSP() { TRSPId = i, Culling = scene.Materials[i].IsTwoSided ? CullMode.None : CullMode.BackFace, U08 = 1, U0C = 0, U10 = 2, U14 = 0, U18 = 1, U1C = 1 });
                }

                // generate material infos

                for (int i = 0; i < scene.MaterialCount; i++)
                {
                    MaterialInfo info = new MaterialInfo();
                    info.MaterialName = scene.Materials[i].Name;
                    MaterialInfos.Add(info);
                }

                WorkingObject.GPR = new GraphicsProgram();
                WorkingObject.GPR.Heap.Name = ((MIWInitControl)MIWActiveStageControl).MIWInitModelName.Text;

                Console.WriteLine("Changing control");
                Controls.Remove(MIWActiveStageControl);
                MIWActiveStageControl = new MIWMaterialSetupControl(this);
                Controls.Add(MIWActiveStageControl);
                MIWActiveStageControl.Show();
            }

            else if (MIWActiveStageControl is MIWMaterialSetupControl)
            {
                int uninitIndex = MaterialInfos.FindIndex(x => !x.Initialized);
                if (uninitIndex != -1)
                {
                    MessageBox.Show($"Material \"{MaterialInfos[uninitIndex].MaterialName}\" has no shader applied to it.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WorkingObject.Save($"{Path.GetDirectoryName(saveFileDialog.FileName)}\\gpr.GPR", $"{Path.GetDirectoryName(saveFileDialog.FileName)}\\mesh.MESH", $"{Path.GetDirectoryName(saveFileDialog.FileName)}\\nodt.NODT");
                }
            }
        }
    }
}