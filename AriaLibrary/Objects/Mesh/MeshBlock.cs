using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Mesh
{
    public abstract class MeshBlock : IMeshBlock
    {
        public abstract string Type { get; }

        public abstract void Read(BinaryReader reader);
        public abstract void Write(BinaryWriter writer);
    }
}
