using Powerups;

namespace Powerups
{
    public static class API
    {
        /// <summary>
        /// Initializes necessary dependencies.
        /// </summary>
        /// <param name="debug">Should debug logging be enabled.</param>
        public static void Initialize(bool debug = false)
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            CosturaUtility.Initialize();
            var powerupManager = new PowerupManager();
            Debug = debug;
        }

        private static bool _initialized = false;
        
        /// <summary>
        /// Controls Debug Logs. If true, the Api will output debug logs.
        /// </summary>
        public static bool Debug { get; set; } = false;
        
        /// <summary>
        /// The main instance of the powerup manager.
        /// </summary>
        public static PowerupManager PowerupManager => PowerupManager.Singleton;
    }
}