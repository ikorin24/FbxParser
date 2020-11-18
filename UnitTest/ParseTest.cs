#nullable enable
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using FbxTools;

namespace UnitTest
{
    public class ParseTest
    {
        private const string TestFbx = "../../../testfile/Dice.fbx";

        [Fact]
        public void Parse()
        {
            Assert.True(true);
            using var stream = File.OpenRead(TestFbx);
            using var fbx = FbxParser.Parse(stream);
            foreach(var node in fbx.Nodes) {
                Dump(node);
            }
            return;

            static void Dump(in FbxNode node)
            {
                foreach(var prop in node.Properties) {
                    if(prop.Type == FbxPropertyType.Int32) {
                        Debug.WriteLine(prop.AsInt32());
                    }
                }

                foreach(var n in node.Children) {
                    Dump(n);
                }
            }
        }
    }
}
