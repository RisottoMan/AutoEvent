namespace AutoEvent.Interfaces
{
    public interface IEvent
    {
        /// <summary>
        /// the name of the event (in ev run [CommandName])
        /// </summary>
        string CommandName { get; }

        /// <summary>Title</summary>
        string Name { get; }

        /// <summary>Color</summary>
        string Color { get; }

        /// <summary>Description</summary>
        string Description { get; }

        /// <summary>On starting</summary>
        void OnStart();

        /// <summary>On stopping</summary>
        void OnStop();
    }
}
