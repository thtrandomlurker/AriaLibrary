﻿using System;
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
using Assimp.Unmanaged;
using System.Security.Cryptography;

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
        public BONE OutputBoneData;
        public ModelImportWizard(string path)
        {
            InitializeComponent();
            mSourceFilePath = path;
            // set the title bar
            Text = $"Model Import Wizard: {Path.GetFileName(mSourceFilePath)}";
            WorkingMaterialData = new List<MaterialData>();
            WorkingMeshData = new List<MeshData>();
            OutputBoneData = new BONE();
        }
        private static Matrix4x4 GetWorldTransform(Node aiNode)
        {
            var transform = aiNode.Transform;

            while ((aiNode = aiNode.Parent) != null)
                transform *= aiNode.Transform;

            return transform.ToNumerics();
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

                MeshNodes = Scene.RootNode.Children.Where(x => x.MeshCount != 0).ToList();


                WorkingObject.NODT.Remark = new REM("Model created using AriaLibrary v0.1");
                // create base nodes
                NODE rootNode = new NODE();
                rootNode.NodeName = WorkingObject.NODT.StringBuffer.StringList.Strings.Count;
                WorkingObject.NODT.StringBuffer.StringList.Strings.Add(((MIWInitControl)MIWActiveStageControl).MIWInitModelName.Text);
                rootNode.NodeId = WorkingObject.NODT.ChildNodes.Count;
                rootNode.NodeChild = -1;
                rootNode.NodeParent = -1;
                rootNode.NodeMatrix = Matrix4x4.Identity;

                WorkingObject.NODT.ChildNodes.Add(rootNode);

                NODE drawNode = new NODE();
                drawNode.NodeName = WorkingObject.NODT.StringBuffer.StringList.Strings.Count;
                WorkingObject.NODT.StringBuffer.StringList.Strings.Add(((MIWInitControl)MIWActiveStageControl).MIWInitDSNAMode.Text);
                drawNode.NodeId = WorkingObject.NODT.ChildNodes.Count;
                drawNode.NodeChild = -1;
                drawNode.NodeParent = -1;
                drawNode.NodeMatrix = Matrix4x4.Identity;

                WorkingObject.NODT.ChildNodes.Add(drawNode);

                NODE objectNode = new NODE();
                objectNode.NodeName = WorkingObject.NODT.StringBuffer.StringList.Strings.Count;
                WorkingObject.NODT.StringBuffer.StringList.Strings.Add(((MIWInitControl)MIWActiveStageControl).MIWInitModelName.Text + "_model");
                objectNode.NodeId = WorkingObject.NODT.ChildNodes.Count;
                objectNode.NodeChild = -1;
                objectNode.NodeParent = -1;
                objectNode.NodeMatrix = Matrix4x4.Identity;

                WorkingObject.NODT.ChildNodes.Add(objectNode);

                NODE meshNode = new NODE();
                meshNode.NodeName = WorkingObject.NODT.StringBuffer.StringList.Strings.Count;
                WorkingObject.NODT.StringBuffer.StringList.Strings.Add("SET_model");
                meshNode.NodeId = WorkingObject.NODT.ChildNodes.Count;
                meshNode.NodeChild = -1;
                meshNode.NodeParent = -1;
                meshNode.NodeMatrix = Matrix4x4.Identity;
                meshNode.InstanceData = new INST();
                meshNode.InstanceData.MeshCluster.MeshClusterId = 0;
                meshNode.InstanceData.MeshCluster.MeshClusterName = WorkingObject.NODT.StringBuffer.StringList.Strings.Count;
                WorkingObject.NODT.StringBuffer.StringList.Strings.Add("SET_model-geom");

                int cmesh = 0;

                // check if scene contains bones
                bool hasBone = false;
                foreach (var node in MeshNodes)
                {
                    foreach (int meshIndex in node.MeshIndices)
                    {
                        if (Scene.Meshes[meshIndex].BoneCount > 0)
                        {
                            hasBone = true;
                            foreach (var bone in Scene.Meshes[meshIndex].Bones)
                            {
                                if (!skinBoneList.Contains(bone.Name))
                                    skinBoneList.Add(bone.Name);
                            }
                        }

                        MeshData mesh = new MeshData();
                        mesh.MeshName = $"importMesh{cmesh}-geom-P{cmesh}";
                        mesh.BufferName = $"importMesh{cmesh}-geom-S{cmesh}";
                        mesh.SetName = $"SET_{mesh.MeshName}";
                        mesh.SourceMesh = Scene.Meshes[meshIndex];

                        // we can create prim data here as it should directly correlate to the materials created later
                        // also cluster data

                        CLUS cluster = new CLUS();
                        cluster.ClusterId = meshNode.InstanceData.MeshCluster.Clusters.Count;
                        cluster.ClusterName = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        cluster.Neg1 = -1;
                        meshNode.InstanceData.MeshCluster.Clusters.Add(cluster);
                        mesh.PrimitiveData = new PRIM();
                        mesh.PrimitiveData.PrimitiveID = cmesh++;
                        mesh.PrimitiveData.ObjectName = 0;  // this is hard embedded into the code of this program
                        mesh.PrimitiveData.MeshName = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        mesh.PrimitiveData.MeshNameDupe = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        WorkingObject.MESH.StringBuffer.StringList.Strings.Add(mesh.MeshName);
                        mesh.PrimitiveData.SetPolygonName = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        WorkingObject.MESH.StringBuffer.StringList.Strings.Add(mesh.SetName);
                        mesh.PrimitiveData.MaterialID = Scene.Meshes[meshIndex].MaterialIndex;
                        mesh.PrimitiveData.U18 = -1;
                        WorkingMeshData.Add(mesh);
                    }
                }

                WorkingObject.NODT.ChildNodes.Add(meshNode);
                // create the BRNT if so
                if (hasBone)
                {
                    WorkingObject.BRNT = new BRNT();

                    void AddBones(Node aiNode)
                    {
                        Bone bone = new Bone();
                        bone.BoneName = aiNode.Name;
                        Matrix4x4 boneMat = aiNode.Transform.ToNumerics();
                        Matrix4x4.Invert(boneMat, out Matrix4x4 invMat);
                        Matrix4x4.Decompose(invMat, out Vector3 scl, out System.Numerics.Quaternion rot, out Vector3 loc);
                        bone.Translation = new Vector3(boneMat[0,3], boneMat[1,3], boneMat[2,3]);
                        Vector3 eulerRot = MathHelper.QuaternionToEulerAngles(rot.W, rot.X, rot.Y, rot.Z);
                        eulerRot *= (float)(180 / Math.PI);  // conv all at once
                        bone.Rotation = eulerRot;
                        bone.Scale = new Vector3(scl.X, scl.Y, scl.Z);
                        bone.BoneID = (short)WorkingObject.BRNT.Bones.Count;
                        bone.SkinID = (short)skinBoneList.FindIndex(x => x == aiNode.Name);
                        bone.PossibleFlags = -256;
                        bone.U18 = -1;
                        bone.U1E = -1;
                        bone.BoneNameHash = StringHelper.GetStringHash(bone.BoneName);
                        if (aiNode.ChildCount == 1)
                        {
                            bone.ChildID = (short)(WorkingObject.BRNT.Bones.Count + 1);
                        }
                        else
                        {
                            bone.ChildID = -1;
                        }
                        if (WorkingObject.BRNT.Bones.Count != 0)
                        {
                            if (aiNode.Parent != null)
                            {
                                bone.BoneParent = WorkingObject.BRNT.Bones.First(x => x.BoneName == aiNode.Parent.Name).BoneID;
                            }
                        }
                        else
                        {
                            bone.BoneParent = -1;
                        }
                        WorkingObject.BRNT.Bones.Add(bone);

                        BOIF boneInfo = new BOIF();
                        boneInfo.BoneName = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                        WorkingObject.MESH.StringBuffer.StringList.Strings.Add(bone.BoneName);
                        boneInfo.BoneId = bone.BoneID;

                        IMTX inverseMatrix = new IMTX();
                        Matrix4x4 mat = GetWorldTransform(aiNode);
                        inverseMatrix.Matrix[0] = mat.M11;
                        inverseMatrix.Matrix[1] = mat.M12;
                        inverseMatrix.Matrix[2] = mat.M13;
                        inverseMatrix.Matrix[3] = mat.M21;
                        inverseMatrix.Matrix[4] = mat.M22;
                        inverseMatrix.Matrix[5] = mat.M23;
                        inverseMatrix.Matrix[6] = mat.M31;
                        inverseMatrix.Matrix[7] = mat.M32;
                        inverseMatrix.Matrix[8] = mat.M33;
                        inverseMatrix.Matrix[9] = mat.M41;
                        inverseMatrix.Matrix[10] = mat.M42;
                        inverseMatrix.Matrix[11] = mat.M43;
                        OutputBoneData.BoneInformation.Add(boneInfo);
                        OutputBoneData.InverseMatrices.Add(inverseMatrix);

                        NODE nodtNode = new NODE();
                        nodtNode.NodeId = WorkingObject.NODT.ChildNodes.Count;
                        nodtNode.NodeParent = -1;
                        nodtNode.NodeChild = -1;
                        nodtNode.NodeName = WorkingObject.NODT.StringBuffer.StringList.Strings.Count;
                        WorkingObject.NODT.StringBuffer.StringList.Strings.Add(aiNode.Name);
                        nodtNode.NodeMatrix = GetWorldTransform(aiNode);
                        WorkingObject.NODT.ChildNodes.Add(nodtNode);

                        foreach (var child in aiNode.Children)
                        {
                            AddBones(child);
                        }
                    }
                    AddBones(Scene.RootNode.Children.First(x => x.MeshCount == 0).Children[0]);  // start from skeleton root. maybe.
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

                WorkingObject.GPR = new GraphicsProgram();
                WorkingObject.GPR.Platform = "GXM";
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
                        mesh.IndexBuffer.Buffer = BufferName.Mesh;
                        int cur = 0;
                        foreach (var index in mesh.SourceMesh.GetShortIndices())
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(index), 0, mesh.IndexBuffer.BufferData, cur, 2);
                            cur += 2;
                        }
                        mesh.VertexBuffer = new VXBF();
                        mesh.VertexBuffer.Name = mesh.BufferName;
                        mesh.VertexBuffer.Data.U00 = 0;
                        mesh.VertexBuffer.Data.U04 = 0;
                        mesh.VertexBuffer.Data.VertexStride = WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexStride;
                        mesh.VertexBuffer.Data.VertexCount = mesh.SourceMesh.VertexCount;
                        mesh.VertexBuffer.BufferData = new byte[mesh.VertexBuffer.Data.VertexCount * mesh.VertexBuffer.Data.VertexStride];
                        mesh.VertexBuffer.Buffer = BufferName.Mesh;


                        byte[][] tIndices = new byte[mesh.SourceMesh.VertexCount][];
                        float[][] tWeights = new float[mesh.SourceMesh.VertexCount][];
                        int[] tIndicesCurPos = new int[mesh.SourceMesh.VertexCount];

                        for (int i = 0; i < mesh.SourceMesh.VertexCount; i++)
                        {
                            tIndices[i] = new byte[4];
                            tWeights[i] = new float[4];
                        }

                        foreach (var bone in mesh.SourceMesh.Bones)
                        {
                            foreach (var weight in bone.VertexWeights)
                            {
                                if (tIndicesCurPos[weight.VertexID] < 4)
                                {
                                    Console.WriteLine(weight.VertexID);
                                    tIndices[weight.VertexID][tIndicesCurPos[weight.VertexID]] = (byte)WorkingObject.BRNT.Bones.First(x => x.BoneName == bone.Name).SkinID;
                                    tWeights[weight.VertexID][tIndicesCurPos[weight.VertexID]] = weight.Weight;
                                }
                                tIndicesCurPos[weight.VertexID]++;
                            }
                        }

                        // fixup weights
                        foreach (var weight in tWeights)
                        {
                            float sum = weight[0] + weight[1] + weight[2] + weight[3];
                            if (!(0.9999f < sum && sum < 1.0001f))
                            {
                                float diff = 1f - sum;
                                if (diff < 0f)
                                {
                                    weight[0] -= diff;
                                }
                                else
                                {
                                    weight[0] += diff;
                                }
                            }
                        }


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
                                        cPos += 12;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_NORMAL:
                                        vertex[cPos] = (byte)(mesh.SourceMesh.Normals[i].X * 128f); 
                                        vertex[cPos + 1] = (byte)(mesh.SourceMesh.Normals[i].Y * 128f);
                                        vertex[cPos + 2] = (byte)(mesh.SourceMesh.Normals[i].Z * 128f);
                                        cPos += 3;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_TANGENT:
                                        vertex[cPos] = (byte)(mesh.SourceMesh.Tangents[i].X * 128f);
                                        vertex[cPos + 1] = (byte)(mesh.SourceMesh.Tangents[i].Y * 128f);
                                        vertex[cPos + 2] = (byte)(mesh.SourceMesh.Tangents[i].Z * 128f);
                                        vertex[cPos + 3] = 127;
                                        cPos += 4;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_TEXCOORD:
                                        if (mesh.SourceMesh.TextureCoordinateChannelCount >= WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexSemanticIndices[j])
                                        {
                                            Buffer.BlockCopy(BitConverter.GetBytes((Half)mesh.SourceMesh.TextureCoordinateChannels[WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexSemanticIndices[j]][i].X), 0, vertex, cPos, 2);
                                            Buffer.BlockCopy(BitConverter.GetBytes((Half)mesh.SourceMesh.TextureCoordinateChannels[WorkingMaterialData[mesh.SourceMesh.MaterialIndex].VertexSemanticIndices[j]][i].Y), 0, vertex, cPos + 2, 2);
                                            cPos += 4;
                                        }
                                        else
                                        {
                                            Buffer.BlockCopy(BitConverter.GetBytes((int)0), 0, vertex, cPos, 4);
                                            cPos += 4;
                                        }
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_BLENDWEIGHT:
                                        vertex[cPos] = (byte)(tWeights[i][0] * 255f);
                                        vertex[cPos + 1] = (byte)(tWeights[i][1] * 255f);
                                        vertex[cPos + 2] = (byte)(tWeights[i][2] * 255f);
                                        vertex[cPos + 3] = (byte)(tWeights[i][3] * 255f);
                                        cPos += 4;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_BLENDINDICES:
                                        Buffer.BlockCopy(tIndices[i], 0, vertex, cPos, 4);
                                        cPos += 4;
                                        break;
                                    case SceGxmParameterSemantic.SCE_GXM_PARAMETER_SEMANTIC_COLOR:
                                        if (mesh.SourceMesh.HasVertexColors(0))
                                        {
                                            vertex[cPos] = (byte)(mesh.SourceMesh.VertexColorChannels[0][i].R * 255f);
                                            vertex[cPos+1] = (byte)(mesh.SourceMesh.VertexColorChannels[0][i].G * 255f);
                                            vertex[cPos+2] = (byte)(mesh.SourceMesh.VertexColorChannels[0][i].B * 255f);
                                            vertex[cPos+3] = (byte)(mesh.SourceMesh.VertexColorChannels[0][i].A * 255f);
                                            cPos += 4;
                                        }
                                        else
                                        {
                                            Buffer.BlockCopy(BitConverter.GetBytes((int)-1), 0, vertex, cPos, 4);
                                            cPos += 4;
                                        }
                                        break;
                                }
                            }
                            Buffer.BlockCopy(vertex, 0, mesh.VertexBuffer.BufferData, mesh.VertexBuffer.Data.VertexStride * i, mesh.VertexBuffer.Data.VertexStride);
                        }
                        mesh.VertexState = new VXST();
                        mesh.VertexState.Name = mesh.MeshName;
                        mesh.VertexState.Data.VXBFCount = 1;
                        mesh.VertexState.Data.FaceIndexCount = mesh.SourceMesh.FaceCount * 3;
                        mesh.VertexState.Data.VertexBindingObjectReference = mesh.VertexBindingObject.Data;
                        mesh.VertexState.Data.VertexArrayReference = mesh.VertexAttributes.Data;
                        mesh.VertexState.Data.VertexBufferReferences.Add(mesh.VertexBuffer.Data);
                        mesh.VertexState.Data.IndexBufferReference = mesh.IndexBuffer.Data;
                    }
                    // we *done* in this
                    // now this is going to be a bit scuffed but, we should be able to do this now
                    // start with trsp
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.MESH.ChildNodes.Add(mat.MaterialTransparencySetting);
                    }
                    // next effe
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.MESH.ChildNodes.Add(mat.MaterialEffect);
                    }
                    // csts
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.MESH.ChildNodes.Add(mat.VertexConstants);
                        WorkingObject.MESH.ChildNodes.Add(mat.FragmentConstants);
                    }
                    // samp
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.MESH.ChildNodes.Add(mat.MaterialSampler);
                    }
                    // mate
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.MESH.ChildNodes.Add(mat.Material);
                    }
                    // prim
                    VARI vari = new VARI();
                    foreach (var msh in WorkingMeshData)
                    {
                        vari.PRIMs.Add(msh.PrimitiveData);
                    }
                    vari.U04 = WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
                    WorkingObject.MESH.StringBuffer.StringList.Strings.Add("Default");
                    vari.U08 = -1;
                    vari.U0C = -1;
                    WorkingObject.MESH.ChildNodes.Add(vari);
                    WorkingObject.MESH.ChildNodes.Add(OutputBoneData);
                    // next GPR
                    // mesh data
                    foreach (var msh in WorkingMeshData)
                    {
                        WorkingObject.GPR.Heap.Sections.Add(msh.VertexBindingObject);
                        WorkingObject.GPR.Heap.Sections.Add(msh.VertexAttributes);
                        WorkingObject.GPR.Heap.Sections.Add(msh.IndexBuffer);
                        WorkingObject.GPR.Heap.Sections.Add(msh.VertexBuffer);
                    }
                    // VXST
                    foreach (var msh in WorkingMeshData)
                    {
                        WorkingObject.GPR.Heap.Sections.Add(msh.VertexState);
                    }
                    // vertex shader data
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.GPR.Heap.Sections.Add(mat.ShaderMetadata);
                        WorkingObject.GPR.Heap.Sections.Add(mat.VertexShader);
                        WorkingObject.GPR.Heap.Sections.Add(mat.VertexShaderBinding);
                    }
                    // vertex shader data
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.GPR.Heap.Sections.Add(mat.PixelShader);
                        WorkingObject.GPR.Heap.Sections.Add(mat.PixelShaderConstantBinding);
                        WorkingObject.GPR.Heap.Sections.Add(mat.PixelShaderSamplerBinding);
                    }
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.GPR.Heap.Sections.AddRange(mat.ShaderConstants);
                    }
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.GPR.Heap.Sections.AddRange(mat.SamplerStates);
                    }
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.GPR.Heap.Sections.Add(mat.VXSB);
                    }
                    foreach (var mat in WorkingMaterialData)
                    {
                        WorkingObject.GPR.Heap.Sections.Add(mat.PXSB);
                    }
                }
                using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Select a location to save the created model file", Filter = "IA/VT Model|*.mdl" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        WorkingObject.SavePackage(sfd.FileName);
                    }
                }
                this.Close();
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