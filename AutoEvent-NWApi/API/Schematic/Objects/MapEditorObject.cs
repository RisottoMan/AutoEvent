
namespace AutoEvent.API.Schematic.Objects
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

        //public IndicatorObject AttachedIndicator;

        public RoomName ForcedRoomType
        {
            get => _forcedRoom;
            set
            {
                CurrentRoom = null;
                _forcedRoom = value;
            }
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

                //return CurrentRoom.Type == RoomType.Surface ? transform.position : CurrentRoom.transform.InverseTransformPoint(transform.position);
                return Vector3.zero;
            }
        }

        public Vector3 RelativeRotation
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                /*
                Vector3 rotation = CurrentRoom.Type == RoomType.Surface ? transform.eulerAngles : transform.eulerAngles - CurrentRoom.transform.eulerAngles;

                if (gameObject.TryGetComponent(out ObjectRotationComponent rotationComponent))
                {
                    if (rotationComponent.XisRandom)
                        rotation.x = -1f;

                    if (rotationComponent.YisRandom)
                        rotation.y = -1f;

                    if (rotationComponent.ZisRandom)
                        rotation.z = -1f;
                }
                */
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
            //if (ForcedRoomType != RoomName.Unnamed)
            //    //return Room.Get(x => x.Type == ForcedRoomType).OrderBy(x => (x.Position - Position).sqrMagnitude).First();
            //    return RoomIdentifier.AllRoomIdentifiers.First(r => r.Name == RoomName.Outside);

            //RoomIdentifier? room = RoomIdentifier.FindParentRoom(gameObject);

            //if (room != null && room.Name == RoomName.Outside && Position.y <= 500f)
            //    room = Room.List.OrderBy(x => (x.Position - Position).sqrMagnitude).First();

            //return room != null ? room : Room.Get(RoomName.Outside);
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

        private RoomName _forcedRoom = RoomName.Unnamed;
    }
}