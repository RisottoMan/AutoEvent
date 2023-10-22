
namespace MER.Lite.Serializable
{
    using System;
    using Enums;

    [Serializable]
    public class SchematicSerializable : SerializableObject
    {
        public SchematicSerializable()
        {
        }

        public SchematicSerializable(string schematicName) => SchematicName = schematicName;

        public string SchematicName { get; set; } = "None";

        public CullingType CullingType { get; set; } = CullingType.None;

        public bool IsPickable { get; set; }
    }
}
