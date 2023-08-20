
namespace AutoEvent.API.Schematic.Objects
{
    using AdminToys;
    using MapGeneration;
    using Serializable;
    using UnityEngine;

    public class PrimitiveObject : MapEditorObject
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private PrimitiveObjectToy _primitiveObjectToy;

        private void Awake()
        {
            _transform = transform;
            _primitiveObjectToy = GetComponent<PrimitiveObjectToy>();
        }

        public PrimitiveObject Init(PrimitiveSerializable primitiveSerializable)
        {
            Base = primitiveSerializable;
            //Primitive.MovementSmoothing = 60;
            _prevScale = transform.localScale;

            ForcedRoomType = primitiveSerializable.RoomType == RoomName.Unnamed ? FindRoom().Name : primitiveSerializable.RoomType;

            UpdateObject();
            _primitiveObjectToy.enabled = false;

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            _primitiveObjectToy.MovementSmoothing = 60;

            UpdateObject();

            return this;
        }

        public PrimitiveSerializable Base;

        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody is not null)
                    return _rigidbody;

                if (TryGetComponent(out _rigidbody))
                    return _rigidbody;

                return _rigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            UpdateTransformProperties();
            _primitiveObjectToy.PrimitiveType = Base.PrimitiveType;
            _primitiveObjectToy.MaterialColor = GetColorFromString(Base.Color);

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

        private Vector3 _prevScale;
    }
}
