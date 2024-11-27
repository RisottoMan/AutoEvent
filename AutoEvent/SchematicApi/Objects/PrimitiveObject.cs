namespace MER.Lite.Objects
{
    using AdminToys;
    using Serializable;
    using UnityEngine;

    /// <summary>
    /// The component added to <see cref="PrimitiveSerializable"/>.
    /// </summary>
    public class PrimitiveObject : MapEditorObject
    {
        private Transform _transform;
        private PrimitiveObjectToy _primitiveObjectToy;

        private void Awake()
        {
            _transform = transform;
            _primitiveObjectToy = GetComponent<PrimitiveObjectToy>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveObject"/> class.
        /// </summary>
        /// <param name="primitiveSerializable">The required <see cref="PrimitiveSerializable"/>.</param>
        /// <returns>The initialized <see cref="PrimitiveObject"/> instance.</returns>
        public PrimitiveObject Init(PrimitiveSerializable primitiveSerializable)
        {
            Base = primitiveSerializable;
            _prevScale = transform.localScale;
            
            UpdateObject();
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
        /// The base <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public PrimitiveSerializable Base { get; private set; }

        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                _primitiveObjectToy.enabled = !value;
                _primitiveObjectToy.NetworkMovementSmoothing = (byte)(value ? 0 : 60);
                _isStatic = value;
            }
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            UpdateTransformProperties();
            _primitiveObjectToy.NetworkPrimitiveType = Base.PrimitiveType;
            _primitiveObjectToy.NetworkMaterialColor = GetColorFromString(Base.Color);
            _primitiveObjectToy.NetworkPrimitiveFlags = Base.PrimitiveFlags;

            if (IsSchematicBlock && _prevScale == transform.localScale)
                return;
            
            _prevScale = transform.localScale;
            base.UpdateObject();
        }
        private void UpdateTransformProperties()
        {
            _primitiveObjectToy.NetworkPosition = _transform.position;
            _primitiveObjectToy.NetworkRotation = new LowPrecisionQuaternion(_transform.rotation);
            _primitiveObjectToy.NetworkScale = _transform.root != _transform ? Vector3.Scale(_transform.localScale, _transform.root.localScale) : _transform.localScale;
        }

        private bool _isStatic;
        private Vector3 _prevScale;
    }
}
