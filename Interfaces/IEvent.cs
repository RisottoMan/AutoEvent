using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Interfaces
{
    public interface IEvent
    {
        /// <summary>
        /// Название ивента в команде, при котором он вызывается. Например: ev_run [CommandName].
        /// </summary>
        string CommandName { get; }

        /// <summary>Название</summary>
        string Name { get; }

        /// <summary>Цвет</summary>
        string Color { get; }

        /// <summary>Описание</summary>
        string Description { get; }

        /// <summary>При запуске</summary>
        void OnStart();

        /// <summary>При окончании</summary>
        void OnStop();
    }
}
