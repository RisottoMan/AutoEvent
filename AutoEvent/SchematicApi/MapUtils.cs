using PluginAPI.Core;

namespace MER.Lite
{
    using System.IO;
    using PluginAPI.Helpers;
    using Serializable;
    using Utf8Json;

    public static class MapUtils
    {
        public static SchematicObjectDataList GetSchematicDataByName(string schematicName)
        {
            string dirPath = Path.Combine(API.SchematicLocation, schematicName);
            
            if(API.Debug)
                Log.Debug($"Path exists: {Directory.Exists(dirPath)}, Directory Path: {dirPath}");
            if (!Directory.Exists(dirPath))
                return null;

            string schematicPath = Path.Combine(dirPath, $"{schematicName}.json");
            if(API.Debug)
                Log.Debug($"File Exists: {File.Exists(schematicPath)}, Schematic Path: {schematicPath}");
            if (!File.Exists(schematicPath))
                return null;

            SchematicObjectDataList data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicPath));
            data.Path = dirPath;

            return data;
        }
    }
}
