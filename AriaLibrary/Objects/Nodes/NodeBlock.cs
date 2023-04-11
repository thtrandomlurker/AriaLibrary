using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Nodes
{
    public abstract class NodeBlock : INodeBlock
    {
        public abstract string Type { get; }

        public abstract void Read(BinaryReader reader);
        public abstract void Write(BinaryWriter writer);
    }
}
