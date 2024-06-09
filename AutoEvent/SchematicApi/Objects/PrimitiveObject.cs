using System.Collections.Generic;
using MEC;

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
        private Rigidbody? _rigidbody;
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
            _primitiveObjectToy.MovementSmoothing = 60;

            _primitiveObjectToy.NetworkIsStatic = true;
            base.UpdateObject();
            UpdateObject();

            return this;
        }

        public override MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            _primitiveObjectToy.MovementSmoothing = 60;

            UpdateObject();
            IsStatic = true;

            return this;
        }

        /// <summary>
        /// The base <see cref="PrimitiveSerializable"/>.
        /// </summary>
        public PrimitiveSerializable Base { get; private set; }

        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody is not null)
                    return _rigidbody;

                if (TryGetComponent(out _rigidbody))
                    return _rigidbody!;

                return _rigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }
        
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

            if (IsSchematicBlock)
                return;

            if (_primitiveObjectToy.NetworkIsStatic)
                Timing.RunCoroutine(RefreshStatic());
        }

        private IEnumerator<float> RefreshStatic()
        {
            _primitiveObjectToy.NetworkIsStatic = false;
            yield return Timing.WaitForOneFrame;
            _primitiveObjectToy.NetworkIsStatic = true;
        }
        
        private void UpdateTransformProperties()
        {
            _primitiveObjectToy.NetworkPosition = _transform.position;
            _primitiveObjectToy.NetworkRotation = new LowPrecisionQuaternion(_transform.rotation);
            _primitiveObjectToy.NetworkScale = _transform.root != _transform ? Vector3.Scale(_transform.localScale, _transform.root.localScale) : _transform.localScale;
        }

        private bool _isStatic;
    }
}
