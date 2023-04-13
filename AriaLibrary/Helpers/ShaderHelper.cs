using AriaLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;

using AriaLibrary.Objects.GraphicsProgram;

namespace AriaLibrary.Helpers
{
    public static class ShaderHelper
    {
        public static List<string> GetInputNames(Stream shaderStream)
        {
            List<string> inputs = new List<string>();
            using (BinaryReader reader = new BinaryReader(shaderStream))
            {
                Console.WriteLine(new string(reader.ReadChars(3)));
                reader.BaseStream.Seek(0x24, SeekOrigin.Begin);
                int inputCount = reader.ReadInt32();
                int inputsOffset = reader.ReadInt32();
                reader.BaseStream.Seek(inputsOffset - 4, SeekOrigin.Current);
                for (int i = 0; i < inputCount; i++)
                {
                    int stringOffset = reader.ReadInt32();
                    inputs.Add(StringReader.ReadNullTerminatedStringAtOffset(reader, stringOffset - 4, SeekOrigin.Current));
                    reader.BaseStream.Seek(0x0C, SeekOrigin.Current);
                }
            }
            return inputs;
        }
    }
}
