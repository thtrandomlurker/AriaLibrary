using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Mesh
{
    public interface IMeshBlock
    {
        string Type { get; }

        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}
