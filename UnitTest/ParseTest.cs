﻿using Xunit;
using FbxTools;
using FbxTools.Internal;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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
                Assert.True(fbx.Nodes.Count > 0);

                var positions = fbx.FindChild(Objects)
                                   .FindChild(Geometry)
                                   .FindChild(Vertices)
                                   .Properties[0].AsDoubleArray();

                var indices = fbx.FindChild(Objects)
                                 .FindChild(Geometry)
                                 .FindChild(PolygonVertexIndex)
                                 .Properties[0].AsInt32Array();

                var normals = fbx.FindChild(Objects)
                                 .FindChild(Geometry)
                                 .FindChild(LayerElementNormal)
                                 .FindChild(Normals)
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
            using(var stream = File.OpenRead(fileName))
            using(var fbx = FbxParser.Parse(stream)) {

                FbxNode objects;
                {
                    const string Objects = "Objects";
                    var ObjectsASCII = Objects.ToASCII();

                    objects = fbx.FindChild(ObjectsASCII);
                    Assert.True(objects == fbx.FindChild(Objects));
                    {
                        Assert.True(fbx.TryFindChild(ObjectsASCII, out var node) && objects == node);
                    }
                    {
                        Assert.True(fbx.TryFindChild(Objects, out var node) && objects == node);
                    }
                    Assert.True(objects == fbx.Nodes[fbx.FindChildIndex(ObjectsASCII)]);
                    Assert.True(objects == fbx.Nodes[fbx.FindChildIndex(Objects)]);
                    Span<int> buf = new int[fbx.Nodes.Count];
                    var indexList = buf.Slice(0, fbx.FindChildIndexAll(ObjectsASCII, buf)).ToArray();
                    Assert.True(buf.Slice(0, fbx.FindChildIndexAll(Objects, buf)).SequenceEqual(indexList));
                    Assert.True(fbx.FindChildIndexAll(Objects).SequenceEqual(indexList));
                    Assert.True(fbx.FindChildIndexAll(ObjectsASCII).SequenceEqual(indexList));
                    var contains = indexList.Contains(fbx.FindChildIndex(Objects));
                    Assert.True(contains);
                }

                {
                    const string Geometry = "Geometry";
                    var GeometryASCII = Geometry.ToASCII();

                    var geometryNode = objects.FindChild(GeometryASCII);
                    Assert.True(geometryNode == objects.FindChild(Geometry));
                    {
                        Assert.True(objects.TryFindChild(GeometryASCII, out var node) && geometryNode == node);
                    }
                    {
                        Assert.True(objects.TryFindChild(Geometry, out var node) && geometryNode == node);
                    }
                    Assert.True(geometryNode == objects.Children[objects.FindChildIndex(GeometryASCII)]);
                    Assert.True(geometryNode == objects.Children[objects.FindChildIndex(Geometry)]);
                    Span<int> buf = new int[objects.Children.Count];

                    var indexList = buf.Slice(0, objects.FindChildIndexAll(GeometryASCII, buf)).ToArray();
                    Assert.True(buf.Slice(0, objects.FindChildIndexAll(Geometry, buf)).SequenceEqual(indexList));
                    Assert.True(objects.FindChildIndexAll(Geometry).SequenceEqual(indexList));
                    Assert.True(objects.FindChildIndexAll(GeometryASCII).SequenceEqual(indexList));
                    var contains = indexList.Contains(objects.FindChildIndex(Geometry));
                    Assert.True(contains);
                }

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
