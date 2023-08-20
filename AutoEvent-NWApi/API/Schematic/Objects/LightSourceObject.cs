
namespace AutoEvent.API.Schematic.Objects
{
    using AdminToys;
    using MapGeneration;
    using Mirror;
    using Serializable;
    using UnityEngine;

    public class LightSourceObject : MapEditorObject
    {
        private Transform _transform;
        private LightSourceToy _lightSourceToy;

        private void Awake()
        {
            _transform = transform;
            _lightSourceToy = GetComponent<LightSourceToy>();
        }

        public LightSourceObject Init(LightSourceSerializable lightSourceSerializable, bool spawn = true)
        {
            Base = lightSourceSerializable;
            _lightSourceToy.MovementSmoothing = 60;

            ForcedRoomType = lightSourceSerializable.RoomType != RoomName.Unnamed ? lightSourceSerializable.RoomType : FindRoom().Name;
            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            _lightSourceToy.enabled = false;

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            _lightSourceToy.MovementSmoothing = 60;

            UpdateObject();

            return this;
        }

        public LightSourceSerializable Base;

        public override bool IsRotatable => false;

        public override bool IsScalable => false;

        public override void UpdateObject()
        {
            if (!IsSchematicBlock)
            {
                _lightSourceToy.Position = _transform.position;
                _lightSourceToy.LightColor = GetColorFromString(Base.Color);
                _lightSourceToy.LightIntensity = Base.Intensity;
                _lightSourceToy.LightRange = Base.Range;
                _lightSourceToy.LightShadows = Base.Shadows;
            }
            else
            {
                _lightSourceToy.LightColor = GetColorFromString(Base.Color);
                _lightSourceToy.LightIntensity = Base.Intensity;
                _lightSourceToy.LightRange = Base.Range;
                _lightSourceToy.LightShadows = Base.Shadows;
            }

            UpdateTransformProperties();
        }

        private void UpdateTransformProperties()
        {
            _lightSourceToy.NetworkPosition = _transform.position;
        }
    }
}
