using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Nodes
{
    public interface INode
    {
        string Type { get; }

        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}
