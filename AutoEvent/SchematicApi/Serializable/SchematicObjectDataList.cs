namespace MER.Lite.Serializable
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Enums;

    [Serializable]
    public class SchematicObjectDataList
    {
        public string Path;

        public int RootObjectId { get; set; }

        public List<SchematicBlockData> Blocks { get; set; } = new();
    }

    public class SchematicBlockData
    {
        public virtual string Name { get; set; }

        public virtual int ObjectId { get; set; }

        public virtual int ParentId { get; set; }

        public virtual string AnimatorName { get; set; }

        public virtual Vector3 Position { get; set; }

        public virtual Vector3 Rotation { get; set; }

        public virtual Vector3 Scale { get; set; }

        public virtual BlockType BlockType { get; set; }

        public virtual Dictionary<string, object> Properties { get; set; }
    }
}