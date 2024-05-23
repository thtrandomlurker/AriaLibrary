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

namespace IAModelEditor.GUI.Forms
{
    public partial class MIWShaderInfo : Form
    {
        public KPack ShaderPackage;
        public string SourceName;
        public MIWShaderInfo(string shaderPackagePath)
        {
            InitializeComponent();
            KPack shaderPackage = new KPack();
            shaderPackage.Load(shaderPackagePath);
            ShaderPackage = shaderPackage;
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

            List<SceGxmProgramParameter> vertexParameters = ShaderHelper.GetParameters(ShaderPackage.Files[0].Stream, true);
            List<SceGxmProgramParameter> fragmentParameters = ShaderHelper.GetParameters(ShaderPackage.Files[1].Stream, true);

            foreach (var param in vertexParameters)
            {
                Console.WriteLine(param.ParameterName);
                Console.WriteLine(param.SemanticName);
                Console.WriteLine(param.SemanticIndex);
                Console.WriteLine(param.Category);
                Console.WriteLine(param.ResourceIndex);
                Console.WriteLine(param.ArraySize);
            }
            foreach (var param in fragmentParameters)
            {
                Console.WriteLine(param.ParameterName);
                Console.WriteLine(param.SemanticName);
                Console.WriteLine(param.SemanticIndex);
                Console.WriteLine(param.Category);
                Console.WriteLine(param.ResourceIndex);
                Console.WriteLine(param.ArraySize);
            }


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

        private void MIWShaderConfirmation_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MIWShaderVertexAttributeList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
