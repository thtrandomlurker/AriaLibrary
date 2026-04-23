using AriaLibrary.Helpers;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Nodes;
using Assimp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAModelEditor.ImportHelpers
{
    public enum AttributeDataSource
    {
        Position,
        Normal,
        Tangent,
        Bitangent,
        UV0,
        UV1,
        UV2,
        UV3,
        BlendIndices,
        BlendWeights,
        Color0,
        Color1
    }
    public class MeshData
    {
        public Mesh? SourceMesh { get; set; }
        // MESH data
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PRIM? PrimitiveData { get; set; }
        // GPR data
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VXBO? VertexBindingObject { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VXAR? VertexAttributes { get; set; }
        public List<SceGxmParameterSemantic>? VertexSemantics { get; set; }
        public List<AttributeDataSource>? VertexAttributeDataSources { get; set; }
        public List<int>? VertexSemanticIndices { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IXBF? IndexBuffer { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VXBF? VertexBuffer { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VXST? VertexState { get; set; }

        public int VertexStride { get; set; }

        public string MeshName = "";
        public string BufferName = "";
        public string SetName = "";
        public string SetPolygonName = "";

        public MeshData(Mesh sourceMesh)
        {

            SourceMesh = sourceMesh;
            PrimitiveData = new PRIM();
            VertexBindingObject = new VXBO();
            VertexAttributes = new VXAR();
            IndexBuffer = new IXBF();
            VertexBuffer = new VXBF();
            VertexState = new VXST();
            VertexAttributeDataSources = new List<AttributeDataSource>();
        }
    }
}
