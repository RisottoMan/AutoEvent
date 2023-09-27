
namespace AutoEvent.API.Schematic.Objects
{
    using AdminToys;
    using MapGeneration;
    using Mirror;
    using Serializable;
    using UnityEngine;

    public class LightSourceObjects : MapEditorObject
    {
        private Transform _transform;
        private LightSourceToy _lightSourceToy;

        private void Awake()
        {
            _transform = transform;
            _lightSourceToy = GetComponent<LightSourceToy>();
        }

        public LightSourceObjects Init(LightSourcesSerializable lightSourcesSerializable, bool spawn = true)
        {
            Base = lightSourcesSerializable;
            _lightSourceToy.MovementSmoothing = 60;

            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            _lightSourceToy.enabled = false;

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            DebugLogger.LogDebug("Init Light Object");
            base.Init(block);

            Base = new(block);
            _lightSourceToy.MovementSmoothing = 60;

            UpdateObject();

            return this;
        }

        public LightSourcesSerializable Base;

        public override bool IsRotatable => false;

        public override bool IsScalable => false;

        public override void UpdateObject()
        {
            DebugLogger.LogDebug($"Updating Light: Position: {Position} Color: {Base.Color}, Intensity: {Base.Intensity}, Range: {Base.Range}, Shadows: {Base.Shadows}");
            _lightSourceToy.Position = _transform.position;
            _lightSourceToy.LightColor = GetColorFromString(Base.Color);
            _lightSourceToy.LightIntensity = Base.Intensity;
            _lightSourceToy.LightRange = Base.Range;
            _lightSourceToy.LightShadows = Base.Shadows;

            UpdateTransformProperties();
        }

        private void UpdateTransformProperties()
        {
            _lightSourceToy.NetworkPosition = _transform.position;
        }
    }
}
