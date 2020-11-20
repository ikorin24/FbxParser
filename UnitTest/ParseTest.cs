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
        private const string TestFbx = "../../../testfile/Dice.fbx";

        [Fact]
        public void Parse()
        {
            using(var stream = File.OpenRead(TestFbx))
            using(var fbx = FbxParser.Parse(stream)) {
                Assert.True(fbx.NodesCount > 0);
            }
            UnmanagedMemoryHelper.AssertResourceReleased();
        }
    }
}
