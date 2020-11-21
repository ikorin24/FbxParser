#nullable enable
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using FbxTools;
using FbxTools.Internal;

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
            }
            UnmanagedMemoryHelper.AssertResourceReleased();
        }
    }
}
