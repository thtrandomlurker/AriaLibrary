using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Nodes;
using Assimp;

namespace IAModelEditor.ImportHelpers
{
    public class MaterialValidity
    {
        public bool Valid;
        public string Message;

        public MaterialValidity(bool valid, string message)
        {
            Valid = valid;
            Message = message;
        }
    }
    public class MaterialData
    {
        public Material? SourceMaterial;
        // MESH material related sections
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EFFE MaterialEffect { get; set; }  // read shader from GPR or external

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SAMP MaterialSampler { get; set; }  // read smst from GPR

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CSTS VertexConstants { get; set; }  // read shco from gpr

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CSTS FragmentConstants { get; set; }  // read shco from gpr

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TRSP MaterialTransparencySetting { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MATE Material { get; set; }

        // GPR material related sections
        public List<SHCO> ShaderConstants { get; set; }
        public List<SMST> SamplerStates { get; set; }


        public SHMI ShaderMetadata { get; set; }  // usually nulled

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VXSH VertexShader { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SHBI VertexShaderBinding { get; set; } // contains per-material parameters, bound to the vertex shader from constants

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PXSH PixelShader { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SHBI PixelShaderConstantBinding { get; set; } // contains per-material parameters, bound from constants

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SHBI PixelShaderSamplerBinding { get; set; } // contains per-material parameters, bound from constants


        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VXSB? VXSB { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PXSB? PXSB { get; set; }

        public byte[]? VertexProgram;  // Vertex shader program
        public byte[]? FragmentProgram;  // Fragment shader program
        // internal
        public string? ShaderName { get; set; }  // Stores the name of the shader to be used
        public int ShaderPermutation;  // Stores the permutation index. Useless?

        public string? MaterialName { get; set; }
        public bool Initialized = false;

        public MaterialValidity IsValid(STRL stringList)
        {
            // validate vertex program
            if (VertexProgram == null || VertexProgram?.Length == 0)
            {
                return new MaterialValidity(false, "Vertex program is null");
            }

            // validate fragment program
            if (FragmentProgram == null || FragmentProgram?.Length == 0)
            {
                return new MaterialValidity(false, "Fragment program is null");
            }

            if (MaterialName == null)
            {
                return new MaterialValidity(false, "Material name cannot be null. I don't even know how this could've happened");
            }
            
            if (MaterialName == "")
            {
                return new MaterialValidity(false, "Material name cannot be empty.");
            }

            if (VertexConstants == null)
            {
                return new MaterialValidity(false, "Material lacks Vertex Constants. This will cause a GPU crash.");
            }
            else
            {
                if (VertexConstants.ConstantValues.Count == 0)
                {
                    return new MaterialValidity(false, "Material lacks Vertex Constants. This will cause a GPU crash.");
                }
                else
                {
                    // disregard intellisense, this shouldn't be null because we'd have thrown by now
                    List<string> expectedValues = ShaderHelper.GetPerMaterialConstantNames(new MemoryStream(VertexProgram)).Where(x => !x.StartsWith("in_")).ToList();
                    Dictionary<string, bool> matchedValues = new Dictionary<string, bool>();
                    foreach (string cstvName in expectedValues)
                    {
                        matchedValues.Add(cstvName, false);
                    }
                    foreach (var cstv in VertexConstants.ConstantValues)
                    {
                        try
                        {
                            matchedValues[stringList.Strings[cstv.ConstantName]] = true;
                        }
                        catch
                        {
                            return new MaterialValidity(false, $"Material contains an invalid vertex constant: \"{stringList.Strings[cstv.ConstantName]}\".");
                        }
                    }
                    foreach (var item in matchedValues)
                    {
                        if (!item.Value)
                        {
                            return new MaterialValidity(false, $"Material is missing a required vertex constant: \"{item.Key}\".");
                        }
                    }
                }
            }

            if (FragmentConstants == null)
            {
                return new MaterialValidity(false, "Material lacks Fragment Constants. This will cause a GPU crash.");
            }
            else
            {
                if (FragmentConstants.ConstantValues.Count == 0)
                {
                    return new MaterialValidity(false, "Material lacks Fragment Constants. This will cause a GPU crash.");
                }
                else
                {
                    // disregard intellisense, this shouldn't be null because we'd have thrown by now
                    List<string> expectedValues = ShaderHelper.GetPerMaterialConstantNames(new MemoryStream(FragmentProgram));
                    Dictionary<string, bool> matchedValues = new Dictionary<string, bool>();
                    foreach (string cstvName in expectedValues)
                    {
                        matchedValues.Add(cstvName, false);
                    }
                    foreach (var cstv in FragmentConstants.ConstantValues)
                    {
                        try
                        {
                            matchedValues[stringList.Strings[cstv.ConstantName]] = true;
                        }
                        catch
                        {
                            return new MaterialValidity(false, $"Material contains an invalid fragment constant: \"{stringList.Strings[cstv.ConstantName]}\".");
                        }
                    }
                    foreach (var item in matchedValues)
                    {
                        if (!item.Value)
                        {
                            return new MaterialValidity(false, $"Material is missing a required fragment constant: \"{item.Key}\".");
                        }
                    }
                }
            }

            if (MaterialSampler == null)
            {
                return new MaterialValidity(false, "Material is missing sampler information. This may possibly cause a GPU crash");
            }
            else
            {
                if (MaterialSampler.SSTVs.Count == 0)
                {
                    return new MaterialValidity(false, "Material is missing sampler information. This may possibly cause a GPU crash.");
                }
                else
                {
                    // disregard intellisense, this shouldn't be null because we'd have thrown by now
                    List<string> expectedValues = ShaderHelper.GetSamplerNames(new MemoryStream(FragmentProgram));
                    Dictionary<string, bool> matchedValues = new Dictionary<string, bool>();
                    foreach (string sstvName in expectedValues)
                    {
                        matchedValues.Add(sstvName, false);
                    }
                    foreach (var sstv in MaterialSampler.SSTVs)
                    {
                        try
                        {
                            matchedValues[stringList.Strings[sstv.TextureSlot]] = true;
                        }
                        catch
                        {
                            return new MaterialValidity(false, $"Material contains an invalid sampler texture: \"{stringList.Strings[sstv.TextureSlot]}\".");
                        }
                    }
                    foreach (var item in matchedValues)
                    {
                        if (!item.Value)
                        {
                            return new MaterialValidity(false, $"Material is missing a required sampler texture: \"{item.Key}\".");
                        }
                    }
                }
            }



            return new MaterialValidity(true, "Material passes minimum criterea");
        }


        public MaterialData()
        {
            MaterialEffect = new EFFE();
            MaterialSampler = new SAMP();
            VertexConstants = new CSTS();
            FragmentConstants = new CSTS();
            MaterialTransparencySetting = new TRSP();
            Material = new MATE();
            ShaderConstants = new List<SHCO>();
            SamplerStates = new List<SMST>();
        }
    }
}
