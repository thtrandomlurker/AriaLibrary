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
        public List<MaterialData> WorkingMaterialData;
        public List<MeshData> WorkingMeshData;
        public ObjectGroup WorkingObject;
        public ModelImportWizard(string path)
        {
            InitializeComponent();
            mSourceFilePath = path;
            // set the title bar
            Text = $"Model Import Wizard: {Path.GetFileName(mSourceFilePath)}";
            WorkingMaterialData = new List<MaterialData>();
            WorkingMeshData = new List<MeshData>();
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

                // a naive assumption

                List<Node> meshNodes = scene.RootNode.Children.Skip(1).ToList();
                
                // check if scene contains bones
                bool hasBone = false;
                foreach (var mesh in meshNodes)
                {
                    foreach (int meshIndex in mesh.MeshIndices)
                    {
                        if (scene.Meshes[meshIndex].BoneCount > 0)
                        {
                            hasBone = true;
                            foreach (var bone in scene.Meshes[meshIndex].Bones)
                            {
                                skinBoneList.Add(bone.Name);
                            }
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
                        Matrix4x4 transposedMat = Matrix4x4.Transpose(boneMat);
                        Matrix4x4.Decompose(transposedMat, out Vector3 scl, out System.Numerics.Quaternion rot, out Vector3 loc);
                        bone.Translation = loc;
                        Vector3 eulerRot = MathHelper.QuaternionToEulerAngles(rot.W, rot.X, rot.Y, rot.Z);
                        eulerRot *= (float)(180 / Math.PI);  // conv all at once
                        bone.Rotation = eulerRot;
                        bone.Scale = scl;
                        bone.BoneID = (short)WorkingObject.BRNT.Bones.Count;
                        bone.SkinID = (short)skinBoneList.FindIndex(x => x == aiNode.Name);
                        bone.PossibleFlags = -256;
                        bone.U18 = -1;
                        bone.U1E = -1;
                        if (aiNode.ChildCount == 1)
                        {
                            bone.BoneID = (short)(WorkingObject.BRNT.Bones.Count + 1);
                        }
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
                    AddBones(scene.RootNode.Children[0].Children[0]);  // start from skeleton root.
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
                    MaterialData info = new MaterialData();
                    info.MaterialName = scene.Materials[i].Name;
                    info.SourceMaterial = scene.Materials[i];
                    WorkingMaterialData.Add(info);
                }

                foreach (var node in meshNodes)
                {
                    foreach (var meshIndex in node.MeshIndices)
                    {
                        MeshData mesh = new MeshData();
                        mesh.MeshName = node.Name;
                        WorkingMeshData.Add(mesh);
                    }
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
                int uninitIndex = WorkingMaterialData.FindIndex(x => !x.Initialized);
                if (uninitIndex != -1)
                {
                    MessageBox.Show($"Material \"{WorkingMaterialData[uninitIndex].MaterialName}\" has no shader applied to it.");
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