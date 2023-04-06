using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class REM : MeshBlock
    {
        public override string Type => "REM";
        public string Comment;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            Comment = new string(reader.ReadChars(dataSize).TakeWhile(x => x != '\0').ToArray());
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'R', 'E', 'M', '\0' });
            int len = PositionHelper.PadValue(Encoding.UTF8.GetByteCount(Comment), 4);
            writer.Write(len);
            writer.Write(Encoding.UTF8.GetBytes(Comment));
            PositionHelper.AlignWriter(writer, 4);
        }

        public REM(string message="")
        {
            Comment = message;
        }
    }
}
