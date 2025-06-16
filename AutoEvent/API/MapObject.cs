using System.Collections.Generic;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace AutoEvent.API;

public class MapObject
{
    public List<GameObject> AttachedBlocks { get; set; } = new();
    public GameObject GameObject { get; set; }
    public Vector3 Position
    {
        get => this.GameObject.transform.position;
        set => this.GameObject.transform.position = value;
    }
    public Vector3 Rotation
    {
        get => this.GameObject.transform.eulerAngles;
        set => this.GameObject.transform.eulerAngles = value;
    }
    public void Destroy() => SchematicObject.Destroy(this.GameObject);
}