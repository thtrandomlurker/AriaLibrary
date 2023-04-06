using AriaLibrary.Objects;
using AriaLibrary.Objects.GraphicsProgram;
using AriaLibrary.Objects.GraphicsProgram.Nodes;
using AriaLibrary.Objects.Nodes;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace AriaLibraryTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                NODT nodeTree = new NODT();
                nodeTree.Load(args[0]);
                foreach (var nodeBlock in nodeTree.ChildNodes)
                {
                    if (nodeBlock is NODE node)
                    {
                        Console.WriteLine(nodeTree.StringBuffer.StringList.Strings[node.NodeName]);
                        Console.WriteLine(node.U04);
                        Console.WriteLine(node.U08);
                        if (node.InstanceData != null)
                        {
                            Console.WriteLine("THIS IS INSTANCE DATA");
                            Console.WriteLine(nodeTree.StringBuffer.StringList.Strings[node.InstanceData.MeshCluster.MeshClusterName]);
                            foreach (CLUS cluster in node.InstanceData.MeshCluster.Clusters)
                            {
                                Console.WriteLine(nodeTree.StringBuffer.StringList.Strings[cluster.ClusterName]);
                            }
                            Console.WriteLine("END OF INSTANCE DATA");
                        }
                    }
                }
                //nodeTree.Save("test.NODT");
            }
        }
    }
}