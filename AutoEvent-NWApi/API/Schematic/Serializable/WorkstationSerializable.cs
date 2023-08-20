using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.API.Schematic.Serializable
{
    using System;

    [Serializable]
    public class WorkstationSerializable : SerializableObject
    {
        public WorkstationSerializable()
        {
        }

        public WorkstationSerializable(bool isInteractable)
        {
            IsInteractable = isInteractable;
        }

        public WorkstationSerializable(SchematicBlockData block)
        {
            IsInteractable = block.Properties.ContainsKey("IsInteractable");
        }

        public bool IsInteractable { get; set; } = true;
    }
}
