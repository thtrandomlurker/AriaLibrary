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
