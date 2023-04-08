using AriaLibrary.Archives;
using AriaLibrary.Helpers;
using System.Text;

namespace KPacker
{
    static class Program
    {
        static string GetExtFromMagic(Stream stream)
        {
            char[] magic;
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                magic = reader.ReadChars(4);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
            }
            string ext = new string(magic).Split('\0')[0];
            return ext;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No Input Specified.");
                return;
            }

            if (File.Exists(args[0]))
            {
                Console.WriteLine($"Unpacking file {args[0]}");
                // create the output dir
                string odir = Path.GetFileNameWithoutExtension(args[0]);
                Directory.CreateDirectory(odir);
                string s = Path.DirectorySeparatorChar.ToString();

                // package info
                KPack package = new KPack();
                package.Load(args[0]);
                int cfile = 0;
                foreach(var file in package.Files)
                {
                    file.Open();
                    if (file.Stream != null)
                    {
                        Stream outfile = File.OpenWrite($"{odir}{s}{cfile}.{GetExtFromMagic(file.Stream)}");
                        using (BinaryReader reader = new BinaryReader(file.Stream, Encoding.UTF8, true))
                        {
                            using (BinaryWriter writer = new BinaryWriter(outfile))
                            {
                                writer.Write(reader.ReadBytes(file.Size));
                            }
                        }
                        file.Close();
                    }
                    cfile++;
                }
                // eofunc
                return;
            }

            if (Directory.Exists(args[0]))
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: \"folder_to_pack\" \"output_archive.ext\"");
                }
                Console.WriteLine("Input is Directory: %s", args[0]);
                string[] fList = Directory.GetFiles(args[0]);
                int archiveFileBase = PositionHelper.PadValue(12 + (8 * fList.Length), 0x40);
                KPack package = new KPack();
                // for each file, add it
                foreach (var file in fList)
                {
                    Stream fileStream = File.OpenRead(file);
                    KPackFile kFile = new KPackFile(archiveFileBase, (int)fileStream.Length, fileStream, fileStream);

                    package.Files.Add(kFile);

                    archiveFileBase = PositionHelper.PadValue(archiveFileBase + (int)fileStream.Length, 0x40);
                }
                package.Save(args[1]);
                return;
            }
        }
    }
}