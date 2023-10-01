namespace AutoEvent.API.Schematic
{
    using System.IO;
    using PluginAPI.Helpers;
    using Serializable;
    using Utf8Json;

    public static class MapUtils
    {
        public static SchematicObjectDataList GetSchematicDataByName(string schematicName)
        {
            string dirPath = Path.Combine(AutoEvent.Singleton.Config.SchematicsDirectoryPath, schematicName);
            
            DebugLogger.LogDebug($"Path exists: {Directory.Exists(dirPath)}, Directory Path: {dirPath}");
            if (!Directory.Exists(dirPath))
                return null;

            string schematicPath = Path.Combine(dirPath, $"{schematicName}.json");
            DebugLogger.LogDebug($"File Exists: {File.Exists(schematicPath)}, Schematic Path: {schematicPath}");
            if (!File.Exists(schematicPath))
                return null;

            SchematicObjectDataList data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicPath));
            data.Path = dirPath;

            return data;
        }
    }
}
