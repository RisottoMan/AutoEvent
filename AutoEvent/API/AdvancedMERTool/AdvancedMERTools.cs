using System;
using System.Collections.Generic;
using System.IO;
using AutoEvent.API.AdvancedMERTool.Objects;
using AutoEvent.API.AdvancedMERTool.Serializable;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using MER.Lite.Objects;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Utf8Json;

namespace AutoEvent.API.AdvancedMERTool;

public class AdvancedMERTools
{
    private EventManager manager;
    
    public static AdvancedMERTools Singleton;
    
    public List<InteractablePickupObject> InteractablePickups = new List<InteractablePickupObject> { };
    
    public List<CustomColliderObject> CustomColliders = new List<CustomColliderObject> { };
    
    public Dictionary<Type, RandomExecutionModule> TypeSingletonPair = new Dictionary<Type, RandomExecutionModule> { };

    private SchematicObject SchematicObject;

    public void OnEnabled(SchematicObject schematicObject)
    {
        SchematicObject = schematicObject;
        
        DataLoad<InteractablePickupSerializable, InteractablePickupObject>("Pickups");
        DataLoad<CustomColliderSerializable, CustomColliderObject>("Colliders");
        
        Singleton = this;
        manager = new EventManager();
        Players.SearchPickUpItem += manager.OnItemSearching;
    }

    public void OnDisabled()
    {
        InteractablePickups.Clear();
        CustomColliders.Clear();
        
        Players.SearchPickUpItem -= manager.OnItemSearching;
        
        manager = null;
        Singleton = null;
    }
    
    public void DataLoad<Tdto, Tclass>(string name) where Tdto : AMERTDTO where Tclass : AMERTInteractable, new()
    {
        string path = Path.Combine(SchematicObject.DirectoryPath, SchematicObject.Base.SchematicName + $"-{name}.json");
        if (File.Exists(path))
        {
            List<Tdto> ts = JsonSerializer.Deserialize<List<Tdto>>(File.ReadAllText(path));
            foreach (Tdto dto in ts)
            {
                Transform target = FindObjectWithPath(SchematicObject.transform, dto.ObjectId);
                Tclass tclass = target.gameObject.AddComponent<Tclass>();
                tclass.Base = dto;
            }
        }
    }
    
    public static Transform FindObjectWithPath(Transform target, string pathO)
    {
        if (pathO != "")
        {
            string[] path = pathO.Split(' ');
            for (int i = path.Length - 1; i > -1; i--)
            {
                if (target.childCount == 0 || target.childCount <= int.Parse(path[i].ToString()))
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Logger, "Advanced MER tools: Could not find appropriate child!", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                    break;
                }
                target = target.GetChild(int.Parse(path[i]));
            }
        }
        return target;
    }
}