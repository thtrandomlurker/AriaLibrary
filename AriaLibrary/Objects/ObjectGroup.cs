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

namespace AriaLibrary.Objects
{
    // not an actual type, meant to help with grouping data
    public class ObjectGroup
    {
        public KPack SourcePackage;
        public GraphicsProgram.GraphicsProgram GPR;
        public MESH MESH;
        public NODT NODT;

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
                        foreach (var input in vxsb.Data.ShaderBind0.Inputs)
                        {
                            effStream.WriteLine($"input {input.InputName} {input.U04} {input.U08}");
                        }
                    }

                    if (vxsb.Data.ShaderBind1 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {vxsb.Data.ShaderBind1.U00}");
                        effStream.WriteLine($"u04 {vxsb.Data.ShaderBind1.U04}");
                        foreach (var input in vxsb.Data.ShaderBind1.Inputs)
                        {
                            effStream.WriteLine($"input {input.InputName} {input.U04} {input.U08}");
                        }
                    }

                    effStream.WriteLine($"ps_shaderbinds {(pxsb.Data.ShaderBind0 != null ? 1 : -1)} {(pxsb.Data.ShaderBind1 != null ? 1 : -1)}");
                    if (pxsb.Data.ShaderBind0 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {pxsb.Data.ShaderBind0.U00}");
                        effStream.WriteLine($"u04 {pxsb.Data.ShaderBind0.U04}");
                        foreach (var input in pxsb.Data.ShaderBind0.Inputs)
                        {
                            effStream.WriteLine($"input {input.InputName} {input.U04} {input.U08}");
                        }
                    }

                    if (pxsb.Data.ShaderBind1 != null)
                    {
                        effStream.WriteLine($"shaderbind0");
                        effStream.WriteLine($"u00 {pxsb.Data.ShaderBind1.U00}");
                        effStream.WriteLine($"u04 {pxsb.Data.ShaderBind1.U04}");
                        foreach (var input in pxsb.Data.ShaderBind1.Inputs)
                        {
                            effStream.WriteLine($"input {input.InputName} {input.U04} {input.U08}");
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
                        if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.UnsignedByteIndex)
                        {
                            for (int v = 0; v < vxar.Data.VertexAttributes[a].Count; v++)
                            {
                                mshStream.Write($" {vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex].BufferData[vxar.Data.VertexAttributes[a].Offset + v + (i * ((VXBF)vxbfs[vxar.Data.VertexAttributes[a].VertexBufferIndex]).Data.VertexStride)]}");
                            }
                            mshStream.Write("\n");
                        }
                        else if (vxar.Data.VertexAttributes[a].DataType == VertexAttributeDataType.SignedByteIndex)
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
                for (int i = 3; i < SourcePackage.Files.Count; i++)
                {
                    SourcePackage.Files[i].Offset = PositionHelper.PadValue(0x40 + SourcePackage.Files[i-1].Offset + SourcePackage.Files[i-1].Size, 0x40);
                }
                SourcePackage.Save(filePath);
                meshStream.Close();
                gprStream.Close();
                nodtStream.Close();
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
