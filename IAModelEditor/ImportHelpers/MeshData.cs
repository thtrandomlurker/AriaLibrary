using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAModelEditor.ImportHelpers
{
    public class MeshData
    {
        // MESH data
        public PRIM? PrimitiveData;
        // GPR data
        public VXBO? VertexBindingObject;
        public VXAR? VertexAttributes;
        public IXBF? IndexBuffer;
        public VXBF? VertexBuffer;
        public VXST? VertexState;

        public string MeshName = "";
    }
}
