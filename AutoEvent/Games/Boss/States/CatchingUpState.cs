using PluginAPI.Core;
using System;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class CatchingUpState : IBossState
{
    private GameObject _santaObject;
    private float speed = 0.5f;
    private Player _target;
    public string Name { get; } = "CatchingUp";
    public string Description { get; } = "The state in which Santa Claus runs after one player and makes him a minion-Santa";
    public int Stage { get; } = 1;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; } = new TimeSpan(0, 0, 10);

    public void Init(Plugin plugin)
    {
        _santaObject = plugin.santaObject;
    }

    public void Update()
    {
        _target ??= Player.GetPlayers().Where(r => !r.IsSCP).ToList().RandomItem();

        Vector3 direction = _target.Position - _santaObject.transform.position;
        Vector3 velocity = new Vector3(direction.x, 0, direction.z).normalized * speed;

        if (Vector3.Distance(_target.Position, _santaObject.transform.position) > 2f)
        {
            _santaObject.transform.position += velocity;
            _santaObject.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
        else
        {
            _target = null;
        }
    }

    public void Deinit()
    {

    }
}
