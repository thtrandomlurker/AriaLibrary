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
                    Console.WriteLine(bone.BoneName);
                    Console.WriteLine(bone.Translation);
                    Console.WriteLine(bone.Rotation);
                    Console.WriteLine(bone.Scale);
                    Node boneToNode = new Node(bone.BoneName);

                    Matrix4x4 trsMatrix = Matrix4x4.FromTranslation(new Vector3D(bone.Translation.X, bone.Translation.Y, bone.Translation.Z));
                    Matrix4x4 rotMatrix = Matrix4x4.FromEulerAnglesXYZ(bone.Rotation.X * (float)Math.PI / 180, bone.Rotation.Y * (float)Math.PI / 180, bone.Rotation.Z * (float)Math.PI / 180);
                    Matrix4x4 sclMatrix = Matrix4x4.FromScaling(new Vector3D(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));
                    Matrix4x4 outMatrix = Matrix4x4.Identity;
                    outMatrix *= trsMatrix;
                    outMatrix *= rotMatrix;
                    outMatrix *= sclMatrix;
                    outMatrix.Inverse();
                    boneToNode.Transform = outMatrix;
                    Node parentNode = null;
                    if (bone.BoneParent != -1)
                        scene.RootNode.FindNode(BRNT.Bones[bone.BoneParent].BoneName).Children.Add(boneToNode);
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
                        Matrix4x4 trsMatrix = Matrix4x4.FromTranslation(new Vector3D(bone.Translation.X, bone.Translation.Y, bone.Translation.Z));
                        Matrix4x4 rotMatrix = Matrix4x4.FromEulerAnglesXYZ(bone.Rotation.X * (float)Math.PI / 180, bone.Rotation.Y * (float)Math.PI / 180, bone.Rotation.Z * (float)Math.PI / 180);
                        Matrix4x4 sclMatrix = Matrix4x4.FromScaling(new Vector3D(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));
                        Matrix4x4 outMatrix = Matrix4x4.Identity;
                        outMatrix *= trsMatrix;
                        outMatrix *= rotMatrix;
                        outMatrix *= sclMatrix;

                        Assimp.Bone aiBone = new Assimp.Bone();
                        aiBone.Name = bone.BoneName;
                        aiBone.OffsetMatrix = outMatrix;
                        aiBone.OffsetMatrix.Transpose();
                        mesh.Bones.Add(aiBone);
                    }
                }

                VXST vxst = (VXST)GPR.Heap.Sections.Where(x => x.Type == "VXST").ToList()[prim.PrimitiveID];

                VXAR vxar = (VXAR)GPR.Heap.Sections.Where(x => x.Type == "VXAR").ToList()[prim.PrimitiveID];

                IXBF ixbf = (IXBF)GPR.Heap.Sections.Where(x => x.Type == "IXBF").ToList()[prim.PrimitiveID];

                VXBF vxbf = (VXBF)GPR.Heap.Sections.Where(x => x.Type == "VXBF").ToList()[prim.PrimitiveID];

                // diry code
                MATE mate = (MATE)MESH.ChildNodes.Where(x => x.Type == "MATE").ToList()[prim.MaterialID];
                SAMP samp = (SAMP)MESH.ChildNodes.Where(x => x.Type == "SAMP").ToList()[mate.SamplerID];  // samplers should always be linear

                EFFE effe = (EFFE)MESH.ChildNodes.Where(x => x.Type == "EFFE").ToList()[mate.EffectID];

                VXSH vs = (VXSH)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effe.TPAS.VertexShaderName] && x.Type == "VXSH").ToList()[0];

                List<string> attributeNames = ShaderHelper.GetInputNames(new MemoryStream(vs.BufferData));

                Material material = new Material();
                material.Name = MESH.StringBuffer.StringList.Strings[mate.Name1];

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
                                    mesh.TextureCoordinateChannels[0].Add(uv);
                                    break;
                                case "in_Col00":
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
                                default:
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

        public void ExportMeshToOBJ(string outDir, int meshIndex)
        {
            VARI vari = (VARI)MESH.ChildNodes.Where(x => x.Type == "VARI").ToList()[0];
            StreamWriter objStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}_{MESH.StringBuffer.StringList.Strings[vari.PRIMs[meshIndex].MeshName]}.obj");
            StreamWriter mtlStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}_{MESH.StringBuffer.StringList.Strings[vari.PRIMs[meshIndex].MeshName]}.mtl");
            StreamWriter grpStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}_{MESH.StringBuffer.StringList.Strings[vari.PRIMs[meshIndex].MeshName]}.grp");

            // write bone names to the GRP
            if (BRNT != null)
            {
                foreach (var bone in BRNT.Bones)
                {
                    if (bone.SkinID != -1)
                        grpStream.WriteLine($"bone {bone.SkinID} {bone.BoneName}");
                }
            }
            

            VXST vxst = (VXST)GPR.Heap.Sections.Where(x => x.Type == "VXST").ToList()[meshIndex];

            VXAR vxar = (VXAR)GPR.Heap.Sections.Where(x => x.Type == "VXAR").ToList()[meshIndex];

            IXBF ixbf = (IXBF)GPR.Heap.Sections.Where(x => x.Type == "IXBF").ToList()[meshIndex];
            List<GPRSection> vxbfs = GPR.Heap.Sections.Where(x => x.Type == "VXBF").Skip(meshIndex).Take(vxst.Data.VertexBufferReferences.Count()).ToList();

            // diry code
            MATE mate = (MATE)MESH.ChildNodes.Where(x => x.Type == "MATE").ToList()[vari.PRIMs[meshIndex].MaterialID];
            SAMP samp = (SAMP)MESH.ChildNodes.Where(x => x.Type == "SAMP").ToList()[mate.SamplerID];  // samplers should always be linear

            objStream.WriteLine($"mtllib {MESH.StringBuffer.StringList.Strings[vari.PRIMs[meshIndex].MeshName]}.mtl");

            mtlStream.WriteLine($"newmtl {MESH.StringBuffer.StringList.Strings[mate.Name1]}");
            mtlStream.WriteLine($"map_Kd {Path.GetFileName(MESH.StringBuffer.StringList.Strings[samp.SSTVs.Where(x => MESH.StringBuffer.StringList.Strings[x.TextureSlot] == "Albedo0").ToList()[0].TextureSourcePath])}");

            objStream.WriteLine($"usemtl {MESH.StringBuffer.StringList.Strings[mate.Name1]}");

            EFFE effe = (EFFE)MESH.ChildNodes.Where(x => x.Type == "EFFE").ToList()[mate.EffectID];

            VXSH vs = (VXSH)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effe.TPAS.VertexShaderName] && x.Type == "VXSH").ToList()[0];

            List<string> attributeNames = ShaderHelper.GetInputNames(new MemoryStream(vs.BufferData));

            int vertexCount = vxst.Data.VertexBufferReferences[0].VertexCount;
            for (int i = 0; i < vertexCount; i++)
            {
                for (int a = 0; a < vxar.Data.VertexAttributes.Count; a++)
                {
                    switch (attributeNames[a])
                    {
                        case "in_Pos0":
                            objStream.Write($"v");
                            if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(float)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 255}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(float)(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 127}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {BitConverter.ToHalf(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {BitConverter.ToSingle(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                objStream.Write("\n");
                            }
                            break;
                        case "in_vN0":
                            objStream.Write($"vn");
                            if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(float)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 255}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(float)(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 127}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {BitConverter.ToHalf(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {BitConverter.ToSingle(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                objStream.Write("\n");
                            }
                            break;
                        case "in_Uv0":
                            objStream.Write($"vt");
                            if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(float)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 255}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {(float)(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 127}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {BitConverter.ToHalf(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                objStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    objStream.Write($" {BitConverter.ToSingle(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                objStream.Write("\n");
                            }
                            break;
                        case "in_BlendIndex":
                            grpStream.Write($"bi");
                            if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {(float)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 255}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {(float)(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 127}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {BitConverter.ToHalf(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {BitConverter.ToSingle(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                grpStream.Write("\n");
                            }
                            break;
                        case "in_BlendWeight":
                            grpStream.Write($"bw");
                            if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {(float)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 255}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {(float)(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 127}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {BitConverter.ToHalf(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                grpStream.Write("\n");
                            }
                            else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                            {
                                for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                                {
                                    grpStream.Write($" {BitConverter.ToSingle(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                                }
                                grpStream.Write("\n");
                            }
                            break;
                    }
                }
            }
            for (int i = 0; i < vxst.Data.FaceIndexCount; i += 3)
            {
                objStream.WriteLine($"f {BitConverter.ToInt16(ixbf.BufferData, (i * 2)) + 1}/{BitConverter.ToInt16(ixbf.BufferData, (i * 2)) + 1}/{BitConverter.ToInt16(ixbf.BufferData, (i * 2)) + 1} {BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 2) + 1}/{BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 2) + 1}/{BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 2) + 1} {BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 4) + 1}/{BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 4) + 1}/{BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 4) + 1}");
            }
            objStream.Close(); mtlStream.Close(); grpStream.Close();
        }

        // TO-DO: Make this entire block not completely awful

        public void ExportModelAsModifiedOBJ(string outDir)
        {
            // object data
            StreamWriter mshStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}.msh");
            // skinning data
            StreamWriter grpStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}.grp");
            // effects
            StreamWriter effStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}.eff");
            // constants
            StreamWriter cstStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}.cst");
            // samplers
            StreamWriter smpStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}.smp");
            // material data
            StreamWriter mtlStream = File.CreateText($"{outDir}\\{GPR.Heap.Name}.mtl");

            VARI meshVari = (VARI)MESH.ChildNodes.Where(x => x.Type == "VARI").ToList()[0];

            foreach (var node in MESH.ChildNodes)
            {
                if (node is EFFE effeNode)
                {
                    effStream.WriteLine($"effect {effeNode.EffectID}");
                    effStream.WriteLine($"name {MESH.StringBuffer.StringList.Strings[effeNode.EffectName]}");
                    effStream.WriteLine($"file {MESH.StringBuffer.StringList.Strings[effeNode.EffectFileName]}");
                    effStream.WriteLine($"type {MESH.StringBuffer.StringList.Strings[effeNode.EffectType]}");
                    // dump shader programs
                    Stream vs = File.Create($"{outDir}\\{MESH.StringBuffer.StringList.Strings[effeNode.EffectFileName]}.vs");
                    Stream ps = File.Create($"{outDir}\\{MESH.StringBuffer.StringList.Strings[effeNode.EffectFileName]}.ps");
                    VXSH vxsh = (VXSH)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effeNode.TPAS.VertexShaderName] && x.Type == "VXSH").ToList()[0];
                    PXSH pxsh = (PXSH)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effeNode.TPAS.PixelShaderName] && x.Type == "PXSH").ToList()[0];
                    foreach (var uniform in vxsh.Data.Uniforms)
                    {
                        effStream.WriteLine($"vs_uniform {uniform.Name} {uniform.Offset} {uniform.U08} {uniform.Size}");
                    }
                    foreach (var uniform in pxsh.Data.Uniforms)
                    {
                        effStream.WriteLine($"ps_uniform {uniform.Name} {uniform.Offset} {uniform.U08} {uniform.Size}");
                    }
                    VXSB vxsb = (VXSB)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effeNode.TPAS.VertexShaderName] && x.Type == "VXSB").ToList()[0];
                    PXSB pxsb = (PXSB)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effeNode.TPAS.PixelShaderName] && x.Type == "PXSB").ToList()[0];
                    effStream.WriteLine($"vs_shaderbinds {(vxsb.Data.ShaderBind0 != null ? 1 : -1)} {(vxsb.Data.ShaderBind1 != null ? 1 : -1)}");
                    if (vxsb.Data.ShaderBind0 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {vxsb.Data.ShaderBind0.U00}");
                        effStream.WriteLine($"u04 {vxsb.Data.ShaderBind0.U04}");
                        foreach (var input in vxsb.Data.ShaderBind0.Parameters)
                        {
                            effStream.WriteLine($"input {input.ParameterName} {input.ParameterResourceIndex} {input.ParameterArraySize}");
                        }
                    }

                    if (vxsb.Data.ShaderBind1 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {vxsb.Data.ShaderBind1.U00}");
                        effStream.WriteLine($"u04 {vxsb.Data.ShaderBind1.U04}");
                        foreach (var input in vxsb.Data.ShaderBind1.Parameters)
                        {
                            effStream.WriteLine($"input {input.ParameterName} {input.ParameterResourceIndex} {input.ParameterArraySize}");
                        }
                    }

                    effStream.WriteLine($"ps_shaderbinds {(pxsb.Data.ShaderBind0 != null ? 1 : -1)} {(pxsb.Data.ShaderBind1 != null ? 1 : -1)}");
                    if (pxsb.Data.ShaderBind0 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {pxsb.Data.ShaderBind0.U00}");
                        effStream.WriteLine($"u04 {pxsb.Data.ShaderBind0.U04}");
                        foreach (var input in pxsb.Data.ShaderBind0.Parameters)
                        {
                            effStream.WriteLine($"input {input.ParameterName} {input.ParameterResourceIndex} {input.ParameterArraySize}");
                        }
                    }

                    if (pxsb.Data.ShaderBind1 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {pxsb.Data.ShaderBind1.U00}");
                        effStream.WriteLine($"u04 {pxsb.Data.ShaderBind1.U04}");
                        foreach (var input in pxsb.Data.ShaderBind1.Parameters)
                        {
                            effStream.WriteLine($"input {input.ParameterName} {input.ParameterResourceIndex} {input.ParameterArraySize}");
                        }
                    }

                    vs.Write(vxsh.BufferData);
                    ps.Write(pxsh.BufferData);
                    vs.Close();
                    ps.Close();
                    // TPAS can be inferred from the eff data
                }
                if (node is CSTS cstsNode)
                {
                    cstStream.WriteLine($"constant_set {cstsNode.ConstantSetID}");
                    foreach (var cstv in cstsNode.ConstantValues)
                    {
                        SHCO shaderConstant = (SHCO)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[cstv.ConstantDataName] && x.Type == "SHCO").ToList()[0];
                        cstStream.WriteLine($"constant {MESH.StringBuffer.StringList.Strings[cstv.ConstantName]} {MESH.StringBuffer.StringList.Strings[cstv.ConstantDataName]}");
                        foreach (var constant in shaderConstant.Data.Constants)
                        {
                            cstStream.WriteLine($"value {constant[0]} {constant[1]} {constant[2]} {constant[3]}");
                        }
                    }
                }
                else if (node is SAMP sampNode)
                {
                    smpStream.WriteLine($"sampler {sampNode.SamplerID}");
                    foreach (var tex in sampNode.SSTVs)
                    {
                        smpStream.WriteLine($"texture {MESH.StringBuffer.StringList.Strings[tex.TextureName]}");
                        smpStream.WriteLine($"file {MESH.StringBuffer.StringList.Strings[tex.TextureSourcePath]}");
                        smpStream.WriteLine($"type {MESH.StringBuffer.StringList.Strings[tex.TextureSlot]}");
                    }
                }
                else if (node is MATE mateNode)
                {
                    mtlStream.WriteLine($"material {mateNode.MaterialID}");
                    mtlStream.WriteLine($"name1 {MESH.StringBuffer.StringList.Strings[mateNode.Name1]}");
                    mtlStream.WriteLine($"name2 {MESH.StringBuffer.StringList.Strings[mateNode.Name2]}");
                    mtlStream.WriteLine($"# shader refers to a shader effect. the number is the id of the effect as defined in the .eff");
                    mtlStream.WriteLine($"shader {mateNode.EffectID}");
                    mtlStream.WriteLine($"# vs_constants refers to the constant set used by the vertex shader. the number is the id of the constant set as defined in the .cst");
                    mtlStream.WriteLine($"vs_constants {mateNode.VertexConstantID}");
                    mtlStream.WriteLine($"neg1 {mateNode.Neg1}");
                    mtlStream.WriteLine($"# ps_constants refers to the constant set used by the pixel shader. the number is the id of the constant set as defined in the .cst");
                    mtlStream.WriteLine($"ps_constants {mateNode.PixelConstantID}");
                    mtlStream.WriteLine($"# sampler refers to the sampler holding this material's textures. the number is the id of the sampler as defined in the .smp");
                    mtlStream.WriteLine($"sampler {mateNode.SamplerID}");
                }
                else if (node is BONE boneNode)
                {
                    foreach (var boif in boneNode.BoneInformation)
                    {
                        grpStream.WriteLine($"bone {MESH.StringBuffer.StringList.Strings[boif.BoneName]}");
                    }
                }
            }

            int sectionMeshDataIdx = 0;

            foreach (var prim in meshVari.PRIMs)
            {
                mshStream.WriteLine($"primitive {prim.PrimitiveID}");
                mshStream.WriteLine($"name1 {MESH.StringBuffer.StringList.Strings[prim.MeshName]}");
                mshStream.WriteLine($"name2 {MESH.StringBuffer.StringList.Strings[prim.SetPolygonName]}");
                mshStream.WriteLine($"object {MESH.StringBuffer.StringList.Strings[prim.ObjectName]}");
                mshStream.WriteLine($"material {prim.MaterialID}");

                VXST vxst = (VXST)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[prim.MeshName] && x.Type == "VXST").ToList()[0];
                VXBO vxbo = (VXBO)GPR.Heap.Sections[sectionMeshDataIdx];
                VXAR vxar = (VXAR)GPR.Heap.Sections[sectionMeshDataIdx+1];
                IXBF ixbf = (IXBF)GPR.Heap.Sections[sectionMeshDataIdx+2];
                List<GPRSection> vxbfs = GPR.Heap.Sections.Skip(sectionMeshDataIdx + 3).Take(vxst.Data.VXBFCount).ToList();
                sectionMeshDataIdx += 3 + vxst.Data.VXBFCount;

                // diry code
                MATE mate = (MATE)MESH.ChildNodes.Where(x => x.Type == "MATE").ToList()[prim.MaterialID];

                EFFE effe = (EFFE)MESH.ChildNodes.Where(x => x.Type == "EFFE").ToList()[mate.EffectID];

                VXSH vs = (VXSH)GPR.Heap.Sections.Where(x => x.Name == MESH.StringBuffer.StringList.Strings[effe.TPAS.VertexShaderName] && x.Type == "VXSH").ToList()[0];

                List<string> attributeNames = ShaderHelper.GetInputNames(new MemoryStream(vs.BufferData));

                mshStream.WriteLine($"attributes");
                for (int i = 0; i < vxar.Data.VertexAttributes.Count; i++)
                {
                    mshStream.WriteLine($"attribute {attributeNames[i]}");
                    mshStream.WriteLine($"offset {vxar.Data.VertexAttributes[i].Offset}");
                    mshStream.WriteLine($"buffer {vxar.Data.VertexAttributes[i].VertexBufferIndex}");
                    mshStream.WriteLine($"count {vxar.Data.VertexAttributes[i].Count}");
                    mshStream.WriteLine($"type {vxar.Data.VertexAttributes[i].DataType}");
                }

                int vertexCount = vxst.Data.VertexBufferReferences[0].VertexCount;
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int a = 0; a < vxar.Data.VertexAttributes.Count; a++)
                    {
                        mshStream.Write(attributeNames[a]);
                        if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByte)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                            }
                            mshStream.Write("\n");
                        }
                        else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByte)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                            }
                            mshStream.Write("\n");
                        }
                        else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteNormalized)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {(float)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 255}");
                            }
                            mshStream.Write("\n");
                        }
                        else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteNormalized)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {(float)(sbyte)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)] / 127}");
                            }
                            mshStream.Write("\n");
                        }
                        else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.HalfFloat)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {BitConverter.ToHalf(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 2) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                            }
                            mshStream.Write("\n");
                        }
                        else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.Float)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {BitConverter.ToSingle(vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData, vxar.Data.VertexAttributes[a].Offset + (v * 4) + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride))}");
                            }
                            mshStream.Write("\n");
                        }
                    }
                }
                for (int i = 0; i < vxst.Data.FaceIndexCount; i += 3)
                {
                    mshStream.WriteLine($"face {BitConverter.ToInt16(ixbf.BufferData, (i * 2))} {BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 2)} {BitConverter.ToInt16(ixbf.BufferData, (i * 2) + 4)}");
                }
            }
            mshStream.Close();
            grpStream.Close();
            effStream.Close();
            cstStream.Close();
            smpStream.Close();
            mtlStream.Close();
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
                    BRNT.Save(brntStream);
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
