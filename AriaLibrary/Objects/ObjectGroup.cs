using AriaLibrary.Archives;
using AriaLibrary.Objects.GraphicsProgram;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Security;
using System.Reflection.Metadata;
using Assimp;
using Assimp.Unmanaged;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using Assimp.Configs;
using System.ComponentModel.DataAnnotations;

namespace AriaLibrary.Objects
{
    // not an actual type, meant to help with grouping data
    public class ObjectGroup
    {
        public KPack SourcePackage;
        public GraphicsProgram.GraphicsProgram GPR;
        public MESH MESH;
        public NODT NODT;
        public BRNT? BRNT;

        public void ImportModelFromFBX(string filePath/*, string baseShaderPackagePath*/)
        {


            AssimpContext context = new AssimpContext();

            // (credits to skyth) borrow this from MikuMikuLibrary https://github.com/blueskythlikesclouds/MikuMikuLibrary/blob/master/MikuMikuLibrary/Objects/Processing/Assimp/AssimpSceneHelper.cs
            context.SetConfig(new FBXPreservePivotsConfig(false));
            context.SetConfig(new MaxBoneCountConfig(64));
            context.SetConfig(new MeshTriangleLimitConfig(524288));
            context.SetConfig(new MeshVertexLimitConfig(32768));
            context.SetConfig(new VertexBoneWeightLimitConfig(4));
            context.SetConfig(new VertexCacheSizeConfig(63));

            Scene scene = context.ImportFile(filePath,
            PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate |
            PostProcessSteps.SplitLargeMeshes | PostProcessSteps.LimitBoneWeights |
            PostProcessSteps.ImproveCacheLocality | PostProcessSteps.SortByPrimitiveType |
            PostProcessSteps.SplitByBoneCount | PostProcessSteps.FlipUVs);

            // we'll need this later

            int meshTrspInsertBase = 1;

            int gprInsertPosition = 0;

            // load package containing the shader
            // to-do: figure out how to actually use this
            /*KPack shaderPack = new KPack();
            shaderPack.Load(baseShaderPackagePath);

            shaderPack.Files[0].Open();
            shaderPack.Files[1].Open();

            // get the attributes/inputs
            List<string> vertexInputNames = ShaderHelper.GetInputNames(shaderPack.Files[0].Stream);
            List<string> fragmentInputNames = ShaderHelper.GetInputNames(shaderPack.Files[1].Stream);

            shaderPack.Files[0].Close();
            shaderPack.Files[1].Close();*/

            // clear existing mesh data from all relevant areas
            // get clean GPR sections
            string[] grpDoNotInclude = new string[] {"VXBO", "VXAR", "IXBF", "VXBF", "VXST"};
            List<GPRSection> cleanGPRSections = GPR.Heap.Sections.Where(x => !grpDoNotInclude.Contains(x.Type)).ToList();

            List<GPRSection> VXSTs = new List<GPRSection>();

            string[] meshDoNotInclude = new string[] {"TRSP"};
            List<NodeBlock> cleanMeshNodes = MESH.ChildNodes.Where(x => !meshDoNotInclude.Contains(x.Type)).ToList();

            //MESH newMeshBlock = new MESH();

            /*for (int m = 0; m < scene.Materials.Count; m++)
            {
                Material material = scene.Materials[m];
            }*/

            VARI vari = (VARI)cleanMeshNodes.First(x => x.Type == "VARI");

            NODE meshInstanceNode = (NODE)NODT.ChildNodes.First(x => x.Type == "NODE" && ((NODE)x).InstanceData != null);

            vari.PRIMs.Clear();

            meshInstanceNode.InstanceData.MeshCluster.Clusters.Clear();

            int meshPartsBaseId = 0;
            foreach (var mesh in scene.Meshes)
            {
                Console.WriteLine("writing mesh");
                // create trsp
                TRSP trsp = new TRSP();

                trsp.TRSPId = meshPartsBaseId;

                trsp.Culling = scene.Materials[mesh.MaterialIndex].IsTwoSided ? CullMode.None : CullMode.BackFace;

                trsp.U08 = 1;
                trsp.U0C = 0;
                trsp.U10 = 2;
                trsp.U14 = 0;
                trsp.U18 = 1;
                trsp.U1C = 1;

                cleanMeshNodes.Insert(meshTrspInsertBase, trsp);

                MATE basisMaterial = null;

                // find the first material that matches the criteria we need
                foreach (MATE material in MESH.ChildNodes.Where(x => x.Type == "MATE"))
                {
                    EFFE matEffect = (EFFE)MESH.ChildNodes.First(x => x.Type == "EFFE" && ((EFFE)x).EffectID == material.EffectID);
                    VXSH vs = (VXSH)GPR.Heap.Sections.First(x => x.Type == "VXSH" && x.Name == MESH.StringBuffer.StringList.Strings[matEffect.TPAS.VertexShaderName]);
                    PXSH ps = (PXSH)GPR.Heap.Sections.First(x => x.Type == "PXSH" && x.Name == MESH.StringBuffer.StringList.Strings[matEffect.TPAS.PixelShaderName]);

                    List<string> vertexAttributes = ShaderHelper.GetInputNames(new MemoryStream(vs.BufferData));
                    List<string> pixelAttributes = ShaderHelper.GetInputNames(new MemoryStream(ps.BufferData));

                    if (vertexAttributes.Contains("in_Pos0") && vertexAttributes.Contains("in_vN0") && vertexAttributes.Contains("in_Uv0") && vertexAttributes.Contains("in_BlendIndex") && vertexAttributes.Contains("in_BlendWeight") && !vertexAttributes.Contains("in_Col0"))
                    {
                        if (pixelAttributes.Contains("Albedo0") && !pixelAttributes.Contains("envTexture"))
                        {
                            basisMaterial = material;
                            break;
                        }
                    }
                }

                if (basisMaterial == null)
                {
                    throw new InvalidDataException("The MDL contains no suitable material to import a new model with");
                }

                // create prim for now since we're skipping everything else as a test
                PRIM prim = new PRIM();
                prim.PrimitiveID = meshPartsBaseId;
                prim.MeshName = MESH.StringBuffer.StringList.Strings.Count;
                prim.SetPolygonName = MESH.StringBuffer.StringList.Strings.Count;
                prim.ObjectName = 0;
                prim.MeshNameDupe = MESH.StringBuffer.StringList.Strings.Count;
                prim.MaterialID = basisMaterial.MaterialID;
                prim.U18 = -1;
                MESH.StringBuffer.StringList.Strings.Add($"MeshData-geom-P{meshPartsBaseId}");

                CLUS cluster = new CLUS();
                cluster.ClusterId = meshPartsBaseId;
                cluster.ClusterName = NODT.StringBuffer.StringList.Strings.Count;
                cluster.Neg1 = -1;
                NODT.StringBuffer.StringList.Strings.Add($"MeshData-geom-P{meshPartsBaseId}");

                meshInstanceNode.InstanceData.MeshCluster.Clusters.Add(cluster);

                vari.PRIMs.Add(prim);

                // proceed to generate GPR data
                VXBO vxbo = new VXBO();
                VXAR vxar = new VXAR();
                IXBF ixbf = new IXBF();
                VXBF vxbf = new VXBF();
                VXST vxst = new VXST();
                vxbo.Name = $"MeshData-geom-P{meshPartsBaseId}";
                vxar.Name = $"MeshData-geom-P{meshPartsBaseId}";
                ixbf.Name = $"MeshData-geom-P{meshPartsBaseId}";
                vxbf.Name = $"MeshData-geom-P{meshPartsBaseId}_vxbf";
                vxst.Name = $"MeshData-geom-P{meshPartsBaseId}";

                // attribute info should always be the same
                // pos
                vxar.Data.VertexAttributes.Add(new VertexAttribute() { Offset = 0, VertexBufferIndex = 0, Count = 3, DataType = VertexAttributeDataType.Float });
                // normal
                vxar.Data.VertexAttributes.Add(new VertexAttribute() { Offset = 12, VertexBufferIndex = 0, Count = 3, DataType = VertexAttributeDataType.SignedByteNormalized });
                // uv
                vxar.Data.VertexAttributes.Add(new VertexAttribute() { Offset = 15, VertexBufferIndex = 0, Count = 2, DataType = VertexAttributeDataType.HalfFloat });
                // indices
                vxar.Data.VertexAttributes.Add(new VertexAttribute() { Offset = 19, VertexBufferIndex = 0, Count = 4, DataType = VertexAttributeDataType.UnsignedByte });
                // weights
                vxar.Data.VertexAttributes.Add(new VertexAttribute() { Offset = 23, VertexBufferIndex = 0, Count = 4, DataType = VertexAttributeDataType.UnsignedByteNormalized });

                // now put it into the vxbf

                int[][] tIndices = new int[mesh.VertexCount][];
                float[][] tWeights = new float[mesh.VertexCount][];
                int[] tIndicesCurPos = new int[mesh.VertexCount];

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    tIndices[i] = new int[4];
                    tWeights[i] = new float[4];
                }

                foreach (var bone in mesh.Bones)
                {
                    foreach (var weight in bone.VertexWeights)
                    {
                        Console.WriteLine(weight.VertexID);
                        tIndices[weight.VertexID][tIndicesCurPos[weight.VertexID]] = BRNT.Bones.First(x => x.BoneName == bone.Name).SkinID;
                        tWeights[weight.VertexID][tIndicesCurPos[weight.VertexID]] = weight.Weight;
                        tIndicesCurPos[weight.VertexID]++;
                    }
                }

                MemoryStream tVertexStream = new MemoryStream();

                using (BinaryWriter vertWriter = new BinaryWriter(tVertexStream, Encoding.UTF8, true))
                {
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        vertWriter.Write(BitConverter.GetBytes(mesh.Vertices[i].X));
                        vertWriter.Write(BitConverter.GetBytes(mesh.Vertices[i].Y));
                        vertWriter.Write(BitConverter.GetBytes(mesh.Vertices[i].Z));
                        vertWriter.Write((byte)(sbyte)(mesh.Normals[i].X * 127));
                        vertWriter.Write((byte)(sbyte)(mesh.Normals[i].Y * 127));
                        vertWriter.Write((byte)(sbyte)(mesh.Normals[i].Z * 127));
                        vertWriter.Write(BitConverter.GetBytes((Half)mesh.TextureCoordinateChannels[0][i].X));
                        vertWriter.Write(BitConverter.GetBytes((Half)mesh.TextureCoordinateChannels[0][i].Y));
                        vertWriter.Write((byte)tIndices[i][0]);
                        vertWriter.Write((byte)tIndices[i][1]);
                        vertWriter.Write((byte)tIndices[i][2]);
                        vertWriter.Write((byte)tIndices[i][3]);
                        vertWriter.Write((byte)(sbyte)(tWeights[i][0] * 255));
                        vertWriter.Write((byte)(sbyte)(tWeights[i][1] * 255));
                        vertWriter.Write((byte)(sbyte)(tWeights[i][2] * 255));
                        vertWriter.Write((byte)(sbyte)(tWeights[i][3] * 255));
                    }
                }

                vxbf.BufferData = tVertexStream.ToArray();
                tVertexStream.Close();

                vxbf.Data.VertexCount = mesh.VertexCount;
                vxbf.Data.VertexStride = 27;

                vxst.Data.FaceIndexCount = mesh.FaceCount * 3;
                vxst.Data.VXBFCount = 1;

                MemoryStream tIndexStream = new MemoryStream();

                using (BinaryWriter indexWriter = new BinaryWriter(tIndexStream, Encoding.UTF8, true))
                {
                    foreach (var face in mesh.Faces)
                    {
                        indexWriter.Write(BitConverter.GetBytes((short)face.Indices[0]));
                        indexWriter.Write(BitConverter.GetBytes((short)face.Indices[1]));
                        indexWriter.Write(BitConverter.GetBytes((short)face.Indices[2]));
                    }
                }

                ixbf.BufferData = tIndexStream.ToArray();
                tIndexStream.Close();

                vxst.Data.VertexBufferReferences.Add(vxbf.Data);

                cleanGPRSections.Insert(gprInsertPosition, vxbo);
                cleanGPRSections.Insert(gprInsertPosition + 1, vxar);
                cleanGPRSections.Insert(gprInsertPosition + 2, ixbf);
                cleanGPRSections.Insert(gprInsertPosition + 3, vxbf);
                gprInsertPosition += 4;
                VXSTs.Add(vxst);
                

                meshPartsBaseId++;
            }
            cleanGPRSections.InsertRange(gprInsertPosition, VXSTs);
            MESH.StringBuffer.StringCount = MESH.StringBuffer.StringList.Strings.Count;
            NODT.StringBuffer.StringCount = NODT.StringBuffer.StringList.Strings.Count;
            GPR.Heap.Sections = cleanGPRSections;
            MESH.ChildNodes = cleanMeshNodes;
        }

        public void ExportModelToFBX(string filePath)
        {
            AssimpContext context = new AssimpContext();

            Scene scene = new Scene();
            scene.RootNode = new Node(GPR.Heap.Name);

            // skeleton if applicable

            if (BRNT != null)
            {
                foreach (var bone in BRNT.Bones)
                {
                    Node boneToNode = new Node(bone.BoneName);

                    Matrix4x4 trsMatrix = Matrix4x4.FromTranslation(new Vector3D(-bone.Translation.X, -bone.Translation.Y, -bone.Translation.Z));
                    Matrix4x4 rotMatrix = Matrix4x4.FromEulerAnglesXYZ(bone.Rotation.X * (float)Math.PI / 180, bone.Rotation.Y * (float)Math.PI / 180, bone.Rotation.Z * (float)Math.PI / 180);
                    Matrix4x4 sclMatrix = Matrix4x4.FromScaling(new Vector3D(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));
                    Matrix4x4 outMatrix = Matrix4x4.Identity;
                    outMatrix *= trsMatrix;
                    outMatrix *= rotMatrix;
                    outMatrix *= sclMatrix;
                    outMatrix.Inverse();

                    boneToNode.Transform = outMatrix;
                    if (bone.BoneParent != -1)
                    {
                        scene.RootNode.FindNode(BRNT.Bones[bone.BoneParent].BoneName).Children.Add(boneToNode);
                    }
                    else
                    {
                        scene.RootNode.Children.Add(boneToNode);
                    }
                }
            }

            // meshes

            VARI vari = (VARI)MESH.ChildNodes.First(x => x.Type == "VARI");

            foreach (PRIM prim in vari.PRIMs)
            {
                Node meshNode = new Node(MESH.StringBuffer.StringList.Strings[prim.MeshName]);

                Mesh mesh = new Mesh();
                mesh.Name = MESH.StringBuffer.StringList.Strings[prim.MeshName];
                mesh.PrimitiveType = PrimitiveType.Triangle;

                if (BRNT != null)
                {
                    foreach (var bone in BRNT.Bones)
                    {
                        Matrix4x4 offsetMatrix = AssimpHelpers.CalculateNodeMatrixWS(scene.RootNode.FindNode(bone.BoneName));
                        offsetMatrix.Inverse();

                        Assimp.Bone aiBone = new Assimp.Bone();
                        aiBone.Name = bone.BoneName;
                        aiBone.OffsetMatrix = offsetMatrix;
                        mesh.Bones.Add(aiBone);
                    }
                }

                VXST vxst = (VXST)GPR.Heap.Sections.Where(x => x.Type == "VXST").ToList()[prim.PrimitiveID];

                VXAR vxar = (VXAR)GPR.Heap.Sections.Where(x => x.Type == "VXAR").ToList()[prim.PrimitiveID];

                IXBF ixbf = (IXBF)GPR.Heap.Sections.Where(x => x.Type == "IXBF").ToList()[prim.PrimitiveID];

                VXBF vxbf = (VXBF)GPR.Heap.Sections.Where(x => x.Type == "VXBF").ToList()[prim.PrimitiveID];

                // diry code
                TRSP trsp = (TRSP)MESH.ChildNodes.Where(x => x.Type == "TRSP").ToList()[prim.PrimitiveID];

                MATE mate = (MATE)MESH.ChildNodes.Where(x => x.Type == "MATE").ToList()[prim.MaterialID];
                SAMP samp = (SAMP)MESH.ChildNodes.Where(x => x.Type == "SAMP").ToList()[mate.SamplerID];  // samplers should always be linear

                EFFE effe = (EFFE)MESH.ChildNodes.Where(x => x.Type == "EFFE").ToList()[mate.EffectID];

                VXSH vs = (VXSH)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effe.TPAS.VertexShaderName] && x.Type == "VXSH").ToList()[0];

                List<string> attributeNames = ShaderHelper.GetInputNames(new MemoryStream(vs.BufferData));

                Material material = new Material();
                material.Name = MESH.StringBuffer.StringList.Strings[mate.Name1];
                material.IsTwoSided = trsp.Culling == CullMode.None;

                foreach (var sstv in samp.SSTVs)
                {
                    switch (MESH.StringBuffer.StringList.Strings[sstv.TextureSlot])
                    {
                        case "Albedo0":
                            TextureSlot diffSlot = new TextureSlot();
                            diffSlot.TextureType = TextureType.Diffuse;
                            diffSlot.FilePath = Path.GetFileName(MESH.StringBuffer.StringList.Strings[sstv.TextureSourcePath]);
                            material.AddMaterialTexture(diffSlot);
                            break;
                        case "envTexture":
                            TextureSlot envSlot = new TextureSlot();
                            envSlot.TextureType = TextureType.Reflection;
                            envSlot.FilePath = Path.GetFileName(MESH.StringBuffer.StringList.Strings[sstv.TextureSourcePath]);
                            material.AddMaterialTexture(envSlot);
                            break;
                    }
                }

                scene.Materials.Add(material);
                mesh.MaterialIndex = prim.MaterialID;

                List<int[]> tWeightIndices = new List<int[]>();
                List<float[]> tWeights = new List<float[]>();
                int vertexCount = vxst.Data.VertexBufferReferences[0].VertexCount;
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int a = 0; a < vxar.Data.VertexAttributes.Count; a++)
                    {
                        if (vxar.Data.VertexAttributes[a].VertexBufferIndex == 0)
                        {
                            switch (attributeNames[a])
                            {
                                case "in_Pos0":
                                    Vector3D vert = new Vector3D();
                                    if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = (sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = BitConverter.ToUInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = BitConverter.ToInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = (float)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 255;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = (float)(sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 127;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = (float)BitConverter.ToHalf(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            vert[v] = (float)BitConverter.ToSingle(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    mesh.Vertices.Add(vert);
                                    break;
                                case "in_vN0":
                                    Vector3D normal = new Vector3D();
                                    if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = (sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = BitConverter.ToUInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = BitConverter.ToInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = (float)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 255;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = (float)(sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 127;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = (float)BitConverter.ToHalf(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            normal[v] = (float)BitConverter.ToSingle(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    mesh.Normals.Add(normal);
                                    break;
                                case "in_Uv0":
                                    Vector3D uv = new Vector3D();
                                    if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = (sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = BitConverter.ToUInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = BitConverter.ToInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = (float)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 255;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = (float)(sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 127;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = (float)BitConverter.ToHalf(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            uv[v] = (float)BitConverter.ToSingle(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    uv[1] *= -1;
                                    mesh.TextureCoordinateChannels[0].Add(uv);
                                    break;
                                case "in_Col0":
                                    Color4D col = new Color4D();
                                    if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = (sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = BitConverter.ToUInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = BitConverter.ToInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = (float)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 255;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = (float)(sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 127;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = (float)BitConverter.ToHalf(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            col[v] = (float)BitConverter.ToSingle(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    mesh.VertexColorChannels[0].Add(col);
                                    break;
                                case "in_BlendIndex":
                                    int[] indices = new int[4];
                                    if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            indices[v] = vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            indices[v] = (sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))];
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            indices[v] = BitConverter.ToUInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedShort)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            indices[v] = BitConverter.ToInt16(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                                    {
                                        throw new InvalidDataException("How is there a float type being put into an int array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                                    {
                                        throw new InvalidDataException("How is there a float type being put into an int array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                                    {
                                        throw new InvalidDataException("How is there a float type being put into an int array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                                    {
                                        throw new InvalidDataException("How is there a float type being put into an int array this makes no sense");
                                    }
                                    tWeightIndices.Add(indices);
                                    break;
                                case "in_BlendWeight":
                                    float[] weights = new float[4];
                                    if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                                    {
                                        throw new InvalidDataException("How is there an integer type being put into a float array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                                    {
                                        throw new InvalidDataException("How is there an integer type being put into a float array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedShort)
                                    {
                                        throw new InvalidDataException("How is there an integer type being put into a float array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedShort)
                                    {
                                        throw new InvalidDataException("How is there an integer type being put into a float array this makes no sense");
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            weights[v] = (float)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 255;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            weights[v] = (float)(sbyte)vxbf.BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * (vxbf.Data.VertexStride))] / 127;
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            weights[v] = (float)BitConverter.ToHalf(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                                    {
                                        for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                        {
                                            weights[v] = (float)BitConverter.ToSingle(vxbf.BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * (vxbf.Data.VertexStride)));
                                        }
                                    }
                                    tWeights.Add(weights);
                                    break;
                            }
                        }
                    }
                }
                for (int i = 0; i < vxst.Data.FaceIndexCount; i += 3)
                {
                    mesh.Faces.Add(new Face(new int[3] { BitConverter.ToInt16(ixbf.BufferData, (i * 2)), BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 2), BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 4) }));
                }
                for (int v = 0; v < vxbf.Data.VertexCount; v++)
                {
                    for (int w = 0; w < 4; w++)
                    {
                        int boneIndex = BRNT.Bones.Find(x => x.SkinID == tWeightIndices[v][w]).BoneID;
                        mesh.Bones[boneIndex].VertexWeights.Add(new VertexWeight(v, tWeights[v][w]));
                    }
                }

                scene.Meshes.Add(mesh);
                meshNode.MeshIndices.Add(prim.PrimitiveID);
                scene.RootNode.Children.Add(meshNode);
                
            }
            string formatId = new AssimpContext().GetSupportedExportFormats()
                .First(x => x.FileExtension.Equals("fbx", StringComparison.OrdinalIgnoreCase)).FormatId;

            context.ExportFile(scene, filePath, formatId, Assimp.PostProcessSteps.None);
        }

        public void Load(KPack package)
        {
            package.Files[0].Open();
            package.Files[1].Open();
            package.Files[2].Open();
            MESH.Load(package.Files[0].Stream);
            GPR.Load(package.Files[1].Stream);
            NODT.Load(package.Files[2].Stream);
            package.Files[0].Close();
            package.Files[1].Close();
            package.Files[2].Close();

            if (package.Files.Count() > 3)
            {
                // safe to assume it has a BRNT
                package.Files[3].Open();
                BRNT = new BRNT();
                BRNT.Load(package.Files[3].Stream);
                package.Files[3].Close();
            }
            SourcePackage = package;
        }

        public void Load(Stream gprStream, Stream meshStream, Stream nodtStream)
        {
            GPR.Load(gprStream);
            MESH.Load(meshStream);
            NODT.Load(nodtStream);
        }

        public void Load(string gprPath, string meshPath, string nodtPath)
        {
            GPR.Load(gprPath);
            MESH.Load(meshPath);
            NODT.Load(nodtPath);
        }

        public void LoadPackage(string filePath)
        {
            SourcePackage = new KPack();
            SourcePackage.Load(filePath);
            Load(SourcePackage);
        }

        public void SavePackage(string filePath)
        {
            if (SourcePackage != null)
            {
                foreach (var file in SourcePackage.Files)
                {
                    file.Open();
                }
                Stream meshStream = new MemoryStream();
                Stream gprStream = new MemoryStream();
                Stream nodtStream = new MemoryStream();
                Stream brntStream = new MemoryStream();
                MESH.Save(meshStream, true);
                GPR.Save(gprStream, true);
                NODT.Save(nodtStream, true);
                SourcePackage.Files[0].Stream = meshStream;
                SourcePackage.Files[1].Stream = gprStream;
                SourcePackage.Files[2].Stream = nodtStream;
                SourcePackage.Files[0].BaseStream = null;
                SourcePackage.Files[1].BaseStream = null;
                SourcePackage.Files[2].BaseStream = null;
                // regenerate offsets and sizes
                SourcePackage.Files[0].Size = (int)meshStream.Length;
                SourcePackage.Files[1].Size = (int)gprStream.Length;
                SourcePackage.Files[2].Size = (int)nodtStream.Length;
                SourcePackage.Files[0].Offset = 0x40;
                SourcePackage.Files[1].Offset = PositionHelper.PadValue(0x40 + (int)meshStream.Length, 0x40);
                SourcePackage.Files[2].Offset = PositionHelper.PadValue(0x40 + SourcePackage.Files[1].Offset + SourcePackage.Files[1].Size, 0x40);
                int fileBase = 3;
                if (BRNT != null)
                {
                    BRNT.Save(brntStream, true);
                    SourcePackage.Files[3].Size = (int)brntStream.Length;
                    SourcePackage.Files[3].Offset = PositionHelper.PadValue(0x40 + SourcePackage.Files[2].Offset + SourcePackage.Files[2].Size, 0x40);
                    SourcePackage.Files[3].BaseStream = null;
                    SourcePackage.Files[3].Stream = brntStream;
                    fileBase++;
                }
                for (int i = fileBase; i < SourcePackage.Files.Count; i++)
                {
                    SourcePackage.Files[i].Offset = PositionHelper.PadValue(0x40 + SourcePackage.Files[i-1].Offset + SourcePackage.Files[i-1].Size, 0x40);
                }
                SourcePackage.Save(filePath);
                meshStream.Close();
                gprStream.Close();
                nodtStream.Close();
                brntStream.Close();
            }
        }

        public void Save(Stream gprStream, Stream meshStream, Stream nodtStream)
        {
            GPR.Save(gprStream);
            MESH.Save(meshStream);
            NODT.Save(nodtStream);
        }

        public void Save(string gprPath, string meshPath, string nodtPath)
        {
            GPR.Save(gprPath);
            MESH.Save(meshPath);
            NODT.Save(nodtPath);
        }

        public ObjectGroup()
        {
            GPR = new GraphicsProgram.GraphicsProgram();
            MESH = new MESH();
            NODT = new NODT();
        }
    }
}
