using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class CLUS : NodeBlock
    {
        public override string Type => "CLUS";
        public int ClusterId;
        public int ClusterName;
        public int Neg1;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            ClusterId = reader.ReadInt32();
            ClusterName = reader.ReadInt32();
            Neg1 = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'C', 'L', 'U', 'S' });
            writer.Write(0x0C);
            writer.Write(ClusterId);
            writer.Write(ClusterName);
            writer.Write(Neg1);
        }

        public CLUS()
        {
        }
    }
}
