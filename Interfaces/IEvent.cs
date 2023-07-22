namespace AutoEvent.Interfaces
{
    public interface IEvent
    {
        /// <summary>Title</summary>
        string Name { get; }

        /// <summary>Description</summary>
        string Description { get; }

        /// <summary>Color</summary>
        string Color { get; }

        /// <summary>
        /// the name of the event (in ev run [CommandName])
        /// </summary>
        string CommandName { get; }

        /// <summary>On starting</summary>
        void OnStart();

        /// <summary>On stopping</summary>
        void OnStop();
    }
}
