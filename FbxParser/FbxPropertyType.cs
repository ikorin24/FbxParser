#nullable enable

namespace FbxTools
{
    /// <summary>Fbx Property Type enum</summary>
    public enum FbxPropertyType
    {
        /// <summary>property is <see cref="int"/></summary>
        Int32,
        /// <summary>property is <see cref="short"/></summary>
        Int16,
        /// <summary>property is <see cref="long"/></summary>
        Int64,
        /// <summary>property is <see cref="float"/></summary>
        Float,
        /// <summary>property is <see cref="double"/></summary>
        Double,
        /// <summary>property is <see cref="bool"/></summary>
        Bool,
        /// <summary>property is <see cref="string"/> like value</summary>
        String,
        /// <summary>property is array like object of type <see cref="int"/></summary>
        Int32Array,
        /// <summary>property is array like object of type <see cref="long"/></summary>
        Int64Array,
        /// <summary>property is array like object of type <see cref="float"/></summary>
        FloatArray,
        /// <summary>property is array like object of type <see cref="double"/></summary>
        DoubleArray,
        /// <summary>property is array like object of type <see cref="bool"/></summary>
        BoolArray,
        /// <summary>property is array like object of type <see cref="byte"/></summary>
        ByteArray,
    }
}
