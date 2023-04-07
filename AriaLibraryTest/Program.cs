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
                Console.WriteLine("Attempting save that should break everything and crash harder than a car going 500mph into a wall");
                gpr.Save("test.gpr");
            }
        }
    }
}