
namespace MER.Lite.Serializable
{
    using MapGeneration;
    using UnityEngine;
    public abstract class SerializableObject
    {
        public virtual Vector3 Position { get; set; }

        public virtual Vector3 Rotation { get; set; }

        public virtual Vector3 Scale { get; set; } = Vector3.one;

        public virtual RoomName RoomType { get; set; } = RoomName.Unnamed;
    }
}
