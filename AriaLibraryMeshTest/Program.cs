using AriaLibrary.Objects;
using AriaLibrary.Objects.GraphicsProgram;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Mesh;
using System.Linq.Expressions;

namespace AriaLibraryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                Mesh mesh = new Mesh();
                mesh.Load(args[0]);
                mesh.Save("test.MESH");
            }
        }
    }
}