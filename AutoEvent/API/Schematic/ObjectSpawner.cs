namespace AutoEvent.API.Schematic
{
    using MapGeneration;
    using Serializable;
    using UnityEngine;
    using System.Linq;
    using global::AutoEvent.API.Schematic.Objects;

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

            GameObject gameObject = new($"CustomAutoEventSchematic-{schematicObject.SchematicName}")
            {
                transform =
                {
                    position = forcedPosition ?? Vector3.zero,
                    rotation = forcedRotation ?? Quaternion.identity,
                },
            };

            SchematicObject schematicObjectComponent = gameObject.AddComponent<SchematicObject>().Init(schematicObject, data);
            gameObject.transform.localScale = forcedScale ?? schematicObject.Scale;
            return schematicObjectComponent;
        }
    }
}
