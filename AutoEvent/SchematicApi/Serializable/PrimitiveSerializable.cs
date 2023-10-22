namespace MER.Lite.Serializable
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PrimitiveSerializable : SerializableObject
    {
        public PrimitiveSerializable()
        {
        }

        public PrimitiveSerializable(PrimitiveType primitiveType, string color)
        {
            PrimitiveType = primitiveType;
            Color = color;
        }

        public PrimitiveSerializable(SchematicBlockData block)
        {
            PrimitiveType = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString());
            Color = block.Properties["Color"].ToString();
        }

        public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

        public string Color { get; set; } = "red";
    }
}
