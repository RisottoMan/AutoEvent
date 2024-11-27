namespace MER.Lite.Objects
{
    using AdminToys;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSourceObject"/> class.
        /// </summary>
        /// <param name="lightSourceSerializable">The required <see cref="LightSourceSerializable"/>.</param>
        /// <param name="spawn">A value indicating whether the component should be spawned.</param>
        /// <returns>The initialized <see cref="LightSourceObject"/> instance.</returns>
        public LightSourceObject Init(LightSourcesSerializable lightSourcesSerializable, bool spawn = true)
        {
            Base = lightSourcesSerializable;

            UpdateObject();

            if (spawn)
                NetworkServer.Spawn(gameObject);

            IsStatic = false;

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);

            UpdateObject();
            IsStatic = true;

            return this;
        }

        /// <summary>
        /// The base <see cref="LightSourceSerializable"/>.
        /// </summary>
        public LightSourcesSerializable Base;

        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                _lightSourceToy.enabled = !value;
                _lightSourceToy.NetworkMovementSmoothing = (byte)(value ? 0 : 60);
                _isStatic = value;
            }
        }

        /// <inheritdoc cref="MapEditorObject.IsRotatable"/>
        public override bool IsRotatable => false;

        /// <inheritdoc cref="MapEditorObject.IsScalable"/>
        public override bool IsScalable => false;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            _lightSourceToy.Position = _transform.position;
            _lightSourceToy.LightColor = GetColorFromString(Base.Color);
            _lightSourceToy.LightIntensity = Base.Intensity;
            _lightSourceToy.LightRange = Base.Range;

            if (Base.Shadows)
            {
                _lightSourceToy.ShadowType = LightShadows.Soft;
            }
            else
            {
                _lightSourceToy.ShadowType = LightShadows.None;
            }

            UpdateTransformProperties();
        }

        private void UpdateTransformProperties()
        {
            _lightSourceToy.NetworkPosition = _transform.position;
        }

        private bool _isStatic;
    }
}
