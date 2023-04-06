using AriaLibrary.Objects;
using AriaLibrary.Objects.GraphicsProgram;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using System.Linq.Expressions;

namespace AriaLibraryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                GraphicsProgram gpr = new GraphicsProgram();
                gpr.Load(args[0]);
                Console.WriteLine(gpr.Heap.Name);

                foreach (var s in gpr.Heap.Sections)
                {
                    if (s is VXST vertexState)
                    {
                        foreach (var attr in vertexState.Data.VertexArrayReference.VertexAttributes)
                        {
                            Console.WriteLine($"Offset: {attr.Offset}, BufferIndex: {attr.VertexBufferIndex}, Count: {attr.Count}, DataType: {Enum.GetName(attr.DataType)}");
                        }
                    }
                }
            }
        }
    }
}