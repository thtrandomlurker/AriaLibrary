using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class MESC : NodeBlock
    {
        public override string Type => "MESC";
        public int MeshClusterId;
        public int MeshClusterName;
        public List<CLUS> Clusters;

        public int GetSize()
        {
            return 0x08 + (0x14 * Clusters.Count());
        }

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            MeshClusterId = reader.ReadInt32();
            MeshClusterName = reader.ReadInt32();
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                // Skip CLUS magic
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                CLUS clus = new CLUS();
                clus.Read(reader);
                Clusters.Add(clus);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'M', 'E', 'S', 'C' });
            writer.Write(0x08 + (0x14 * Clusters.Count()));
            writer.Write(MeshClusterId);
            writer.Write(MeshClusterName);
            foreach (CLUS clus in Clusters)
                clus.Write(writer);
        }

        public MESC()
        {
            Clusters = new List<CLUS>();
        }
    }
}
