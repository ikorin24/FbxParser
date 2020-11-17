#nullable enable
using System;
using System.IO;
using Xunit;
using FbxTools;

namespace UnitTest
{
    public class ParseTest
    {
        private const string TestFbx = "test.fbx";

        [Fact]
        public void Parse()
        {
            using var stream = File.OpenRead(TestFbx);
            using var fbx = FbxParser.Parse(stream);
            foreach(var node in fbx.Nodes) {
                foreach(var prop in node.Properties) {
                    switch(prop.Type) {
                        case FbxPropertyType.Int32:
                            prop.AsInt32();
                            break;
                        case FbxPropertyType.Int16:
                            prop.AsInt16();
                            break;
                        case FbxPropertyType.Int64:
                            prop.AsInt64();
                            break;
                        case FbxPropertyType.Float:
                            prop.AsFloat();
                            break;
                        case FbxPropertyType.Double:
                            prop.AsDouble();
                            break;
                        case FbxPropertyType.Bool:
                            prop.AsBool();
                            break;
                        case FbxPropertyType.String:
                            prop.AsString();
                            break;
                        case FbxPropertyType.Int32Array:
                            prop.AsInt32Array();
                            break;
                        case FbxPropertyType.Int64Array:
                            prop.AsInt64Array();
                            break;
                        case FbxPropertyType.FloatArray:
                            prop.AsFloatArray();
                            break;
                        case FbxPropertyType.DoubleArray:
                            prop.AsDoubleArray();
                            break;
                        case FbxPropertyType.BoolArray:
                            prop.AsBoolArray();
                            break;
                        case FbxPropertyType.ByteArray:
                            prop.AsByteArray();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
