using AriaLibrary.Archives;
using AriaLibrary.Objects;

namespace MDL2OBJ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                KPack package = new KPack();
                package.Load(args[0]);

                ObjectGroup obj = new ObjectGroup();
                obj.Load(package);
                obj.ExportModelAsModifiedOBJ(args[1]);
            }
            else
            {
                Console.WriteLine("Usage: \"*.mdl\" \"outdir\"");
            }
        }
    }
}