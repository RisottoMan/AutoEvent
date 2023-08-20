namespace AutoEvent.API.Schematic.Serializable
{
    using System;
    using UnityEngine;
    using YamlDotNet.Serialization;

    [Serializable]
    public class LightSourceSerializable : SerializableObject
    {
        public LightSourceSerializable()
        {
        }

        public LightSourceSerializable(string color, float intensity, float range, bool shadows)
        {
            Color = color;
            Intensity = intensity;
            Range = range;
            Shadows = shadows;
        }

        public LightSourceSerializable(SchematicBlockData block)
        {
            Color = block.Properties["Color"].ToString();
            Intensity = float.Parse(block.Properties["Intensity"].ToString());
            Range = float.Parse(block.Properties["Range"].ToString());
            Shadows = bool.Parse(block.Properties["Shadows"].ToString());
        }

        public string Color { get; set; } = "white";

        public float Intensity { get; set; } = 1f;

        public float Range { get; set; } = 1f;

        public bool Shadows { get; set; } = true;

        [YamlIgnore] public override Vector3 Rotation { get; set; }

        [YamlIgnore] public override Vector3 Scale { get; set; }
    }
}
