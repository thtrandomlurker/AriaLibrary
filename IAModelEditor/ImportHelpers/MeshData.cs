using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Nodes;
using Assimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAModelEditor.ImportHelpers
{
    public class MeshData
    {
        public Mesh? SourceMesh;
        // MESH data
        public PRIM? PrimitiveData;
        // GPR data
        public VXBO? VertexBindingObject;
        public VXAR? VertexAttributes;
        public IXBF? IndexBuffer;
        public VXBF? VertexBuffer;
        public VXST? VertexState;

        public string MeshName = "";
        public string SetName = "";
    }
}
