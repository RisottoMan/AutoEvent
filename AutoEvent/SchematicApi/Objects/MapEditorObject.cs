namespace MER.Lite.Objects
{
    using System;
    using System.Linq;
    using MapGeneration;
    using Mirror;
    using PluginAPI.Core;
    using Serializable;
    using UnityEngine;

    public abstract class MapEditorObject : MonoBehaviour
    {
        public virtual bool IsRotatable => true;

        public virtual bool IsScalable => true;

        public virtual void UpdateObject()
        {
            NetworkServer.UnSpawn(gameObject);
            NetworkServer.Spawn(gameObject);
        }

        public virtual MapEditorObject Init(SchematicBlockData block)
        {
            IsSchematicBlock = true;

            GameObject gO = gameObject;
            gO.name = block.Name;
            gO.transform.localPosition = block.Position;

            if (IsRotatable)
                gO.transform.localEulerAngles = block.Rotation;

            if (IsScalable)
                gO.transform.localScale = block.Scale;

            return this;
        }

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                UpdateObject();
            }
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set
            {
                if (!IsRotatable)
                    throw new InvalidOperationException($"{name} can not be rotated!");

                transform.rotation = value;
                UpdateObject();
            }
        }

        public Vector3 EulerAngles
        {
            get => Rotation.eulerAngles;
            set => Rotation = Quaternion.Euler(value);
        }

        public Vector3 Scale
        {
            get => transform.localScale;
            set
            {
                if (!IsScalable)
                    throw new InvalidOperationException($"{name} can not be rescaled!");

                transform.localScale = value;
                UpdateObject();
            }
        }

        public RoomIdentifier CurrentRoom { get; private set; }

        public bool IsSchematicBlock { get; internal set; }

        public Vector3 RelativePosition
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                return Vector3.zero;
            }
        }

        public Vector3 RelativeRotation
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                return Vector3.zero;
            }
        }

        public RoomName RoomType
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                return CurrentRoom.Name;
            }
        }

        public RoomIdentifier FindRoom()
        {
            return RoomIdentifier.AllRoomIdentifiers.First(r => r.Name == RoomName.Outside);
        }

        public static Color GetColorFromString(string colorText)
        {
            Color color = new(-1f, -1f, -1f);
            string[] charTab = colorText.Split(':');

            if (charTab.Length >= 4)
            {
                if (float.TryParse(charTab[0], out float red))
                    color.r = red / 255f;

                if (float.TryParse(charTab[1], out float green))
                    color.g = green / 255f;

                if (float.TryParse(charTab[2], out float blue))
                    color.b = blue / 255f;

                if (float.TryParse(charTab[3], out float alpha))
                    color.a = alpha;

                return color != new Color(-1f, -1f, -1f) ? color : Color.magenta * 3f;
            }

            if (colorText[0] != '#' && colorText.Length == 8)
                colorText = '#' + colorText;

            return ColorUtility.TryParseHtmlString(colorText, out color) ? color : Color.magenta * 3f;
        }

        public void Destroy() => Destroy(gameObject);

        public override string ToString() => $"{name} {Position} {Rotation.eulerAngles} {Scale}";

        internal Player prevOwner;
    }
}