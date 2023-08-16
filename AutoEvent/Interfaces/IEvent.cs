using MapEditorReborn.API.Features.Objects;

namespace AutoEvent.Interfaces
{
    public interface IEvent
    {
        /// <summary>Title</summary>
        string Name { get; }

        /// <summary>Description</summary>
        string Description { get; }

        /// <summary>Author of the mini-game</summary>
        string Author { get; }

        /// <summary>Name of schematic</summary>
        string MapName { get; }

        /// <summary>Command to run game (in ev run [CommandName])</summary>
        string CommandName { get; }

        /// <summary>On starting</summary>
        void OnStart();

        /// <summary>On stopping</summary>
        void OnStop();
    }
}
