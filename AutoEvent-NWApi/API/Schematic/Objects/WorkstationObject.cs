namespace AutoEvent.API.Schematic.Objects
{
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration.Distributors;
    using Serializable;
    using UnityEngine;

    public class WorkstationObject : MapEditorObject
    {
        private void Awake()
        {
            Workstation = GetComponent<WorkstationController>();
            StructurePositionSync = GetComponent<StructurePositionSync>();
        }

        public WorkstationObject Init(WorkstationSerializable workStationSerializable)
        {
            Base = workStationSerializable;
            UpdateObject();

            return this;
        }

        public MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            UpdateObject();

            return this;
        }

        public WorkstationSerializable Base;

        public WorkstationController Workstation { get; private set; }

        public StructurePositionSync StructurePositionSync { get; private set; }

        public override void UpdateObject()
        {
            StructurePositionSync.Network_position = transform.position;
            StructurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(transform.rotation.eulerAngles.y / 5.625f);
            Workstation.NetworkStatus = (byte)(Base.IsInteractable ? 0 : 4);

            if (!IsSchematicBlock)
                base.UpdateObject();
        }
    }
}