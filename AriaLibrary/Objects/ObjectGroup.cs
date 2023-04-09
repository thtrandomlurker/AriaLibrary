using AriaLibrary.Objects.GraphicsProgram;
using AriaLibrary.Objects.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects
{
    // not an actual type, meant to help with grouping data
    public class ObjectGroup
    {
        public GraphicsProgram.GraphicsProgram GPR;
        public MESH MESH;
        public NODT NODT;

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
