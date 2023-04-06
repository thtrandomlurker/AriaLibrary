﻿using AriaLibrary.Objects;
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
                /*foreach (var block in mesh.MeshBlocks)
                {
                    if (args.Any(x => x == block.Type))
                    {
                        if (block is EFFE effe)
                        {
                            Console.WriteLine(mesh.StringBlock.StringList.Strings[effe.EffectName]);
                            Console.WriteLine(mesh.StringBlock.StringList.Strings[effe.EffectFileName]);
                            Console.WriteLine(mesh.StringBlock.StringList.Strings[effe.EffectType]);
                        }
                        if (block is EFFE effe)
                        {
                            Console.WriteLine(mesh.StringBlock.StringList.Strings[effe.EffectName]);
                            Console.WriteLine(mesh.StringBlock.StringList.Strings[effe.EffectFileName]);
                            Console.WriteLine(mesh.StringBlock.StringList.Strings[effe.EffectType]);
                        }
                    }
                }*/

            }
        }
    }
}