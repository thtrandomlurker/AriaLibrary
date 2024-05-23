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
using AriaLibrary.Objects.GraphicsProgram.Nodes;
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
        public AssimpContext Context;
        public Scene Scene;
        public List<Node> MeshNodes;
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

                Context = new AssimpContext();

                // (credits to skyth) borrow this from MikuMikuLibrary https://github.com/blueskythlikesclouds/MikuMikuLibrary/blob/master/MikuMikuLibrary/Objects/Processing/Assimp/AssimpSceneHelper.cs
                Context.SetConfig(new FBXPreservePivotsConfig(false));
                Context.SetConfig(new MaxBoneCountConfig(64));
                Context.SetConfig(new MeshTriangleLimitConfig(524288));
                Context.SetConfig(new MeshVertexLimitConfig(32768));
                Context.SetConfig(new VertexBoneWeightLimitConfig(4));
                Context.SetConfig(new VertexCacheSizeConfig(63));

                Scene = Context.ImportFile(mSourceFilePath,
                PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate |
                PostProcessSteps.SplitLargeMeshes | PostProcessSteps.LimitBoneWeights |
                PostProcessSteps.ImproveCacheLocality | PostProcessSteps.SortByPrimitiveType |
                PostProcessSteps.SplitByBoneCount | PostProcessSteps.FlipUVs);

                List<string> skinBoneList = new List<string>();

                // a naive assumption

                MeshNodes = Scene.RootNode.Children.Skip(1).ToList();
                
                // check if scene contains bones
                bool hasBone = false;
                foreach (var mesh in MeshNodes)
                {
                    foreach (int meshIndex in mesh.MeshIndices)
                    {
                        if (Scene.Meshes[meshIndex].BoneCount > 0)
                        {
                            hasBone = true;
                            foreach (var bone in Scene.Meshes[meshIndex].Bones)
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
                    AddBones(Scene.RootNode.Children[0].Children[0]);  // start from skeleton root.
                }

                Console.WriteLine("BRNT PREPASS FINISHED");

                // start with the TRSPs (Base on Materials, *NOT* on meshes, it's TRANSPARENCY settings. per mat, not mesh.)
                // seemingly no reference from anything though... based on index?
                for (int i = 0; i < Scene.MaterialCount; i++)
                {
                    WorkingObject.MESH.ChildNodes.Add(new TRSP() { TRSPId = i, Culling = Scene.Materials[i].IsTwoSided ? CullMode.None : CullMode.BackFace, U08 = 1, U0C = 0, U10 = 2, U14 = 0, U18 = 1, U1C = 1 });
                }

                // generate material infos

                for (int i = 0; i < Scene.MaterialCount; i++)
                {
                    MaterialData info = new MaterialData();
                    info.MaterialName = Scene.Materials[i].Name.Replace('.', '_');
                    info.SourceMaterial = Scene.Materials[i];
                    WorkingMaterialData.Add(info);
                }

                int cmesh = 0;
                foreach (var node in MeshNodes)
                {
                    foreach (var meshIndex in node.MeshIndices)
                    {
                        MeshData mesh = new MeshData();
                        mesh.MeshName = node.Name;
                        mesh.SourceMesh = Scene.Meshes[meshIndex];

                        // we can create prim data here as it should directly correlate to the materials created later
                        mesh.PrimitiveData = new PRIM();
                        mesh.PrimitiveData.PrimitiveID = cmesh++;
                        mesh.PrimitiveData.ObjectName = 0;  // this is hard embedded into the code of this program
                        mesh.PrimitiveData.MeshName = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        mesh.PrimitiveData.MeshNameDupe = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        WorkingObject.MESH.StringBuffer.StringList.Strings.Add(mesh.MeshName.Replace('.', '_') + $"_{meshIndex}");
                        mesh.PrimitiveData.SetPolygonName = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        WorkingObject.MESH.StringBuffer.StringList.Strings.Add("Set_" + mesh.MeshName.Replace('.', '_') + $"_{meshIndex}");
                        mesh.PrimitiveData.MaterialID = Scene.Meshes[meshIndex].MaterialIndex;
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
                else
                {
                    // we can process the mesh data now. we have *everything*
                    foreach (var mesh in WorkingMeshData)
                    {
                        mesh.VertexBindingObject = new VXBO();
                        mesh.VertexBindingObject.Name = mesh.MeshName;
                        mesh.VertexAttributes = new VXAR();
                        mesh.VertexAttributes.Name = mesh.MeshName;
                        mesh.VertexAttributes.Data = WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexArray.Data;
                        mesh.IndexBuffer = new IXBF();
                        mesh.IndexBuffer.Name = mesh.MeshName;
                        mesh.IndexBuffer.BufferData = new byte[mesh.SourceMesh.FaceCount * 3 * 2];
                        int cur = 0;
                        foreach (var index in mesh.SourceMesh.GetShortIndices())
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(index), 0, mesh.IndexBuffer.BufferData, cur, 2);
                            cur++;
                        }
                        mesh.VertexBuffer = new VXBF();
                        mesh.VertexBuffer.Name = mesh.MeshName;
                        mesh.VertexBuffer.Data.U00 = 0;
                        mesh.VertexBuffer.Data.U04 = 0;
                        mesh.VertexBuffer.Data.VertexStride = 4 * mesh.VertexAttributes.Data.VertexAttributes.Count;
                        mesh.VertexBuffer.Data.VertexCount = mesh.SourceMesh.VertexCount;
                        for (int i = 0; i < mesh.SourceMesh.VertexCount; i++)
                        {
                            byte[] vertex = new byte[mesh.VertexBuffer.Data.VertexStride];
                            int cPos = 0;
                            for (int j = 0; j < WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexSemantics.Count; j++)
                            {
                                switch (WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexSemantics[j])
                                {
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_POSITION:
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Vertices[i].X), 0, vertex, cPos, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Vertices[i].Y), 0, vertex, cPos + 4, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Vertices[i].Z), 0, vertex, cPos + 8, 4);
                                        cPos += 16;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_NORMAL:
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Normals[i].X), 0, vertex, cPos, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Normals[i].Y), 0, vertex, cPos + 4, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Normals[i].Z), 0, vertex, cPos + 8, 4);
                                        cPos += 16;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_TEXCOORD:
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.TextureCoordinateChannels[j][i].X), 0, vertex, cPos, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.TextureCoordinateChannels[j][i].Y), 0, vertex, cPos + 4, 4);
                                        cPos += 16;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_TANGENT:
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Tangents[i].X), 0, vertex, cPos, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Tangents[i].Y), 0, vertex, cPos + 4, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Tangents[i].Z), 0, vertex, cPos + 8, 4);
                                        cPos += 16;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_BLENDWEIGHT:
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Tangents[i].X), 0, vertex, cPos, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Tangents[i].Y), 0, vertex, cPos + 4, 4);
                                        Buffer.BlockCopy(BitConverter.GetBytes(mesh.SourceMesh.Tangents[i].Z), 0, vertex, cPos + 8, 4);
                                        cPos += 16;
                                        break;
                                }
                            }
                        }
                    }
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