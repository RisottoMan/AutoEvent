namespace MER.Lite.Serializable
{
    using System;
    using UnityEngine;
    using AdminToys;

    /// <summary>
    /// A tool used to easily handle primitives.
    /// </summary>
    [Serializable]
    public class PrimitiveSerializable : SerializableObject
    {
        public PrimitiveSerializable()
        {
        }

        public PrimitiveSerializable(PrimitiveType primitiveType, string color, PrimitiveFlags primitiveFlags)
        {
            PrimitiveType = primitiveType;
            Color = color;
            PrimitiveFlags = primitiveFlags;
        }

        public PrimitiveSerializable(SchematicBlockData block)
        {
            PrimitiveType = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString());
            Color = block.Properties["Color"].ToString();
            
            if (block.Properties.TryGetValue("PrimitiveFlags", out object flags))
            {
                PrimitiveFlags = (PrimitiveFlags)Enum.Parse(typeof(PrimitiveFlags), flags.ToString());
            }
            else
            {
                // Backward compatibility
                PrimitiveFlags primitiveFlags = PrimitiveFlags.Visible;
                if (block.Scale.x >= 0f)
                    primitiveFlags |= PrimitiveFlags.Collidable;

                PrimitiveFlags = primitiveFlags;
            }
        }

        public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

        public string Color { get; set; } = "red";
        /// <summary>
        /// Gets or sets the <see cref="PrimitiveSerializable"/>'s flags.
        /// </summary>
        public PrimitiveFlags PrimitiveFlags { get; set; } = (PrimitiveFlags)3;
    }
}
