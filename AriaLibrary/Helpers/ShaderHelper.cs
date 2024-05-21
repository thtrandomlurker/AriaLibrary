using AriaLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;

using AriaLibrary.Objects.GraphicsProgram;
using System.Reflection.PortableExecutable;
using System.Drawing.Printing;

namespace AriaLibrary.Helpers
{
    // Referenced from Vita3K  https://github.com/Vita3K/Vita3K
    public enum SceGxmParameterType : byte
    {
        SCE_GXM_PARAMETER_TYPE_F32,
        SCE_GXM_PARAMETER_TYPE_F16,
        SCE_GXM_PARAMETER_TYPE_C10,
        SCE_GXM_PARAMETER_TYPE_U32,
        SCE_GXM_PARAMETER_TYPE_S32,
        SCE_GXM_PARAMETER_TYPE_U16,
        SCE_GXM_PARAMETER_TYPE_S16,
        SCE_GXM_PARAMETER_TYPE_U8,
        SCE_GXM_PARAMETER_TYPE_S8,
        SCE_GXM_PARAMETER_TYPE_AGGREGATE
    }
    public enum SceGxmParameterCategory : byte
    {
        SCE_GXM_PARAMETER_CATEGORY_ATTRIBUTE,
        SCE_GXM_PARAMETER_CATEGORY_UNIFORM,
        SCE_GXM_PARAMETER_CATEGORY_SAMPLER,
        SCE_GXM_PARAMETER_CATEGORY_AUXILIARY_SURFACE,
        SCE_GXM_PARAMETER_CATEGORY_UNIFORM_BUFFER
    }
    public enum SceGxmParameterSemantic : byte
    {
        SCE_GXM_PARAMETER_SEMANTIC_NONE,
        SCE_GXM_PARAMETER_SEMANTIC_ATTR,
        SCE_GXM_PARAMETER_SEMANTIC_BCOL,
        SCE_GXM_PARAMETER_SEMANTIC_BINORMAL,
        SCE_GXM_PARAMETER_SEMANTIC_BLENDINDICES,
        SCE_GXM_PARAMETER_SEMANTIC_BLENDWEIGHT,
        SCE_GXM_PARAMETER_SEMANTIC_COLOR,
        SCE_GXM_PARAMETER_SEMANTIC_DIFFUSE,
        SCE_GXM_PARAMETER_SEMANTIC_FOGCOORD,
        SCE_GXM_PARAMETER_SEMANTIC_NORMAL,
        SCE_GXM_PARAMETER_SEMANTIC_POINTSIZE,
        SCE_GXM_PARAMETER_SEMANTIC_POSITION,
        SCE_GXM_PARAMETER_SEMANTIC_SPECULAR,
        SCE_GXM_PARAMETER_SEMANTIC_TANGENT,
        SCE_GXM_PARAMETER_SEMANTIC_TEXCOORD,
        SCE_GXM_PARAMETER_SEMANTIC_INDEX,
        SCE_GXM_PARAMETER_SEMANTIC_INSTANCE
    }

    public class SceGxmProgramParameter
    {
        public string ParameterName;
        public SceGxmParameterType DataType;
        public SceGxmParameterCategory Category;
        public byte ContainerIndex;
        public byte ComponentCount;
        public SceGxmParameterSemantic Semantic;
        public byte SemanticIndex;
        public int ArraySize;
        public int ResourceIndex;

        public void Read(BinaryReader reader)
        {
            int nameOffset = reader.ReadInt32();
            byte TypeCat = reader.ReadByte();
            DataType = (SceGxmParameterType)((TypeCat & 0xF0) >> 4);
            Category = (SceGxmParameterCategory)(TypeCat & 0x0F);
            byte ContCount = reader.ReadByte();
            ContainerIndex = (byte)((ContCount & 0xF0) >> 4);
            ComponentCount = (byte)(ContCount & 0x0F);
            Semantic = (SceGxmParameterSemantic)reader.ReadByte();
            SemanticIndex = reader.ReadByte();
            ArraySize = reader.ReadInt32();
            ResourceIndex = reader.ReadInt32();
            ParameterName = StringReader.ReadNullTerminatedStringAtOffset(reader, nameOffset - 16, SeekOrigin.Current);
        }

    }
    public static class ShaderHelper
    {
        // These values should only be set by the renderer
        public static string[] RendererParams =
        { 
          "WorldITXf",
          "WorldVPXf",
          "WorldXf",
          "cameraPosition",
          "iDiffuseCol0",
          "jointTable",
          "iDiffuseCol0",
          "iExposure",
          "iLightDir",
          "iLightCol0",
          "iTimerBais",
          "illuminationSCALAR",
          "illuminationBias",
          "iFogCol0",
          "iFogNear",
          "iScrollUV0"
        };

        public static List<string> GetPerMaterialConstantNames(Stream shaderStream, bool leaveOpen = false)
        {
            List<string> allInputs = GetInputNames(shaderStream, leaveOpen);

            return allInputs.Where(x => !RendererParams.Contains(x) && x.StartsWith("i")).ToList();
        }

        public static List<string> GetSamplerNames(Stream shaderStream, bool leaveOpen = false)
        {
            List<string> allInputs = GetInputNames(shaderStream, leaveOpen);

            return allInputs.Where(x => !RendererParams.Contains(x) && !x.StartsWith("i")).ToList();
        }

        public static List<string> GetInputNames(Stream shaderStream, bool leaveOpen = false)
        {
            List<string> inputs = new List<string>();
            using (BinaryReader reader = new BinaryReader(shaderStream, Encoding.UTF8, leaveOpen))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                Console.WriteLine(new string(reader.ReadChars(3)));
                reader.BaseStream.Seek(0x24, SeekOrigin.Begin);
                int inputCount = reader.ReadInt32();
                int inputsOffset = reader.ReadInt32();
                reader.BaseStream.Seek(inputsOffset - 4, SeekOrigin.Current);
                for (int i = 0; i < inputCount; i++)
                {
                    int stringOffset = reader.ReadInt32();
                    inputs.Add(StringReader.ReadNullTerminatedStringAtOffset(reader, stringOffset - 4, SeekOrigin.Current));
                    reader.BaseStream.Seek(0x0C, SeekOrigin.Current);
                }
            }
            return inputs;
        }

        public static List<SceGxmProgramParameter> GetParameters(Stream shaderStream, bool leaveOpen = false)
        {
            List<SceGxmProgramParameter> parameters = new List<SceGxmProgramParameter>();
            using (BinaryReader reader = new BinaryReader(shaderStream, Encoding.UTF8, leaveOpen))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                Console.WriteLine(new string(reader.ReadChars(3)));
                reader.BaseStream.Seek(0x24, SeekOrigin.Begin);
                int inputCount = reader.ReadInt32();
                int inputsOffset = reader.ReadInt32();
                reader.BaseStream.Seek(inputsOffset - 4, SeekOrigin.Current);
                for (int i = 0; i < inputCount; i++)
                {
                    SceGxmProgramParameter parameter = new SceGxmProgramParameter();
                    parameter.Read(reader);
                    parameters.Add(parameter);
                }
            }
            return parameters;
        }
        public static int GetResourceIndex(Stream shaderStream, string resourceName)
        {
            return GetParameters(shaderStream, true).First(x => x.ParameterName == resourceName).ResourceIndex;
        }
    }
}
