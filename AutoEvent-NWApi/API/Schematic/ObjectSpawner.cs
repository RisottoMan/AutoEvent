namespace AutoEvent.API.Schematic
{
    using MapGeneration;
    using Serializable;
    using UnityEngine;
    using System.Linq;
    using global::AutoEvent.API.Schematic.Objects;
    using Random = UnityEngine.Random;

    public static class ObjectSpawner
    {
        public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion? rotation = null, Vector3? scale = null, SchematicObjectDataList data = null)
        {
            return SpawnSchematic(new SchematicSerializable(schematicName), position, rotation, scale, data);
        }

        public static SchematicObject SpawnSchematic(SchematicSerializable schematicObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null, SchematicObjectDataList data = null)
        {
            if (data == null)
            {
                data = MapUtils.GetSchematicDataByName(schematicObject.SchematicName);

                if (data == null)
                    return null;
            }

            RoomIdentifier room = null;

            if (schematicObject.RoomType != RoomName.Unnamed)
            {
                room = RoomIdentifier.AllRoomIdentifiers.First(r => r.Name == RoomName.Outside);
            }

            GameObject gameObject = new($"CustomSchematic-{schematicObject.SchematicName}")
            {
                transform =
                {
                    position = forcedPosition ?? GetRelativePosition(schematicObject.Position, room),
                    rotation = forcedRotation ?? GetRelativeRotation(schematicObject.Rotation, room),
                },
            };

            SchematicObject schematicObjectComponent = gameObject.AddComponent<SchematicObject>().Init(schematicObject, data);
            gameObject.transform.localScale = forcedScale ?? schematicObject.Scale;
            return schematicObjectComponent;
        }

        public static Vector3 GetRelativePosition(Vector3 position, RoomIdentifier room) => room.Name == RoomName.Outside ? position : room.transform.TransformPoint(position);
        public static Quaternion GetRelativeRotation(Vector3 rotation, RoomIdentifier room)
        {
            if (rotation.x == -1f)
                rotation.x = Random.Range(0f, 360f);

            if (rotation.y == -1f)
                rotation.y = Random.Range(0f, 360f);

            if (rotation.z == -1f)
                rotation.z = Random.Range(0f, 360f);

            if (room == null)
                return Quaternion.Euler(rotation);

            return room.Name == RoomName.Outside ? Quaternion.Euler(rotation) : room.transform.rotation * Quaternion.Euler(rotation);
        }
    }
}
