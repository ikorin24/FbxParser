using Xunit;
using FbxTools;
using FbxTools.Internal;
using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace UnitTest
{
    public class ParseTest
    {
        [Theory]
        [InlineData("../../../testfile/Dice.fbx")]
        [InlineData("../../../testfile/green_frog.fbx")]
        public void Parse(string fileName)
        {
            // FbxObject (root)
            //               |`-- "Objects"
            //               |`-- ...    |`-- "Geometry"
            //               |`-- ...    |`-- ...     |`-- "Vertices" { [0]: double[], ... }
            //                `-- ...    |`-- ...     |`-- "PolygonVertexIndex" { [0]: int[], ... }
            //                            `-- ...     |`-- "LayerElementNormal"
            //                                        |`-- ...               |`-- "Normals" { [0]: double[], ... }
            //                                         `-- ...               |`-- ...
            //                                                                `-- ...

            var Objects = "Objects".ToASCII();
            var Geometry = "Geometry".ToASCII();
            var Vertices = "Vertices".ToASCII();
            var PolygonVertexIndex = "PolygonVertexIndex".ToASCII();
            var LayerElementNormal = "LayerElementNormal".ToASCII();
            var Normals = "Normals".ToASCII();

            UnmanagedMemoryHelper.Initialize();
            using(var stream = File.OpenRead(fileName))
            using(var fbx = FbxParser.Parse(stream)) {
                Assert.True(fbx.Nodes.Length > 0);

                var positions = fbx.Find(Objects)
                                   .Find(Geometry)
                                   .Find(Vertices)
                                   .Properties[0].AsDoubleArray();

                var indices = fbx.Find(Objects)
                                 .Find(Geometry)
                                 .Find(PolygonVertexIndex)
                                 .Properties[0].AsInt32Array();

                var normals = fbx.Find(Objects)
                                 .Find(Geometry)
                                 .Find(LayerElementNormal)
                                 .Find(Normals)
                                 .Properties[0].AsDoubleArray();

                Assert.True(positions.Length > 0);
                Assert.True(indices.Length > 0);
                Assert.True(normals.Length > 0);
            }
            UnmanagedMemoryHelper.AssertResourceReleased();
        }

        [Theory]
        [InlineData("../../../testfile/Dice.fbx")]
        public void Find(string fileName)
        {
            var Objects = "Objects".ToASCII();
            var Geometry = "Geometry".ToASCII();
            var Vertices = "Vertices".ToASCII();

            using(var stream = File.OpenRead(fileName))
            using(var fbx = FbxParser.Parse(stream)) {

                Span<int> buf = stackalloc int[fbx.Nodes.Length];
                ref readonly var objectsNode = ref fbx.Find(Objects);
                ref readonly var objectsNode2 = ref fbx.Nodes[fbx.FindIndex(Objects)];
                Assert.True(Unsafe.AreSame(ref Unsafe.AsRef(objectsNode),
                                           ref Unsafe.AsRef(objectsNode2)));
                Assert.True(buf.Slice(0, fbx.FindIndexAll(Objects, buf))
                                            .Contains(fbx.FindIndex(Objects)));


                Span<int> buf2 = stackalloc int[objectsNode.Children.Length];
                ref readonly var geometryNode = ref objectsNode.Find(Geometry);
                ref readonly var geometryNode2 = ref objectsNode.Children[objectsNode.FindIndex(Geometry)];
                Assert.True(Unsafe.AreSame(ref Unsafe.AsRef(geometryNode),
                                           ref Unsafe.AsRef(geometryNode2)));

                Assert.True(buf2.Slice(0, objectsNode.FindIndexAll(Geometry, buf2))
                                                     .Contains(objectsNode.FindIndex(Geometry)));
            }
        }
    }


    internal static class HelperExtension
    {
        public static ReadOnlySpan<byte> ToASCII(this string str)
        {
            return Encoding.ASCII.GetBytes(str);    // only for unit test :)
        }

        public static bool Contains<T>(this Span<T> source, T value)
        {
            foreach(var item in source) {
                if(EqualityComparer<T>.Default.Equals(item, value)) {
                    return true;
                }
            }
            return false;
        }
    }
}
