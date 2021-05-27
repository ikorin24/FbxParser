using System;
using System.IO;
using System.Text;
using FbxTools;

namespace RuntimeDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var stream = File.OpenRead("julia.fbx"))
            using (var fbx = FbxParser.Parse(stream))
            {
                Console.WriteLine(fbx.ToString());
                foreach (var item in fbx.Nodes)
                {
                    var name = Encoding.ASCII.GetString(item.Name);
                    Console.WriteLine(name);
                }
            }
        }
    }
}
