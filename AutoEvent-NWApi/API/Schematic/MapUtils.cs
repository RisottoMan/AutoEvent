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
            string dirPath = Path.Combine(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics"), schematicName);
            if (!Directory.Exists(dirPath))
                return null;

            string schematicPath = Path.Combine(dirPath, $"{schematicName}.json");
            if (!File.Exists(schematicPath))
                return null;

            SchematicObjectDataList data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicPath));
            data.Path = dirPath;

            return data;
        }
    }
}
