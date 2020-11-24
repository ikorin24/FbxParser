using Xunit;
using FbxTools;
using FbxTools.Internal;
using System;
using System.IO;
using System.Text;

namespace UnitTest
{
    public class ParseTest
    {
        [Theory]
        [InlineData("../../../testfile/Dice.fbx")]
        [InlineData("../../../testfile/green_frog.fbx")]
        public void Parse(string fileName)
        {
            UnmanagedMemoryHelper.Initialize();
            using(var stream = File.OpenRead(fileName))
            using(var fbx = FbxParser.Parse(stream)) {
                Assert.True(fbx.Nodes.Length > 0);

                ref readonly var geometryNode = ref fbx.Find("Objects".ToASCII())
                                                       .Find("Geometry".ToASCII());

                var positions = geometryNode.Find("Vertices".ToASCII())
                                            .Properties[0]
                                            .AsDoubleArray();
                var indices = geometryNode.Find("PolygonVertexIndex".ToASCII())
                                          .Properties[0]
                                          .AsInt32Array();
                var normals = geometryNode.Find("LayerElementNormal".ToASCII())
                                          .Find("Normals".ToASCII())
                                          .Properties[0].AsDoubleArray();

                Assert.True(positions.Length > 0);
                Assert.True(indices.Length > 0);
                Assert.True(normals.Length > 0);
            }
            UnmanagedMemoryHelper.AssertResourceReleased();
        }
    }


    internal static class StringHelper
    {
        public static ReadOnlySpan<byte> ToASCII(this string str)
        {
            return Encoding.ASCII.GetBytes(str);    // only for unit test :)
        }
    }
}
