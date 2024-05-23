﻿using AriaLibrary.Archives;
using AriaLibrary.Helpers;
using AriaLibrary.Objects;
using AriaLibrary.Objects.Nodes;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using IAModelEditor.GUI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using IAModelEditor.ImportHelpers;

namespace IAModelEditor.GUI.Forms.ModelImportWizard
{
    public partial class MIWShaderSelector : Form
    {
        public KPack ShaderPackage;
        new public ModelImportWizard ParentForm;
        new public MIWMaterialSetupControl Parent;
        public string SourceName;
        public MIWShaderSelector(string shaderPackagePath, ModelImportWizard parentForm, MIWMaterialSetupControl parent)
        {
            InitializeComponent();
            KPack shaderPackage = new KPack();
            shaderPackage.Load(shaderPackagePath);
            ShaderPackage = shaderPackage;
            ParentForm = parentForm;
            Parent = parent;
            SourceName = Path.GetFileNameWithoutExtension(shaderPackagePath);
            Text = $"Shader Selector: {SourceName}";
            for (int i = 0; i < shaderPackage.Files.Count / 2; i++)
            {
                MIWShaderList.Items.Add($"Permutation {i}");
            }

            // init the info
            ShaderPackage.Files[0].Open();
            ShaderPackage.Files[1].Open();
            List<string> vertexAttributes = ShaderHelper.GetPerMaterialConstantNames(ShaderPackage.Files[0].Stream, true);
            List<string> fragmentAttributes = ShaderHelper.GetPerMaterialConstantNames(ShaderPackage.Files[1].Stream, true);
            List<string> fragmentSamplers = ShaderHelper.GetSamplerNames(ShaderPackage.Files[1].Stream, true);

            MIWShaderVertexAttributeList.Items.Clear();
            MIWShaderVertexAttributeList.Items.AddRange(vertexAttributes.Where(x => x.StartsWith("in_")).ToArray());
            MIWShaderVertexConstList.Items.Clear();
            MIWShaderVertexConstList.Items.AddRange(vertexAttributes.Where(x => !x.StartsWith("in_")).ToArray());
            MIWShaderFragmentSampList.Items.Clear();
            MIWShaderFragmentSampList.Items.AddRange(fragmentSamplers.ToArray());
            MIWShaderFragmentConstList.Items.Clear();
            MIWShaderFragmentConstList.Items.AddRange(fragmentAttributes.ToArray());
            ShaderPackage.Files[0].Close();
            ShaderPackage.Files[1].Close();
            MIWShaderList.SelectedIndex = 0;
        }

        private void MIWShaderList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Open();
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Open();
            List<string> vertexAttributes = ShaderHelper.GetPerMaterialConstantNames(ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Stream, true);
            List<string> fragmentAttributes = ShaderHelper.GetPerMaterialConstantNames(ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Stream, true);
            List<string> fragmentSamplers = ShaderHelper.GetSamplerNames(ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Stream, true);

            MIWShaderVertexAttributeList.Items.Clear();
            MIWShaderVertexAttributeList.Items.AddRange(vertexAttributes.Where(x => x.StartsWith("in_")).ToArray());
            MIWShaderVertexConstList.Items.Clear();
            MIWShaderVertexConstList.Items.AddRange(vertexAttributes.Where(x => !x.StartsWith("in_")).ToArray());
            MIWShaderFragmentSampList.Items.Clear();
            MIWShaderFragmentSampList.Items.AddRange(fragmentSamplers.ToArray());
            MIWShaderFragmentConstList.Items.Clear();
            MIWShaderFragmentConstList.Items.AddRange(fragmentAttributes.ToArray());
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Close();
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Close();
        }

        private void MakeMaterial(int shaderIndex, int materialIndex)
        {
            int CurrentStringIndex()
            {
                return ParentForm.WorkingObject.MESH.StringBuffer.StringList.Strings.Count;
            }

            void AddString(string s)
            {
                ParentForm.WorkingObject.MESH.StringBuffer.StringList.Strings.Add(s);
            }

            ShaderPackage.Files[(shaderIndex * 2)].Open();
            ShaderPackage.Files[(shaderIndex * 2) + 1].Open();
            ParentForm.WorkingMaterialData[materialIndex].VertexProgram = new byte[ShaderPackage.Files[(shaderIndex * 2)].Size];
            ParentForm.WorkingMaterialData[materialIndex].FragmentProgram = new byte[ShaderPackage.Files[(shaderIndex * 2)+1].Size];

            // so we begin with the TRPS
            ParentForm.WorkingMaterialData[materialIndex].MaterialTransparencySetting = new TRSP() { TRSPId = materialIndex, Culling = CullMode.None, U08 = 1, U0C = 0, U10 = 2, U14 = 0, U18 = 1, U1C = 1 };

            // then the EFFE
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect = new EFFE();
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.EffectID = materialIndex;
            // set effect name index then push string to string list
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.EffectName = CurrentStringIndex();  // current index
            AddString(ParentForm.WorkingMaterialData[materialIndex].MaterialName + "-fx");

            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.EffectFileName = CurrentStringIndex();  // current index
            AddString(SourceName + ".cgfx");

            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.EffectType = CurrentStringIndex();  // current index
            AddString("Shader");

            // create TPAS for EFFE
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.TPAS.U00 = -1;
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.TPAS.U04 = -1;
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.TPAS.TPASId = materialIndex;
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.TPAS.VertexShaderName = CurrentStringIndex();
            ParentForm.WorkingMaterialData[materialIndex].MaterialEffect.TPAS.PixelShaderName = CurrentStringIndex();
            // Name must match PXSH/VXSH/Related Sections.
            AddString(ParentForm.WorkingMaterialData[materialIndex].MaterialName + "-fx");

            // to follow suit, create the relevant GPR data

            // SHMI is very simple
            ParentForm.WorkingMaterialData[materialIndex].ShaderMetadata = new SHMI();
            ParentForm.WorkingMaterialData[materialIndex].ShaderMetadata.Data.U0C = -1;
            ParentForm.WorkingMaterialData[materialIndex].ShaderMetadata.Data.U10 = -1;
            // name must match that of the EFFE.
            ParentForm.WorkingMaterialData[materialIndex].ShaderMetadata.Name = ParentForm.WorkingMaterialData[materialIndex].MaterialName + "-fx";

            // VXSH is a little more complex. we need to get every input and map it to a uniform entry
            ParentForm.WorkingMaterialData[materialIndex].VertexShader = new VXSH();
            // name must match to EFFE declaration
            ParentForm.WorkingMaterialData[materialIndex].VertexShader.Name = ParentForm.WorkingMaterialData[materialIndex].MaterialName + "-fx";

            List<SceGxmProgramParameter> vertexParameters = ShaderHelper.GetParameters(ShaderPackage.Files[(shaderIndex * 2)].Stream, true);

            foreach (var uniform in vertexParameters.Where(x => x.Category == SceGxmParameterCategory.SCE_GXM_PARAMETER_CATEGORY_UNIFORM)) {
                VertexShaderUniform vxUniform = new VertexShaderUniform();
                vxUniform.Name = uniform.ParameterName;
                vxUniform.ResourceIndex = uniform.ResourceIndex;
                vxUniform.Size = uniform.ArraySize * 4;
                ParentForm.WorkingMaterialData[materialIndex].VertexShader.Data.Uniforms.Add(vxUniform);
            }
            // then set the shader in binary data
            ParentForm.WorkingMaterialData[materialIndex].VertexShader.BufferData = new byte[ShaderPackage.Files[(shaderIndex * 2)].Size];
            ShaderPackage.Files[(shaderIndex * 2)].Stream.Read(ParentForm.WorkingMaterialData[materialIndex].VertexShader.BufferData);

            // vertex shbi. defines constant inputs
            ParentForm.WorkingMaterialData[materialIndex].VertexShaderBinding = new SHBI();
            foreach(var input in vertexParameters.Where(x => x.Category == SceGxmParameterCategory.SCE_GXM_PARAMETER_CATEGORY_UNIFORM && !ShaderHelper.RendererParams.Contains(x.ParameterName)))
            {
                ShaderParameter shbiParam = new ShaderParameter();
                shbiParam.ParameterName = input.ParameterName;
                shbiParam.ParameterResourceIndex = input.ResourceIndex;
                shbiParam.ParameterArraySize = input.ArraySize * 4;

                ParentForm.WorkingMaterialData[materialIndex].VertexShaderBinding.Data.Parameters.Add(shbiParam);
            }

            // this also gives us enough information to create the CSTS/CSTV/SHCO
            ParentForm.WorkingMaterialData[materialIndex].VertexConstants = new CSTS();
            ParentForm.WorkingMaterialData[materialIndex].VertexConstants.ConstantSetID = (materialIndex * 2);  // 2 per mat. For safety. Surely having this with a shader that doesn't utilize it won't cause issues.
            foreach (var input in ParentForm.WorkingMaterialData[materialIndex].VertexShaderBinding.Data.Parameters)
            {
                CSTV constValue = new CSTV();
                constValue.ConstantName = CurrentStringIndex();
                AddString(input.ParameterName);
                constValue.ConstantDataName = CurrentStringIndex();
                AddString(input.ParameterName + "-" + ParentForm.WorkingMaterialData[materialIndex].MaterialName);
                
                SHCO shaderConst = new SHCO();
                shaderConst.Name = input.ParameterName + "-" + ParentForm.WorkingMaterialData[materialIndex].MaterialName;
                shaderConst.Data.Constants.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                
                ParentForm.WorkingMaterialData[materialIndex].VertexConstants.ConstantValues.Add(constValue);
                
                // if it doesn't already exist, add. for the sake of PXSH SHCOs
                if (ParentForm.WorkingMaterialData[materialIndex].ShaderConstants.FindIndex(x => x.Name == shaderConst.Name) == -1)
                    ParentForm.WorkingMaterialData[materialIndex].ShaderConstants.Add(shaderConst);
            }
            ShaderPackage.Files[(shaderIndex * 2)].Close();

            // follow suit for PXSH
            ParentForm.WorkingMaterialData[materialIndex].PixelShader = new PXSH();
            // name must match EFFE declaration
            ParentForm.WorkingMaterialData[materialIndex].PixelShader.Name = ParentForm.WorkingMaterialData[materialIndex].MaterialName + "-fx";
            
            List<SceGxmProgramParameter> fragmentParameters = ShaderHelper.GetParameters(ShaderPackage.Files[(shaderIndex * 2) + 1].Stream, true);

            foreach (var uniform in fragmentParameters.Where(x => x.Category == SceGxmParameterCategory.SCE_GXM_PARAMETER_CATEGORY_UNIFORM))
            {
                PixelShaderUniform pxUniform = new PixelShaderUniform();
                pxUniform.Name = uniform.ParameterName;
                pxUniform.ResourceIndex = uniform.ResourceIndex;
                pxUniform.Size = uniform.ArraySize * 4;
                ParentForm.WorkingMaterialData[materialIndex].PixelShader.Data.Uniforms.Add(pxUniform);
            }
            // then set the shader in binary data
            ParentForm.WorkingMaterialData[materialIndex].PixelShader.BufferData = new byte[ShaderPackage.Files[(shaderIndex * 2) + 1].Size];
            ShaderPackage.Files[(shaderIndex * 2) + 1].Stream.Read(ParentForm.WorkingMaterialData[materialIndex].PixelShader.BufferData);

            // pixel input SHBI

            ParentForm.WorkingMaterialData[materialIndex].PixelShaderConstantBinding = new SHBI();
            foreach (var input in fragmentParameters.Where(x => x.Category == SceGxmParameterCategory.SCE_GXM_PARAMETER_CATEGORY_UNIFORM && !ShaderHelper.RendererParams.Contains(x.ParameterName)))
            {
                ShaderParameter shbiParam = new ShaderParameter();
                shbiParam.ParameterName = input.ParameterName;
                shbiParam.ParameterResourceIndex = input.ResourceIndex;
                shbiParam.ParameterArraySize = input.ArraySize * 4;

                ParentForm.WorkingMaterialData[materialIndex].PixelShaderConstantBinding.Data.Parameters.Add(shbiParam);
            }

            // this also gives us enough information to create the CSTS/CSTV/SHCO
            ParentForm.WorkingMaterialData[materialIndex].FragmentConstants = new CSTS();
            ParentForm.WorkingMaterialData[materialIndex].FragmentConstants.ConstantSetID = (materialIndex * 2);  // 2 per mat. For safety. Surely having this with a shader that doesn't utilize it won't cause issues.
            foreach (var input in ParentForm.WorkingMaterialData[materialIndex].PixelShaderConstantBinding.Data.Parameters)
            {
                CSTV constValue = new CSTV();
                constValue.ConstantName = CurrentStringIndex();
                AddString(input.ParameterName);
                constValue.ConstantDataName = CurrentStringIndex();
                AddString(input.ParameterName + "-" + ParentForm.WorkingMaterialData[materialIndex].MaterialName);

                SHCO shaderConst = new SHCO();
                shaderConst.Name = input.ParameterName + "-" + ParentForm.WorkingMaterialData[materialIndex].MaterialName;
                shaderConst.Data.Constants.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

                ParentForm.WorkingMaterialData[materialIndex].FragmentConstants.ConstantValues.Add(constValue);

                // if it doesn't already exist, add. for the sake of PXSH SHCOs
                if (ParentForm.WorkingMaterialData[materialIndex].ShaderConstants.FindIndex(x => x.Name == shaderConst.Name) == -1)
                    ParentForm.WorkingMaterialData[materialIndex].ShaderConstants.Add(shaderConst);
            }


            ParentForm.WorkingMaterialData[materialIndex].PixelShaderSamplerBinding = new SHBI();
            foreach (var input in fragmentParameters.Where(x => x.Category == SceGxmParameterCategory.SCE_GXM_PARAMETER_CATEGORY_SAMPLER))
            {
                ShaderParameter shbiParam = new ShaderParameter();
                shbiParam.ParameterName = input.ParameterName;
                shbiParam.ParameterResourceIndex = input.ResourceIndex;
                shbiParam.ParameterArraySize = input.ArraySize;

                ParentForm.WorkingMaterialData[materialIndex].PixelShaderSamplerBinding.Data.Parameters.Add(shbiParam);
            }

            // this also also gives us enough information to generate sampler information.
            ParentForm.WorkingMaterialData[materialIndex].MaterialSampler = new SAMP();
            foreach (var input in ParentForm.WorkingMaterialData[materialIndex].PixelShaderSamplerBinding.Data.Parameters)
            {
                SSTV samplerTextureView = new SSTV();
                samplerTextureView.TextureSlot = CurrentStringIndex();
                AddString(input.ParameterName);
                // prompt user for texture
                using (OpenFileDialog ofd = new OpenFileDialog() { Title = $"Please select a file for material \"{ParentForm.WorkingMaterialData[materialIndex].MaterialName}\" texture {input.ParameterName}.", Filter = "DDS Files|*.dds|GXT Files|*.gxt, *.mxt"})
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string textureFileName = Path.GetFileName(ofd.FileName);
                        samplerTextureView.TextureSourcePath = CurrentStringIndex();
                        AddString(textureFileName);
                    }
                }
                samplerTextureView.TextureName = CurrentStringIndex();
                AddString(input.ParameterName + "-" + ParentForm.WorkingMaterialData[materialIndex].MaterialName);

                SMST samplerState = new SMST();
                samplerState.Data.U00 = 1;
                samplerState.Data.U04 = 1;
                samplerState.Data.U08 = 512;
                samplerState.Data.U0C = 0;
                samplerState.Data.U10 = 0;
                samplerState.Data.U14 = 0;
                samplerState.Data.U18 = 31;
                samplerState.Data.U1C = 134217728;
                samplerState.Name = input.ParameterName + "-" + ParentForm.WorkingMaterialData[materialIndex].MaterialName;

                ParentForm.WorkingMaterialData[materialIndex].MaterialSampler.SSTVs.Add(samplerTextureView);
                ParentForm.WorkingMaterialData[materialIndex].SamplerStates.Add(samplerState);
            }
            ShaderPackage.Files[(shaderIndex * 2) + 1].Close();

            // now we create the PXSB and VXSB
            ParentForm.WorkingMaterialData[materialIndex].VXSB = new VXSB();
            ParentForm.WorkingMaterialData[materialIndex].VXSB.Data.VertexShaderData = ParentForm.WorkingMaterialData[materialIndex].VertexShader.Data;
            ParentForm.WorkingMaterialData[materialIndex].VXSB.Data.ShaderBind0 = ParentForm.WorkingMaterialData[materialIndex].VertexShaderBinding.Data;
            ParentForm.WorkingMaterialData[materialIndex].VXSB.Name = ParentForm.WorkingMaterialData[materialIndex].VertexShader.Name;

            ParentForm.WorkingMaterialData[materialIndex].PXSB = new PXSB();
            ParentForm.WorkingMaterialData[materialIndex].PXSB.Data.PixelShaderData = ParentForm.WorkingMaterialData[materialIndex].PixelShader.Data;
            ParentForm.WorkingMaterialData[materialIndex].PXSB.Data.PixelShaderConstBind = ParentForm.WorkingMaterialData[materialIndex].PixelShaderConstantBinding.Data;
            ParentForm.WorkingMaterialData[materialIndex].PXSB.Data.PixelShaderSamplerBind = ParentForm.WorkingMaterialData[materialIndex].PixelShaderSamplerBinding.Data;
            ParentForm.WorkingMaterialData[materialIndex].PXSB.Name = ParentForm.WorkingMaterialData[materialIndex].PixelShader.Name;

            // at this point, the GPR is set for material data.

            ParentForm.WorkingMaterialData[materialIndex].Material = new MATE();
            ParentForm.WorkingMaterialData[materialIndex].Material.MaterialID = materialIndex;
            ParentForm.WorkingMaterialData[materialIndex].Material.Name1 = CurrentStringIndex();
            ParentForm.WorkingMaterialData[materialIndex].Material.Name2 = CurrentStringIndex();
            AddString(ParentForm.WorkingMaterialData[materialIndex].MaterialName);
            ParentForm.WorkingMaterialData[materialIndex].Material.VertexConstantID = (materialIndex * 2);
            ParentForm.WorkingMaterialData[materialIndex].Material.PixelConstantID = (materialIndex * 2) + 1;
            ParentForm.WorkingMaterialData[materialIndex].Material.EffectID = materialIndex;
            ParentForm.WorkingMaterialData[materialIndex].Material.SamplerID = materialIndex;
            ParentForm.WorkingMaterialData[materialIndex].Material.Neg1 = -1;

            // and the MESH is set for material data.

            ParentForm.WorkingMaterialData[materialIndex].Initialized = true;
        }

        private void MIWShaderConfirmation_Click(object sender, EventArgs e)
        {
            MakeMaterial(MIWShaderList.SelectedIndex, Parent.MIWMaterialListbox.SelectedIndex);

            this.Close();
        }
    }
}
