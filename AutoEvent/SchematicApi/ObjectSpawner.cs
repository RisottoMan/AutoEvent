namespace MER.Lite
{
    using Serializable;
    using UnityEngine;
    using System;
    using MER.Lite.Objects;
    using Object = UnityEngine.Object;

    public static class ObjectSpawner
    {
        public static PrimitiveObject SpawnPrimitive(PrimitiveSerializable primitiveObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            GameObject gameObject = Object.Instantiate(ObjectHelper.PrimitiveBaseObject.gameObject, forcedPosition ?? Vector3.zero, forcedRotation ?? Quaternion.identity);
            gameObject.transform.localScale = forcedScale ?? primitiveObject.Scale;

            return gameObject.AddComponent<PrimitiveObject>().Init(primitiveObject);
        }

        [Obsolete]
        public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion? rotation = null, Vector3? scale = null, SchematicObjectDataList data = null)
        {
            return SpawnSchematic(new SchematicSerializable(schematicName), position, rotation, scale, false, data);
        }

        public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion? rotation = null, Vector3? scale = null, bool isStatic = false, SchematicObjectDataList data = null)
        {
            return SpawnSchematic(new SchematicSerializable(schematicName), position, rotation, scale, isStatic, data);
        }

        [Obsolete]
        public static SchematicObject SpawnSchematic(SchematicSerializable schematicObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null, SchematicObjectDataList data = null)
        {
            return SpawnSchematic(schematicObject, forcedPosition, forcedRotation, forcedScale, false, data);
        }

        public static SchematicObject SpawnSchematic(SchematicSerializable schematicObject, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null, bool isStatic = false, SchematicObjectDataList data = null)
        {
            if (data == null)
            {
                data = MapUtils.GetSchematicDataByName(schematicObject.SchematicName);

                if (data == null)
                    return null;
            }

            GameObject gameObject = new($"CustomAutoEventSchematic-{schematicObject.SchematicName}")
            {
                transform =
                {
                    position = forcedPosition ?? Vector3.zero,
                    rotation = forcedRotation ?? Quaternion.identity,
                },
            };

            SchematicObject schematicObjectComponent = gameObject.AddComponent<SchematicObject>().Init(schematicObject, data, isStatic);
            gameObject.transform.localScale = forcedScale ?? schematicObject.Scale;
            if (schematicObjectComponent.IsStatic)
                schematicObjectComponent.UpdateObject();

            return schematicObjectComponent;
        }
    }
}
