using AriaLibrary.Archives;
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

namespace IAModelEditor.GUI.Forms.ModelImportWizard
{
    public partial class MIWShaderSelector : Form
    {
        public KPack ShaderPackage;
        new public Form ParentForm;
        new public UserControl Parent;
        public string SourceName;
        public MIWShaderSelector(string shaderPackagePath, Form parentForm, UserControl parent)
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
            ShaderPackage.Files[(shaderIndex * 2)].Open();
            ShaderPackage.Files[(shaderIndex * 2) + 1].Open();
            ((ModelImportWizard)ParentForm).MaterialInfos[materialIndex].VertexProgram = new byte[ShaderPackage.Files[(shaderIndex * 2)].Size];
            ((ModelImportWizard)ParentForm).MaterialInfos[materialIndex].FragmentProgram = new byte[ShaderPackage.Files[(shaderIndex * 2)+1].Size];
        }

        private void MIWShaderConfirmation_Click(object sender, EventArgs e)
        {
            MakeMaterial(MIWShaderList.SelectedIndex, ((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex);

            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Open();
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Open();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexProgram = new byte[ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Size];
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].FragmentProgram = new byte[ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Size];
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Stream.Read(((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexProgram, 0, ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Size);
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Stream.Read(((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].FragmentProgram, 0, ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Size);
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2)].Close();
            ShaderPackage.Files[(MIWShaderList.SelectedIndex * 2) + 1].Close();

            EFFE matEffe;

            if (((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialEffect == null)
            {
                matEffe = new EFFE();
            }
            else
            {
                matEffe = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialEffect;
            }

            matEffe.EffectID = ((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex;
            if (!((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Contains("Shader"))
            {
                ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add("Shader");
            }
            matEffe.EffectType = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == "Shader");
            if (!((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Contains($"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx"))
            {
                ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add($"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx");
            }
            matEffe.EffectName = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx");
            if (!((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Contains($"{SourceName}.cgfx"))
            {
                ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add($"{SourceName}.cgfx");
            }
            matEffe.EffectFileName = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == $"{SourceName}.cgfx");

            matEffe.TPAS.U00 = -1;
            matEffe.TPAS.U04 = -1;
            matEffe.TPAS.TPASId = ((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex;
            matEffe.TPAS.VertexShaderName = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx");
            matEffe.TPAS.PixelShaderName = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx");
            matEffe.TPAS.U14 = -1;

            // We need the shader params. not just names, but the full set of params.

            List<SceGxmProgramParameter> vertexParameters = ShaderHelper.GetParameters(new MemoryStream(((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexProgram));
            List<SceGxmProgramParameter> fragmentParameters = ShaderHelper.GetParameters(new MemoryStream(((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].FragmentProgram));

            // debug

            Console.WriteLine("Vertex Shader: ");

            foreach (var vertParam in vertexParameters)
            {
                Console.WriteLine(vertParam.ParameterName);
                Console.WriteLine(vertParam.DataType);
                Console.WriteLine(vertParam.Semantic);
                Console.WriteLine(vertParam.ComponentCount);
                Console.WriteLine(vertParam.ArraySize);
            }

            Console.WriteLine("Fragment Shader: ");

            foreach (var fragParam in fragmentParameters)
            {
                Console.WriteLine(fragParam.ParameterName);
                Console.WriteLine(fragParam.DataType);
                Console.WriteLine(fragParam.Semantic);
                Console.WriteLine(fragParam.ComponentCount);
                Console.WriteLine(fragParam.ArraySize);
            }

            // by this point, the EFFE is fully handled
            // next would be the GPR

            // the best way to do this would be to make dummy data for now

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShaderBinding = new SHBI();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShaderBinding.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";

            foreach (var uniform in vertexParameters.Where(x => !ShaderHelper.RendererParams.Contains(x.ParameterName) && !x.ParameterName.StartsWith("in_")))
            {
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShaderBinding.Data.Parameters.Add(new ShaderParameter() { ParameterName = uniform.ParameterName, ParameterResourceIndex = uniform.ResourceIndex, ParameterArraySize = uniform.ArraySize * uniform.ComponentCount });
            }

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI = new SHMI();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U00 = 0;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U04 = 0;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U08 = "";
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U0C = -1;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U10 = -1;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U14 = 0;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U18 = 0;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Data.U1C = 0;

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].SHMI.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShader = new VXSH();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShader.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";

            foreach (var uniform in vertexParameters.Where(x => ShaderHelper.RendererParams.Contains(x.ParameterName)))
            {
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShader.Data.Uniforms.Add(new VertexShaderUniform() { Name = uniform.ParameterName, ResourceIndex = uniform.ResourceIndex, Size = uniform.ArraySize * uniform.ComponentCount});
            }
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShader.BufferData = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexProgram;

            // Vertex shader done...? maybe? i have no clue if it'll work or not

            // pxsh just has all?
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShader = new PXSH();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShader.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShader.Buffer = BufferName.Mesh;
            foreach (var uniform in fragmentParameters)
            {
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShader.Data.Uniforms.Add(new PixelShaderUniform() { Name = uniform.ParameterName, ResourceIndex = uniform.ResourceIndex, Size = uniform.ComponentCount * uniform.ArraySize });
            }

            // pixel shbi for consts

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderConstantBinding = new SHBI();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderConstantBinding.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";

            foreach (var uniform in vertexParameters.Where(x => !ShaderHelper.RendererParams.Contains(x.ParameterName) && x.ParameterName.StartsWith('i')))
            {
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShaderBinding.Data.Parameters.Add(new ShaderParameter() { ParameterName = uniform.ParameterName, ParameterResourceIndex = uniform.ResourceIndex, ParameterArraySize = uniform.ArraySize * uniform.ComponentCount });
            }

            // pixel shbi for samplers *if applicable*

            if (fragmentParameters.Count(x => !ShaderHelper.RendererParams.Contains(x.ParameterName) && !x.ParameterName.StartsWith('i')) != 0)
            {

                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderSamplerBinding = new SHBI();
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderSamplerBinding.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialSampler = new SAMP();

                foreach (var uniform in vertexParameters.Where(x => !ShaderHelper.RendererParams.Contains(x.ParameterName) && !x.ParameterName.StartsWith('i')))
                {
                    ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderSamplerBinding.Data.Parameters.Add(new ShaderParameter() { ParameterName = uniform.ParameterName, ParameterResourceIndex = uniform.ResourceIndex, ParameterArraySize = uniform.ArraySize});
                    // add the sampler uniform name if needed
                    if (!((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Contains(uniform.ParameterName))
                    {
                        ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add(uniform.ParameterName);
                    }
                    // and the sstv name
                    if (!((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Contains($"{uniform.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}"))
                    {
                        ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add($"{uniform.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}");
                    }
                    // and of course a dummy entry for the texture name, just to reserve it
                    ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialSampler.SSTVs.Add(new SSTV() { TextureSlot = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == uniform.ParameterName), TextureName = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == $"{uniform.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}"), TextureSourcePath = 0 });

                    // and of course a dummy entry for the texture name, just to reserve it
                    ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add("DummyTex");
                }


            }

            // all that's left is the P/VXSB
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VXSB = new VXSB();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VXSB.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VXSB.Data.VertexShaderData = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShader.Data;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VXSB.Data.ShaderBind0 = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShaderBinding.Data;

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PXSB = new PXSB();
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PXSB.Name = $"{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}-fx";
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PXSB.Data.PixelShaderData = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShader.Data;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PXSB.Data.PixelShaderConstBind = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderConstantBinding.Data;
            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PXSB.Data.PixelShaderSamplerBind = ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].PixelShaderSamplerBinding?.Data;

            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].ShaderConstants = new List<SHCO>();




            foreach (var param in vertexParameters.Where(x => !ShaderHelper.RendererParams.Contains(x.ParameterName) && x.ParameterName.StartsWith('i') && !x.ParameterName.StartsWith("in_")))
            {
                if (!((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Contains($"{param.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}"))
                {
                    ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.Add($"{param.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}");
                }

                int idx = ((ModelImportWizard)ParentForm).WorkingObject.MESH.StringBuffer.StringList.Strings.FindIndex(x => x == $"{param.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}");

                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexConstants.ConstantValues.Add(new CSTV() { ConstantName = idx, ConstantDataName = idx });

                SHCO newSHCO = new SHCO() { Name = $"{param.ParameterName}-{((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialName}" };
                newSHCO.Data.Constants.Add(new Vector4(1));
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].ShaderConstants.Add(newSHCO);

                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].VertexShaderBinding.Data.Parameters.Add(new ShaderParameter() { ParameterName = param.ParameterName, ParameterResourceIndex = param.ResourceIndex, ParameterArraySize = param.ComponentCount});
            }




            if (((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialEffect == null)
            {
                ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].MaterialEffect = matEffe;
            }


            ((ModelImportWizard)ParentForm).MaterialInfos[((MIWMaterialSetupControl)Parent).MIWMaterialListbox.SelectedIndex].Initialized = true;

            this.Close();
        }
    }
}
