namespace MER.Lite
{
    public static class API
    {
        /// <summary>
        /// Initializes MER Lite
        /// </summary>
        /// <param name="schematicLocation">The location for the schematic folder.</param>
        /// <param name="debug">Whether debug logs should be enabled.</param>
        public static void Initialize(string schematicLocation, bool debug = false)
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;

            SchematicLocation = schematicLocation;
            Debug = debug;
            CosturaUtility.Initialize();
        }
        
        private static bool _initialized = false;

        /// <summary>
        /// Controls Debug Logs. If true, the Api will output debug logs.
        /// </summary>
        public static bool Debug { get; set; }
        
        /// <summary>
        /// The location of the schematics folder.
        /// </summary>
        public static string SchematicLocation { get; set; }
    }
}